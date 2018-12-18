namespace UCommerce.RazorStore.Api.Model
{
    public class ProductInformation
    {
        public string NiceUrl { get; set; }
        public PriceCalculationViewModel PriceCalculation { get; set; }

        public string Sku { get; set; }

    }
}