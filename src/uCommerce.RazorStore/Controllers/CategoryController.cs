using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UCommerce.Api;
using UCommerce.Content;
using UCommerce.EntitiesV2;
using Umbraco.Web.Mvc;
using UCommerce.Extensions;
using UCommerce.Infrastructure;
using UCommerce.RazorStore.Models;
using UCommerce.RazorStore.Services.Commands;
using UCommerce.Runtime;
using UCommerce.Search.Facets;
using Umbraco.Web.Models;

namespace UCommerce.RazorStore.Controllers
{
    public class CategoryController : RenderMvcController
    {
        public ActionResult Index(RenderModel model)
        {
            var categoryViewModel = new CategoryViewModel();
            var currentCategory = SiteContext.Current.CatalogContext.CurrentCategory;


            categoryViewModel.Name = currentCategory.DisplayName();
            categoryViewModel.Description = currentCategory.Description();
            categoryViewModel.CatalogId = currentCategory.ProductCatalog.Id;
            categoryViewModel.CategoryId = currentCategory.Id;

            if (!HasBannerImage(currentCategory))
            {
                var media = ObjectFactory.Instance.Resolve<IImageService>().GetImage(currentCategory.ImageMediaId).Url;
                categoryViewModel.BannerImageUrl = media;
            }

            categoryViewModel.Products = MapProductsInCategories(currentCategory);

            return base.View("/Views/Catalog.cshtml", categoryViewModel);
        }

        private bool HasBannerImage(Category category)
        {
            return string.IsNullOrEmpty(category.ImageMediaId);
        }

        private IList<ProductViewModel> MapProducts(ICollection<Documents.Product> productsInCategory)
        {
            IList<ProductViewModel> productViews = new List<ProductViewModel>();

            foreach (var product in productsInCategory)
            {
                var productViewModel = new ProductViewModel();

                productViewModel.Sku = product.Sku;
                productViewModel.Name = product.Name;
                productViewModel.ThumbnailImageUrl = product.ThumbnailImageUrl;

                productViews.Add(productViewModel);
            }

            return productViews;
        }

        private IList<ProductViewModel> MapProductsInCategories(Category category)
        {
            IList<Facet> facetsForQuerying = System.Web.HttpContext.Current.Request.QueryString.ToFacets();
            var productsInCategory = new List<ProductViewModel>();

            foreach (var subcategory in category.Categories)
            {
                productsInCategory.AddRange(MapProductsInCategories(subcategory));
            }

            productsInCategory.AddRange(MapProducts(SearchLibrary.GetProductsFor(category, facetsForQuerying)));

            return productsInCategory;
        }
    }
}