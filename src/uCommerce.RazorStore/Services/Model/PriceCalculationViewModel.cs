using System.Globalization;

namespace UCommerce.RazorStore.Services.Model
{
    public class PriceCalculationViewModel
    {
         public PriceCalculationViewModel() { }

        public PriceViewModel Discount { get; set; }

        public bool IsDiscounted { get; set; }

        public PriceViewModel ListPrice { get; set; }

        public PriceViewModel YourPrice { get; set; }

        public MoneyViewModel YourTax { get; set; }
    }

    public class PriceViewModel
    {
        public PriceViewModel() {}

        public MoneyViewModel Amount { get; set; }
        public MoneyViewModel AmountExclTax { get; set; }

        public MoneyViewModel AmountInclTax { get; set; }
    }

    public class MoneyViewModel
    {
        public MoneyViewModel() { }
        public MoneyViewModel(decimal value, CurrencyViewModel currency) {}
        public MoneyViewModel(decimal value, CultureInfo cultureInfo, CurrencyViewModel currency) {}

        public CultureInfo Culture { get; }
        public CurrencyViewModel Currency { get; }
        public decimal Value { get; set; }
        public string DisplayValue { get; set; }

    }
}