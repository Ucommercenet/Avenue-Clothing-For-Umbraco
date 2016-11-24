using System.Collections.Generic;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using UCommerce.Api;
using UCommerce.Infrastructure;
using UCommerce.Publishing.Model;
using UCommerce.Publishing.Runtime;
using UCommerce.RazorStore.Models;
using Umbraco.Web;

namespace UCommerce.RazorStore.Controllers
{
    public class BreadcrumbsController : SurfaceController
    {
        public ActionResult Index()
        {
            IList<BreadcrumbsViewModel> breadcrumbs = new List<BreadcrumbsViewModel>();

            var catalogContext = ObjectFactory.Instance.Resolve<ICatalogContext>();
            var catalogLibrary = ObjectFactory.Instance.Resolve<ICatalogLibrary>();

           Category lastCategory = null;
           Product product = catalogContext.CurrentProduct;

            foreach (var category in catalogContext.CurrentCategoryPath)
            {
                var breadcrumb = new BreadcrumbsViewModel()
                {
                    BreadcrumbName = category.DisplayName,
                    BreadcrumbUrl = catalogLibrary.GetNiceUrlForCategory(category)
                };
                lastCategory = category;
                breadcrumbs.Add(breadcrumb);
            }

            if (product != null)
            {
                var breadcrumb = new BreadcrumbsViewModel()
                {
                    BreadcrumbName = product.DisplayName,
                    BreadcrumbUrl = catalogLibrary.GetNiceUrlForProduct(lastCategory != null ? lastCategory.CatalogId : catalogContext.CurrentCatalogId, lastCategory, product)
                };
                breadcrumbs.Add(breadcrumb);
            }

            if (product == null && lastCategory == null)
            {
                var currentNode = UmbracoContext.Current.PublishedContentRequest.PublishedContent;
                foreach (var level in currentNode.Ancestors().Where("Visible"))
                {
                    var breadcrumb = new BreadcrumbsViewModel()
                    {
                        BreadcrumbName = level.Name,
                        BreadcrumbUrl = level.Url
                    };
                breadcrumbs.Add(breadcrumb);
                }
                var currentBreadcrumb = new BreadcrumbsViewModel()
                {
                    BreadcrumbName = currentNode.Name,
                    BreadcrumbUrl = currentNode.Url
                };
                breadcrumbs.Add(currentBreadcrumb);
            }

            return View("/Views/PartialView/Breadcrumbs.cshtml", breadcrumbs);
        }
    }
}