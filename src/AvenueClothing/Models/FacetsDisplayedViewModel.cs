using System.Collections.Generic;

namespace AvenueClothing.Models
{
    public class FacetsDisplayedViewModel
            {
        public FacetsDisplayedViewModel()
        {
            Facets = new List<FacetViewModel>();
        }
        public IList<FacetViewModel> Facets { get; set; }

    }
}