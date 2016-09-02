using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.RazorStore.Models;
using UCommerce.Runtime;
using Umbraco.Web.Mvc;
using UCommerce.Pipelines;

namespace UCommerce.RazorStore.Controllers
{
    public class ReviewController : SurfaceController
    {
        // GET: Review
        [HttpGet]
        public ActionResult Index()
        {
            Product currentProduct = SiteContext.Current.CatalogContext.CurrentProduct;
            var mappedProduct = new ProductViewModel();

            if (currentProduct.ProductReviews.Any())
            {
                mappedProduct.Sku = currentProduct.Sku;
                mappedProduct.Reviews = MapReviews(currentProduct);
            }

            return View("/Views/PartialView/ProductReview.cshtml", mappedProduct);
        }

        private IList<ProductReviewViewModel> MapReviews(Product product)
        {
            var reviews = new List<ProductReviewViewModel>();
            foreach (var review in product.ProductReviews)
            {
                ProductReviewViewModel reviewModel = new ProductReviewViewModel();
                reviewModel.Name = review.Customer.FirstName + " " + review.Customer.LastName;
                reviewModel.Email = review.Customer.EmailAddress;
                reviewModel.Title = review.ReviewHeadline;
                reviewModel.Comments = review.ReviewText;
                reviewModel.Rating = review.Rating;

                reviews.Add(reviewModel);
            }

            return reviews;
        }


        [HttpPost]
        public ActionResult Index(ProductReviewViewModel formReview)
        {

            var product = SiteContext.Current.CatalogContext.CurrentProduct;
            var category = SiteContext.Current.CatalogContext.CurrentCategory;

            var request = System.Web.HttpContext.Current.Request;
            var basket = SiteContext.Current.OrderContext.GetBasket();

            if (request.Form.AllKeys.All(x => x != "review-product"))
            {
                return View();
            }

            var name = formReview.Name;
            var email = formReview.Email;
            var rating = Convert.ToInt32(formReview.Rating) * 20;
            var reviewHeadline = formReview.Title;
            var reviewText = formReview.Comments;

            if (basket.PurchaseOrder.Customer == null)
            {
                basket.PurchaseOrder.Customer = new Customer() { FirstName = name, LastName = String.Empty, EmailAddress = email };
            }
            else
            {
                basket.PurchaseOrder.Customer.FirstName = name;
                if (basket.PurchaseOrder.Customer.LastName == null)
                {
                    basket.PurchaseOrder.Customer.LastName = String.Empty;
                }
                basket.PurchaseOrder.Customer.EmailAddress = email;
            }

            basket.PurchaseOrder.Customer.Save();

            var review = new ProductReview()
            {
                ProductCatalogGroup = SiteContext.Current.CatalogContext.CurrentCatalogGroup,
                ProductReviewStatus = ProductReviewStatus.SingleOrDefault(s => s.Name == "New"),
                CreatedOn = DateTime.Now,
                CreatedBy = "System",
                Product = product,
                Customer = basket.PurchaseOrder.Customer,
                Rating = rating,
                ReviewHeadline = reviewHeadline,
                ReviewText = reviewText,
                Ip = request.UserHostName
            };

            product.AddProductReview(review);

            PipelineFactory.Create<ProductReview>("ProductReview").Execute(review);

            return Redirect(CatalogLibrary.GetNiceUrlForProduct(product, category));

        }
    }
}