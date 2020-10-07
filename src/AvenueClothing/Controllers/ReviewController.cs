﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ucommerce.Api;
using Ucommerce.Catalog.Status;
using Ucommerce.EntitiesV2;
using Ucommerce.Infrastructure;
using Ucommerce.Pipelines;
using AvenueClothing.Models;
using Ucommerce.Search.Slugs;
using Umbraco.Web.Mvc;
using ICatalogContext = Ucommerce.Api.ICatalogContext;

namespace AvenueClothing.Controllers
{
	public class ReviewController : SurfaceController
	{
		private IUrlService UrlService => ObjectFactory.Instance.Resolve<IUrlService>();
		private ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
		private IOrderContext OrderContext => ObjectFactory.Instance.Resolve<IOrderContext>();
		private ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();

		// GET: Review
		[HttpGet]
		public ActionResult Index()
		{
			var currentProduct = CatalogContext.CurrentProduct;
			var currentProductV2 = Product.FirstOrDefault(x => x.Sku == currentProduct.Sku);
			var mappedProduct = new ProductViewModel();

			if (currentProductV2.ProductReviews.Any(r => r.ProductReviewStatus.ProductReviewStatusId == (int)ProductReviewStatusCode.Approved))
			{
				mappedProduct.Reviews = MapReviews(currentProductV2);
			}

			mappedProduct.Sku = currentProduct.Sku;
			return View("/Views/PartialView/ProductReview.cshtml", mappedProduct);
		}

		private IList<ProductReviewViewModel> MapReviews(Product product)
		{
			var reviews = new List<ProductReviewViewModel>();
			foreach (var review in product.ProductReviews.Where(r => r.ProductReviewStatus.ProductReviewStatusId == (int)ProductReviewStatusCode.Approved))
			{
				var reviewModel = new ProductReviewViewModel
				{
					Name = review.Customer.FirstName + " " + review.Customer.LastName,
					Email = review.Customer.EmailAddress,
					Title = review.ReviewHeadline,
					CreatedOn = review.CreatedOn,
					Comments = review.ReviewText,
					Rating = review.Rating
				};

				reviews.Add(reviewModel);
			}
			return reviews;
		}


		[HttpPost]
		public ActionResult Index(ProductReviewViewModel formReview)
		{
			var product = CatalogLibrary.GetProduct(formReview.ProductSku);
			var productV2 = Product.FirstOrDefault(x => x.Guid == product.Guid);

			var catalogGroup = CatalogContext.CurrentCatalogGroup;
			var catalogGroupV2 = ProductCatalogGroup.FirstOrDefault(x => x.Guid == catalogGroup.Guid);

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
				{ FirstName = name, LastName = String.Empty, EmailAddress = email };
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
				ProductCatalogGroup = catalogGroupV2,
				ProductReviewStatus = ProductReviewStatus.SingleOrDefault(s => s.Name == "New"),
				CreatedOn = DateTime.Now,
				CreatedBy = "System",
				Product = productV2,
				Customer = basket.PurchaseOrder.Customer,
				Rating = rating,
				ReviewHeadline = reviewHeadline,
				ReviewText = reviewText,
				Ip = request.UserHostName
			};
			productV2.AddProductReview(review);

			PipelineFactory.Create<ProductReview>("ProductReview").Execute(review);

			//TODO: Add Category to this url.
			return Redirect(UrlService.GetUrl(CatalogContext.CurrentCatalog, product));
		}
	}
}
