﻿using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;
using UCommerce.Api;
using uCommerce.RazorStore.ServiceStack.Model;

namespace uCommerce.RazorStore.ServiceStack.Commands
{
    using System.Linq;

    public class Search
    {
        public string Keyword { get; set; }
    }
    public class SearchResponse : IHasResponseStatus
    {
        public SearchResponse(IEnumerable<UCommerce.EntitiesV2.Product> products)
        {
            Variations = new List<ProductVariation>();

            foreach (var product in products)
            {
                Variations.Add(new ProductVariation
                    {
                        Sku = product.Sku,
                        VariantSku = product.VariantSku,
                        ProductName = product.Name,
                        Url = CatalogLibrary.GetNiceUrlForProduct(product),
                        Properties = product.ProductProperties.Select(prop => new ProductProperty()
                            {
                                Id = prop.Id,
                                Name = prop.ProductDefinitionField.Name,
                                Value = prop.Value
                            })
                    });
            }
        }

        public ResponseStatus ResponseStatus { get; set; }

        public ICollection<ProductVariation> Variations { get; set; }
    }
    public class SearchService : ServiceBase<Search>
    {
        protected override object Run(Search request)
        {
            var products = UCommerce.EntitiesV2.Product.Find(p =>
                                                                p.VariantSku == null
                                                                && p.DisplayOnSite
                                                                &&
                                                                (
                                                                    p.Sku.Contains(request.Keyword)
                                                                    || p.Name.Contains(request.Keyword)
                                                                    || p.ProductDescriptions.Any(d => d.DisplayName.Contains(request.Keyword) || d.ShortDescription.Contains(request.Keyword) || d.LongDescription.Contains(request.Keyword))
                                                                )
                                                            );
            return new SearchResponse(products);
        }

    }
}