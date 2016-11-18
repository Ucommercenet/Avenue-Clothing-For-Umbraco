using System.Collections.Generic;
using System.Web.Mvc;
using UCommerce.Api;
using UCommerce.Content;
using UCommerce.EntitiesV2;
using Umbraco.Web.Mvc;
using UCommerce.Extensions;
using UCommerce.Infrastructure;
using UCommerce.RazorStore.Models;
using UCommerce.Runtime;
using UCommerce.Search.Facets;
using Umbraco.Web.Models;

namespace UCommerce.RazorStore.Controllers
{
    public class CategoryController : RenderMvcController
    {
        public override ActionResult Index(RenderModel model)
        {
            var currentCategory = SiteContext.Current.CatalogContext.CurrentCategory;

            var categoryViewModel = new CategoryViewModel
            {
                Name = currentCategory.DisplayName(),
                Description = currentCategory.Description(),
                CatalogId = currentCategory.ProductCatalog.Id,
                CategoryId = currentCategory.Id,
                Products = MapProductsInCategories(currentCategory)
            };


            if (!HasBannerImage(currentCategory))
            {
                var media = ObjectFactory.Instance.Resolve<IImageService>().GetImage(currentCategory.ImageMediaId).Url;
                categoryViewModel.BannerImageUrl = media;
            }

            return View("/Views/Catalog.cshtml", categoryViewModel);
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
                var productViewModel = new ProductViewModel
                {
                    Sku = product.Sku,
                    Name = product.Name,
                    ThumbnailImageUrl = product.ThumbnailImageUrl
                };


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