using System.Collections.Generic;
using System.Web.Mvc;
using Umbraco.Web;
using Umbraco.Web.Composing;
using Umbraco.Web.Models;
using Umbraco.Web.Routing;

namespace AvenueClothing.Models
{
	public class AddressDetailsViewModel : ContentModel
	{
		public AddressDetailsViewModel() : base (Current.UmbracoContext.PublishedRequest.PublishedContent)
        {
			ShippingAddress = new AddressViewModel();
			BillingAddress = new AddressViewModel();
			AvailableCountries = new List<SelectListItem>();
            IsShippingAddressDifferent = false;
        }
		public AddressViewModel ShippingAddress { get; set; }

		public AddressViewModel BillingAddress { get; set; }

        public bool IsShippingAddressDifferent { get; set; }

		public IList<SelectListItem> AvailableCountries { get; set; }
	}
}