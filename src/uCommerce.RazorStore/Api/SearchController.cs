using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using UCommerce.Api;
using UCommerce.RazorStore.Api.Model;
using UCommerce.SystemHttp.Models;

namespace UCommerce.RazorStore.Api
{
    [RoutePrefix("ucommerceapi")]
    public class SearchController : ApiController
    {
        
        [Route("razorstore/search/")]
        [HttpPost]
        public IHttpActionResult Search([FromBody] SearchRequest request)
        {
            var products = UCommerce.EntitiesV2.Product.Find(p =>
                p.VariantSku == null
                && p.DisplayOnSite
                &&
                (
                    p.Sku.Contains(request.Keyword)
                    || p.Name.Contains(request.Keyword)
                    || p.ProductDescriptions.Any(d => d.DisplayName.Contains(request.Keyword) || d.ShortDescription.Contains(request.Keyword) || d.LongDescription.Contains(request.Keyword))
                )
            );


            return Json(MapResult(products));
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
                    Url = CatalogLibrary.GetNiceUrlForProduct(product),
                    Properties = product.ProductProperties.Select(prop => new ProductProperty()
                    {
                        Id = prop.Id,
                        Name = prop.ProductDefinitionField.Name,
                        Value = prop.Value
                    })
                });
            }

            return variations;
        }
    }
}