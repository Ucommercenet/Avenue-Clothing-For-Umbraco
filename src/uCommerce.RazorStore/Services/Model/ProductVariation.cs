namespace UCommerce.RazorStore.Services.Model
{
    using System.Collections.Generic;

    public class ProductVariation
    {
        public string Sku { get; set; }
        public string VariantSku { get; set; }
        public string ProductName { get; set; }
        public string Url { get; set; }
        public IEnumerable<ProductProperty> Properties { get; set; }
    }
}