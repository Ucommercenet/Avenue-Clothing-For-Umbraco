using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Web;
using Umbraco.Web.Models;

namespace UCommerce.RazorStore.Models
{
    public class ProductsViewModel: RenderModel
    {
        public ProductsViewModel() : base(UmbracoContext.Current.PublishedContentRequest.PublishedContent)
        {
            Products = new List<ProductViewModel>();
        }

        public IList<ProductViewModel> Products;
    }
}