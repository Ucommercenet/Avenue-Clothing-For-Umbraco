using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UCommerce.MasterClass.Website.Models
{
	public class CategoryNavigationViewModel
	{
		public CategoryNavigationViewModel()
		{
			Categories = new List<CategoryViewModel>();
		}
		public IList<CategoryViewModel> Categories { get; set; } 
	}
}