using UCommerce.Infrastructure;
using UCommerce.Publishing.Model;
using UCommerce.Publishing.Runtime;
using System.Collections.Generic;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;
using UCommerce.RazorStore.Services.Model;

namespace UCommerce.RazorStore.Services.Commands
{

    public class Search
    {
        public string Keyword { get; set; }
    }
    public class SearchResponse : IHasResponseStatus
    {
        public SearchResponse()
        {
        }

        public SearchResponse(IEnumerable<Product> products)
        {
            var catalogLibrary = ObjectFactory.Instance.Resolve<ICatalogLibrary>();

            Variations = new List<ProductVariation>();

            foreach (var product in products)
            {
                Variations.Add(new ProductVariation
                    {
                        Sku = product.Sku,
                        VariantSku = product.VariantSku,
                        ProductName = product.DisplayName,
                        Url = catalogLibrary.GetNiceUrlForProduct(product)
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
            var searchLibrary = ObjectFactory.Instance.Resolve<ISearchLibrary>();
            var products = searchLibrary.FindProducts(request.Keyword);
            return new SearchResponse(products);
        }

    }
}