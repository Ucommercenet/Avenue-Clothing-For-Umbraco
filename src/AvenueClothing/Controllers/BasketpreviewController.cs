﻿using System.Linq;
using System.Net;
using System.Web.Mvc;
using Ucommerce.Api;
using Ucommerce.EntitiesV2;
using Ucommerce.Infrastructure;
using AvenueClothing.Models;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Money = Ucommerce.Money;

namespace AvenueClothing.Controllers
{
    public class BasketpreviewController : RenderMvcController
    {
        public ITransactionLibrary TransactionLibrary => ObjectFactory.Instance.Resolve<ITransactionLibrary>();

        [HttpGet]
        public override ActionResult Index(ContentModel m)
        {
            PurchaseOrderViewModel model = MapOrder();
            return View("/Views/Preview.cshtml", model);
        }

        private PurchaseOrderViewModel MapOrder()
        {
            PurchaseOrder basket = TransactionLibrary.GetBasket(false);

            var basketModel = new PurchaseOrderViewModel();

            basketModel.BillingAddress = TransactionLibrary.GetBillingInformation();
            basketModel.ShipmentAddress = TransactionLibrary.GetShippingInformation();

            foreach (var orderLine in basket.OrderLines)
            {
                var orderLineModel = new OrderlineViewModel();
                orderLineModel.ProductName = orderLine.ProductName;
                orderLineModel.Sku = orderLine.Sku;
                orderLineModel.VariantSku = orderLine.VariantSku;
                orderLineModel.Total =
                    new Money(orderLine.Total.GetValueOrDefault(), orderLine.PurchaseOrder.BillingCurrency.ISOCode).ToString();
                orderLineModel.Tax =
                    new Money(orderLine.VAT, basket.BillingCurrency.ISOCode).ToString();
                orderLineModel.Price =
                    new Money(orderLine.Price, basket.BillingCurrency.ISOCode).ToString();
                orderLineModel.PriceWithDiscount =
                    new Money(orderLine.Price - orderLine.Discount, basket.BillingCurrency.ISOCode).ToString();
                orderLineModel.Quantity = orderLine.Quantity;
                orderLineModel.Discount = orderLine.Discount;

                basketModel.OrderLines.Add(orderLineModel);
            }

            basketModel.DiscountTotal =
                new Money(basket.DiscountTotal.GetValueOrDefault(), basket.BillingCurrency.ISOCode).ToString();
            basketModel.DiscountAmount = basket.DiscountTotal.GetValueOrDefault();
            basketModel.SubTotal = new Money(basket.SubTotal.GetValueOrDefault(), basket.BillingCurrency.ISOCode).ToString();
            basketModel.OrderTotal =
                new Money(basket.OrderTotal.GetValueOrDefault(), basket.BillingCurrency.ISOCode).ToString();
            basketModel.TaxTotal = new Money(basket.TaxTotal.GetValueOrDefault(), basket.BillingCurrency.ISOCode).ToString();
            basketModel.ShippingTotal =
                new Money(basket.ShippingTotal.GetValueOrDefault(), basket.BillingCurrency.ISOCode).ToString();
            basketModel.PaymentTotal =
                new Money(basket.PaymentTotal.GetValueOrDefault(), basket.BillingCurrency.ISOCode).ToString();


            var shipment = basket.Shipments.FirstOrDefault();
            if (shipment != null)
            {
                basketModel.ShipmentName = shipment.ShipmentName;
                basketModel.ShipmentAmount = basket.ShippingTotal.GetValueOrDefault();
            }

            var payment = basket.Payments.FirstOrDefault();
            if (payment != null)
            {
                basketModel.PaymentName = payment.PaymentMethodName;
                basketModel.PaymentAmount = basket.PaymentTotal.GetValueOrDefault();
            }

            ViewBag.RowSpan = 4;
            if (basket.DiscountTotal > 0)
            {
                ViewBag.RowSpan++;
            }

            if (basket.ShippingTotal > 0)
            {
                ViewBag.RowSpan++;
            }

            if (basket.PaymentTotal > 0)
            {
                ViewBag.RowSpan++;
            }

            return basketModel;
        }

        [HttpPost]
        public ActionResult Index()
        {
            var payment = TransactionLibrary.GetBasket().Payments.First();
            if (payment.PaymentMethod.PaymentMethodServiceName == null)
            {
                var parent = PublishedRequest.PublishedContent.AncestorOrSelf("basket");            
                var confirmation = parent.Children(x => x.Name == "Confirmation").FirstOrDefault();            
                return Redirect(confirmation.Url);
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var paymentUrl = TransactionLibrary.GetPaymentPageUrl(payment);
            return Redirect(paymentUrl);
        }
    }
}