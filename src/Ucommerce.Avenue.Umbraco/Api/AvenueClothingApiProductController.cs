using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Ucommerce.Api;
using UCommerce.Catalog.Models;
using UCommerce.Infrastructure;
using UCommerce.RazorStore.Api.Model;
using UCommerce.Search;
using UCommerce.Search.Models;
using ProductProperty = UCommerce.RazorStore.Api.Model.ProductProperty;

namespace UCommerce.RazorStore.Api
{
    [RoutePrefix("ucommerceapi")]
    public class AvenueClothingApiProductController : ApiController
    {
        private readonly ICatalogLibrary _catalogLibrary = ObjectFactory.Instance.Resolve<ICatalogLibrary>();
        private readonly IUrlService _urlService = ObjectFactory.Instance.Resolve<IUrlService>();


        [Route("razorstore/products/getproductvariations")]
        [HttpPost]
        public IHttpActionResult GetProductVariations([FromBody] GetProductVariationsRequest request)
        {
            var product = _catalogLibrary.Find(request.ProductSku);

            var variants = _catalogLibrary.GetVariants(product.Guid);
            var variations = variants.Select(variant => new ProductVariation
            {
                Sku = variant.Sku,
                VariantSku = variant.VariantSku,
                ProductName = variant.Name,
                //TODO: how to handle this with the new entities?
                //Properties = variant.ProductProperties.Select(prop => new ProductProperty()
                //{
                //    Id = prop.Id,
                //    Name = prop.ProductDefinitionField.Name,
                //    Value = prop.Value
                //})
            }).ToList();

            return Json(new {Variations = variations});
        }

        [Route("razorstore/products/getvariantskufromselection")]
        [HttpPost]
        public IHttpActionResult GetVariantSkuFromSelectionRequest([FromBody] GetVariantSkuFromSelectionRequest request)
        {
            var product = _catalogLibrary.Find(request.ProductSku);
            Product variant = null;

            if (product.Variants.Any() && request.VariantProperties.Any()) // If there are variant values we'll need to find the selected variant
            {
                //TODO: untangle THIS!
                //variant = product.Variants
                //    .FirstOrDefault(v => v.ProductProperties
                //        .Where(pp => pp.ProductDefinitionField.DisplayOnSite &&
                //                     pp.ProductDefinitionField.IsVariantProperty && 
                //                     !pp.ProductDefinitionField.Deleted && pp.Value != null && 
                //                     pp.Value != String.Empty).All(p =>
                //            request.VariantProperties.Any(kv => kv.Key.Equals(p.ProductDefinitionField.Name, StringComparison.InvariantCultureIgnoreCase) 
                //                                                && kv.Value.ToString().Equals(p.Value, StringComparison.InvariantCultureIgnoreCase))));
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
                //Properties = variant.ProductProperties.Select(prop => new ProductProperty
                //{
                //    Id = prop.Id,
                //    Name = prop.ProductDefinitionField.Name,
                //    Value = prop.Value
                //})
            };

            return Json(new {Variant = variantModel});
        }


        [Route("razorstore/products/getproductinformation")]
        [HttpPost]
        public IHttpActionResult GetProductInformation([FromBody] GetProductInformationRequest request)
        {
            ProductCatalog catalog = _catalogLibrary.GetCatalog(request.CatalogId);
            Product product = _catalogLibrary.Find(request.Sku);
            Category category = _catalogLibrary.GetCategory(request.CategoryId);
            string niceUrl = _urlService.GetUrl(catalog, new[] {category}, new[] {product});

            //PriceCalculation priceCalculation = CatalogLibrary.CalculatePrice(product);

            //Currency currency = priceCalculation.YourPrice.Amount.Currency;

            ProductInformation productInformation = new ProductInformation()
            {
                NiceUrl = niceUrl,
            };
            //    PriceCalculation = new PriceCalculationViewModel()
            //    {
            //        Discount = new PriceViewModel()
            //        {
            //            Amount = new MoneyViewModel()
            //            {
            //                Value = priceCalculation.Discount.Amount.Value,
            //                DisplayValue = new Money(priceCalculation.Discount.Amount.Value, currency).ToString()
            //            },
            //            AmountExclTax = new MoneyViewModel()
            //            {
            //                Value = priceCalculation.Discount.AmountExclTax.Value,
            //                DisplayValue = new Money(priceCalculation.Discount.AmountExclTax.Value, currency).ToString()
            //            },
            //            AmountInclTax = new MoneyViewModel()
            //            {
            //                Value = priceCalculation.Discount.AmountInclTax.Value,
            //                DisplayValue = new Money(priceCalculation.Discount.AmountInclTax.Value, currency).ToString()
            //            }
            //        },
            //        IsDiscounted = priceCalculation.IsDiscounted,
            //        YourPrice = new PriceViewModel()
            //        {
            //            Amount = new MoneyViewModel()
            //            {
            //                Value = priceCalculation.YourPrice.Amount.Value,
            //                DisplayValue = new Money(priceCalculation.YourPrice.Amount.Value, currency).ToString()
            //            },
            //            AmountInclTax = new MoneyViewModel()
            //            {
            //                Value = priceCalculation.YourPrice.AmountInclTax.Value,
            //                DisplayValue = new Money(priceCalculation.YourPrice.AmountInclTax.Value, currency).ToString()
            //            },
            //            AmountExclTax = new MoneyViewModel()
            //            {
            //                Value = priceCalculation.YourPrice.AmountExclTax.Value,
            //                DisplayValue = new Money(priceCalculation.YourPrice.AmountExclTax.Value, currency).ToString()
            //            }
            //        },
            //        ListPrice = new PriceViewModel()
            //        {
            //            Amount = new MoneyViewModel()
            //            {
            //                Value = priceCalculation.ListPrice.Amount.Value,
            //                DisplayValue = new Money(priceCalculation.ListPrice.Amount.Value, currency).ToString()
            //            },
            //            AmountExclTax = new MoneyViewModel()
            //            {
            //                Value = priceCalculation.ListPrice.AmountExclTax.Value,
            //                DisplayValue = new Money(priceCalculation.ListPrice.AmountExclTax.Value, currency).ToString()
            //            },
            //            AmountInclTax = new MoneyViewModel()
            //            {
            //                Value = priceCalculation.ListPrice.AmountInclTax.Value,
            //                DisplayValue = new Money(priceCalculation.ListPrice.AmountInclTax.Value, currency).ToString()
            //            }
            //        }
            //    }
            //};
            productInformation.Sku = product.Sku;

            return Json(productInformation);
        }
    }
}