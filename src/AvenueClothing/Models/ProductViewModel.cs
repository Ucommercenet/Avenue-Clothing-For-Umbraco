using System.Collections.Generic;
using Umbraco.Web.Composing;
using Umbraco.Web.Models;

namespace AvenueClothing.Models
{
    public class ProductPageViewModel : ContentModel
    {
        public ProductPageViewModel() : base(Current.UmbracoContext.PublishedRequest.PublishedContent)
        {
        }

        public ProductViewModel ProductViewModel { get; set; }

        public bool AddedToBasket { get; set; }

        public bool ItemAlreadyExists { get; set; }
    }

    public class ProductViewModel : ContentModel
    {
        public ProductViewModel() : base(Current.UmbracoContext.PublishedRequest.PublishedContent)
        {
            Variants = new List<ProductViewModel>();
            Properties = new List<ProductPropertiesViewModel>();
            Reviews = new List<ProductReviewViewModel>();
        }

        public bool IsVariant { get; set; }

        public bool IsProductFamily { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string LongDescription { get; set; }

        public IList<ProductViewModel> Variants { get; set; }

        public string Sku { get; set; }

        public string VariantSku { get; set; }

        public string ThumbnailImageUrl { get; set; }

        public IList<ProductPropertiesViewModel> Properties { get; set; }

        public IList<ProductReviewViewModel> Reviews { get; set; }

        public string TaxCalculation { get; set; }

        public bool IsOrderingAllowed { get; set; }
        
        public string Tax { get ; set ; }
        
        public string Price { get ; set ; }
    }
}