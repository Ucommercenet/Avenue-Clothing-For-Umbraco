using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ucommerce;
using Ucommerce.Api;
using Ucommerce.Api.PriceCalculation;
using Ucommerce.Infrastructure;
using AvenueClothing.Models;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Money = Ucommerce.Money;

namespace AvenueClothing.Controllers
{
    public class ShippingController : RenderMvcController
    {
        public ITransactionLibrary TransactionLibrary => ObjectFactory.Instance.Resolve<ITransactionLibrary>();

        [HttpGet]
        public override ActionResult Index(ContentModel model)
        {
            var shipping = new ShippingViewModel {AvailableShippingMethods = new List<SelectListItem>() };

            var shippingInformation = TransactionLibrary.GetShippingInformation();

            var availableShippingMethods = TransactionLibrary.GetShippingMethods(shippingInformation.Country);

            var basket = TransactionLibrary.GetBasket();

            shipping.SelectedShippingMethodId = basket.Shipments.FirstOrDefault() != null
                ? basket.Shipments.First().ShippingMethod.ShippingMethodId
                : -1;

            foreach (var availableShippingMethod in availableShippingMethods)
            {
                var price = availableShippingMethod.GetPriceForCurrency(basket.BillingCurrency);
                var formattedPrice = new Money((price == null ? 0 : price.Price), basket.BillingCurrency);

                shipping.AvailableShippingMethods.Add(new SelectListItem()
                {
                    Selected = shipping.SelectedShippingMethodId == availableShippingMethod.ShippingMethodId,
                    Text = $@" {availableShippingMethod.Name} ({formattedPrice})",
                    Value = availableShippingMethod.ShippingMethodId.ToString()
                });
            }

            shipping.ShippingCountry = shippingInformation.Country.Name;

            return base.View("/Views/Shipping.cshtml", shipping);
        }

        [HttpPost]
        public ActionResult Index(ShippingViewModel shipping)
        {
            TransactionLibrary.CreateShipment(shipping.SelectedShippingMethodId,
                Ucommerce.Constants.DefaultShipmentAddressName, overwriteExisting: true);
            TransactionLibrary.ExecuteBasketPipeline();

            var parent = PublishedRequest.PublishedContent.AncestorOrSelf("basket");
            var payment = parent.Children(x => x.Name == "Payment").FirstOrDefault();
            return Redirect(payment.Url);
        }
    }
}