using System;
using System.Linq;
using System.Web.Mvc;
using Ucommerce.Api;
using Ucommerce.EntitiesV2;
using Ucommerce.Infrastructure;
using AvenueClothing.Models;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Money = Ucommerce.Api.PriceCalculation.Money;

namespace AvenueClothing.Controllers
{
    public class EmailController : RenderMvcController
    {
        public IOrderContext OrderContext => ObjectFactory.Instance.Resolve<IOrderContext>();
        
        // GET: UcommerceEmail
        public override ActionResult Index(ContentModel model)
        {
            PurchaseOrder basket = OrderContext.GetBasket(Guid.Parse(Request.QueryString["orderGuid"])).PurchaseOrder;

            var basketModel = new PurchaseOrderViewModel
            {
                BillingAddress = basket.BillingAddress,
                ShipmentAddress = basket.GetShippingAddress(Ucommerce.Constants.DefaultShipmentAddressName)
            };

            foreach (var orderLine in basket.OrderLines)
            {
                var orderLineModel = new OrderlineViewModel
                {
                    ProductName = orderLine.ProductName,
                    Sku = orderLine.Sku,
                    VariantSku = orderLine.VariantSku,
                    Total = new Money(orderLine.Total.GetValueOrDefault(),
                        orderLine.PurchaseOrder.BillingCurrency.ISOCode).ToString(),
                    Tax = new Money(orderLine.VAT, basket.BillingCurrency.ISOCode).ToString(),
                    Price = new Money(orderLine.Price, basket.BillingCurrency.ISOCode).ToString(),
                    PriceWithDiscount = new Money(orderLine.Price - orderLine.Discount, basket.BillingCurrency.ISOCode)
                        .ToString(),
                    Quantity = orderLine.Quantity,
                    Discount = orderLine.Discount
                };

                basketModel.OrderLines.Add(orderLineModel);
            }

            basketModel.DiscountTotal = new Money(basket.DiscountTotal.GetValueOrDefault(), basket.BillingCurrency.ISOCode).ToString();
            basketModel.DiscountAmount = basket.DiscountTotal.GetValueOrDefault();
            basketModel.SubTotal = new Money(basket.SubTotal.GetValueOrDefault(), basket.BillingCurrency.ISOCode).ToString();
            basketModel.OrderTotal = new Money(basket.OrderTotal.GetValueOrDefault(), basket.BillingCurrency.ISOCode).ToString();
            basketModel.TaxTotal = new Money(basket.TaxTotal.GetValueOrDefault(), basket.BillingCurrency.ISOCode).ToString();
            basketModel.ShippingTotal = new Money(basket.ShippingTotal.GetValueOrDefault(), basket.BillingCurrency.ISOCode).ToString();
            basketModel.PaymentTotal = new Money(basket.PaymentTotal.GetValueOrDefault(), basket.BillingCurrency.ISOCode).ToString();


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

            return View("/Views/UcommerceEmail.cshtml", basketModel);
        }
    }
}
