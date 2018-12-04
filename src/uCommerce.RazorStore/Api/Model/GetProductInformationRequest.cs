namespace UCommerce.RazorStore.Api.Model
{
    public class GetProductInformationRequest
    {
        public int? CatalogId { get; set; }
        public string Sku { get; set; }
        public int? CategoryId { get; set; }
    }
}