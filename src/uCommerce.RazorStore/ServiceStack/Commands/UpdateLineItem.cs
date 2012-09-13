using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;
using UCommerce.Api;
using UCommerce.Transactions;

namespace uCommerce.RazorStore.ServiceStack.Commands
{
    using System.Linq;

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
            UpdatedLine = new LineItem()
            {
                OrderLineId = line.OrderLineId,
                Quantity = line.Quantity,
                Sku = line.Sku,
                VariantSku = line.VariantSku,
                Price = line.Price,
                ProductName = line.ProductName,
                Total = line.Total,
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