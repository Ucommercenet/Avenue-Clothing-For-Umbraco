using System.Collections.Generic;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.Extensions;
using UCommerce.RazorStore.Models;
using UCommerce.Runtime;
namespace UCommerce.RazorStore.Controllers
{
    public class BreadcrumbsController : SurfaceController
    {
        public ActionResult Index()
        {
           IList<BreadcrumbsViewModel> breadcrumbs = new List<BreadcrumbsViewModel>();
           Category lastCategory = null;
           Product product = SiteContext.Current.CatalogContext.CurrentProduct;

            foreach (var category in SiteContext.Current.CatalogContext.CurrentCategories)
            {
                var breadcrumb = new BreadcrumbsViewModel()
                {
                    BreadcrumbName = category.DisplayName(),
                    BreadcrumbUrl = CatalogLibrary.GetNiceUrlForCategory(category)
                };
                lastCategory = category;
                breadcrumbs.Add(breadcrumb);
            }

            if (product != null)
            {
                var breadcrumb = new BreadcrumbsViewModel()
                {
                    BreadcrumbName = product.DisplayName(),
                    BreadcrumbUrl = CatalogLibrary.GetNiceUrlForProduct(product, lastCategory)
                };
                breadcrumbs.Add(breadcrumb);
            }

            return View("/Views/PartialView/Breadcrumbs.cshtml", breadcrumbs);
        }
    }
}