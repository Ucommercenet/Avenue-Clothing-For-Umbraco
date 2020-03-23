using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
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
    public class PaymentController : RenderMvcController
    {
        public ITransactionLibrary TransactionLibrary => ObjectFactory.Instance.Resolve<ITransactionLibrary>();

        [HttpGet]
        public override ActionResult Index(ContentModel model)
        {
            var paymentViewModel = new PaymentViewModel();
            paymentViewModel.AvailablePaymentMethods = new List<SelectListItem>();

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
                var formattedFee = new Money((fee == null ? 0 : fee.Fee), basket.BillingCurrency.ISOCode);

                option.Text = String.Format(" {0} ({1} + {2}%)", availablePaymentMethod.Name, formattedFee,
                    feePercent.ToString("0.00"));
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