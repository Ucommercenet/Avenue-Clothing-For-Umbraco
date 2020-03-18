namespace AvenueClothing.Api.Model
{
    public class GetProductInformationResponse
    {
        public PriceCalculationViewModel PriceCalculation { get; set; }
        public string NiceUrl { get; set; }
        public string Sku { get; set; }
    }
}