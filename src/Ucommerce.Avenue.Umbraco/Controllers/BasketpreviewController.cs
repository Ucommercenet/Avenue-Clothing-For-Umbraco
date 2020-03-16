using System.Linq;
using System.Web.Mvc;
using UCommerce;
using Ucommerce.Api;
using Ucommerce.Api.PriceCalculation;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.RazorStore.Models;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace Ucommerce.Avenue.Umbraco.Controllers
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
                    new ApiMoney(orderLine.Total.GetValueOrDefault(), orderLine.PurchaseOrder.BillingCurrency.ISOCode).ToString();
                orderLineModel.Tax =
                    new ApiMoney(orderLine.VAT, basket.BillingCurrency.ISOCode).ToString();
                orderLineModel.Price =
                    new ApiMoney(orderLine.Price, basket.BillingCurrency.ISOCode).ToString();
                orderLineModel.PriceWithDiscount =
                    new ApiMoney(orderLine.Price - orderLine.Discount, basket.BillingCurrency.ISOCode).ToString();
                orderLineModel.Quantity = orderLine.Quantity;
                orderLineModel.Discount = orderLine.Discount;

                basketModel.OrderLines.Add(orderLineModel);
            }

            basketModel.DiscountTotal =
                new ApiMoney(basket.DiscountTotal.GetValueOrDefault(), basket.BillingCurrency.ISOCode).ToString();
            basketModel.DiscountAmount = basket.DiscountTotal.GetValueOrDefault();
            basketModel.SubTotal = new ApiMoney(basket.SubTotal.GetValueOrDefault(), basket.BillingCurrency.ISOCode).ToString();
            basketModel.OrderTotal =
                new ApiMoney(basket.OrderTotal.GetValueOrDefault(), basket.BillingCurrency.ISOCode).ToString();
            basketModel.TaxTotal = new ApiMoney(basket.TaxTotal.GetValueOrDefault(), basket.BillingCurrency.ISOCode).ToString();
            basketModel.ShippingTotal =
                new ApiMoney(basket.ShippingTotal.GetValueOrDefault(), basket.BillingCurrency.ISOCode).ToString();
            basketModel.PaymentTotal =
                new ApiMoney(basket.PaymentTotal.GetValueOrDefault(), basket.BillingCurrency.ISOCode).ToString();


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
            TransactionLibrary.RequestPayments();

            var parent = PublishedRequest.PublishedContent.AncestorOrSelf("basket");
            var confirmation = parent.Children(x => x.Name == "Confirmation").FirstOrDefault();
            return Redirect(confirmation.Url);
        }
    }
}