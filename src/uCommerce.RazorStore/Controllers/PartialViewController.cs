using System;
using System.Collections.Generic;
using System.Web.Mvc;
using UCommerce.Infrastructure;
using UCommerce.Publishing.Model;
using UCommerce.Publishing.Runtime;
using UCommerce.RazorStore.Models;
using Umbraco.Web.Mvc;

namespace UCommerce.RazorStore.Controllers
{
    public class PartialViewController : SurfaceController
    {
        private ICatalogLibrary CatalogLibrary { get; set; }

        public PartialViewController()
        {
            CatalogLibrary = ObjectFactory.Instance.Resolve<ICatalogLibrary>();
        }

        public ActionResult CategoryNavigation()
        {
            var categoryNavigationModel = new CategoryNavigationViewModel();

            ICollection<Category> rootCategories = GetRootCategories();

            categoryNavigationModel.Categories = MapCategories(rootCategories);

            return View("/views/PartialView/CategoryNavigation.cshtml", categoryNavigationModel);
        }

        private IList<CategoryViewModel> MapCategories(ICollection<Category> categoriesToMap)
        {
            var categoriesToReturn = new List<CategoryViewModel>();

            foreach (var category in categoriesToMap)
            {
                var categoryViewModel = new CategoryViewModel
                {
                    Name = category.DisplayName,
                    Url = GetNiceUrlFor(category),
                    Categories = MapCategories(GetCategories(category))
                };

                categoriesToReturn.Add(categoryViewModel);
            }

            return categoriesToReturn;
        }

        private IList<Category> GetRootCategories()
        {
            return CatalogLibrary.GetRootCategories();
        }

        private string GetNiceUrlFor(Category category)
        {
            return CatalogLibrary.GetNiceUrlForCategory(category);
        }

        private IList<Category> GetCategories(Category category)
        {
            return CatalogLibrary.GetCategories(category.Guid);
        }
    }
}