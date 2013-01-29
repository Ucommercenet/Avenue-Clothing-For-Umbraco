using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;
using UCommerce.Api;

namespace uCommerce.RazorStore.ServiceStack.Commands
{
    using System.Linq;

    using UCommerce;
    using UCommerce.Runtime;

    using uCommerce.RazorStore.ServiceStack.Model;

    public class UpdateLineItem
    {
        public int NewQuantity { get; set; }
        public int OrderLineId { get; set; }
    }
    public class UpdateLineItemResponse : IHasResponseStatus
    {
        public UpdateLineItemResponse(UCommerce.EntitiesV2.OrderLine line)
        {
            if (line == null)
            {
                UpdatedLine = new LineItem();
                return;
            }

            var currency = SiteContext.Current.CatalogContext.CurrentCatalog.PriceGroup.Currency;
            var lineTotal = new Money(line.Total.Value, currency);

            UpdatedLine = new LineItem()
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
        }

        public ResponseStatus ResponseStatus { get; set; }

        public LineItem UpdatedLine { get; set; }
    }
    public class UpdateLineItemService : ServiceBase<UpdateLineItem>
    {
        protected override object Run(UpdateLineItem request)
        {
            TransactionLibrary.UpdateLineItem(request.OrderLineId, request.NewQuantity);
            TransactionLibrary.ExecuteBasketPipeline();
            var newLine = TransactionLibrary.GetBasket().PurchaseOrder.OrderLines.FirstOrDefault(l => l.OrderLineId == request.OrderLineId);
            return new UpdateLineItemResponse(newLine);
        }
    }
}