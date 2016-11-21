using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using UCommerce.Infrastructure;
using UCommerce.Publishing.Model;
using UCommerce.Publishing.Runtime;
using UCommerce.RazorStore.Models;
using Umbraco.Web.Models;

namespace UCommerce.RazorStore.Controllers
{
    public class CategoryController : RenderMvcController
    {
        public override ActionResult Index(RenderModel model)
        {
            var currentCategory = GetCurrentCategory();

            var categoryViewModel = new CategoryViewModel
            {
                Name = currentCategory.DisplayName,
                Description = currentCategory.Description,
                CatalogId = currentCategory.CatalogId,
                CategoryId = currentCategory.CategoryId,
                Products = MapProductsInCategories(currentCategory)
            };


            if (!HasBannerImage(currentCategory))
            {
                var media = currentCategory.PrimaryImageUrl;
                categoryViewModel.BannerImageUrl = media;
            }

            return View("/Views/Catalog.cshtml", categoryViewModel);
        }

        private bool HasBannerImage(Category category)
        {
            return string.IsNullOrEmpty(category.PrimaryImageUrl);
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
            //IList<Facet> facetsForQuerying = System.Web.HttpContext.Current.Request.QueryString.ToFacets();
            var productsInCategory = new List<ProductViewModel>();

            foreach (var childCategory in GetChildCategories(category.Guid))
            {
                productsInCategory.AddRange(MapProductsInCategories(childCategory));
            }

            var products = GetProductsInCategory(category.Guid);
            productsInCategory.AddRange(MapProducts(products));
            // TODO: Take facets into acount.
            //productsInCategory.AddRange(MapProducts(SearchLibrary.GetProductsFor(category, facetsForQuerying)));

            return productsInCategory;
        }

        private Category GetCurrentCategory()
        {
            var catalogContext = ObjectFactory.Instance.Resolve<ICatalogContext>();
            var currentCategory = catalogContext.CurrentCategory;

            return currentCategory;
        }

        private IList<Product> GetProductsInCategory(Guid categoryId)
        {
            var catalogLibrary = ObjectFactory.Instance.Resolve<ICatalogLibrary>();
            return catalogLibrary.GetProducts(categoryId);
        }

        private IList<Category> GetChildCategories(Guid categoryGuid)
        {
            var catalogLibrary = ObjectFactory.Instance.Resolve<ICatalogLibrary>();
            return catalogLibrary.GetCategories(categoryGuid);
        }
    }
}