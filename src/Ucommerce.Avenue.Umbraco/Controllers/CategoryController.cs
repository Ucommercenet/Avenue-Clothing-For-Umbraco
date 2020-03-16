using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using Ucommerce.Api;
using Ucommerce.Api.Search;
using UCommerce.Infrastructure;
using UCommerce.RazorStore.Models;
using UCommerce.Search;
using UCommerce.Search.FacetsV2;
using UCommerce.Search.Models;
using Umbraco.Core;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace Ucommerce.Avenue.Umbraco.Controllers
{
    public class CategoryController : RenderMvcController
    {
        private IIndex<Category> CategoryIndex => ObjectFactory.Instance.Resolve<IIndex<Category>>();
        private IIndex<Product> ProductIndex => ObjectFactory.Instance.Resolve<IIndex<Product>>();
        
        [System.Web.Mvc.Route("/demo-store/categories/{category}")]
        [System.Web.Mvc.Route("/demo-store/categories/{category}/{*subcategories}")]
        public ActionResult Show(ContentModel model, [FromUri] string category, [FromUri] string[] subcategories)
        {
            var currentCategory = CategoryIndex.Find()
                .Where(cat => cat.Slug == category)
                .SingleOrDefault();

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

        
        
        
        
        
        private IList<ProductViewModel> MapProducts(IEnumerable<Product> productsInCategory)
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
            FacetDictionary facetsForQuerying = System.Web.HttpContext.Current.Request.QueryString.ToFacetDictionary();
            var productsInCategory = new List<ProductViewModel>();

            ResultSet<Category> subCategories = CategoryIndex.Find().Where(cat => category.Categories.Contains(cat.Guid)).ToList();
            var products = ProductIndex.Find()
                .Where(p => p.Categories.ContainsAny(category.Categories))
                .Where(facetsForQuerying).ToFacets();

            foreach (Guid subCategory in category.Categories)
            {
                var productsInSubCategory = products.Where(p => p.Categories.Contains(subCategory)).ToList();
                productsInCategory.AddRange(MapProducts(productsInSubCategory.ToList()));
            }

            productsInCategory.AddRange(MapProducts(ProductIndex.Find().Where(facetsForQuerying).ToList()));

            return productsInCategory;
        }
    }
}