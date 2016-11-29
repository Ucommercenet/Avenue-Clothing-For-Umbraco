using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UCommerce.Api;
using UCommerce.RazorStore.Models;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace UCommerce.RazorStore.Controllers
{
	public class ShippingController : RenderMvcController
    {
		public ActionResult Index(RenderModel model)
		{
			var shipping = new ShippingViewModel();
			shipping.AvailableShippingMethods = new List<SelectListItem>();

			var shippingInformation = TransactionLibrary.GetShippingInformation();

			var availableShippingMethods = TransactionLibrary.GetShippingMethods(shippingInformation.Country);

            var basket = TransactionLibrary.GetBasket().PurchaseOrder;

            shipping.SelectedShippingMethodId = basket.Shipments.FirstOrDefault() != null
                ? basket.Shipments.FirstOrDefault().ShippingMethod.ShippingMethodId : -1;

			foreach (var availableShippingMethod in availableShippingMethods)
			{
			    var price = availableShippingMethod.GetPriceForCurrency(basket.BillingCurrency);
                var formattedprice = new Money((price == null ? 0 : price.Price), basket.BillingCurrency);

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
			TransactionLibrary.CreateShipment(shipping.SelectedShippingMethodId, overwriteExisting: true);
			TransactionLibrary.ExecuteBasketPipeline();

            var shop = shipping.Content.AncestorsOrSelf().FirstOrDefault(x => x.DocumentTypeAlias.Equals("home"));
            var basket = shop.DescendantsOrSelf().FirstOrDefault(x => x.DocumentTypeAlias.Equals("basket"));
            var payment = basket.FirstChild(x => x.DocumentTypeAlias.Equals("payment"));
            return Redirect(payment.Url);
        }
	}
}