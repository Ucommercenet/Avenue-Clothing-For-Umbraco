using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Web;
using Umbraco.Web.Models;


namespace UCommerce.RazorStore.Models
{
    public class CategoryViewModel: RenderModel
    {
        public CategoryViewModel() : base(UmbracoContext.Current.PublishedContentRequest.PublishedContent)
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
        
        public int CategoryId { get; set; }

        public int CatalogId { get; set; }


    }
}