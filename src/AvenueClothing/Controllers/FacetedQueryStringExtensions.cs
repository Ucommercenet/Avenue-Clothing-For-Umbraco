using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Ucommerce.Infrastructure;
using Ucommerce.Search.Facets;
using Umbraco.Core;

namespace AvenueClothing.Controllers
{
    public static class FacetedQueryStringExtensions
    {

        public static FacetDictionary ToFacets(this NameValueCollection target)
        {
            var productDefinition = ObjectFactory.Instance.Resolve<Ucommerce.Search.IIndexDefinition<Ucommerce.Search.Models.Product>>();
            var facets = productDefinition.Facets.Select(x => new KeyValuePair<string, string>(x.Key, x.Value.ToString())).ToDictionary(x => x.Key, x => x.Value);
            string[] facetsKeys = new string[facets.Keys.Count];

            facets.Keys.CopyTo(facetsKeys, 0);

            var parameters = new Dictionary<string, string>();

            foreach (var queryString in HttpContext.Current.Request.QueryString.AllKeys)
            {
                parameters[queryString] = HttpContext.Current.Request.QueryString[queryString];
            }

            parameters.RemoveAll(p => !facetsKeys.Contains(p.Key));

            var facetsForQuerying = new FacetDictionary();

            foreach (var parameter in parameters)
            {
                var facet = new Facet { FacetValues = new List<FacetValue>(), Name = parameter.Key };
                foreach (var value in parameter.Value.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    facet.FacetValues.Add(new FacetValue() { Value = value });
                }


                facetsForQuerying.Add(parameter.Key, facet.FacetValues);
            }

            return facetsForQuerying;
        }
    }
}