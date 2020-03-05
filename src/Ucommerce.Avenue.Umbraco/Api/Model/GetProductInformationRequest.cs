using System;

namespace UCommerce.RazorStore.Api.Model
{
    public class GetProductInformationRequest
    {
        public Guid? CatalogGuid { get; set; }
        public string Sku { get; set; }
        public Guid? CategoryGuid { get; set; }
    }
}