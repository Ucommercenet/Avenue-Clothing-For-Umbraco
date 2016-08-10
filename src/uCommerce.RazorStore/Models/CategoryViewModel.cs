using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UCommerce.RazorStore.Models
{
	public class CategoryViewModel
	{
		public CategoryViewModel()
		{
			Categories = new List<CategoryViewModel>();
			Products = new List<ProductViewModel>();
		}
		public string Url { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public IList<CategoryViewModel> Categories { get; set; }

		public IList<ProductViewModel> Products { get; set; } 
	}
}