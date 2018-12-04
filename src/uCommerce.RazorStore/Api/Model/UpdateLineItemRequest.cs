namespace UCommerce.RazorStore.Api.Model
{
    public class UpdateLineItemRequest
    {
        public int NewQuantity { get; set; }
        public int OrderLineId { get; set; }
    }
}