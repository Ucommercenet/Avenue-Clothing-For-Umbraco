using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;
using UCommerce.Catalog;
using UCommerce.Transactions;

namespace uCommerce.RazorStore.ServiceStack.Commands
{
    public class AddToBasket
    {
        public string CatalogName { get; set; }
        public int Quantity { get; set; }
        public string Sku { get; set; }
        public string VariantSku { get; set; }
        public bool AddToExistingLine { get; set; }
    }
    public class AddToBasketResponse : IHasResponseStatus
    {
        public ResponseStatus ResponseStatus { get; set; }
    }
    public class AddToBasketService : ServiceBase<AddToBasket>
    {
        protected override object Run(AddToBasket request)
        {
            if(string.IsNullOrWhiteSpace(request.CatalogName))
                request.CatalogName = CatalogLibrary.GetCatalog().Name;
            TransactionLibrary.AddToBasket(request.CatalogName, request.Quantity,request.Sku,request.VariantSku, request.AddToExistingLine);
            TransactionLibrary.ExecuteBasketPipeline();
            return new AddToBasketResponse();
        }
    }
}