using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.MasterClass.Website.Models;
using Umbraco.Web.Mvc;
namespace UCommerce.MasterClass.Website.Controllers
{
	public class BasketController : RenderMvcController
    {
		public ActionResult Index()
		{
			PurchaseOrder basket = UCommerce.Api.TransactionLibrary.GetBasket(false).PurchaseOrder;
			
			var basketModel = new PurchaseOrderViewModel();

			basketModel.OrderTotal = new Money(basket.OrderTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();

			foreach (var orderLine in basket.OrderLines)
			{
				var orderLineViewModel = new OrderlineViewModel();
				orderLineViewModel.Quantity = orderLine.Quantity;
				orderLineViewModel.ProductName = orderLine.ProductName;
				orderLineViewModel.Sku = orderLine.Sku;
				orderLineViewModel.VariantSku = orderLine.VariantSku;
				orderLineViewModel.Total = new Money(orderLine.Total.GetValueOrDefault(), basket.BillingCurrency).ToString();

				orderLineViewModel.OrderLineId = orderLine.OrderLineId;

				basketModel.OrderLines.Add(orderLineViewModel);
			}

			return View("/Views/Basket.cshtml", basketModel);
		}

		[HttpPost]
		public ActionResult Index(PurchaseOrderViewModel model)
		{
			foreach (var orderLine in model.OrderLines)
			{
				var newQuantity = orderLine.Quantity;
				
				if (model.RemoveOrderlineId == orderLine.OrderLineId)
					newQuantity = 0;

				UCommerce.Api.TransactionLibrary.UpdateLineItem(orderLine.OrderLineId, newQuantity);
			}

			UCommerce.Api.TransactionLibrary.ExecuteBasketPipeline();
			
			return RedirectToAction("Index");
		}
	}
}