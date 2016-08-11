using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UCommerce.RazorStore.Models
{
    public class MiniBasketViewModel
    {      
        public int NumberOfItems { get; set; }
        public Money Total { get; set; }
    }
}