using System.Linq;
using System.Web.Http;
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
        [System.Web.Mvc.HttpGet]
		public override ActionResult Index(RenderModel model)
		{
			PurchaseOrder basket = TransactionLibrary.GetBasket().PurchaseOrder;
			var basketModel = new PurchaseOrderViewModel();

			foreach (var orderLine in basket.OrderLines)
			{
			    var orderLineViewModel = new OrderlineViewModel
			    {
			        Quantity = orderLine.Quantity,
			        ProductName = orderLine.ProductName,
			        Sku = orderLine.Sku,
			        VariantSku = orderLine.VariantSku,
			        Total = new Money(orderLine.Total.GetValueOrDefault(), basket.BillingCurrency).ToString(),
			        Discount = orderLine.Discount,
			        Tax = new Money(orderLine.VAT, basket.BillingCurrency).ToString(),
			        Price = new Money(orderLine.Price, basket.BillingCurrency).ToString(),
			        ProductUrl = CatalogLibrary.GetNiceUrlForProduct(CatalogLibrary.GetProduct(orderLine.Sku)),
			        PriceWithDiscount = new Money(orderLine.Price - orderLine.Discount, basket.BillingCurrency).ToString(),
			        OrderLineId = orderLine.OrderLineId
			    };


			    basketModel.OrderLines.Add(orderLineViewModel);
			}

            basketModel.OrderTotal = new Money(basket.OrderTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            basketModel.DiscountTotal = new Money(basket.DiscountTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            basketModel.TaxTotal =  new Money(basket.TaxTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
            basketModel.SubTotal = new Money(basket.SubTotal.GetValueOrDefault(), basket.BillingCurrency).ToString();
		   
		    return View("/Views/Basket.cshtml", basketModel);
		}

        [System.Web.Mvc.HttpPost]
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

            var shop = model.Content.AncestorsOrSelf().FirstOrDefault(x => x.DocumentTypeAlias.Equals("home"));
            var basket = shop.DescendantsOrSelf().FirstOrDefault(x => x.DocumentTypeAlias.Equals("basket"));
            return Redirect(basket.Url);
        }
    }

}