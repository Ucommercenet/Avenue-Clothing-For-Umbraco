using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;
using UCommerce.Api;
using UCommerce.Transactions;

namespace uCommerce.RazorStore.ServiceStack.Commands
{
    public class UpdateLineItem
    {
        public int NewQuantity { get; set; }
        public int OrderLineId { get; set; }
    }
    public class UpdateLineItemResponse : IHasResponseStatus
    {
        public ResponseStatus ResponseStatus { get; set; }
    }
    public class UpdateLineItemService : ServiceBase<UpdateLineItem>
    {
        protected override object Run(UpdateLineItem request)
        {
            TransactionLibrary.UpdateLineItem(request.OrderLineId, request.NewQuantity);
            TransactionLibrary.ExecuteBasketPipeline();
            return new UpdateLineItemResponse();
        }
    }
}