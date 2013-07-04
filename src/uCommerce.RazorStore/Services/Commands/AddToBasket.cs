namespace UCommerce.RazorStore.Services.Commands
{
	using ServiceStack.ServiceInterface;
	using ServiceStack.ServiceInterface.ServiceModel;
	using UCommerce.Api;

	public class AddToBasket
	{
		public int? CatalogId { get; set; }
		public int Quantity { get; set; }
		public string Sku { get; set; }
		public string VariantSku { get; set; }
		public bool AddToExistingLine { get; set; }
	}

	public class AddToBasketResponse : IHasResponseStatus
	{
		public ResponseStatus ResponseStatus { get; set; }
	}

	public class AddToBasketService : ServiceBase<AddToBasket>, IUCommerceApiService
	{
		protected override object Run(AddToBasket request)
		{
			TransactionLibrary.AddToBasket(request.Quantity, request.Sku, request.VariantSku, request.AddToExistingLine, true, request.CatalogId);
			return new AddToBasketResponse();
		}
	}
}