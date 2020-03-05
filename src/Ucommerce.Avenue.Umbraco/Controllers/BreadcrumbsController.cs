using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ucommerce.Api;
using Umbraco.Web.Mvc;
using UCommerce.Api;
using UCommerce.RazorStore.Models;
using UCommerce.Runtime;
using UCommerce.Extensions;
using UCommerce.Infrastructure;
using UCommerce.Search;
using UCommerce.Search.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using CatalogContext = Ucommerce.Api.CatalogContext;
using Product = UCommerce.Search.Models.Product;

namespace UCommerce.RazorStore.Controllers
{
    public class BreadcrumbsController : SurfaceController
    {
        public CatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<CatalogLibrary>();
        public CatalogContext CatalogContext => ObjectFactory.Instance.Resolve<CatalogContext>();
        public ISlugService UrlService => ObjectFactory.Instance.Resolve<ISlugService>();

        public ActionResult Index()
        {
            IList<BreadcrumbsViewModel> breadcrumbs = new List<BreadcrumbsViewModel>();
            Category lastCategory = null;
            Product product = CatalogContext.CurrentProduct;

            foreach (var category in CatalogContext.CurrentCategories)
            {
                var breadcrumb = new BreadcrumbsViewModel
                {
                    BreadcrumbName = category.DisplayName,
                    BreadcrumbUrl = UrlService.GetUrl(CatalogContext.CurrentCatalog, new[] {category})
                };
                lastCategory = category;
                breadcrumbs.Add(breadcrumb);
            }

            if (product != null)
            {
                var breadcrumb = new BreadcrumbsViewModel
                {
                    BreadcrumbName = product.DisplayName,
                    BreadcrumbUrl = UrlService.GetUrl(CatalogContext.CurrentCatalog, new[] {lastCategory},
                        new[] {product})
                };
                breadcrumbs.Add(breadcrumb);
            }

            if (product == null && lastCategory == null)
            {
                var currentNode = CurrentPage;
                foreach (var level in currentNode.Ancestors().Where(IsVisible))
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

        protected virtual bool IsVisible(IPublishedContent x)
        {
            return x.Value("umbracoNaviHide", null, null, new Fallback(), true);
        }
    }
}