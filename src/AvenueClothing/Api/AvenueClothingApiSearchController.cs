using System.Collections.Generic;
using System.Web.Http;
using Ucommerce.Api;
using AvenueClothing.Api.Model;
using Ucommerce.Infrastructure;
using Ucommerce.Search;
using Ucommerce.Search.Models;
using Ucommerce.Search.Slugs;

namespace AvenueClothing.Api
{
    [RoutePrefix("Ucommerceapi")]
    public class AvenueClothingApiSearchController : ApiController
    {
        public IUrlService UrlService => ObjectFactory.Instance.Resolve<IUrlService>();
        public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
        public IIndex<Product> ProductIndex => ObjectFactory.Instance.Resolve<IIndex<Product>>();

        [Route("razorstore/search/")]
        [HttpPost]
        public IHttpActionResult Search([FromBody] SearchRequest request)
        {
            var products = ProductIndex.Find()
                .Where(p => p.VariantSku == null
                            && (
                                p.Sku.Contains(request.Keyword)
                                || p.Name.Contains(request.Keyword)
                                || p.DisplayName == Match.FullText(request.Keyword)
                                || p.LongDescription == Match.FullText(request.Keyword)
                                || p.ShortDescription == Match.FullText(request.Keyword)
                            ))
                .ToList();


            return Json(new
            {
                Variations = MapResult(products)
            });
        }

        private IList<ProductVariation> MapResult(ResultSet<Product> products)
        {
            var variations = new List<ProductVariation>();

            foreach (var product in products)
            {
                variations.Add(new ProductVariation
                {
                    Sku = product.Sku,
                    VariantSku = product.VariantSku,
                    ProductName = product.DisplayName,
                    Url = UrlService.GetUrl(CatalogContext.CurrentCatalog, product )
                });
            }

            return variations;
        }
    }
}