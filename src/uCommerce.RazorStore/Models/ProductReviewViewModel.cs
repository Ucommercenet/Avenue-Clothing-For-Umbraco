using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UCommerce.RazorStore.Models
{
    public class ProductReviewViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string Comments { get; set; }
        public int? Rating { get; set; }

        //Check REview model in uCommerce for star rating
    }
}