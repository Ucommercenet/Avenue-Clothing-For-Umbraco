using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ucommerce.Api.Search;
using UCommerce.Infrastructure;
using UCommerce.RazorStore.Models;
using UCommerce.Search.FacetsV2;
using UCommerce.Search.Models;
using Umbraco.Web.Mvc;
using ISiteContext = Ucommerce.Api.ISiteContext;

namespace Ucommerce.Avenue.Umbraco.Controllers
{
    public class FacetsController : SurfaceController
    {
        readonly ISiteContext _siteContext = ObjectFactory.Instance.Resolve<ISiteContext>();
        readonly ISearchLibrary _searchLibrary = ObjectFactory.Instance.Resolve<ISearchLibrary>();

        // GET: Facets
        public ActionResult Index()
        {
            var category = _siteContext.CatalogContext.CurrentCategory;
            var facetValueOutputModel = new FacetsDisplayedViewModel();
            FacetDictionary facetsForQuerying = System.Web.HttpContext.Current.Request.QueryString.ToFacetDictionary();

            if (ShouldDisplayFacets(category))
            {
                IList<Facet> facets = _searchLibrary.GetFacetsFor(category.Guid, facetsForQuerying).Facets;
                if (facets.Any(x => x.FacetValues.Any(y => y.Count > 0)))
                {
                    facetValueOutputModel.Facets = MapFacets(facets);
                }
            }

            return View("/Views/PartialView/Facets.cshtml", facetValueOutputModel);
        }

        private bool ShouldDisplayFacets(Category category)
        {
            var product = _siteContext.CatalogContext.CurrentProduct;

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
                    if (value.Count > 0)
                    {
                        FacetValueViewModel facetVal = new FacetValueViewModel(value.Value, (int) value.Count);
                        facetViewModel.FacetValues.Add(facetVal);
                    }
                }

                facets.Add(facetViewModel);
            }

            return facets;
        }
    }
}