using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ucommerce.Api;
using Ucommerce.Extensions;
using Ucommerce.Infrastructure;
using AvenueClothing.Models;
using Ucommerce.Search;
using Ucommerce.Search.Models;
using Ucommerce.Search.Slugs;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using ICatalogContext = Ucommerce.Api.ICatalogContext;
using Product = Ucommerce.Search.Models.Product;

namespace AvenueClothing.Controllers
{
    public class BreadcrumbsController : SurfaceController
    {
        public ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();
        public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
        public IUrlService UrlService => ObjectFactory.Instance.Resolve<IUrlService>();

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
                    BreadcrumbUrl = UrlService.GetUrl(CatalogContext.CurrentCatalog,
                        new[] {lastCategory}.Compact().ToArray(),
                        product)
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