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
            categoryViewModel.Products = MapProducts(CatalogLibrary.GetProducts(currentCategory));
            
            if (!HasBannerImage(currentCategory))
            {
                var media = ObjectFactory.Instance.Resolve<IImageService>().GetImage(currentCategory.ImageMediaId).Url;
                categoryViewModel.BannerImageUrl = media;
            }

            IList<Facet> facetsForQuerying = System.Web.HttpContext.Current.Request.QueryString.ToFacets();

            List<int> productsInCategory = SearchLibrary.GetProductsFor(currentCategory, facetsForQuerying).Select( x => x.Id).ToList();
            var productRepository = ObjectFactory.Instance.Resolve<IRepository<Product>>();
            var productsForMapping = productRepository.Select(x => productsInCategory.Contains(x.ProductId)).ToList();
            
            categoryViewModel.Products = MapProducts(productsForMapping);
            
            return base.View("/Views/Catalog.cshtml", categoryViewModel);
        }

        private bool HasBannerImage(Category category)
        {      
            return string.IsNullOrEmpty(category.ImageMediaId);
        }

        private IList<ProductViewModel> MapProducts(ICollection<Product> productsInCategory)
        {
            IList<ProductViewModel> productViews = new List<ProductViewModel>();

            foreach (var product in productsInCategory)
            {
                var productViewModel = new ProductViewModel();

                productViewModel.Sku = product.Sku;
                productViewModel.Name = product.DisplayName();
                productViewModel.Url = CatalogLibrary.GetNiceUrlForProduct(product);
                productViewModel.PriceCalculation = CatalogLibrary.CalculatePrice(product);

                var media = ObjectFactory.Instance.Resolve<IImageService>().GetImage(product.ThumbnailImageMediaId).Url;
                productViewModel.ThumbnailImageUrl = media;

                productViews.Add(productViewModel);
            }

            return productViews;
        }
    }

}