using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.RazorStore.Models;
using Umbraco.Web.Mvc;

namespace UCommerce.MasterClass.Website.Controllers
{
	public class PreviewController : RenderMvcController
    {
		public ActionResult Index()
		{
			PurchaseOrderViewModel model = MapOrder();
			return View("/Views/preview.cshtml", model);
		}

		private PurchaseOrderViewModel MapOrder()
		{
			PurchaseOrder basket = TransactionLibrary.GetBasket(false).PurchaseOrder;

			PurchaseOrder order = TransactionLibrary.GetPurchaseOrder(Guid.Parse(Request.QueryString["OrderGuid"]));

			var basketModel = new PurchaseOrderViewModel();

			foreach (var orderLine in basket.OrderLines)
			{
				var orderLineModel = new OrderlineViewModel();
				orderLineModel.ProductName = orderLine.ProductName;
				orderLineModel.Sku = orderLine.Sku;
				orderLineModel.VariantSku = orderLine.VariantSku;
				orderLineModel.Total =
					new Money(orderLine.Total.GetValueOrDefault(), orderLine.PurchaseOrder.BillingCurrency).ToString();

				orderLineModel.Quantity = orderLine.Quantity;
				
				basketModel.OrderLines.Add(orderLineModel);
			}

			basketModel.DiscountTotal = new Money(basket.DiscountTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
			basketModel.SubTotal = new Money(basket.SubTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
			basketModel.OrderTotal = new Money(basket.OrderTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
			basketModel.TaxTotal = new Money(basket.TaxTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
			basketModel.ShippingTotal = new Money(basket.ShippingTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
			basketModel.PaymentTotal = new Money(basket.PaymentTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();

			return basketModel;
		}
		
		[HttpPost]
		public ActionResult Post()
		{
			TransactionLibrary.RequestPayments();

			return View("/Views/Complete.cshtml");
		}
	}
}