using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using UCommerce.Search.Extensions;
using UCommerce.Search.FacetsV2;

namespace Ucommerce.Avenue.Umbraco.Controllers
{
    public static class FacetedQueryStringExtensions
    {
        public static FacetDictionary ToFacetDictionary(this NameValueCollection target)
        {
            var dictionary = new FacetDictionary();
            target.ToFacets().ForEach(kvp => dictionary.Add(kvp.Name, kvp.FacetValues.Select(v => v.Value.ToString())));
            return dictionary;
        }
        
        public static IList<Facet> ToFacets(this NameValueCollection target)
        {
            var parameters = new Dictionary<string, string>();
            foreach (var queryString in HttpContext.Current.Request.QueryString.AllKeys)
            {
                parameters[queryString] = HttpContext.Current.Request.QueryString[queryString];
            }

            if (parameters.ContainsKey("umbDebugShowTrace"))
            {
                parameters.Remove("umbDebugShowTrace");
            }

            if (parameters.ContainsKey("product"))
            {
                parameters.Remove("product");
            }

            if (parameters.ContainsKey("category"))
            {
                parameters.Remove("category");
            }

            if (parameters.ContainsKey("catalog"))
            {
                parameters.Remove("catalog");
            }

            var facetsForQuerying = new List<Facet>();

            foreach (var parameter in parameters)
            {
                var facet = new Facet {FacetValues = new List<FacetValue>(), Name = parameter.Key};
                foreach (var value in parameter.Value.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries))
                {
                    facet.FacetValues.Add(new FacetValue() {Value = value});
                }

                facetsForQuerying.Add(facet);
            }

            return facetsForQuerying;
        }
    }
}