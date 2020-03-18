using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Web;
using Umbraco.Web.Composing;
using Umbraco.Web.Models;

namespace AvenueClothing.Models
{
    public class ProductsViewModel: ContentModel
    {
        public ProductsViewModel() : base(Current.UmbracoContext.PublishedRequest.PublishedContent)
        {
            Products = new List<ProductViewModel>();
        }

        public IList<ProductViewModel> Products;
    }
}