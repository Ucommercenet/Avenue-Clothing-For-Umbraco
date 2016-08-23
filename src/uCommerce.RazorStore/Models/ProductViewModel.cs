using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UCommerce.Api;
using Umbraco.Web;
using Umbraco.Web.Models;

namespace UCommerce.RazorStore.Models
{
	public class ProductViewModel: RenderModel
	{
		public ProductViewModel() :base(UmbracoContext.Current.PublishedContentRequest.PublishedContent)
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

        public string ThumbnailImageUrl { get; set; }

    }
}