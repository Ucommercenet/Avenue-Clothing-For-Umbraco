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
	public class PaymentController : RenderMvcController
    {
		public ActionResult Index()
		{
			var paymentViewModel = new PaymentViewModel();
			paymentViewModel.AvailablePaymentMethods = new List<SelectListItem>();

			PurchaseOrder basket = TransactionLibrary.GetBasket(false).PurchaseOrder;

			Country shippingCountry = TransactionLibrary.GetShippingInformation().Country;

			var availablePaymentMethods = TransactionLibrary.GetPaymentMethods(shippingCountry);

			var existingPayment = basket.Payments.FirstOrDefault();

			paymentViewModel.SelectedPaymentMethodId = existingPayment != null
				? existingPayment.PaymentMethod.PaymentMethodId
				: -1;

			foreach (var availablePaymentMethod in availablePaymentMethods)
			{
				var option = new SelectListItem();
				option.Text = availablePaymentMethod.Name;
				option.Value = availablePaymentMethod.PaymentMethodId.ToString();
				option.Selected = availablePaymentMethod.PaymentMethodId == paymentViewModel.SelectedPaymentMethodId;

				paymentViewModel.AvailablePaymentMethods.Add(option);
			}

			return View("/Views/Payment.cshtml", paymentViewModel);
		}

		[HttpPost]
		public ActionResult Index(PaymentViewModel payment)
		{
			TransactionLibrary.CreatePayment(
				paymentMethodId: payment.SelectedPaymentMethodId, 
				requestPayment: false, 
				amount: -1, 
				overwriteExisting: true);

			TransactionLibrary.ExecuteBasketPipeline();

			return Redirect("/store/checkout/preview");
		}

	}
}