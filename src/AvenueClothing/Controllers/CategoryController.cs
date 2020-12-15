using System.Collections.Generic;
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
using Ucommerce;
using Ucommerce.Search.Definitions;

namespace AvenueClothing.Controllers
{
    public class CategoryController : RenderMvcController
    {
        public ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();
        public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
        private IUrlService _urlService => ObjectFactory.Instance.Resolve<IUrlService>();
        public IIndex<Product> ProductsIndex => ObjectFactory.Instance.Resolve<IIndex<Product>>();


        public override ActionResult Index(ContentModel model)
        {
            uint page = uint.Parse(Request.QueryString["pg"] ?? "1");
            uint take = uint.Parse(Request.QueryString["size"] ?? "12");
            uint skip = take * page - take;

            var currentCategory = CatalogContext.CurrentCategory;
            var products = GetAllSiblingProductsInCategory(currentCategory, skip, take);
                
            var categoryViewModel = new CategoryViewModel
            {
                Name = currentCategory.DisplayName,
                Description = currentCategory.Description,
                CatalogId = currentCategory.ProductCatalog,
                CategoryId = currentCategory.Guid,
                Products = MapProducts(products),
                TotalProducts = products.TotalCount,
                PageSize = take,
                PageNumber = page
            };

            if (!string.IsNullOrEmpty(currentCategory.ImageMediaUrl))
            {
                categoryViewModel.BannerImageUrl = currentCategory.ImageMediaUrl;
            }

            return View("/Views/Catalog.cshtml", categoryViewModel);
        }

        private IList<ProductViewModel> MapProducts(ResultSet<Product> productsInCategory)
        {
            IList<ProductViewModel> productViews = new List<ProductViewModel>();

            var taxRate = CatalogContext.CurrentPriceGroup.TaxRate;
            var currencyIsoCode = CatalogContext.CurrentPriceGroup.CurrencyISOCode;
            foreach (var product in productsInCategory)
            {
                product.PricesInclTax.TryGetValue(CatalogContext.CurrentPriceGroup.Name, out var price);
                product.Taxes.TryGetValue(CatalogContext.CurrentPriceGroup.Name, out var tax);

                var productViewModel = new ProductViewModel
                {
                    Sku = product.Sku,
                    Name = product.DisplayName,
                    ThumbnailImageUrl = product.PrimaryImageUrl,
                    Url = _urlService.GetUrl(CatalogContext.CurrentCatalog,
                        CatalogContext.CurrentCategories.Append(CatalogContext.CurrentCategory).Compact(), product),
                    Price = price > 0 ? new Money(price, currencyIsoCode).ToString() : "",
                    Tax = tax > 0 ? new Money(tax, currencyIsoCode).ToString() : "",
                    Rating = product.Rating
                };

                productViews.Add(productViewModel);
            }

            return productViews;
        }


        private ResultSet<Product> GetAllSiblingProductsInCategory(Category category, uint skip, uint take)
        {
            FacetDictionary facetsForQuerying = System.Web.HttpContext.Current.Request.QueryString.ToFacets();
            var allProducts = (List<Guid>) category["ProductsInAllSubcategories"];

            return ProductsIndex.Find()
                .Where(p => allProducts.Contains(p.Guid))
                .Where(facetsForQuerying)
                .Skip(skip)
                .Take(take)
                .ToList();
        }
    }
}