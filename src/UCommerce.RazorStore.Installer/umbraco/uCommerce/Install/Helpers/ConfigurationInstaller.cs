using System.Collections.Generic;
using System.Linq;
using UCommerce.EntitiesV2;
using UCommerce.EntitiesV2.Factories;
using UCommerce.Infrastructure;
using UCommerce.Security;
using umbraco.cms.businesslogic.web;

namespace UCommerce.RazorStore.Installer.Helpers
{
    public class ConfigurationInstaller
    {
        private Currency _defaultCurrency;
        private PriceGroup _defaultPriceGroup;
        private IList<Country> _countries = new List<Country>();

        public void Configure()
        {
            CreateCurrencies();
            CreatePriceGroups();
            CreateCountries();
            CreateShippingMethods();
            CreatePaymentMethods();
            CreateDataTypes();
            CreateProductDefinitions();
            ConfigureEmails();
        }

        private void CreateCurrencies()
        {
            _defaultCurrency = CreateCurrency("EUR", 100);
            CreateCurrency("GBP", 88);
        }

        private Currency CreateCurrency(string isoCode, int exchangeRate)
        {
            var currency = Currency.SingleOrDefault(c => c.ISOCode == isoCode) ?? new Currency();
            currency.Name = isoCode;
            currency.ISOCode = isoCode;
            currency.Deleted = false;
            currency.ExchangeRate = exchangeRate;
            currency.Save();

            return currency;
        }

        private void CreateCountries()
        {
            _countries.Add(CreateCountry("Denmark", "da-DK"));
            _countries.Add(CreateCountry("United Kingdom", "en-GB"));
        }

        private Country CreateCountry(string name, string cultureCode)
        {
            var country = Country.SingleOrDefault(c => c.Name == name) ?? new Country();
            country.Name = name;
            country.Culture = cultureCode;
            country.Deleted = false;
            country.Save();
            return country;
        }

        private void CreatePriceGroups()
        {
            _defaultPriceGroup = CreatePriceGroup("EUR 15 pct", _defaultCurrency, 15);
        }

        private PriceGroup CreatePriceGroup(string name, Currency currency, decimal vatRate)
        {
            var priceGroup = PriceGroup.SingleOrDefault(c => c.Name == name) ?? new PriceGroupFactory().NewWithDefaults(name);
            priceGroup.Name = name;
            priceGroup.Currency = currency;
            priceGroup.VATRate = vatRate;
            priceGroup.Deleted = false;
            priceGroup.Save();

            return priceGroup;
        }

        private void ConfigureEmails()
        {
            var docType = DocumentType.GetAllAsList().FirstOrDefault(t => t.Alias == "uCommerceEmail");
            if (docType == null)
                return;

            var emails = Document.GetDocumentsOfDocumentType(docType.Id);
            var emailContent = emails.FirstOrDefault(e => e.Text == "Order Confirmation Email");
            if (emailContent == null)
                return;

            var emailType = EmailProfileInformation.FirstOrDefault(p => p.EmailType.Name == "OrderConfirmation");
            foreach (var content in emailType.EmailProfile.EmailContents)
            {
                content.ContentId = emailContent.Id.ToString();
                content.Save();
            }
        }

        private void CreateShippingMethods()
        {
            CreateShippingMethod("Standard (Free)", 0, _defaultCurrency, _defaultPriceGroup);
            CreateShippingMethod("Express", 10, _defaultCurrency, _defaultPriceGroup);
        }

        private void CreateShippingMethod(string name, decimal shippingFee, Currency currency, PriceGroup priceGroup)
        {
            var shippingMethod = ShippingMethod.SingleOrDefault(x => x.Name == name) ?? new ShippingMethodFactory().NewWithDefaults(name);
            //foreach (var shippingMethodPrice in shippingMethod.ShippingMethodPrices)
            //{
            //    shippingMethod.RemoveShippingMethodPrice(shippingMethodPrice);
            //}
            //shippingMethod.AddShippingMethodPrice(new ShippingMethodPrice()
            //                                          {
            //                                              Price = shippingFee,
            //                                              Currency = currency,
            //                                              PriceGroup = priceGroup
            //                                          });
            shippingMethod.ClearEligibleCountries();
            foreach (var country in _countries)
            {
                shippingMethod.AddEligibleCountry(country);
            }
            shippingMethod.Save();
        }

        private void CreatePaymentMethods()
        {
            CreatePaymentMethod("Account", 0, _defaultCurrency, _defaultPriceGroup, 5);
            CreatePaymentMethod("Invoice", 0, _defaultCurrency, _defaultPriceGroup, 0);
        }

        private void CreatePaymentMethod(string name, decimal fee, Currency currency, PriceGroup priceGroup, decimal feePercentage)
        {
            var paymentMethod = PaymentMethod.SingleOrDefault(x => x.Name == name) ?? new PaymentMethodFactory().NewWithDefaults(name);
            paymentMethod.Deleted = false;
            paymentMethod.FeePercent = feePercentage;

            //paymentMethod.AddPaymentMethodFee(new PaymentMethodFee()
            //{
            //    Fee = fee,
            //    Currency = currency,
            //    PriceGroup = priceGroup
            //});
            paymentMethod.ClearEligibleCountries();
            foreach (var country in _countries)
            {
                paymentMethod.AddEligibleCountry(country);
            }
            paymentMethod.Save();
        }

        public void AssignAccessPermissionsToDemoStore()
        {
            var userService = ObjectFactory.Instance.Resolve<IUserService>();
            var user = userService.GetCurrentUser();

            var roleService = ObjectFactory.Instance.Resolve<IRoleService>();
            var roles = roleService.GetAllRoles();

            roleService.AddUserToRoles(user, roles);
        }

        private void CreateDataTypes()
        {
            CreateColourDropDownList();
        }

        private void CreateColourDropDownList()
        {
            var dataTypeEnum = CreateDataType("Colour", "Enum");

            dataTypeEnum.AddDataTypeEnum(GenerateColourDataTypeEnum("Blue", dataTypeEnum));
            dataTypeEnum.AddDataTypeEnum(GenerateColourDataTypeEnum("Green", dataTypeEnum));
            dataTypeEnum.AddDataTypeEnum(GenerateColourDataTypeEnum("Red", dataTypeEnum));
            dataTypeEnum.AddDataTypeEnum(GenerateColourDataTypeEnum("White", dataTypeEnum));
        }

        private DataType CreateDataType(string name, string dataType)
        {
            var dataTypeEnum = DataType.SingleOrDefault(x => x.TypeName == name) ?? new DataTypeFactory().NewWithDefaults(name);

            dataTypeEnum.TypeName = "Colour";
            dataTypeEnum.DefinitionName = dataType;
            dataTypeEnum.Nullable = false;
            dataTypeEnum.ValidationExpression = string.Empty;
            dataTypeEnum.BuiltIn = false;

            dataTypeEnum.Save();

            return dataTypeEnum;
        }

        private DataTypeEnum GenerateColourDataTypeEnum(string colour, DataType parentDataType)
        {
            var dataTypeEnum = DataTypeEnum.SingleOrDefault(x => x.Value == colour && x.DataType.DataTypeId == parentDataType.DataTypeId) ?? new DataTypeEnumFactory().NewWithDefaults(colour);
            dataTypeEnum.Deleted = false;
            dataTypeEnum.DataType = DataType.Get(parentDataType.DataTypeId);
            dataTypeEnum.Save();

            GenericHelpers.DoForEachCulture(language =>
            {
                if (dataTypeEnum.GetDescription(language) == null)
                    dataTypeEnum.AddDescription(new DataTypeEnumDescription { CultureCode = language, DisplayName = colour, Description = colour });
            });

            return dataTypeEnum;
        }

        private void CreateProductDefinitions()
        {
            CreateShirtProductDefinition();
            CreateShoeProductDefinition();
            CreateAccessoryProductDefinition();
        }

        private static ProductDefinition CreateProductDefinition(string name)
        {
            var productDefinition = ProductDefinition.SingleOrDefault(d => d.Name == name) ?? new ProductDefinition();

            productDefinition.Name = name;

            productDefinition.Save();
            return productDefinition;
        }

        private void CreateShirtProductDefinition()
        {
            var productDefinition = CreateProductDefinition("Shirt");
            AddProductDefinitionFieldIfDoesntExist(productDefinition, "CollarSize", "Number", true, true, "Collar Inches");
            AddProductDefinitionFieldIfDoesntExist(productDefinition, "Colour", "Colour", true, true, "Colour");
            AddProductDefinitionFieldIfDoesntExist(productDefinition, "ShowOnHomepage", "Boolean", false, false, "Show On Homepage");
        }

        private void CreateShoeProductDefinition()
        {
            var productDefinition = CreateProductDefinition("Shoe");
            AddProductDefinitionFieldIfDoesntExist(productDefinition, "ShoeSize", "ShortText", true, true, "Shoe Size");
            AddProductDefinitionFieldIfDoesntExist(productDefinition, "ShowOnHomepage", "Boolean", false, false, "Show On Homepage");
        }

        private void CreateAccessoryProductDefinition()
        {
            var productDefinition = CreateProductDefinition("Accessory");
            AddProductDefinitionFieldIfDoesntExist(productDefinition, "ShowOnHomepage", "Boolean", false, false, "Show On Homepage");
        }

        private void AddProductDefinitionFieldIfDoesntExist(ProductDefinition definition, string name, string typeName, bool displayOnWebsite, bool variantProperty, string displayName)
        {
            if (definition.GetDefinitionFields().Any(f => f.Name == name))
                return;

            var field = ProductDefinitionField.SingleOrDefault(f => f.Name == name && f.ProductDefinition.ProductDefinitionId == definition.ProductDefinitionId) ?? new ProductDefinitionFieldFactory().NewWithDefaults(name);
            field.Name = name;
            field.DataType = DataType.SingleOrDefault(d => d.TypeName == typeName);
            field.Deleted = false;
            field.Multilingual = false;
            field.DisplayOnSite = displayOnWebsite;
            field.IsVariantProperty = variantProperty;
            field.RenderInEditor = true;

            //Helpers.DoForEachCulture(language =>
            //    {
            //        if (field.GetDescription(language) == null)
            //            field.AddProductDefinitionFieldDescription(new ProductDefinitionFieldDescription { CultureCode = language, DisplayName = displayName, Description = displayName });
            //    });

            definition.AddProductDefinitionField(field);
            definition.Save();
        }
    }
}