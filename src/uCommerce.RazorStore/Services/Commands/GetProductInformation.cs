using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.RazorStore.Services.Model;

namespace UCommerce.RazorStore.Services.Commands
{
    public class GetProductInformation
    {
        public int? CatalogId { get; set; }
        public string Sku { get; set; }
        public int? CategoryId { get; set; }
    }
    public class GetProductInformationResponse : IHasResponseStatus
    {
        public GetProductInformationResponse() { }
        public GetProductInformationResponse(ProductInformation productInformation)
        {
            PriceCalculation = productInformation.PriceCalculation;
            NiceUrl = productInformation.NiceUrl;
            Sku = productInformation.Sku;
        }

        public ResponseStatus ResponseStatus { get; set; }
        public PriceCalculationViewModel PriceCalculation { get; set; }
        public string NiceUrl { get; set; }
        public string Sku { get; set; }
    }

    public class GetProductInformationService : ServiceBase<GetProductInformation>
    {
        protected override object Run(GetProductInformation request)
        {
            ProductCatalog catalog = CatalogLibrary.GetCatalog(request.CatalogId);
            Product product = CatalogLibrary.GetProduct(request.Sku);
            Category category = CatalogLibrary.GetCategory(request.CategoryId.Value);
            string niceUrl = CatalogLibrary.GetNiceUrlForProduct(product, category, catalog);

            PriceCalculation priceCalculation = CatalogLibrary.CalculatePrice(product);

            Currency currency = priceCalculation.YourPrice.Amount.Currency;

            ProductInformation productInformation = new ProductInformation()
            {
                NiceUrl = niceUrl,
                PriceCalculation = new PriceCalculationViewModel()
                {
                    Discount = new PriceViewModel()
                    {
                        Amount = new MoneyViewModel()
                        {
                            Value = priceCalculation.Discount.Amount.Value,
                            DisplayValue = new Money(priceCalculation.Discount.Amount.Value, currency).ToString()
                        },
                        AmountExclTax = new MoneyViewModel()
                        {
                            Value = priceCalculation.Discount.AmountExclTax.Value,
                            DisplayValue = new Money(priceCalculation.Discount.AmountExclTax.Value, currency).ToString()
                        },
                        AmountInclTax = new MoneyViewModel()
                        {
                            Value = priceCalculation.Discount.AmountInclTax.Value,
                            DisplayValue = new Money(priceCalculation.Discount.AmountInclTax.Value, currency).ToString()
                        }
                    },
                    IsDiscounted = priceCalculation.IsDiscounted,
                    YourPrice = new PriceViewModel()
                    {
                        Amount = new MoneyViewModel()
                        {
                            Value = priceCalculation.YourPrice.Amount.Value,
                            DisplayValue = new Money(priceCalculation.YourPrice.Amount.Value, currency).ToString()
                        },
                        AmountInclTax = new MoneyViewModel()
                        {
                            Value = priceCalculation.YourPrice.AmountInclTax.Value,
                            DisplayValue = new Money(priceCalculation.YourPrice.AmountInclTax.Value, currency).ToString()
                        },
                        AmountExclTax = new MoneyViewModel()
                        {
                            Value = priceCalculation.YourPrice.AmountExclTax.Value,
                            DisplayValue = new Money(priceCalculation.YourPrice.AmountExclTax.Value, currency).ToString()
                        }
                    },
                    ListPrice = new PriceViewModel()
                    {
                        Amount = new MoneyViewModel()
                        {
                            Value = priceCalculation.ListPrice.Amount.Value,
                            DisplayValue = new Money(priceCalculation.ListPrice.Amount.Value, currency).ToString()
                        },
                        AmountExclTax = new MoneyViewModel()
                        {
                            Value = priceCalculation.ListPrice.AmountExclTax.Value,
                            DisplayValue = new Money(priceCalculation.ListPrice.AmountExclTax.Value, currency).ToString()
                        },
                        AmountInclTax = new MoneyViewModel()
                        {
                            Value = priceCalculation.ListPrice.AmountInclTax.Value,
                            DisplayValue = new Money(priceCalculation.ListPrice.AmountInclTax.Value, currency).ToString()
                        }
                    }
                }
            };
            productInformation.Sku = product.Sku;

            return new GetProductInformationResponse(productInformation);
        }
    }
}
