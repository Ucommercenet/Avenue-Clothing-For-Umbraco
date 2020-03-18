using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web;
using Umbraco.Web.Composing;
using Umbraco.Web.Models;

namespace AvenueClothing.Models
{
	public class ShippingViewModel: ContentModel
	{
        public ShippingViewModel() : base(Current.UmbracoContext.PublishedRequest.PublishedContent)
	    {
	        
	    }

        public IList<SelectListItem> AvailableShippingMethods { get; set; }

		public int SelectedShippingMethodId { get; set; }

        public string ShippingCountry { get; set; }
	}
}