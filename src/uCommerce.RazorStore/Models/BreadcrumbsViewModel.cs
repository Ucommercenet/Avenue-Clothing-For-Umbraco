using System;
using System.Collections.Generic;
namespace UCommerce.RazorStore.Models
{
    public class BreadcrumbsViewModel
    {
        public BreadcrumbsViewModel()
        {
            
        }

        public string BreadcrumbName { get; set; }
        public string BreadcrumbUrl { get; set; }
    }
}