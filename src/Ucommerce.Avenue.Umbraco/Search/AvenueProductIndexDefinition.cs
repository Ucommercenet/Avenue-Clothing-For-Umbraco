using System.Collections.Generic;
using UCommerce.Search.Definitions;
using UCommerce.Search.Extensions;
using UCommerce.Search.FacetsV2;

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

            this.Facet("Colour");
            this.Facet("CollarSize");
            this.Facet("ShoeSize");
            
            // TODO: Enable facets on prices once the DrillSidewaysHelpers knows how
            // this.Facet("UnitPrices", new FacetOptions
            // {
            //     Ranges = new List<Range>
            //     {
            //         new Range {From = 0.0, To = 100.0},
            //         new Range {From = 100.0, To = 1000.0},
            //         new Range {From = 1000.0, To = 10000.0},
            //     }
            // });
        }
    }
}