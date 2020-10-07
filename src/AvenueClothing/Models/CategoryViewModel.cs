﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Web;
using Umbraco.Web.Composing;
using Umbraco.Web.Models;


namespace AvenueClothing.Models
{
	public class CategoryViewModel : ContentModel
	{
		public CategoryViewModel() : base(Current.UmbracoContext.PublishedRequest.PublishedContent)
		//public CategoryViewModel()
		{
			Categories = new List<CategoryViewModel>();
			Products = new List<ProductViewModel>();
		}
		public string Url { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public IList<CategoryViewModel> Categories { get; set; }

		public IList<ProductViewModel> Products { get; set; }

		public string BannerImageUrl { get; set; }

		public Guid CategoryId { get; set; }

		public Guid CatalogId { get; set; }

		public int TotalProducts { get; set; }
		public int PageNumber { get; set; }
		public int PageSize { get; set; }

	}
}