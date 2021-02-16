using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AvenueClothing.Api.Model;
using Ucommerce.Infrastructure;
using Ucommerce.Search;
using Ucommerce.Search.Models;

namespace AvenueClothing.Api
{
    [RoutePrefix("Ucommerceapi")]
    public class AvenueClothingApiSearchController : ApiController
    {
        private IIndex<Product> ProductIndex => ObjectFactory.Instance.Resolve<IIndex<Product>>();

        [Route("razorstore/suggest/")]
        [HttpPost]
        public List<string> Suggest([FromBody] SearchRequest request)
        { 
            var result = ProductIndex.Find().ToSuggestions("DisplayName", request.Keyword, true);
            return result.Suggestions.ToList();
        }
    }
}