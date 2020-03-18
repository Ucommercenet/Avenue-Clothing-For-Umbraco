using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Ucommerce.Api;
using Ucommerce.Api.PriceCalculation;
using AvenueClothing.Api.Model;
using UCommerce.Catalog.Models;
using UCommerce.Extensions;
using UCommerce.Infrastructure;
using UCommerce.Search;
using UCommerce.Search.Slugs;
using Category = UCommerce.Search.Models.Category;
using Product = UCommerce.Search.Models.Product;
using ProductCatalog = UCommerce.Search.Models.ProductCatalog;
using ProductProperty = AvenueClothing.Api.Model.ProductProperty;

namespace AvenueClothing.Api
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

            return Json(new {Variant = variantModel});
        }
    }
}
