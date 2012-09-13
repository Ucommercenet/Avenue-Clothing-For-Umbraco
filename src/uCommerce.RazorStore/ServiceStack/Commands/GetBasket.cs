using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;
using UCommerce.Api;
using uCommerce.RazorStore.ServiceStack.Model;

namespace uCommerce.RazorStore.ServiceStack.Commands
{
    using System.Linq;

    using UCommerce.Runtime;

    public class GetBasket
    {
    }
    public class GetBasketResponse : IHasResponseStatus
    {
        public GetBasketResponse(UCommerce.EntitiesV2.Basket basket)
        {
            var currencySymbol = SiteContext.Current.CatalogContext.CurrentCatalog.PriceGroup.Currency.Name;

            Basket = new Basket
                {
                    OrderTotal = basket.PurchaseOrder.OrderTotal,
                    TotalItems = basket.PurchaseOrder.OrderLines.Sum(l => l.Quantity),
                    SubTotal = basket.PurchaseOrder.SubTotal,
                    FormattedOrderTotal = currencySymbol + basket.PurchaseOrder.OrderTotal.Value.ToString("#,##.00"),
                    FormattedTotalItems = basket.PurchaseOrder.OrderLines.Sum(l => l.Quantity).ToString("#,##"),
                    FormattedSubTotal = currencySymbol + basket.PurchaseOrder.SubTotal.Value.ToString("#,##.00"),
                    LineItems = new List<LineItem>()
                };

            foreach (var orderLine in basket.PurchaseOrder.OrderLines)
            {
                var lineItem = new LineItem
                    {
                        OrderLineId = orderLine.OrderLineId,
                        Quantity = orderLine.Quantity,
                        Sku = orderLine.Sku,
                        VariantSku = orderLine.VariantSku,
                        Price = orderLine.Price,
                        ProductName = orderLine.ProductName,
                        Total = orderLine.Total,
                        UnitDiscount = orderLine.UnitDiscount,
                        VAT = orderLine.VAT,
                        VATRate = orderLine.VATRate
                    };
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