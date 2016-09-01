using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UCommerce.Search.Facets;

namespace UCommerce.RazorStore.Models
{
    public class FacetViewModel
    {
        public FacetViewModel() {
            FacetValues = new List<FacetValueViewModel>();
        }
        public IList<FacetValueViewModel> FacetValues { get; set; }
        public string Name { get; set; }

        public string DisplayName { get; set; }
    }
}