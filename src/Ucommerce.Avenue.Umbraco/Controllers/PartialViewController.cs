using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ucommerce.Api;
using UCommerce.Infrastructure;
using UCommerce.RazorStore.Models;
using UCommerce.Search.Models;
using Umbraco.Web.Mvc;

namespace UCommerce.RazorStore.Controllers
{
    public class PartialViewController : SurfaceController
    {
        public ActionResult CategoryNavigation()
        {
            var catalogLibrary = ObjectFactory.Instance.Resolve<CatalogLibrary>();
            var categoryNavigationModel = new CategoryNavigationViewModel();

            IEnumerable<Category> rootCategories = catalogLibrary.GetRootCategories().ToList();

            categoryNavigationModel.Categories = MapCategories(rootCategories);

            return View("/views/PartialView/CategoryNavigation.cshtml", categoryNavigationModel);
        }

        private IList<CategoryViewModel> MapCategories(IEnumerable<Category> categoriesToMap)
        {
            var catalogLibrary = ObjectFactory.Instance.Resolve<CatalogLibrary>();
            var categoriesToReturn = new List<CategoryViewModel>();

            var allSubCategoryIds = categoriesToMap.SelectMany(cat => cat.Categories).Distinct().ToList();
            var subCategoriesById = catalogLibrary.GetCategories(allSubCategoryIds).ToDictionary(cat => cat.Guid);

            foreach (var category in categoriesToMap)
            {
                var categoryViewModel = new CategoryViewModel
                {
                    Name = category.DisplayName, 
                    // TODO: Url = category.Slug
                };
                categoryViewModel.Categories = category.Categories
                    .Select(id => subCategoriesById[id])
                    .Select(cat => new CategoryViewModel
                    {
                        Name = cat.DisplayName,
                        //TODO: Url = cat.Slug
                    })
                    .ToList();

                categoriesToReturn.Add(categoryViewModel);
            }

            return categoriesToReturn;
        }
    }
}