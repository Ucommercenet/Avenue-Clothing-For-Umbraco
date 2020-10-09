﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ucommerce.Api;
using Ucommerce.Infrastructure;
using Ucommerce.Infrastructure.Logging;
using AvenueClothing.Models;
using Ucommerce.Extensions;
using Ucommerce.Search;
using Ucommerce.Search.Extensions;
using Ucommerce.Search.Facets;
using Ucommerce.Search.Slugs;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Category = Ucommerce.Search.Models.Category;
using Money = Ucommerce.Money;
using Product = Ucommerce.Search.Models.Product;
using System;

namespace AvenueClothing.Controllers
{
	public class CategoryController : RenderMvcController
	{
		public ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();
		public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
		private ILoggingService _log => ObjectFactory.Instance.Resolve<ILoggingService>();
		private IUrlService _urlService => ObjectFactory.Instance.Resolve<IUrlService>();

		public override ActionResult Index(ContentModel model)
		{

			var page = Request.QueryString["pg"] ?? "1";
			var pageSize = Request.QueryString["size"] ?? "12";

			using (new SearchCounter(_log, "Made {0} search queries during catalog page display."))
			{
				var currentCategory = CatalogContext.CurrentCategory;
				int totalProductsCount;
				var categoryViewModel = new CategoryViewModel
				{
					Name = currentCategory.DisplayName,
					Description = currentCategory.Description,
					CatalogId = currentCategory.ProductCatalog,
					CategoryId = currentCategory.Guid,
					Products = MapProductsInCategories(currentCategory, out totalProductsCount),
					TotalProducts = totalProductsCount,
					PageSize = Int32.Parse(pageSize),
					PageNumber = Int32.Parse(page)
				};

				if (!string.IsNullOrEmpty(currentCategory.ImageMediaUrl))
				{
					categoryViewModel.BannerImageUrl = currentCategory.ImageMediaUrl;
				}

				return View("/Views/Catalog.cshtml", categoryViewModel);
			}
		}

		private IList<ProductViewModel> MapProducts(ICollection<Product> productsInCategory)
		{
			IList<ProductViewModel> productViews = new List<ProductViewModel>();

			var taxRate = CatalogContext.CurrentPriceGroup.TaxRate;
			var currencyIsoCode = CatalogContext.CurrentPriceGroup.CurrencyISOCode;
			foreach (var product in productsInCategory)
			{
				var productViewModel = new ProductViewModel
				{
					Sku = product.Sku,
					Name = product.DisplayName,
					ThumbnailImageUrl = product.PrimaryImageUrl,
					Url = _urlService.GetUrl(CatalogContext.CurrentCatalog,
					CatalogContext.CurrentCategories.Append(CatalogContext.CurrentCategory).Compact(), product),
					Rating = product.Rating
				};
				if (product.UnitPrices.TryGetValue(CatalogContext.CurrentPriceGroup.Name, out var unitPrice))
				{
					productViewModel.Price = new Money(unitPrice * (1.0M + taxRate), currencyIsoCode).ToString();
					productViewModel.Tax = new Money(unitPrice * taxRate, currencyIsoCode).ToString();
				}

				productViews.Add(productViewModel);
			}

			return productViews;
		}


		private IList<ProductViewModel> MapProductsInCategories(Category category, out int totalProducts)
		{

			var page = Request.QueryString["pg"] ?? "1";
			var pageSize = Request.QueryString["size"] ?? "12";
			var skip = (Int32.Parse(pageSize) * Int32.Parse(page) - Int32.Parse(pageSize));

			IList<Facet> facetsForQuerying = System.Web.HttpContext.Current.Request.QueryString.ToFacets();
			var productsInCategory = new List<ProductViewModel>();


			var subCategories = CatalogLibrary.GetCategories(category.Categories);
			var products =
				CatalogLibrary.GetProducts(category.Categories.Append(category.Guid).ToList(),
					facetsForQuerying.ToFacetDictionary());

			foreach (var subCategory in subCategories)
			{
				var productsInSubCategory = products.Where(p => p.Categories.Contains(subCategory.Guid));
				productsInCategory.AddRange(MapProducts(productsInSubCategory.ToList()));
			}

			productsInCategory.AddRange(MapProducts(products.Where(p => p.Categories.Contains(category.Guid))
				.ToList()));

			totalProducts = productsInCategory.Count;
			productsInCategory = productsInCategory.Skip(skip).Take(Int32.Parse(pageSize)).ToList();

			return productsInCategory;
		}

	}
}