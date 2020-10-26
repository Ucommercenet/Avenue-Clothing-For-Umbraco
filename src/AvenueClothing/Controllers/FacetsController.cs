using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ucommerce.Api;
using Ucommerce.Infrastructure;
using Ucommerce.Search.Extensions;
using AvenueClothing.Models;
using Ucommerce.Search.Facets;
using Ucommerce.Search.Models;
using Umbraco.Core;
using Umbraco.Web.Mvc;
using AvenueClothing.Search;
using ImageProcessor.Imaging.Colors;

namespace AvenueClothing.Controllers
{
	public static class FacetedQueryStringExtensions
	{

		public static IList<Facet> ToFacets(this NameValueCollection target)
		{
			var productDefinition = ObjectFactory.Instance.Resolve<Ucommerce.Search.IIndexDefinition<Ucommerce.Search.Models.Product>>();
			var facets = productDefinition.Facets().Select(x => new KeyValuePair<string, string>(x.Key, x.Value.ToString())).ToDictionary(x => x.Key, x => x.Value);
			string[] facetsKeys = new string[facets.Keys.Count];

			facets.Keys.CopyTo(facetsKeys, 0);

			var parameters = new Dictionary<string, string>();

			foreach (var queryString in HttpContext.Current.Request.QueryString.AllKeys)
			{
				parameters[queryString] = HttpContext.Current.Request.QueryString[queryString];
			}

			parameters.RemoveAll(p => !facetsKeys.Contains(p.Key));

			var facetsForQuerying = new List<Facet>();

			foreach (var parameter in parameters)
			{
				var facet = new Facet { FacetValues = new List<FacetValue>(), Name = parameter.Key };
				foreach (var value in parameter.Value.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
				{
					facet.FacetValues.Add(new FacetValue() { Value = value });
				}


				facetsForQuerying.Add(facet);
			}

			return facetsForQuerying;
		}
	}

	public class FacetsController : SurfaceController
	{
		private ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
		private ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();

		// GET: Facets
		public ActionResult Index()
		{

			var category = CatalogContext.CurrentCategory;
			var facetValueOutputModel = new FacetsDisplayedViewModel();
			IList<Facet> facetsForQuerying = System.Web.HttpContext.Current.Request.QueryString.ToFacets();

			if (ShouldDisplayFacets(category))
			{
				IList<Facet> facets = CatalogLibrary.GetFacets(category.Guid, facetsForQuerying.ToFacetDictionary());
				if (facets.Any(x => x.FacetValues.Any(y => y.Count > 0)))
				{
					facetValueOutputModel.Facets = MapFacets(facets);
				}
			}

			return View("/Views/PartialView/Facets.cshtml", facetValueOutputModel);
		}

		private bool ShouldDisplayFacets(Category category)
		{
			var product = CatalogContext.CurrentProduct;

			return category != null && product == null;
		}

		private IList<FacetViewModel> MapFacets(IList<Facet> facetsInCategory)
		{
			IList<FacetViewModel> facets = new List<FacetViewModel>();

			foreach (var facet in facetsInCategory)
			{
				var facetViewModel = new FacetViewModel();
				facetViewModel.Name = facet.Name;
				facetViewModel.DisplayName = facet.DisplayName;

				if (!facet.FacetValues.Any())
				{
					continue;
				}

				foreach (var value in facet.FacetValues)
				{
					FacetValueViewModel facetVal = new FacetValueViewModel(value.Value, (int)value.Count);
					facetViewModel.FacetValues.Add(facetVal);
				}

				facets.Add(facetViewModel);
			}

			return facets;
		}
	}
}