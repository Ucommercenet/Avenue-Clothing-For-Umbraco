using Ucommerce.Api.PriceCalculation;

namespace UCommerce.RazorStore.Models
{
    public class MiniBasketViewModel
    {      
        public int NumberOfItems { get; set; }
        public ApiMoney Total { get; set; }
        public bool IsEmpty { get; set; }
    }
}