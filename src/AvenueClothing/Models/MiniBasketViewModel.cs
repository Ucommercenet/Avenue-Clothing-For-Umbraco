using Ucommerce.Api.PriceCalculation;

namespace UCommerce.RazorStore.Models
{
    public class MiniBasketViewModel
    {      
        public int NumberOfItems { get; set; }
        public Ucommerce.Api.PriceCalculation.Money Total { get; set; }
        public bool IsEmpty { get; set; }
    }
}