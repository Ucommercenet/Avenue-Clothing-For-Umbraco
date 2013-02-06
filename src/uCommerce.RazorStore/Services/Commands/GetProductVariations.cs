namespace UCommerce.RazorStore.Services.Commands
{
    using System.Collections.Generic;
    using System.Linq;

    using UCommerce.Api;

    using ServiceStack.ServiceInterface;
    using ServiceStack.ServiceInterface.ServiceModel;

    using UCommerce.RazorStore.Services.Model;

    public class GetProductVariations
    {
        public string ProductSku{ get; set; }

    }
    public class GetProductVariationsResponse : IHasResponseStatus
    {
        public GetProductVariationsResponse(UCommerce.EntitiesV2.Product parentProduct)
        {
            
            Variations = parentProduct.Variants.Select(variant => new ProductVariation
                {
                    Sku = variant.Sku,
                    VariantSku = variant.VariantSku,
                    ProductName = variant.Name,
                    Properties = variant.ProductProperties.Select(prop => new ProductProperty()
                        {
                            Id = prop.Id,
                            Name = prop.ProductDefinitionField.Name,
                            Value = prop.Value
                        })
                }).ToList();
        }

        public ResponseStatus ResponseStatus { get; set; }

        public ICollection<ProductVariation> Variations { get; set; }
    }
    public class GetProductVariationsService : ServiceBase<GetProductVariations>, IUCommerceApiService
    {
        protected override object Run(GetProductVariations request)
        {
            var product = CatalogLibrary.GetProduct(request.ProductSku);
            return new GetProductVariationsResponse(product);
        }

    }
}