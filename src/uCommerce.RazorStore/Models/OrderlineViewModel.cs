namespace UCommerce.RazorStore.Models
{
	public class OrderlineViewModel
	{
		public string Total { get; set; }

		public int Quantity { get; set; }

		public int OrderLineId { get; set; }

		public string Sku { get; set; }

		public string VariantSku { get; set; }

		public string ProductName { get; set; }

	}
}