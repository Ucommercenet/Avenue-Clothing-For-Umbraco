using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UCommerce.Api;
using UCommerce.RazorStore.Models;
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

			var shippingInformation = UCommerce.Api.TransactionLibrary.GetShippingInformation();

			var availableShippingMethods = TransactionLibrary.GetShippingMethods(shippingInformation.Country);

			var selectedShippingMethod = TransactionLibrary.GetShippingMethod();

			int selectedShippingMethodId = -1;
			if (selectedShippingMethod != null)
			{
				selectedShippingMethodId = selectedShippingMethod.ShippingMethodId;
			}

		    var basket = TransactionLibrary.GetBasket().PurchaseOrder;

			foreach (var availableShippingMethod in availableShippingMethods)
			{
			    var price = availableShippingMethod.GetPriceForCurrency(basket.BillingCurrency);
                var formattedprice = new Money((price == null ? 0 : price.Price), basket.BillingCurrency);

				shipping.AvailableShippingMethods.Add(new SelectListItem()
				{
					Selected = selectedShippingMethodId == availableShippingMethod.ShippingMethodId,
					Text = availableShippingMethod.Name + "(" + formattedprice + ")",
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
			
			return Redirect("/basket/payment");
		}
	}
}