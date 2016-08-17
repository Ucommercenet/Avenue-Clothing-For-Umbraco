using System.Collections.Generic;
using System.Web.Mvc;
using Umbraco.Web;
using Umbraco.Web.Models;

namespace UCommerce.RazorStore.Models
{
	public class AddressDetailsViewModel : RenderModel
	{
		public AddressDetailsViewModel() : base (UmbracoContext.Current.PublishedContentRequest.PublishedContent)
        {
			ShippingAddress = new AddressViewModel();
			BillingAddress = new AddressViewModel();
			AvailableCountries = new List<SelectListItem>();
		}
		public AddressViewModel ShippingAddress { get; set; }

		public AddressViewModel BillingAddress { get; set; }

		public IList<SelectListItem> AvailableCountries { get; set; }
	}
}