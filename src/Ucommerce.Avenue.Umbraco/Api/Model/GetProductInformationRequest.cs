using System;

namespace Ucommerce.Avenue.Umbraco.Api.Model
{
    public class GetProductInformationRequest
    {
        public Guid? CatalogId { get; set; }
        public string Sku { get; set; }
        public Guid? CategoryId { get; set; }
    }
}