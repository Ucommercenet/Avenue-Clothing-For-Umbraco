using System;
using System.Linq;
using System.Web.Mvc;
using UCommerce;
using Ucommerce.Api.PriceCalculation;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.RazorStore.Models;
using UCommerce.Runtime;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Money = Ucommerce.Api.PriceCalculation.Money;

namespace Ucommerce.Avenue.Umbraco.Controllers
{
    public class EmailController : RenderMvcController
    {
        public IOrderContext OrderContext => ObjectFactory.Instance.Resolve<IOrderContext>();
        
        // GET: uCommerceEmail
        public override ActionResult Index(ContentModel model)
        {

            PurchaseOrder basket = OrderContext.GetBasket(Guid.Parse(Request.QueryString["orderGuid"])).PurchaseOrder;

            var basketModel = new PurchaseOrderViewModel();

            basketModel.BillingAddress = basket.BillingAddress;
            basketModel.ShipmentAddress = basket.GetShippingAddress(UCommerce.Constants.DefaultShipmentAddressName);

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

            return View("/Views/uCommerceEmail.cshtml", basketModel);
        }
    }
}