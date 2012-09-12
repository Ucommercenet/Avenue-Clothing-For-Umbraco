using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;
using UCommerce.Api;
using uCommerce.RazorStore.ServiceStack.Model;

namespace uCommerce.RazorStore.ServiceStack.Commands
{
    public class GetBasket
    {
    }
    public class GetBasketResponse : IHasResponseStatus
    {
        public GetBasketResponse(UCommerce.EntitiesV2.Basket basket)
        {
            Basket = new Basket();
            Basket.OrderTotal = basket.PurchaseOrder.OrderTotal;
            Basket.SubTotal = basket.PurchaseOrder.SubTotal;
            Basket.LineItems = new List<LineItem>();
            foreach (var orderLine in basket.PurchaseOrder.OrderLines)
            {
                var lineItem = new LineItem();
                lineItem.OrderLineId = orderLine.OrderLineId;
                lineItem.Quantity = orderLine.Quantity;
                lineItem.Sku = orderLine.Sku;
                lineItem.VariantSku = orderLine.VariantSku;
                lineItem.Price = orderLine.Price;
                lineItem.ProductName = orderLine.ProductName;
                lineItem.Total = orderLine.Total;
                lineItem.UnitDiscount = orderLine.UnitDiscount;
                lineItem.VAT = orderLine.VAT;
                lineItem.VATRate = orderLine.VATRate;
                Basket.LineItems.Add(lineItem);
            }
        }

        public ResponseStatus ResponseStatus { get; set; }
        public Basket Basket { get; set; }
    }
    public class GetBasketService : ServiceBase<GetBasket>
    {
        protected override object Run(GetBasket request)
        {
            var basket = TransactionLibrary.GetBasket(false);
            return new GetBasketResponse(basket);
        }

    }
}