namespace uCommerce.RazorStore.ServiceStack.Model
{
    public class LineItem
    {
        public int OrderLineId { get; set; }
        public int Quantity { get; set; }
        public string Sku { get; set; }
        public string VariantSku { get; set; }
        public decimal Price { get; set; }
        public string ProductName { get; set; }
        public decimal? Total { get; set; }
        public decimal? UnitDiscount { get; set; }
        public decimal VAT { get; set; }
        public decimal VATRate { get; set; }
    }
}