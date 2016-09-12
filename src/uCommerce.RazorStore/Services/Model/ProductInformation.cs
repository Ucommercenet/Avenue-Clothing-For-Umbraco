namespace UCommerce.RazorStore.Services.Model
{
    public class ProductInformation
    {
         public ProductInformation() { }

         public string NiceUrl { get; set; }
         public PriceCalculationViewModel PriceCalculation { get; set; }

        public string Sku { get; set; }

    }
}