using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UCommerce.Api;
using UCommerce.Content;
using UCommerce.EntitiesV2;
using UCommerce.EntitiesV2.Queries.Marketing;
using UCommerce.Extensions;
using UCommerce.Infrastructure;
using UCommerce.RazorStore.Models;
using UCommerce.Runtime;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Umbraco.Web;



namespace UCommerce.RazorStore.Controllers
{
    public class ProductController : RenderMvcController
    {
        public ActionResult Index(RenderModel model)
        {
            Product currentProduct = SiteContext.Current.CatalogContext.CurrentProduct;

            var productViewModel = new ProductViewModel();

            productViewModel.Sku = currentProduct.Sku;
            productViewModel.PriceCalculation = UCommerce.Api.CatalogLibrary.CalculatePrice(currentProduct);
            productViewModel.Name = currentProduct.DisplayName();
            productViewModel.LongDescription = currentProduct.ShortDescription();
            productViewModel.IsVariant = false;
            productViewModel.IsOrderingAllowed = currentProduct.AllowOrdering;
            productViewModel.TaxCalculation = CatalogLibrary.CalculatePrice(currentProduct).YourTax.ToString();

            if (!string.IsNullOrEmpty(currentProduct.PrimaryImageMediaId))
            {
                var media = ObjectFactory.Instance.Resolve<IImageService>().GetImage(currentProduct.PrimaryImageMediaId).Url;
                productViewModel.ThumbnailImageUrl = media;
            }

            productViewModel.Properties = MapProductProperties(currentProduct);

            //if (currentProduct.ProductReviews.Any())
            //{
            //    productViewModel.Reviews = MapReviews(currentProduct);
            //}

            if (currentProduct.ProductDefinition.IsProductFamily())
            {
                productViewModel.Variants = MapVariants(currentProduct.Variants);
            }

            return base.View("/Views/Product.cshtml", productViewModel);
        }

        private IList<ProductViewModel> MapVariants(ICollection<Product> variants)
        {
            var variantModels = new List<ProductViewModel>();
            foreach (var currentVariant in variants)
            {
                ProductViewModel productModel = new ProductViewModel();
                productModel.Sku = currentVariant.Sku;
                productModel.VariantSku = currentVariant.VariantSku;
                productModel.Name = currentVariant.DisplayName();
                productModel.LongDescription = currentVariant.LongDescription();
                productModel.IsVariant = true;


                variantModels.Add(productModel);
            }
            return variantModels;
        }

        private IList<ProductPropertiesViewModel> MapProductProperties(Product product)
        {
            var productProperties = new List<ProductPropertiesViewModel>();

            var uniqueVariants = from v in product.Variants.SelectMany(p => p.ProductProperties)
                                 where v.ProductDefinitionField.DisplayOnSite
                                 group v by v.ProductDefinitionField into g
                                 select g;

            foreach (var prop in uniqueVariants)
            {
                var productPropertiesViewModel = new ProductPropertiesViewModel();
                productPropertiesViewModel.PropertyName = prop.Key.Name;

                foreach (var value in prop.Select(p => p.Value).Distinct())
                {
                    productPropertiesViewModel.Values.Add(value);
                }
                productProperties.Add(productPropertiesViewModel);
            }

            return productProperties;
        }

        //private IList<ProductReviewViewModel> MapReviews(Product product)
        //{
        //    var reviews = new List<ProductReviewViewModel>();
        //    foreach (var review in product.ProductReviews)
        //    {
        //        ProductReviewViewModel reviewModel = new ProductReviewViewModel();
        //        reviewModel.Name = review.Customer.FirstName + " " + review.Customer.LastName;
        //        reviewModel.Title = review.ReviewHeadline;
        //        reviewModel.Comments = review.ReviewText;
        //        reviewModel.Rating = review.Rating;

        //        reviews.Add(reviewModel);
        //    }

        //    return reviews;
        //}

        [HttpPost]
        public ActionResult Index(AddToBasketViewModel model)
        {
            string variant = GetVariantFromPostData(model.Sku, "variation-");
            TransactionLibrary.AddToBasket(1, model.Sku, variant);
            //return View(new RenderModel(UmbracoContext.Current.PublishedContentRequest.PublishedContent));
            //return View("/Views/Product.cshtml");
            return base.RedirectToAction("Index", "Product");
        }


        private string GetVariantFromPostData(string sku, string prefix)
        {

            var request = System.Web.HttpContext.Current.Request;
            var keys = request.Form.AllKeys.Where(k => k.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase));
            var properties = keys.Select(k => new { Key = k.Replace(prefix, string.Empty), Value = Request.Form[k] }).ToList();

            Product product = SiteContext.Current.CatalogContext.CurrentProduct;
            string variantSku = null;

            // If there are variant values we'll need to find the selected variant
            if (!product.IsVariant && properties.Any())
            {
                var variant = product.Variants.FirstOrDefault(v => v.ProductProperties
                      .Where(pp => pp.ProductDefinitionField.DisplayOnSite
                          && pp.ProductDefinitionField.IsVariantProperty
                          && !pp.ProductDefinitionField.Deleted)
                      .All(p => properties.Any(kv => kv.Key.Equals(p.ProductDefinitionField.Name, StringComparison.InvariantCultureIgnoreCase) && kv.Value.Equals(p.Value, StringComparison.InvariantCultureIgnoreCase))));
                variantSku = variant.VariantSku;
            }
            // Only use the current product where there are no variants
            else if (!product.Variants.Any())
            {
                variantSku = product.Sku;
            }

            return variantSku;
        }

    }
}
