using Ucommerce.Search.Definitions;
using Ucommerce.Search.Extensions;

namespace AvenueClothing.Search
{
    public class AvenueProductIndexDefinition : DefaultProductsIndexDefinition
    {
        public AvenueProductIndexDefinition() : base()
        {
            this.Field(p => p["ShowOnHomepage"], typeof(bool));
            this.Field(p => p["CollarSize"], typeof(string))
                .DisplayName("Collar Size")
                .Facet();
            this.Field(p => p["ShoeSize"], typeof(string))
                .DisplayName("Shoe Size").Facet();
            this.Field(p => p["Colour"], typeof(string))
                .DisplayName("en-GB", "Colour")
                .DisplayName("en-US", "Color")
                .DisplayName("Colour")
                .Facet();
            this.PricesField(p => p.UnitPrices);
        }
    }
}