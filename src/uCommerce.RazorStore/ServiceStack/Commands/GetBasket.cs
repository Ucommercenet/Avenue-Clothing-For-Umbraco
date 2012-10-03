using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;
using UCommerce.Api;
using uCommerce.RazorStore.ServiceStack.Model;

namespace uCommerce.RazorStore.ServiceStack.Commands
{
    using System.Linq;

    using UCommerce;
    using UCommerce.Runtime;

    public class GetBasket
    {
    }
    public class GetBasketResponse : IHasResponseStatus
    {
        public GetBasketResponse(UCommerce.EntitiesV2.Basket basket)
        {
            var currency = SiteContext.Current.CatalogContext.CurrentCatalog.PriceGroup.Currency;

            var po = basket.PurchaseOrder;

            var subTotal = new Money(po.SubTotal.Value, currency);
            var taxTotal = new Money(po.TaxTotal.Value, currency);
            var discountTotal = new Money(po.DiscountTotal.Value, currency);
            var orderTotal = new Money(po.OrderTotal.Value, currency);

            Basket = new Basket
                {
                    SubTotal = po.SubTotal,
                    TaxTotal = po.TaxTotal,
                    DiscountTotal =  po.DiscountTotal,
                    OrderTotal = po.OrderTotal,
                    TotalItems = po.OrderLines.Sum(l => l.Quantity),
                    
                    FormattedSubTotal = subTotal.ToString(),
                    FormattedTaxTotal = taxTotal.ToString(),
                    FormattedDiscountTotal =  discountTotal.ToString(),
                    FormattedOrderTotal = orderTotal.ToString(),
                    FormattedTotalItems = po.OrderLines.Sum(l => l.Quantity).ToString("#,##"),

                    LineItems = new List<LineItem>()
                };

            foreach (var line in po.OrderLines)
            {
                var lineTotal = new Money(line.Total.Value, currency);
                var lineItem = new LineItem
                    {
                        OrderLineId = line.OrderLineId,
                        Quantity = line.Quantity,
                        Sku = line.Sku,
                        VariantSku = line.VariantSku,
                        Price = line.Price,
                        ProductName = line.ProductName,
                        Total = line.Total,
                        FormattedTotal = lineTotal.ToString(),
                        UnitDiscount = line.UnitDiscount,
                        VAT = line.VAT,
                        VATRate = line.VATRate
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