using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AvenueClothing.Models
{
	public class AddToBasketViewModel
	{
		public Guid Product { get; set; }

		public Guid? Variant { get; set; }

	}
}