using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UCommerce;
using Ucommerce.Api;
using Ucommerce.Api.PriceCalculation;
using Ucommerce.Api.Search;
using Ucommerce.Avenue.Umbraco.Api.Model;
using UCommerce.Catalog.Models;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Logging;
using UCommerce.RazorStore.Models;
using UCommerce.Search;
using UCommerce.Search.FacetsV2;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Category = UCommerce.Search.Models.Category;
using Product = UCommerce.Search.Models.Product;

namespace Ucommerce.Avenue.Umbraco.Controllers
{
    public class CategoryController : RenderMvcController
    {
        public ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();
        public ISiteContext SiteContext => ObjectFactory.Instance.Resolve<ISiteContext>();
        public ISearchLibrary SearchLibrary => ObjectFactory.Instance.Resolve<ISearchLibrary>();
        private ILoggingService _log => ObjectFactory.Instance.Resolve<ILoggingService>();

        public override ActionResult Index(ContentModel model)
        {
            using (new SearchCounter(_log, "Made {0} search queries during catalog page display."))
            {
                var currentCategory = SiteContext.CatalogContext.CurrentCategory;

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

        private IList<ProductViewModel> MapProductsInCategories(Category category)
        {
            IList<Facet> facetsForQuerying = System.Web.HttpContext.Current.Request.QueryString.ToFacets();
            var productsInCategory = new List<ProductViewModel>();

            var subCategories = CatalogLibrary.GetCategories(category.Categories);
            var products = SearchLibrary.GetProductsFor(category.Categories, facetsForQuerying);

            foreach (var subCategory in subCategories)
            {
                var productsInSubCategory = products.Where(p => p.Categories.Contains(subCategory.Guid));
                productsInCategory.AddRange(MapProducts(productsInSubCategory.ToList()));
            }

            productsInCategory.AddRange(MapProducts(SearchLibrary.GetProductsFor(category.Guid, facetsForQuerying)
                .Results));

            return productsInCategory;
        }

        private IList<ProductViewModel> MapProducts(ICollection<Product> productsInCategory)
        {
            IList<ProductViewModel> productViews = new List<ProductViewModel>();

            foreach (var product in productsInCategory)
            {
                var productViewModel = new ProductViewModel
                {
                    Sku = product.Sku,
                    Name = product.Name,
                    ThumbnailImageUrl = product.ThumbnailImageUrl,
                };

                productViews.Add(productViewModel);
            }

            return productViews;
        }
        
    }
}