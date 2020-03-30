using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ucommerce.Api;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Logging;
using UCommerce.RazorStore.Models;
using UCommerce.Search;
using UCommerce.Search.Extensions;
using UCommerce.Search.Facets;
using UCommerce.Search.Slugs;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Category = UCommerce.Search.Models.Category;
using Money = Ucommerce.Api.PriceCalculation.Money;
using Product = UCommerce.Search.Models.Product;

namespace Ucommerce.Avenue.Umbraco.Controllers
{
    public class CategoryController : RenderMvcController
    {
        public ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();
        public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
        private ILoggingService _log => ObjectFactory.Instance.Resolve<ILoggingService>();
        private IUrlService _urlService => ObjectFactory.Instance.Resolve<IUrlService>();

        public override ActionResult Index(ContentModel model)
        {
            using (new SearchCounter(_log, "Made {0} search queries during catalog page display."))
            {
                var currentCategory = CatalogContext.CurrentCategory;

                var categoryViewModel = new CategoryViewModel
                {
                    Name = currentCategory.DisplayName,
                    Description = currentCategory.Description,
                    CatalogId = currentCategory.ProductCatalog,
                    CategoryId = currentCategory.Guid,
                    Products = MapProductsInCategories(currentCategory)
                };

                if (!string.IsNullOrEmpty(currentCategory.ImageMediaUrl))
                {
                    categoryViewModel.BannerImageUrl = currentCategory.ImageMediaUrl;
                }

                return View("/Views/Catalog.cshtml", categoryViewModel);
            }
        }

        protected virtual ActionResult RenderView()
        {
            Category currentCategory = CatalogContext.CurrentCategory;
            var categoryViewModel = new CategoryViewModel
            {
                Name = currentCategory.DisplayName,
                Description = currentCategory.Description,
                CatalogId = currentCategory.ProductCatalog,
                CategoryId = currentCategory.Guid,
                Products = MapProductsInCategories(currentCategory)
            };

            if (!string.IsNullOrEmpty(currentCategory.ImageMediaUrl))
            {
                categoryViewModel.BannerImageUrl = currentCategory.ImageMediaUrl;
            }

            return View("/Views/Catalog.cshtml", categoryViewModel);
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
                        CatalogContext.CurrentCategories.Append(CatalogContext.CurrentCategory).Compact(), product)
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


        private IList<ProductViewModel> MapProductsInCategories(Category category)
        {
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

            return productsInCategory;
        }
    }
}