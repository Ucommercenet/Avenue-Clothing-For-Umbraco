using System.Collections.Generic;
using System.Linq;
using Ucommerce.EntitiesV2;
using Ucommerce.Infrastructure.Components.Windsor;
using Ucommerce.Pipelines;
using Ucommerce.Pipelines.Initialization;

namespace AvenueClothing.SettingsCreator
{
    public class CreatePriceGroupTask : IPipelineTask<InitializeArgs>
    {
        [Mandatory] public CreateSettingsHelper CreateSettingsHelper { get; set; }
        public PipelineExecutionResult Execute(InitializeArgs subject)
        {
            // create mirror list of everything
            //
            // in the end, compare with "actual" list
            //
            // Save stuff in mirror list that is not in actual list
            //
            // Delete stuff that is not in mirror list
            //
            // https://github.com/Ucommercenet/Uconnector-Samples/blob/f297e98272ed7487218d225a89f68219f62f5215/src/UCommerce.UConnector/Adapters/Senders/ProductListToUCommerce.cs#L92

            
            
            // Initialize pipeline gone
            //
            // make IHostedService which calls Tasks in specific order
            //
            // IRepository
            //
            // using var dataContext = await _repository.Query<CurrencyEntity>(cancellationToken)
            // var query = await dataContext.Where
            //
            // var currency = new CurrencyEntity()
            //
            // var save = await _repository.Save(currency, cancellationToken);
            // await _repository.Save(cancellationToken, currency[]);
            // await _repository.Delete(cancellationToken, currency[]);
            //
            //
            // try with guids?
            
            var currencyList = new List<Currency>()
            {
                CreateSettingsHelper.CreateCurrencyIfNotExist(ISOCode:"EUR", exchangeRate:100),
                CreateSettingsHelper.CreateCurrencyIfNotExist(ISOCode:"GBP", exchangeRate:88)
            };

            var currencyRepository = Ucommerce.Infrastructure.ObjectFactory.Instance.Resolve<IRepository<Currency>>();
            var currentCurrencyList = currencyRepository.Select().ToList();

            
            foreach (Currency currency in currencyList)
            {
                var currentCurrency = currentCurrencyList.Find(x => x.ISOCode == currency.ISOCode);
                bool exists = currentCurrency != null;
                if (exists)
                {
                    //update
                    currentCurrency.ExchangeRate = currency.ExchangeRate;
                    currentCurrency.Save();
                    currency.Delete();
                }
                else
                {
                    //save
                    currency.Save();
                }
            }
            
            // foreach (var currency in currentCurrencyList.Except(currencyList))
            // {
            //     //delete
            //     currency.Deleted = true;
            //     currency.Save();
            // }
            
            //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            var priceGroupList = new List<PriceGroup>()
            {
                CreateSettingsHelper.CreatePriceGroupIfNotExist(name: "EUR 15 pct", currency: currencyRepository.Select(x => x.ISOCode == "EUR").FirstOrDefault(), vatRate: 0.15m)
            };
            
            var currentPriceGroupList = Ucommerce.Infrastructure.ObjectFactory.Instance.Resolve<IRepository<PriceGroup>>().Select().ToList(); 
            
            foreach (var priceGroup in priceGroupList)
            {
                var currentPriceGroup = currentPriceGroupList.Find(x => x.Name == priceGroup.Name);
                bool exists = currentPriceGroup != null;
                if (exists)
                {
                    //update
                    currentPriceGroup.VATRate = priceGroup.VATRate;
                    currentPriceGroup.Currency = priceGroup.Currency;
                    currentPriceGroup.Save();
                    priceGroup.Delete();
                }
                else
                {
                    //save
                    priceGroup.Save();
                }
            }
            
            foreach (var priceGroup in currentPriceGroupList.Except(priceGroupList))
            {
                //delete
                priceGroup.Deleted = true;
                priceGroup.Save();
            }

            // var euroCurrency = CreateSettingsHelper.CreateCurrencyIfNotExist(ISOCode:"EUR", exchangeRate:100);
            // var poundCurrency = CreateSettingsHelper.CreateCurrencyIfNotExist(ISOCode:"GBP", exchangeRate:88);
            //
            // var eur15PriceGroup = CreateSettingsHelper.CreatePriceGroupIfNotExist(name:"EUR 15 pct", currency:euroCurrency, vatRate:0.15m);

            return PipelineExecutionResult.Success;
        }
    }
}
