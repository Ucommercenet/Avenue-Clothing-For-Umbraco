using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UCommerce.EntitiesV2;
using Umbraco.Web;
using Umbraco.Web.Models;

namespace UCommerce.RazorStore.Models
{
	public class PurchaseOrderViewModel : RenderModel
	{
		public PurchaseOrderViewModel() : base(UmbracoContext.Current.PublishedContentRequest.PublishedContent)
		{
			OrderLines = new List<OrderlineViewModel>();
		}
		public IList<OrderlineViewModel> OrderLines { get; set; }

		public string OrderTotal { get; set; }

		public string SubTotal { get; set; }

		public string TaxTotal { get; set; }

		public string DiscountTotal { get; set; }

		public string ShippingTotal { get; set; }

		public string PaymentTotal { get; set; }

		public int RemoveOrderlineId { get; set; }

        public string ShipmentName { get; set; }

        public string PaymentName { get; set; }

        public decimal ShipmentAmount { get; set; }

        public decimal PaymentAmount { get; set; }

        public decimal DiscountAmount { get; set; }

        public OrderAddress ShipmentAddress { get; set; }

        public OrderAddress BillingAddress { get; set; }

    }
}