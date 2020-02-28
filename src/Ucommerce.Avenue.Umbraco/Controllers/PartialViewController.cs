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

            IEnumerable<Category> rootCategories = catalogLibrary.GetRootCategories().Where(x=> x.Display).ToList();

            categoryNavigationModel.Categories = MapCategories(rootCategories);

            return View("/views/PartialView/CategoryNavigation.cshtml", categoryNavigationModel);
        }

        private IList<CategoryViewModel> MapCategories(IEnumerable<Category> categoriesToMap)
        {
            var catalogLibrary = ObjectFactory.Instance.Resolve<CatalogLibrary>();
            var categoriesToReturn = new List<CategoryViewModel>();

            foreach (var category in categoriesToMap)
            {
                var categoryViewModel = new CategoryViewModel();

                categoryViewModel.Name = category.DisplayName;
                // categoryViewModel.Url = CatalogLibrary.GetNiceUrlForCategory(category);
                categoryViewModel.Categories = MapCategories(catalogLibrary.GetSubCategories(category.Guid).Where(x=> x.Display).ToList());

                categoriesToReturn.Add(categoryViewModel);
            }

            return categoriesToReturn;
        }
    }
}