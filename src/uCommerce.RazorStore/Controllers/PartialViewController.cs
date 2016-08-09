using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.Extensions;
using UCommerce.MasterClass.Website.Models;

namespace UCommerce.MasterClass.Website.Controllers
{
	public class PartialViewController : System.Web.Mvc.Controller
	{
		public ActionResult CategoryNavigation()
		{
			var categoryNavigation = new CategoryNavigationViewModel();

			ICollection<Category> rootCategories = UCommerce.Api.CatalogLibrary.GetRootCategories();

			categoryNavigation.Categories = MapCategories(rootCategories);

			return View("/views/PartialViews/CategoryNavigation.cshtml", categoryNavigation);
		}

		private IList<CategoryViewModel> MapCategories(ICollection<UCommerce.EntitiesV2.Category> categoriesToMap)
		{
			var categoriesToReturn = new List<CategoryViewModel>();

			foreach (var category in categoriesToMap)
			{
				var categoryViewModel = new CategoryViewModel();

				categoryViewModel.Name = category.DisplayName();
				categoryViewModel.Url = "/store/category?category=" + category.CategoryId;
//				categoryViewModel.Url = UCommerce.Api.CatalogLibrary.GetNiceUrlForCategory(category);
				categoryViewModel.Categories = MapCategories(UCommerce.Api.CatalogLibrary.GetCategories(category));

				categoriesToReturn.Add(categoryViewModel);
			}

			return categoriesToReturn;
		} 
	}
}