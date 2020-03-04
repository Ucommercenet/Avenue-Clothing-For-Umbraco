using System;
using System.Linq;
using System.Web.Http;
using Ucommerce.Api;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.RazorStore.Api.Model;
using ProductProperty = UCommerce.RazorStore.Api.Model.ProductProperty;

namespace UCommerce.RazorStore.Api
{
    [RoutePrefix("ucommerceapi")]
    public class AvenueClothingApiProductController : ApiController
    {
        public CatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<CatalogLibrary>();

        [Route("razorstore/products/getproductvariations")]
        [HttpPost]
        public IHttpActionResult GetProductVariations([FromBody] GetProductVariationsRequest request)
        {
            Search.Models.Product product =
                CatalogLibrary.GetProduct(request.ProductSku);

            var variations = product.Variants.Select(variant => new ProductVariation
            {
                Sku = variant.Sku,
                VariantSku = variant.VariantSku,
                ProductName = variant.Name,
                Properties = variant.ProductProperties.Select(prop => new ProductProperty()
                {
                    Id = prop.Id,
                    Name = prop.ProductDefinitionField.Name,
                    Value = prop.Value
                })
            }).ToList();

            return Json(new {Variations = variations});
        }

        [Route("razorstore/products/getvariantskufromselection")]
        [HttpPost]
        public IHttpActionResult GetVariantSkuFromSelectionRequest([FromBody] GetVariantSkuFromSelectionRequest request)
        {
            var product = CatalogLibrary.GetProduct(request.ProductSku);
            UCommerce.EntitiesV2.Product variant = null;

            if (product.Variants.Any() && request.VariantProperties.Any()
            ) // If there are variant values we'll need to find the selected variant
            {
                variant = product.Variants.FirstOrDefault(v =>
                    v.ProductProperties
                        .Where(pp =>
                            pp.ProductDefinitionField.DisplayOnSite && pp.ProductDefinitionField.IsVariantProperty &&
                            !pp.ProductDefinitionField.Deleted && pp.Value != null && pp.Value != String.Empty).All(p =>
                            request.VariantProperties.Any(kv =>
                                kv.Key.Equals(p.ProductDefinitionField.Name,
                                    StringComparison.InvariantCultureIgnoreCase) && kv.Value.ToString()
                                    .Equals(p.Value, StringComparison.InvariantCultureIgnoreCase))));
            }
            else if (!product.Variants.Any()) // Only use the current product where there are no variants
            {
                variant = product;
            }

            var variantModel = new ProductVariation
            {
                Sku = variant.Sku,
                VariantSku = variant.VariantSku,
                ProductName = variant.Name,
                Properties = variant.ProductProperties.Select(prop => new ProductProperty
                {
                    Id = prop.Id,
                    Name = prop.ProductDefinitionField.Name,
                    Value = prop.Value
                })
            };

            return Json(new {Variant = variantModel});
        }


        [Route("razorstore/products/getproductinformation")]
        [HttpPost]
        public IHttpActionResult GetProductInformation([FromBody] GetProductInformationRequest request)
        {
            ProductCatalog catalog = CatalogLibrary.GetCatalog(request.CatalogId);
            Product product = CatalogLibrary.GetProduct(request.Sku);
            Category category = CatalogLibrary.GetCategory(request.CategoryId.Value);
            string niceUrl = CatalogLibrary.GetNiceUrlForProduct(product, category, catalog);

            PriceCalculation priceCalculation =
                CatalogLibrary.CalculatePrices(product);

            Currency currency = priceCalculation.YourPrice.Amount.Currency;

            ProductInformation productInformation = new ProductInformation()
            {
                NiceUrl = niceUrl,
                PriceCalculation = new PriceCalculationViewModel()
                {
                    Discount = new PriceViewModel()
                    {
                        Amount = new MoneyViewModel()
                        {
                            Value = priceCalculation.Discount.Amount.Value,
                            DisplayValue = new Money(priceCalculation.Discount.Amount.Value, currency).ToString()
                        },
                        AmountExclTax = new MoneyViewModel()
                        {
                            Value = priceCalculation.Discount.AmountExclTax.Value,
                            DisplayValue = new Money(priceCalculation.Discount.AmountExclTax.Value, currency).ToString()
                        },
                        AmountInclTax = new MoneyViewModel()
                        {
                            Value = priceCalculation.Discount.AmountInclTax.Value,
                            DisplayValue = new Money(priceCalculation.Discount.AmountInclTax.Value, currency).ToString()
                        }
                    },
                    IsDiscounted = priceCalculation.IsDiscounted,
                    YourPrice = new PriceViewModel()
                    {
                        Amount = new MoneyViewModel()
                        {
                            Value = priceCalculation.YourPrice.Amount.Value,
                            DisplayValue = new Money(priceCalculation.YourPrice.Amount.Value, currency).ToString()
                        },
                        AmountInclTax = new MoneyViewModel()
                        {
                            Value = priceCalculation.YourPrice.AmountInclTax.Value,
                            DisplayValue =
                                new Money(priceCalculation.YourPrice.AmountInclTax.Value, currency).ToString()
                        },
                        AmountExclTax = new MoneyViewModel()
                        {
                            Value = priceCalculation.YourPrice.AmountExclTax.Value,
                            DisplayValue =
                                new Money(priceCalculation.YourPrice.AmountExclTax.Value, currency).ToString()
                        }
                    },
                    ListPrice = new PriceViewModel()
                    {
                        Amount = new MoneyViewModel()
                        {
                            Value = priceCalculation.ListPrice.Amount.Value,
                            DisplayValue = new Money(priceCalculation.ListPrice.Amount.Value, currency).ToString()
                        },
                        AmountExclTax = new MoneyViewModel()
                        {
                            Value = priceCalculation.ListPrice.AmountExclTax.Value,
                            DisplayValue =
                                new Money(priceCalculation.ListPrice.AmountExclTax.Value, currency).ToString()
                        },
                        AmountInclTax = new MoneyViewModel()
                        {
                            Value = priceCalculation.ListPrice.AmountInclTax.Value,
                            DisplayValue =
                                new Money(priceCalculation.ListPrice.AmountInclTax.Value, currency).ToString()
                        }
                    }
                }
            };
            productInformation.Sku = product.Sku;

            return Json(productInformation);
        }
    }
}