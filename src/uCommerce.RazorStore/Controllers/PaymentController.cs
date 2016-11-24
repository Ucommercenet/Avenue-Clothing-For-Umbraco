using System;
using System.Collections.Generic;
using System.Linq;
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
                decimal feePercent = availablePaymentMethod.FeePercent;
                var fee = availablePaymentMethod.GetFeeForCurrency(basket.BillingCurrency);
                var formattedFee = new Money((fee == null ? 0 : fee.Fee), basket.BillingCurrency);

                option.Text = String.Format(" {0} ({1} + {2}%)", availablePaymentMethod.Name, formattedFee,
                    feePercent.ToString("0.00"));
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

            var shop = payment.Content.AncestorsOrSelf().FirstOrDefault(x => x.DocumentTypeAlias.Equals("home"));
            var basket = shop.DescendantsOrSelf().FirstOrDefault(x => x.DocumentTypeAlias.Equals("basket"));
            var preview = basket.FirstChild(x => x.DocumentTypeAlias.Equals("preview"));
            return Redirect(preview.Url);
        }

	}
}