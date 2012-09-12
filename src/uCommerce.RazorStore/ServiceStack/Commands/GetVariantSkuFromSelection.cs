namespace uCommerce.RazorStore.ServiceStack.Commands
{
    using System.Collections.Generic;
    using System.Linq;

    using UCommerce.Api;
    using UCommerce.EntitiesV2;

    using global::ServiceStack.ServiceInterface;
    using global::ServiceStack.ServiceInterface.ServiceModel;

    using uCommerce.RazorStore.ServiceStack.Model;

    using ProductProperty = uCommerce.RazorStore.ServiceStack.Model.ProductProperty;

    public class GetVariantSkuFromSelection
    {
        public string ProductSku { get; set; }
        public string Size { get; set; }
        public string Colour { get; set; }
    }
    public class GetVariantSkuFromSelectionResponse : IHasResponseStatus
    {
        public GetVariantSkuFromSelectionResponse(UCommerce.EntitiesV2.Product parentProduct, string size, string colour)
        {
            var variant = parentProduct.Variants.FirstOrDefault(v =>
                    v.ProductProperties.Any(p => p.ProductDefinitionField.Name == "CollarSize" && p.Value == size)
                    && v.ProductProperties.Any(p => p.ProductDefinitionField.Name == "Colour" && p.Value == colour));

            if (variant == null)
                return;

            Variant = new ProductVariation
                {
                    Sku = variant.Sku,
                    VariantSku = variant.VariantSku,
                    ProductName = variant.Name,
                    Properties = variant.ProductProperties.Select(prop => new ProductProperty
                    {
                        Id = prop.Id,
                        Name = prop.ProductDefinitionField.Name,
                        Value = prop.Value
                    })
                };
        }

        public ResponseStatus ResponseStatus { get; set; }

        public ProductVariation Variant { get; set; }
    }
    public class GetVariantSkuFromSelectionService : ServiceBase<GetVariantSkuFromSelection>
    {
        protected override object Run(GetVariantSkuFromSelection request)
        {
            var product = CatalogLibrary.GetProduct(request.ProductSku);
            return new GetVariantSkuFromSelectionResponse(product, request.Size, request.Colour);
        }

    }
}