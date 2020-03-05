using System.Collections.Generic;
using System.Web.Mvc;
using Ucommerce.Api;
using Ucommerce.Api.Search;
using UCommerce.Content;
using Umbraco.Web.Mvc;
using UCommerce.Infrastructure;
using UCommerce.RazorStore.Models;
using UCommerce.Search;
using UCommerce.Search.FacetsV2;
using UCommerce.Search.Models;
using Umbraco.Web.Models;

namespace UCommerce.RazorStore.Controllers
{
    public class CategoryController : RenderMvcController
    {
        private readonly ICatalogLibrary _catalogLibrary = ObjectFactory.Instance.Resolve<ICatalogLibrary>();
        private readonly ISiteContext _siteContext = ObjectFactory.Instance.Resolve<ISiteContext>();
        private readonly ISearchLibrary _searchLibrary = ObjectFactory.Instance.Resolve<ISearchLibrary>();

        public override ActionResult Index(ContentModel model)
        {
            var currentCategory = _siteContext.CatalogContext.CurrentCategory;

            var categoryViewModel = new CategoryViewModel
            {
                Name = currentCategory.DisplayName,
                Description = currentCategory.DisplayName,
                CatalogGuid = currentCategory.ProductCatalog,
                CategoryGuid = currentCategory.Guid,
                Products = MapProductsInCategories(currentCategory)
            };

            //TODO: we don't have ImageMediaId anymore, just skip the check and set the BannerImageUrl = categoryImageMediaUrl ??
            //if (!HasBannerImage(currentCategory))
            //{
            //    var media = ObjectFactory.Instance.Resolve<IImageService>().GetImage(currentCategory.ImageMediaId).Url;
            //    categoryViewModel.BannerImageUrl = media;
            //}

            return View("/Views/Catalog.cshtml", categoryViewModel);
        }

        //private bool HasBannerImage(Category category)
        //{
        //    return string.IsNullOrEmpty(category.ImageMediaId);
        //}

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
                var subCategory = _catalogLibrary.GetCategory(subCategoryGuid);
                productsInCategory.AddRange(MapProductsInCategories(subCategory));
            }

            productsInCategory.AddRange(MapProducts(_searchLibrary.GetProductsFor(category.Guid, facetsForQuerying).Results));

            return productsInCategory;
        }
    }
}