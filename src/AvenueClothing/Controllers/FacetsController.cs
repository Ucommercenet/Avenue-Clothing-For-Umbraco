using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ucommerce.Api;
using Ucommerce.Infrastructure;
using Ucommerce.Search.Extensions;
using AvenueClothing.Models;
using Ucommerce.Search.Facets;
using Ucommerce.Search.Models;
using Umbraco.Web.Mvc;
using AvenueClothing.Search;
using ImageProcessor.Imaging.Colors;

namespace AvenueClothing.Controllers
{
	public class FacetsController : SurfaceController
	{
		private ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
		private ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();

		// GET: Facets
		public ActionResult Index()
		{

			var category = CatalogContext.CurrentCategory;
			var facetValueOutputModel = new FacetsDisplayedViewModel();
			FacetDictionary facetsForQuerying = System.Web.HttpContext.Current.Request.QueryString.ToFacets();

			if (ShouldDisplayFacets(category))
			{
				IList<Facet> facets = CatalogLibrary.GetFacets(category.Guid, facetsForQuerying);
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