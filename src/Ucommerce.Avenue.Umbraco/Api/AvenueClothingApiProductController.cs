using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Ucommerce.Api;
using Ucommerce.Api.PriceCalculation;
using Ucommerce.Avenue.Umbraco.Api.Model;
using UCommerce.Catalog.Models;
using UCommerce.Extensions;
using UCommerce.Infrastructure;
using UCommerce.Search;
using Category = UCommerce.Search.Models.Category;
using Product = UCommerce.Search.Models.Product;
using ProductCatalog = UCommerce.Search.Models.ProductCatalog;
using ProductProperty = Ucommerce.Avenue.Umbraco.Api.Model.ProductProperty;

namespace Ucommerce.Avenue.Umbraco.Api
{
    [RoutePrefix("ucommerceapi")]
    public class AvenueClothingApiProductController : ApiController
    {
        public IUrlService UrlService => ObjectFactory.Instance.Resolve<IUrlService>();
        public ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();
        public IIndex<Product> ProductsIndex => ObjectFactory.Instance.Resolve<IIndex<Product>>();

        [Route("razorstore/products/getproductvariations")]
        [HttpPost]
        public IHttpActionResult GetProductVariations([FromBody] GetProductVariationsRequest request)
        {
            var product =
                CatalogLibrary.GetProduct(request.ProductSku);

            if (!product.ProductFamily)
                return NotFound();

            var variations = CatalogLibrary.GetVariants(product).Select(variant =>
            {
                var properties = variant
                    .GetUserDefinedFields().Select(field => new ProductProperty()
                {
                    Name = field.Key,
                    Value = field.Value.ToString()
                });
                
                return new ProductVariation
                {
                    Sku = variant.Sku,
                    VariantSku = variant.VariantSku,
                    ProductName = variant.Name,
                    Properties = properties
                };
            }).ToList();

            return Json(new { Variations = variations });
        }

        [Route("razorstore/products/getvariantskufromselection")]
        [HttpPost]
        public IHttpActionResult GetVariantSkuFromSelectionRequest([FromBody] GetVariantSkuFromSelectionRequest request)
        {
            var product = CatalogLibrary.GetProduct(request.ProductSku);
            Product variant = null;

            if (product.ProductFamily && request.VariantProperties.Any()
            ) // If there are variant values we'll need to find the selected variant
            {
                var query = ProductsIndex.Find().Where(p => p.Sku == request.ProductSku);
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

            return Json(new { Variant = variantModel });
        }


        [Route("razorstore/products/getproductinformation")]
        [HttpPost]
        public IHttpActionResult GetProductInformation([FromBody] GetProductInformationRequest request)
        {
            ProductCatalog catalog = CatalogLibrary.GetCatalog(request.CatalogId);
            Category category = CatalogLibrary.GetCategory(request.CategoryId);
            Product product = CatalogLibrary.GetProduct(request.Sku);
            string niceUrl = UrlService.GetUrl(catalog, new[] { category }, new[] { product });

            ProductPriceCalculationResult.Item priceCalculation =
                CatalogLibrary.CalculatePrices(new List<Guid> { product.Guid }).Items.First();


            var includeTax = catalog.ShowPricesIncludingTax;
            ProductInformation productInformation = new ProductInformation
            {
                NiceUrl = niceUrl,
                PriceCalculation = new PriceCalculationViewModel()
                {
                    IsDiscounted = priceCalculation.DiscountPercentage > 0M,
                    Discount = GetPriceViewModel(priceCalculation.DiscountExclTax, priceCalculation.DiscountInclTax, includeTax,
                        priceCalculation.CurrencyISOCode),
                    YourPrice = GetPriceViewModel(priceCalculation.PriceExclTax, priceCalculation.PriceInclTax, includeTax, priceCalculation.CurrencyISOCode),
                    ListPrice = GetPriceViewModel(priceCalculation.ListPriceExclTax, priceCalculation.ListPriceInclTax, includeTax,
                        priceCalculation.CurrencyISOCode),
                },
                Sku = product.Sku
            };

            return Json(productInformation);
        }

        private PriceViewModel GetPriceViewModel(decimal priceExclTax, decimal priceInclTax, bool includeTax, string currency)
        {
            var price = includeTax ? priceInclTax : priceExclTax;
            return new PriceViewModel()
            {
                Amount = GetMoneyViewModel(price, currency),
                AmountExclTax = GetMoneyViewModel(priceExclTax, currency),
                AmountInclTax = GetMoneyViewModel(priceInclTax, currency),
            };
        }

        private MoneyViewModel GetMoneyViewModel(decimal price, string currency)
        {
            return new MoneyViewModel()
            {
                Value = price,
                DisplayValue = new ApiMoney(price, currency).ToString()
            };
        }
    }
}