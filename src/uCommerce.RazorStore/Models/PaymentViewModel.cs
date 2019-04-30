using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web;
using Umbraco.Web.Composing;
using Umbraco.Web.Models;

namespace UCommerce.RazorStore.Models
{
	public class PaymentViewModel: ContentModel
	{
        public PaymentViewModel() : base(Current.UmbracoContext.PublishedRequest.PublishedContent)
        {
	        
	    }

		public IList<SelectListItem> AvailablePaymentMethods { get; set; }

		public int SelectedPaymentMethodId { get; set; }

        public string ShippingCountry { get; set; }
    }
}