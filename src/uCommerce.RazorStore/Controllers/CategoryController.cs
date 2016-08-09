using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.EntitiesV2.Queries.Marketing;
using UCommerce.Extensions;
using UCommerce.MasterClass.Website.Models;
using UCommerce.Runtime;
using UCommerce.Search.Indexers;

namespace UCommerce.MasterClass.Website.Controllers
{
	public class CategoryController : System.Web.Mvc.Controller
	{
		public ActionResult Index()
		{
			var categoryViewModel = new CategoryViewModel();

			var currentCategory = UCommerce.Runtime.SiteContext.Current.CatalogContext.CurrentCategory;
			
			categoryViewModel.Name = currentCategory.DisplayName();
			categoryViewModel.Description = currentCategory.Description();

			var productsInCategory = UCommerce.Api.CatalogLibrary.GetProducts(currentCategory);

			categoryViewModel.Products = MapProducts(productsInCategory);

			return View("/views/category.cshtml",categoryViewModel);
		}

		private IList<ProductViewModel> MapProducts(ICollection<Product> productsInCategory)
		{
			IList<ProductViewModel> productViews = new List<ProductViewModel>();

			foreach (UCommerce.EntitiesV2.Product product in productsInCategory)
			{
				var productViewModel = new ProductViewModel();

				productViewModel.Sku = product.Sku;
				productViewModel.Name = product.DisplayName();
				productViewModel.Url = "/store/product?product=" + product.ProductId;

				productViewModel.PriceCalculation = UCommerce.Api.CatalogLibrary.CalculatePrice(product);
				
				productViews.Add(productViewModel);
			}

			return productViews;
		}
	}
}