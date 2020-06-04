using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ucommerce.Api;
using Ucommerce.EntitiesV2;
using Ucommerce.Infrastructure;
using AvenueClothing.Models;
using Ucommerce;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace AvenueClothing.Controllers
{
    public class PaymentController : RenderMvcController
    {
        public ITransactionLibrary TransactionLibrary => ObjectFactory.Instance.Resolve<ITransactionLibrary>();

        [HttpGet]
        public override ActionResult Index(ContentModel model)
        {
            var paymentViewModel = new PaymentViewModel {AvailablePaymentMethods = new List<SelectListItem>() };

            PurchaseOrder basket = TransactionLibrary.GetBasket(false);

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

                option.Text = $@" {availablePaymentMethod.Name} ({formattedFee} + {feePercent:0.00}%)";
                option.Value = availablePaymentMethod.PaymentMethodId.ToString();
                option.Selected = availablePaymentMethod.PaymentMethodId == paymentViewModel.SelectedPaymentMethodId;

                paymentViewModel.AvailablePaymentMethods.Add(option);
            }

            if (paymentViewModel.AvailablePaymentMethods.Any() &&
                paymentViewModel.AvailablePaymentMethods.All(x => !x.Selected))
            {
                // Always make sure, that one payment method is selected.
                paymentViewModel.AvailablePaymentMethods.First().Selected = true;
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

            var parent = PublishedRequest.PublishedContent.AncestorOrSelf("basket");
            var preview = parent.Children(x => x.Name == "Preview").FirstOrDefault();
            return Redirect(preview.Url);
        }
    }
}