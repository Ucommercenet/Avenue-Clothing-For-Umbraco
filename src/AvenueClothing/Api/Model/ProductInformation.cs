namespace Ucommerce.Avenue.Umbraco.Api.Model
{
    public class ProductInformation
    {
        public string NiceUrl { get; set; }
        public PriceCalculationViewModel PriceCalculation { get; set; }

        public string Sku { get; set; }

    }
}