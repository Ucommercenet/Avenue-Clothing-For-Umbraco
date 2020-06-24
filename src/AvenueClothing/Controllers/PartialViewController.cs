using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ucommerce.Api;
using Ucommerce.Infrastructure;
using AvenueClothing.Models;
using Ucommerce.Search.Models;
using Ucommerce.Search.Slugs;
using Umbraco.Web.Mvc;
using Umbraco.Core.Models;
using AvenueClothing.Enums;

namespace AvenueClothing.Controllers
{
    public class PartialViewController : SurfaceController
    {
        public IUrlService UrlService => ObjectFactory.Instance.Resolve<IUrlService>();
        public ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();
        public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();

        public ActionResult CategoryNavigation(int partialViewType)
        {
            var categoryNavigationModel = new CategoryNavigationViewModel();

            IEnumerable<Category> rootCategories = CatalogLibrary.GetRootCategories().ToList();

            categoryNavigationModel.Categories = MapCategories(rootCategories);

            switch (partialViewType)
            {
                case (int)PartialTypes.CategoryNavigation:
                    return View("/views/PartialView/CategoryNavigation.cshtml", categoryNavigationModel);          
                case (int)PartialTypes.SiteWideCategoryNavigation:
                    return View("/views/PartialView/SiteWideCategoryNavigation.cshtml", categoryNavigationModel);
                case (int)PartialTypes.FeaturedCategoryNavigation:
                    return View("/views/PartialView/FeaturedCategoryNavigation.cshtml", categoryNavigationModel);
                default:
                    return View("/views/PartialView/CategoryNavigation.cshtml", categoryNavigationModel);
            }
            
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