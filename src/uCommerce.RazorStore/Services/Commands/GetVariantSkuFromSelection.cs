namespace UCommerce.RazorStore.Services.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using UCommerce.Api;

    using ServiceStack.ServiceInterface;
    using ServiceStack.ServiceInterface.ServiceModel;

    using UCommerce.RazorStore.Services.Model;

    using ProductProperty = UCommerce.RazorStore.Services.Model.ProductProperty;

    public class GetVariantSkuFromSelection
    {
        public string ProductSku { get; set; }
        public IDictionary<string, string> VariantProperties { get; set; }
    }
    public class GetVariantSkuFromSelectionResponse : IHasResponseStatus
    {
        public GetVariantSkuFromSelectionResponse()
        {
        }

        public GetVariantSkuFromSelectionResponse(UCommerce.EntitiesV2.Product parentProduct, IDictionary<string, string> properties)
        {
            UCommerce.EntitiesV2.Product variant = null;

            if (parentProduct.Variants.Any() && properties.Any()) // If there are variant values we'll need to find the selected variant
            {
                variant = parentProduct.Variants.FirstOrDefault(v => v.ProductProperties.Where(pp => pp.ProductDefinitionField.DisplayOnSite && pp.ProductDefinitionField.IsVariantProperty && !pp.ProductDefinitionField.Deleted && pp.Value != null && pp.Value != String.Empty).All(p => properties.Any(kv => kv.Key.Equals(p.ProductDefinitionField.Name, StringComparison.InvariantCultureIgnoreCase) && kv.Value.Equals(p.Value, StringComparison.InvariantCultureIgnoreCase))));
            }
            else if (!parentProduct.Variants.Any()) // Only use the current product where there are no variants
            {
                variant = parentProduct;
            }

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
    public class GetVariantSkuFromSelectionService : ServiceBase<GetVariantSkuFromSelection>, IUCommerceApiService
    {
        protected override object Run(GetVariantSkuFromSelection request)
        {
            var product = CatalogLibrary.GetProduct(request.ProductSku);
            return new GetVariantSkuFromSelectionResponse(product, request.VariantProperties);
        }

    }
}