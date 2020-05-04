using Ucommerce.Api.PriceCalculation;

namespace AvenueClothing.Models
{
    public class MiniBasketViewModel
    {      
        public int NumberOfItems { get; set; }
        public Ucommerce.Money Total { get; set; }
        public bool IsEmpty { get; set; }
    }
}