using UCommerce.EntitiesV2;
using UCommerce.Runtime;

namespace UCommerce.RazorStore.Services.Commands
{
    using ServiceStack.ServiceInterface;
    using ServiceStack.ServiceInterface.ServiceModel;
    using UCommerce.Api;
    public class GetPriceCalculation
    {
        public int? CatalogId { get; set; }
        public string Sku { get; set; }
    }

    public class GetPriceCalculationResponse : IHasResponseStatus
    {
        public GetPriceCalculationResponse() { }
        public GetPriceCalculationResponse(PriceCalculation priceCalculation)
        {
            PriceCalculation = priceCalculation;
        }

        public ResponseStatus ResponseStatus { get; set; }

        public PriceCalculation PriceCalculation { get; set; }
    }

    public class GetPriceCalculationService : ServiceBase<GetPriceCalculation>
    {
        protected override object Run(GetPriceCalculation request)
        {
            ProductCatalog catalog = CatalogLibrary.GetCatalog(request.CatalogId);
            Product product = CatalogLibrary.GetProduct(request.Sku);

            PriceCalculation priceCalculation = CatalogLibrary.CalculatePrice(product, catalog);

            return new GetPriceCalculationResponse(priceCalculation);
        }
    }
}