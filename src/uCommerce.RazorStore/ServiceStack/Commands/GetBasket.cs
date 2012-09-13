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

            var po = basket.PurchaseOrder;

            Basket = new Basket
                {
                    SubTotal = po.SubTotal,
                    TaxTotal = po.TaxTotal,
                    DiscountTotal =  po.DiscountTotal,
                    OrderTotal = po.OrderTotal,
                    TotalItems = po.OrderLines.Sum(l => l.Quantity),
                    
                    FormattedSubTotal = currencySymbol + po.SubTotal.Value.ToString("#,##.00"),
                    FormattedTaxTotal = currencySymbol + po.TaxTotal.Value.ToString("#,##.00"),
                    FormattedDiscountTotal =  currencySymbol + po.DiscountTotal.Value.ToString("#,##.00"),
                    FormattedOrderTotal = currencySymbol + po.OrderTotal.Value.ToString("#,##.00"),
                    FormattedTotalItems = po.OrderLines.Sum(l => l.Quantity).ToString("#,##"),

                    LineItems = new List<LineItem>()
                };

            foreach (var orderLine in po.OrderLines)
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