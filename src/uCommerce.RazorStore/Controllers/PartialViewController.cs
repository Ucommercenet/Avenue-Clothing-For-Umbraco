using System.Collections.Generic;
using System.Web.Mvc;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.Extensions;
using UCommerce.RazorStore.Models;
using Umbraco.Web.Mvc;

namespace UCommerce.MasterClass.Website.Controllers
{
    public class PartialViewController : SurfaceController
    {
        public ActionResult CategoryNavigation()
        {
            var categoryNavigationModel = new CategoryNavigationViewModel();

            ICollection<Category> rootCategories = CatalogLibrary.GetRootCategories();

            categoryNavigationModel.Categories = MapCategories(rootCategories);

            return View("/views/PartialView/CategoryNavigation.cshtml", categoryNavigationModel);
        }

        private IList<CategoryViewModel> MapCategories(ICollection<Category> categoriesToMap)
        {
            var categoriesToReturn = new List<CategoryViewModel>();

            foreach (var category in categoriesToMap)
            {
                var categoryViewModel = new CategoryViewModel();

                categoryViewModel.Name = category.DisplayName();
                categoryViewModel.Url = CatalogLibrary.GetNiceUrlForCategory(category);
                categoryViewModel.Categories = MapCategories(CatalogLibrary.GetCategories(category));

                categoriesToReturn.Add(categoryViewModel);
            }

            return categoriesToReturn;
        }
    }
}