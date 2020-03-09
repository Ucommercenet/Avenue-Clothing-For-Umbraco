using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ucommerce.Api;
using UCommerce.Catalog.Status;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.RazorStore.Models;
using UCommerce.Runtime;
using UCommerce.Search;
using Umbraco.Web.Mvc;
using ICatalogContext = Ucommerce.Api.ICatalogContext;

namespace Ucommerce.Avenue.Umbraco.Controllers
{
    public class ReviewController : SurfaceController
    {
        public IUrlService UrlService => ObjectFactory.Instance.Resolve<IUrlService>();
        public ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();
        public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
        public IOrderContext OrderContext => ObjectFactory.Instance.Resolve<IOrderContext>();

        // GET: Review
        [HttpGet]
        public ActionResult Index()
        {
            UCommerce.Search.Models.Product currentProduct = CatalogContext.CurrentProduct;
            var mappedProduct = new ProductViewModel();

            // TODO: re-implement after reviews are added
            // if (currentProduct.ProductReviews.Any())
            // {
            //     mappedProduct.Sku = currentProduct.Sku;
            //     mappedProduct.Reviews = MapReviews(currentProduct);
            // }

            return View("/Views/PartialView/ProductReview.cshtml", mappedProduct);
        }

        private IList<ProductReviewViewModel> MapReviews(Product product)
        {
            var reviews = new List<ProductReviewViewModel>();
            foreach (var review in product.ProductReviews)
            {
                if (review.ProductReviewStatus.ProductReviewStatusId != (int) ProductReviewStatusCode.Approved)
                {
                    ProductReviewViewModel reviewModel = new ProductReviewViewModel();
                    reviewModel.Name = review.Customer.FirstName + " " + review.Customer.LastName;
                    reviewModel.Email = review.Customer.EmailAddress;
                    reviewModel.Title = review.ReviewHeadline;
                    reviewModel.CreatedOn = review.CreatedOn;
                    reviewModel.Comments = review.ReviewText;
                    reviewModel.Rating = review.Rating;

                    reviews.Add(reviewModel);
                }
            }

            return reviews;
        }


        [HttpPost]
        public ActionResult Index(ProductReviewViewModel formReview)
        {
            var product = CatalogContext.CurrentProduct;
            var category = CatalogContext.CurrentCategory;

            var request = System.Web.HttpContext.Current.Request;
            var basket = OrderContext.GetBasket();

            if (request.Form.AllKeys.All(x => x != "review-product"))
            {
                return RedirectToAction("Index");
            }

            var name = formReview.Name;
            var email = formReview.Email;
            var rating = Convert.ToInt32(formReview.Rating) * 20;
            var reviewHeadline = formReview.Title;
            var reviewText = formReview.Comments;

            if (basket.PurchaseOrder.Customer == null)
            {
                basket.PurchaseOrder.Customer = new Customer()
                    {FirstName = name, LastName = String.Empty, EmailAddress = email};
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

            // TODO: Implement using 
            // var review = new ProductReview()
            // {
            //     ProductCatalogGroup = CatalogContext.CurrentCatalogGroup,
            //     ProductReviewStatus = ProductReviewStatus.SingleOrDefault(s => s.Name == "New"),
            //     CreatedOn = DateTime.Now,
            //     CreatedBy = "System",
            //     Product = product,
            //     Customer = basket.PurchaseOrder.Customer,
            //     Rating = rating,
            //     ReviewHeadline = reviewHeadline,
            //     ReviewText = reviewText,
            //     Ip = request.UserHostName
            // };
            //
            // product.AddProductReview(review);

            // PipelineFactory.Create<ProductReview>("ProductReview").Execute(review);

            return Redirect(UrlService.GetUrl(CatalogContext.CurrentCatalog, new []{category}, new []{product}));
        }
    }
}