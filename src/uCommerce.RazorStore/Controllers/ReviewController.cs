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
using UCommerce.Pipelines;

namespace UCommerce.RazorStore.Controllers
{
    public class ReviewController : SurfaceController
    {
        // GET: Review
        public ActionResult Index(ProductViewModel mappedProduct)
        {
            Product currentProduct = SiteContext.Current.CatalogContext.CurrentProduct;

            if (currentProduct.ProductReviews.Any())
            {
                mappedProduct.Reviews = MapReviews(currentProduct);
            }
            
            return View("/Views/ProductReviews.cshtml");
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
        public ActionResult Index(Product product, string nameKey, string emailKey, string ratingKey, string headlineKey, string reviewTextKey)
        { 

            var request = System.Web.HttpContext.Current.Request;
            var basket = SiteContext.Current.OrderContext.GetBasket();

            if (request.Form.AllKeys.All(x => x != "review-product"))
            {
                return View();
            }

            var name = request.Form[nameKey];
            var email = request.Form[emailKey];
            var rating = Convert.ToInt32(request.Form[ratingKey]) * 20;
            var reviewHeadline = request.Form[headlineKey];
            var reviewText = request.Form[reviewTextKey];

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

            var review = new UCommerce.EntitiesV2.ProductReview()
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

            return View();
        }
    }
}