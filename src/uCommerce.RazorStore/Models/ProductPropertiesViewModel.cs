using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UCommerce.RazorStore.Models
{
    public class ProductPropertiesViewModel
    {
        public ProductPropertiesViewModel() {
            Values = new List<string>();
        }
        public string PropertyName { get; set; }
        public IList<string> Values { get; set; }


    }
}