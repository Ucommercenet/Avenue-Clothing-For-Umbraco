using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.RazorStore.Models;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace UCommerce.RazorStore.Controllers
{
	public class PaymentController : RenderMvcController
    {
		public ActionResult Index(RenderModel model)
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
                var payment = basket.Payments.FirstOrDefault();
                decimal feePercent = availablePaymentMethod.FeePercent;
                var fee = availablePaymentMethod.GetFeeForCurrency(basket.BillingCurrency);
                var formattedFee = new Money((fee == null ? 0 : fee.Fee), basket.BillingCurrency);

                option.Text = availablePaymentMethod.Name + " (" + formattedFee + " + " + feePercent.ToString("0.00") + "%)";
                option.Value = availablePaymentMethod.PaymentMethodId.ToString();
                option.Selected = availablePaymentMethod.PaymentMethodId == paymentViewModel.SelectedPaymentMethodId;

                paymentViewModel.AvailablePaymentMethods.Add(option);
            }

		    paymentViewModel.ShippingCountry = shippingCountry.Name;

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

			return Redirect("/basket/preview");
		}

	}
}