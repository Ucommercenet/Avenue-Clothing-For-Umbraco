using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UCommerce.Api;

namespace UCommerce.RazorStore.Models
{
	public class ProductViewModel
	{
		public ProductViewModel()
		{
			Variants = new List<ProductViewModel>();
		}
		public bool IsVariant { get; set; }

		public string Name { get; set; }

		public string Url { get; set; }

		public string LongDescription { get; set; }

		public IList<ProductViewModel> Variants { get; set; }

		public string Sku { get; set; }

		public string VariantSku { get; set; }

		public PriceCalculation PriceCalculation { get; set; }
	}
}