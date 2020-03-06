using System.Collections.Generic;
using System.Web.Mvc;
using Ucommerce.Api;
using Ucommerce.Api.Search;
using Umbraco.Web.Mvc;
using UCommerce.Infrastructure;
using UCommerce.RazorStore.Models;
using UCommerce.Search.FacetsV2;
using UCommerce.Search.Models;
using Umbraco.Web.Models;

namespace UCommerce.RazorStore.Controllers
{
    public class CategoryController : RenderMvcController
    {
        public ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();
        public ISiteContext SiteContext => ObjectFactory.Instance.Resolve<ISiteContext>();
        public ISearchLibrary SearchLibrary => ObjectFactory.Instance.Resolve<ISearchLibrary>();

        public override ActionResult Index(ContentModel model)
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

        private IList<ProductViewModel> MapProducts(ICollection<Product> productsInCategory)
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

            foreach (var subCategoryGuid in category.Categories)
            {
                var subCategory = CatalogLibrary.GetCategory(subCategoryGuid);
                productsInCategory.AddRange(MapProductsInCategories(subCategory));
            }

            productsInCategory.AddRange(MapProducts(SearchLibrary.GetProductsFor(category.Guid, facetsForQuerying).Results));

            return productsInCategory;
        }
    }
}