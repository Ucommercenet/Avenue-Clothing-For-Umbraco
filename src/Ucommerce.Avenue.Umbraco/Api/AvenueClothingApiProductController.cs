using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Ucommerce.Api;
using UCommerce.Catalog.Models;
using UCommerce.EntitiesV2;
using UCommerce.Extensions;
using UCommerce.Infrastructure;
using UCommerce.RazorStore.Api.Model;
using UCommerce.Search;
using Category = UCommerce.Search.Models.Category;
using Product = UCommerce.Search.Models.Product;
using ProductCatalog = UCommerce.Search.Models.ProductCatalog;

namespace UCommerce.RazorStore.Api
{
    [RoutePrefix("ucommerceapi")]
    public class AvenueClothingApiProductController : ApiController
    {
        public IUrlService UrlService => ObjectFactory.Instance.Resolve<IUrlService>();
        public ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();
        public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
        public IIndex<Product> ProductsIndex => ObjectFactory.Instance.Resolve<IIndex<Product>>();

        [Route("razorstore/products/getproductvariations")]
        [HttpPost]
        public IHttpActionResult GetProductVariations([FromBody] GetProductVariationsRequest request)
        {
            Search.Models.Product product =
                CatalogLibrary.GetProduct(request.ProductSku);

            if (!product.ProductFamily)
                return NotFound();

            var variations = CatalogLibrary.GetVariants(product).Select(variant => new ProductVariation
            {
                Sku = variant.Sku,
                VariantSku = variant.VariantSku,
                ProductName = variant.Name,
            }).ToList();

            return Json(new {Variations = variations});
        }

        [Route("razorstore/products/getvariantskufromselection")]
        [HttpPost]
        public IHttpActionResult GetVariantSkuFromSelectionRequest([FromBody] GetVariantSkuFromSelectionRequest request)
        {
            Product product = CatalogLibrary.GetProduct(request.ProductSku);
            Product variant = null;

            if (product.ProductFamily && request.VariantProperties.Any()
            ) // If there are variant values we'll need to find the selected variant
            {
                var query = ProductsIndex.Find();
                request.VariantProperties.ForEach(property =>
                {
                    query = query.Where(p => p[property.Key] == property.Value);
                });
                variant = query.FirstOrDefault();
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
            string niceUrl = UrlService.GetUrl(catalog, new[] {category}, new[] {product});

            ProductPriceCalculationResult.Item priceCalculation =
                CatalogLibrary.CalculatePrices(new List<Guid> {product.Guid}).Items.First();

            Currency currency = Currency.Get(priceCalculation.CurrencyISOCode);

            ProductInformation productInformation = new ProductInformation()
            {
                NiceUrl = niceUrl,
                PriceCalculation = new PriceCalculationViewModel()
                {
                    // TODO: Switch between excl and incl tax based on Catalog setting
                    
                    Discount = new PriceViewModel()
                    {
                        Amount = new MoneyViewModel()
                        {
                            Value = priceCalculation.DiscountExclTax,
                            DisplayValue = new Money(priceCalculation.DiscountExclTax, currency).ToString()
                        },
                        AmountExclTax = new MoneyViewModel()
                        {
                            Value = priceCalculation.DiscountExclTax,
                            DisplayValue = new Money(priceCalculation.DiscountExclTax, currency).ToString()
                        },
                        AmountInclTax = new MoneyViewModel()
                        {
                            Value = priceCalculation.DiscountInclTax,
                            DisplayValue = new Money(priceCalculation.DiscountInclTax, currency).ToString()
                        }
                    },
                    IsDiscounted = priceCalculation.DiscountPercentage > 0M,
                    YourPrice = new PriceViewModel()
                    {
                        Amount = new MoneyViewModel()
                        {
                            Value = priceCalculation.PriceExclTax,
                            DisplayValue = new Money(priceCalculation.PriceExclTax, currency).ToString()
                        },
                        AmountInclTax = new MoneyViewModel()
                        {
                            Value = priceCalculation.PriceInclTax,
                            DisplayValue =
                                new Money(priceCalculation.PriceInclTax, currency).ToString()
                        },
                        AmountExclTax = new MoneyViewModel()
                        {
                            Value = priceCalculation.PriceExclTax,
                            DisplayValue =
                                new Money(priceCalculation.PriceExclTax, currency).ToString()
                        }
                    },
                    ListPrice = new PriceViewModel()
                    {
                        Amount = new MoneyViewModel()
                        {
                            Value = priceCalculation.ListPriceExclTax,
                            DisplayValue = new Money(priceCalculation.ListPriceExclTax, currency).ToString()
                        },
                        AmountExclTax = new MoneyViewModel()
                        {
                            Value = priceCalculation.ListPriceInclTax,
                            DisplayValue =
                                new Money(priceCalculation.ListPriceInclTax, currency).ToString()
                        },
                        AmountInclTax = new MoneyViewModel()
                        {
                            Value = priceCalculation.ListPriceExclTax,
                            DisplayValue =
                                new Money(priceCalculation.ListPriceExclTax, currency).ToString()
                        }
                    }
                }
            };
            productInformation.Sku = product.Sku;

            return Json(productInformation);
        }
    }
}