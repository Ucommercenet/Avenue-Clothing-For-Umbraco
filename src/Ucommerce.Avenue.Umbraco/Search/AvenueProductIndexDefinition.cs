using UCommerce.Search.Definitions;
using UCommerce.Search.Extensions;

namespace Ucommerce.Avenue.Umbraco.Search
{
    public class AvenueProductIndexDefinition : DefaultProductsIndexDefinition
    {
        public AvenueProductIndexDefinition() : base()
        {
            this.Field(p => p["ShowOnHomepage"], typeof(bool));
            this.Field(p => p["CollarSize"], typeof(string));
            this.Field(p => p["ShoeSize"], typeof(string));
            this.Field(p => p["Colour"], typeof(string));
            this.PricesField(p => p.UnitPrices.ToString());
            this.Facet("Colour");
            this.Facet("CollarSize");
            this.Facet("ShoeSize");
        }
    }
}