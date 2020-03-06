using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Ucommerce.Api;
using UCommerce.Api;
using UCommerce.Infrastructure;
using UCommerce.RazorStore.Api.Model;
using UCommerce.Search;
using UCommerce.Search.Models;
using UCommerce.SystemHttp.Models;

namespace UCommerce.RazorStore.Api
{
    [RoutePrefix("ucommerceapi")]
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
                                || p.DisplayName.Contains(request.Keyword)
                                || p.LongDescription.Contains(request.Keyword)
                                || p.ShortDescription.Contains(request.Keyword)
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
                    Url = UrlService.GetUrl(CatalogContext.CurrentCatalog, new[] { product }),
                    //TODO: is it used?

                    //Properties = product.ProductProperties.Select(prop => new ProductProperty()
                    //{
                    //    Id = prop.Id,
                    //    Name = prop.ProductDefinitionField.Name,
                    //    Value = prop.Value
                    //})
                });
            }

            return variations;
        }
    }
}