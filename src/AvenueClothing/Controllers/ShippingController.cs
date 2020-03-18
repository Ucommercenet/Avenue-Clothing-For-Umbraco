using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UCommerce;
using Ucommerce.Api;
using Ucommerce.Api.PriceCalculation;
using UCommerce.Infrastructure;
using UCommerce.RazorStore.Models;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Money = Ucommerce.Api.PriceCalculation.Money;

namespace Ucommerce.Avenue.Umbraco.Controllers
{
    public class ShippingController : RenderMvcController
    {
        public ITransactionLibrary TransactionLibrary => ObjectFactory.Instance.Resolve<ITransactionLibrary>();

        [HttpGet]
        public override ActionResult Index(ContentModel model)
        {
            var shipping = new ShippingViewModel();
            shipping.AvailableShippingMethods = new List<SelectListItem>();

            var shippingInformation = TransactionLibrary.GetShippingInformation();

            var availableShippingMethods = TransactionLibrary.GetShippingMethods(shippingInformation.Country);

            var basket = TransactionLibrary.GetBasket();

            shipping.SelectedShippingMethodId = basket.Shipments.FirstOrDefault() != null
                ? basket.Shipments.FirstOrDefault().ShippingMethod.ShippingMethodId
                : -1;

            foreach (var availableShippingMethod in availableShippingMethods)
            {
                var price = availableShippingMethod.GetPriceForCurrency(basket.BillingCurrency);
                var formattedprice = new Money((price == null ? 0 : price.Price), basket.BillingCurrency.ISOCode);

                shipping.AvailableShippingMethods.Add(new SelectListItem()
                {
                    Selected = shipping.SelectedShippingMethodId == availableShippingMethod.ShippingMethodId,
                    Text = String.Format(" {0} ({1})", availableShippingMethod.Name, formattedprice),
                    Value = availableShippingMethod.ShippingMethodId.ToString()
                });
            }

            shipping.ShippingCountry = shippingInformation.Country.Name;

            return base.View("/Views/Shipping.cshtml", shipping);
        }

        [HttpPost]
        public ActionResult Index(ShippingViewModel shipping)
        {
            TransactionLibrary.CreateShipment(shipping.SelectedShippingMethodId, UCommerce.Constants.DefaultShipmentAddressName, overwriteExisting: true);
            TransactionLibrary.ExecuteBasketPipeline();

            var parent = PublishedRequest.PublishedContent.AncestorOrSelf("basket");
            var payment = parent.Children(x => x.Name == "Payment").FirstOrDefault();
            return Redirect(payment.Url);
        }
    }
}