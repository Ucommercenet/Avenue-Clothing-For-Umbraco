using System.Globalization;
using System.Web.Mvc;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.RazorStore.Models;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
namespace UCommerce.RazorStore.Controllers
{
	public class BasketController : RenderMvcController
    {
		public ActionResult Index(RenderModel model)
		{
			PurchaseOrder basket = UCommerce.Api.TransactionLibrary.GetBasket(false).PurchaseOrder;
			var basketModel = new PurchaseOrderViewModel();

			foreach (var orderLine in basket.OrderLines)
			{
				var orderLineViewModel = new OrderlineViewModel();

				orderLineViewModel.Quantity = orderLine.Quantity;
				orderLineViewModel.ProductName = orderLine.ProductName;
				orderLineViewModel.Sku = orderLine.Sku;
				orderLineViewModel.VariantSku = orderLine.VariantSku;
				orderLineViewModel.Total = new Money(orderLine.Total.GetValueOrDefault(), basket.BillingCurrency).ToString();
			    orderLineViewModel.Discount = orderLine.Discount;
			    orderLineViewModel.Tax = new Money(orderLine.VAT, basket.BillingCurrency).ToString();
			    orderLineViewModel.Price = new Money(orderLine.Price, basket.BillingCurrency).ToString();
			    orderLineViewModel.ProductUrl = CatalogLibrary.GetNiceUrlForProduct(CatalogLibrary.GetProduct(orderLine.Sku));
			    orderLineViewModel.PriceWithDiscount = new Money(orderLine.Price - orderLine.Discount, basket.BillingCurrency).ToString();
                orderLineViewModel.OrderLineId = orderLine.OrderLineId;

            	basketModel.OrderLines.Add(orderLineViewModel);
			}

            basketModel.OrderTotal = new Money(basket.OrderTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            basketModel.DiscountTotal = new Money(basket.DiscountTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            basketModel.TaxTotal =  new Money(basket.TaxTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            basketModel.SubTotal = new Money(basket.SubTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
		   
		    return base.View("/Views/Basket.cshtml", basketModel);
		}

        [HttpPost]
        public ActionResult Index(PurchaseOrderViewModel model)
        {
            foreach (var orderLine in model.OrderLines)
            {
                var newQuantity = orderLine.Quantity;

                if (model.RemoveOrderlineId == orderLine.OrderLineId)
                    newQuantity = 0;

                TransactionLibrary.UpdateLineItem(orderLine.OrderLineId, newQuantity);
            }

            TransactionLibrary.ExecuteBasketPipeline();

            return Redirect("/basket/address");
        }
    }

}