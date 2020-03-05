using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Ucommerce.Api;
using UCommerce.Api;
using UCommerce.Infrastructure;
using UCommerce.RazorStore.Api.Model;
using UCommerce.Search;
using UCommerce.SystemHttp.Models;

namespace UCommerce.RazorStore.Api
{
    [RoutePrefix("ucommerceapi")]
    public class AvenueClothingApiSearchController : ApiController
    {
        public ISlugService UrlService => ObjectFactory.Instance.Resolve<ISlugService>();
        public CatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<CatalogLibrary>();
        public CatalogContext CatalogContext => ObjectFactory.Instance.Resolve<CatalogContext>();

        [Route("razorstore/search/")]
        [HttpPost]
        public IHttpActionResult Search([FromBody] SearchRequest request)
        {
            // TODO: 
            
            var products = UCommerce.EntitiesV2.Product.Find(p =>
                p.VariantSku == null
                && p.DisplayOnSite
                &&
                (
                    p.Sku.Contains(request.Keyword)
                    || p.Name.Contains(request.Keyword)
                    || p.ProductDescriptions.Any(d =>
                        d.DisplayName.Contains(request.Keyword) || d.ShortDescription.Contains(request.Keyword) ||
                        d.LongDescription.Contains(request.Keyword))
                )
            );


            return Json(new
            {
                Variations = MapResult(products)
            });
        }

        private IList<ProductVariation> MapResult(IList<UCommerce.EntitiesV2.Product> products)
        {
            var variations = new List<ProductVariation>();

            foreach (var product in products)
            {
                variations.Add(new ProductVariation
                {
                    Sku = product.Sku,
                    VariantSku = product.VariantSku,
                    ProductName = product.ProductDescriptions.First().DisplayName,
                    Properties = product.ProductProperties.Select(prop => new ProductProperty()
                    {
                        Id = prop.Id,
                        Name = prop.ProductDefinitionField.Name,
                        Value = prop.Value
                    }),
                    // TODO: re-implement
                    // Url = UrlService.GetUrl(CatalogContext.CurrentCatalog, new[] {product})
                });
            }

            return variations;
        }
    }
}