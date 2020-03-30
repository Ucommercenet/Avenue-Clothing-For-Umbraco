using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ucommerce.Api;
using UCommerce.Infrastructure;
using UCommerce.RazorStore.Models;
using UCommerce.Search.Models;
using UCommerce.Search.Slugs;
using Umbraco.Web.Mvc;

namespace Ucommerce.Avenue.Umbraco.Controllers
{
    public class PartialViewController : SurfaceController
    {
        public IUrlService UrlService => ObjectFactory.Instance.Resolve<IUrlService>();
        public ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();
        public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();

        public ActionResult CategoryNavigation()
        {
            var categoryNavigationModel = new CategoryNavigationViewModel();

            IEnumerable<Category> rootCategories = CatalogLibrary.GetRootCategories().ToList();

            categoryNavigationModel.Categories = MapCategories(rootCategories);

            return View("/views/PartialView/CategoryNavigation.cshtml", categoryNavigationModel);
        }

        private IList<CategoryViewModel> MapCategories(IEnumerable<Category> categoriesToMap)
        {
            var categoriesToReturn = new List<CategoryViewModel>();

            var allSubCategoryIds = categoriesToMap.SelectMany(cat => cat.Categories).Distinct().ToList();
            var subCategoriesById = CatalogLibrary.GetCategories(allSubCategoryIds).ToDictionary(cat => cat.Guid);

            foreach (var category in categoriesToMap)
            {
                var categoryViewModel = new CategoryViewModel
                {
                    Name = category.DisplayName,
                    Url = UrlService.GetUrl(CatalogContext.CurrentCatalog,
                        new[] { category })
                };
                categoryViewModel.Categories = category.Categories
                    .Where(id => subCategoriesById.ContainsKey(id))
                    .Select(id => subCategoriesById[id])
                    .Select(cat => new CategoryViewModel
                    {
                        Name = cat.DisplayName,
                        Url = UrlService.GetUrl(CatalogContext.CurrentCatalog, new[] { category, cat })
                    })
                    .ToList();

                categoriesToReturn.Add(categoryViewModel);
            }

            return categoriesToReturn;
        }
    }
}