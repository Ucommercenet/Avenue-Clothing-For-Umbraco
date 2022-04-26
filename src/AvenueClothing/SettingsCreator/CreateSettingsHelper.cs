using System.Linq;
using Ucommerce;
using Ucommerce.EntitiesV2;
using Ucommerce.Infrastructure.Components.Windsor;

namespace AvenueClothing.SettingsCreator
{
    public class CreateSettingsHelper
    {
        [Mandatory] public IRepository<Definition> DefinitionRepository { get; set; }
        [Mandatory] public IRepository<DefinitionType> DefinitionTypeRepository { get; set; }
        [Mandatory] public IRepository<DefinitionField> DefinitionFieldRepository { get; set; }
        [Mandatory] public IRepository<Country> CountryRepository { get; set; }
        [Mandatory] public IRepository<DataType> DataTypeRepository { get; set; }
        [Mandatory] public IRepository<DataTypeEnum> DataTypeEnumRepository { get; set; }
        [Mandatory] public IRepository<EmailType> EmailTypeRepository { get; set; }
        [Mandatory] public IRepository<EmailProfile> EmailProfileRepository { get; set; }
        [Mandatory] public IRepository<EmailProfileInformation> EmailProfileInformationRepository { get; set; }
        [Mandatory] public IRepository<EmailContent> EmailContentRepository { get; set; }
        [Mandatory] public IRepository<PriceGroup> PriceGroupRepository { get; set; }
        [Mandatory] public IRepository<Currency> CurrencyRepository { get; set; }
        [Mandatory] public IRepository<ProductRelationType> ProductRelationTypeRepository { get; set; }
        [Mandatory] public IRepository<ProductDefinition> ProductDefinitionRepository { get; set; }
        [Mandatory] public IRepository<DefinitionRelation> DefinitionRelationRepository { get; set; }
        [Mandatory] public IRepository<ProductDefinitionField> ProductDefinitionFieldRepository { get; set; }
        [Mandatory] public IRepository<ProductDefinitionRelation> ProductDefinitionRelationRepository { get; set; }
        [Mandatory] public IRepository<ProductDefinitionFieldDescription> ProductDefinitionFieldDescriptionRepository { get; set; }
        [Mandatory] public IRepository<DataTypeEnumDescription> DataTypeEnumDescriptionRepository { get; set; }
        [Mandatory] public IRepository<OrderNumberSerie> OrderNumberSerieRepository { get; set; }
        [Mandatory] public IRepository<ProductRelation> ProductRelationRepository { get; set; }
        
        public CreateSettingsHelper(IRepository<Definition> definitionRepository, IRepository<DefinitionType> definitionTypeRepository, IRepository<DefinitionField> definitionFieldRepository, IRepository<Country> countryRepository, IRepository<DataType> dataTypeRepository, IRepository<DataTypeEnum> dataTypeEnumRepository, IRepository<EmailType> emailTypeRepository, IRepository<EmailProfile> emailProfileRepository, IRepository<EmailProfileInformation> emailProfileInformationRepository, IRepository<EmailContent> emailContentRepository, IRepository<Currency> currencyRepository, IRepository<PriceGroup> priceGroupRepository, IRepository<ProductRelationType> productRelationTypeRepository, IRepository<ProductDefinition> productDefinitionRepository, IRepository<DefinitionRelation> definitionRelationRepository, IRepository<ProductDefinitionField> productDefinitionFieldRepository, IRepository<ProductDefinitionRelation> productDefinitionRelationRepository, IRepository<DataTypeEnumDescription> dataTypeEnumDescriptionRepository, IRepository<OrderNumberSerie> orderNumberSerieRepository, IRepository<ProductRelation> productRelationRepository)
        {   
            DefinitionRepository = definitionRepository;
            DefinitionTypeRepository = definitionTypeRepository;
            DefinitionFieldRepository = definitionFieldRepository;
            CountryRepository = countryRepository;
            DataTypeRepository = dataTypeRepository;
            DataTypeEnumRepository = dataTypeEnumRepository;
            EmailTypeRepository = emailTypeRepository;
            EmailProfileRepository = emailProfileRepository;
            EmailProfileInformationRepository = emailProfileInformationRepository;
            EmailContentRepository = emailContentRepository;
            CurrencyRepository = currencyRepository;
            PriceGroupRepository = priceGroupRepository;
            ProductRelationTypeRepository = productRelationTypeRepository;
            ProductDefinitionRepository = productDefinitionRepository;
            DefinitionRelationRepository = definitionRelationRepository;
            ProductDefinitionFieldRepository = productDefinitionFieldRepository;
            ProductDefinitionRelationRepository = productDefinitionRelationRepository;
            DataTypeEnumDescriptionRepository = dataTypeEnumDescriptionRepository;
            OrderNumberSerieRepository = orderNumberSerieRepository;
            ProductRelationRepository = productRelationRepository;
        }
        
        /// <summary>
        /// Creates a new Definition
        /// <br/>
        /// If one is already in the database, it is fetched instead of creating a new one.
        /// <br/>
        /// Definition can contain different fields for holding information, and can be for both Catalogs, Categories, Catalog Groups, Campaign Items, Payment Methods.
        /// <br/>
        /// var myCategoryDefinition = CreateSettingsHelper.CreateDefinitionIfNotExist("My Category Definition", <see cref="CreateDefinitionTypeIfNotExist">categoryDefinitionType</see>, "Configuration for my Category");
        /// </summary>
        /// <param name="name">The name of the Definition</param>
        /// <param name="definitionType">The DefinitionType to use for this Definition, find/create one with <see cref="CreateDefinitionTypeIfNotExist"/></param>
        /// <param name="description">The description for the Definition</param>
        /// <returns>The Definition, either found or created.</returns>
        public Definition CreateDefinitionIfNotExist(string name, DefinitionType definitionType, string description)
        {
            var definition = DefinitionRepository.Select().FirstOrDefault(x => x.Name == name);
            
            if (definition != null)
                return definition;
            
            definition = new global::Ucommerce.EntitiesV2.Definition
            {
                Name = name,
                DefinitionType = definitionType,
                Description = description,
                SortOrder = 0,
                BuiltIn = false,
                Deleted = false
            };
            definition.Save();

            return definition;
        }
        
        /// <summary>
        /// Creates a new DefinitionType
        /// <br/>
        /// If one is already in the database, it is fetched instead of creating a new one.
        /// <br/>
        /// DefinitionType is the Type which a Definition lives in. Usually you do not want to create new ones of these, only fetch the current ones.
        /// <br/>
        /// var paymentMethodDefinitionType = CreateSettingsHelper.CreateDefinitionTypeIfNotExist("PaymentMethod Definitions");
        /// </summary>
        /// <param name="name">The name of the DefinitionType</param>
        /// <returns>The DefinitionType, either found or created.</returns>
        public DefinitionType CreateDefinitionTypeIfNotExist(string name)
        {
            var definitionType = DefinitionTypeRepository.Select().FirstOrDefault(x => x.Name == name);
            
            if (definitionType != null)
                return definitionType;
            
            definitionType = new global::Ucommerce.EntitiesV2.DefinitionType
            {
                Name = name,
                Deleted = false
            };
            definitionType.Save();

            return definitionType;
        }
        
        /// <summary>
        /// Creates a new DefinitionField
        /// <br/>
        /// If one is already in the database, it is fetched instead of creating a new one.
        /// <br/>
        /// DefinitionField is used to add a Field to a Definition with a specific DataType. This should not be confused with ProductDefinitionField which is adding DefinitionFields to Products.
        /// <br/>
        /// var myCategoryDefinitionField = CreateSettingsHelper.CreateDefinitionFieldIfNotExist("ShortTextOnCategory", myCategoryDefinition, shortTextDataType); 
        /// </summary>
        /// <param name="name">The Name of the DefinitionField</param>
        /// <param name="definition">The Definition you want to add a Field to, find/create one with <see cref="CreateDefinitionIfNotExist"/></param>
        /// <param name="dataType">The DataType you want to use for this field, find/create one with <see cref="CreateDataTypeIfNotExist"/></param>
        /// <returns>The DefinitionField, either found or created</returns>
        public DefinitionField CreateDefinitionFieldIfNotExist(string name, Definition definition, DataType dataType)
        {
            var definitionField = DefinitionFieldRepository.Select().FirstOrDefault(x => x.Name == name && x.Definition == definition);
            
            if (definitionField != null)
                return definitionField;
            
            definitionField = new global::Ucommerce.EntitiesV2.DefinitionField
            {
                Name = name,
                Definition = definition,
                SortOrder = 0,
                DataType = dataType,
                BuiltIn = false,
                Deleted = false
            };
            definitionField.Save();

            return definitionField;
        }
        
        /// <summary>
        /// Creates a new Country
        /// <br/>
        /// If one is already in the database, it is fetched instead of creating a new one.
        /// <br/>
        /// Country is used to allow different geopraphical limitations in the Payment Methods and Shipping Methods in the back-office.
        /// <br/>
        /// var ukCountry = CreateSettingsHelper.CreateCountryIfNotExist(name: "United Kingdom", culture: "en-GB");
        /// </summary>
        /// <param name="name">The name of the country.</param>
        /// <param name="culture">The CultureCode of the country.</param>
        /// <returns>The country, either found or created.</returns>
        public Country CreateCountryIfNotExist(string name, string culture)
        {
            var country = CountryRepository.Select().FirstOrDefault(x => x.Name == name);
            
            if (country != null)
                return country;

            country = new global::Ucommerce.EntitiesV2.Country
            {
                Name = name,
                Culture = culture,
                Deleted = false
            };
            country.Save();

            return country;
        }
        
        /// <summary>
        /// Creates a new DataTypeEnum
        /// <br/>
        /// If one is already in the database, it is fetched instead of creating a new one.
        /// <br/>
        /// DataTypeEnum is used for adding values to DataTypes of the type Enum.
        /// <br/>
        /// var blueColourType = CreateSettingsHelper.CreateDataTypeEnumIfNotExist(<see cref="CreateDataTypeIfNotExist">colourDataType</see>, "Blue");
        /// </summary>
        /// <param name="dataType">The DataType you want to add an Enum option to, find/create one with <see cref="CreateDataTypeIfNotExist"/></param>
        /// <param name="value">The value of the option. Use <see cref="CreateDataTypeEnumDescriptionIfNotExist"/> if you want to add translations of an Enum</param>
        /// <returns>The DataTypeEnum, either found or created.</returns>
        public DataTypeEnum CreateDataTypeEnumIfNotExist(DataType dataType, string value)
        {
            var dataTypeEnum = DataTypeEnumRepository.Select().FirstOrDefault(x => x.Value == value);
            
            if (dataTypeEnum != null)
                return dataTypeEnum;
            
            dataTypeEnum = new global::Ucommerce.EntitiesV2.DataTypeEnum
            {
                Value = value,
                DataType = dataType,
                Deleted = false
            };
            dataTypeEnum.Save();

            return dataTypeEnum;
        }

        /// <summary>
        /// Creates a new DataType.
        /// <br/>
        /// If one is already in the database, it is fetched instead of creating a new one.
        /// <br/>
        /// DataType is the Types used for DefinitionFields, to specify what the Type of data is saved on this field. Create DefinitionFields with <see cref="CreateDefinitionFieldIfNotExist"/>. Remember to add Values with <see cref="CreateDataTypeEnumIfNotExist"/> if you select Enum.
        /// <br/>
        /// var colourDataType = CreateSettingsHelper.CreateDataTypeIfNotExist("Colour", "", "Enum");
        /// </summary>
        /// <param name="name">The name of the DataType.</param>
        /// <param name="validationExpression">The ValidationExpression if the DataType should have one.</param>
        /// <param name="definitionName">The name of the Editor Type, out of the box options include: "DatePicker", "DateTime", "Number", "RichText", "Boolean", "EnumMultiSelect", "Enum", "LongText", "ShortText", "ContentPickerMultiSelect", "ImagePickerMultiSelect", "DefinitionTypePicker", "DefinitionPicker", "ComponentTypePicker", "List", "EmailContent", "Media", "Content"</param>
        /// <returns>The DataType, either found or created</returns>
        public DataType CreateDataTypeIfNotExist(string name, string validationExpression, string definitionName)
        {
            var dataType = DataTypeRepository.Select().FirstOrDefault(x => x.TypeName == name);
            
            if (dataType != null)
                return dataType;
            
            dataType = new global::Ucommerce.EntitiesV2.DataType
            {
                Name = name,
                Nullable = false,
                ValidationExpression = validationExpression,
                DefinitionName = definitionName,
                Deleted = false
            };
            dataType.Save();

            return dataType;
        }
        
        /// <summary>
        /// Creates a new EmailContent.
        /// <br/>
        /// If one is already in the database, it is fetched instead of creating a new one.
        /// <br/>
        /// EmailContent is used to tie a Content ID from the CMS, to the EmailProfile, which allows it to use the formatting for sending emails, in multiple languages.
        /// <br/>
        /// var orderConfirmationEmailContentGB = CreateSettingsHelper.CreateEmailContentIfNotExist(emailType: <see cref="CreateEmailTypeIfNotExist">confirmationEmailType</see>, emailProfile: <see cref="CreateEmailProfileIfNotExist">defaultEmailProfile</see>, subject:"Order Confirmation for your purchase", contentId:emailCmsContentId, cultureCode:"en-GB");
        /// </summary>
        /// <param name="emailType">The EmailType to add this content to, find/create one with <see cref="CreateEmailTypeIfNotExist"/></param>
        /// <param name="emailProfile">The EmailProfile to add this content to, find/create one with <see cref="CreateEmailProfileIfNotExist"/></param>
        /// <param name="subject">Subject of the Email</param>
        /// <param name="contentId">Content ID from the CMS which points to the Content Page of the EmailTemplate</param>
        /// <param name="cultureCode">Culture Code of the EmailContent</param>
        /// <returns>The EmailContent, either found or created.</returns>
        public EmailContent CreateEmailContentIfNotExist(EmailType emailType, EmailProfile emailProfile, string subject, string contentId, string cultureCode)
        {
            var emailContent = EmailContentRepository.Select().FirstOrDefault(x => x.EmailType == emailType && x.EmailProfile == emailProfile && x.CultureCode == cultureCode);
            
            if (emailContent != null)
                return emailContent;
            
            emailContent = new global::Ucommerce.EntitiesV2.EmailContent()
            {
                EmailType = emailType,
                EmailProfile = emailProfile,
                Subject = subject,
                ContentId = contentId,
                CultureCode = cultureCode
            };

            emailProfile.EmailContents.Add(emailContent);
            emailProfile.Save();
            emailContent.Save();
            return emailContent;
        }

        /// <summary>
        /// Creates a new EmailProfileInformation
        /// <br/>
        /// If one is already in the database, it is fetched instead of creating a new one.
        /// <br/>
        /// EmailProfileInformation contains the from information of the EmailProfile for a specific EmailType.
        /// <br/>
        /// var uCommerceDemoShopEmailProfileInformation = CreateSettingsHelper.CreateEmailProfileInformationIfNotExist(emailType:<see cref="CreateEmailTypeIfNotExist">confirmationEmailType</see>, emailProfile:<see cref="CreateEmailProfileIfNotExist">defaultEmailProfile</see>, fromName:"uCommerce Demo Shop", fromAddress: "demo@uCommerce.dk");
        /// </summary>
        /// <param name="emailType">The EmailType to add the Information to, find/create one with <see cref="CreateEmailTypeIfNotExist"/></param>
        /// <param name="emailProfile">The EmailProfile to add the Information to, find/createo one with <see cref="CreateEmailProfileIfNotExist"/></param>
        /// <param name="fromName">The fromname of the email.</param>
        /// <param name="fromAddress">The address from which you want to send the email.</param>
        /// <param name="ccAddress">The cc address.</param>
        /// <param name="bccAddress">The bcc address.</param>
        /// <returns>The EmailProfileInformation, either found or created.</returns>
        public EmailProfileInformation CreateEmailProfileInformationIfNotExist(EmailType emailType, EmailProfile emailProfile, string fromName, string fromAddress, string ccAddress = "", string bccAddress = "")
        {
            var emailProfileInformation = EmailProfileInformationRepository.Select().FirstOrDefault(x => x.EmailType == emailType && x.EmailProfile == emailProfile && x.FromAddress == fromAddress);
            
            if (emailProfileInformation != null)
                return emailProfileInformation;
            
            emailProfileInformation = new global::Ucommerce.EntitiesV2.EmailProfileInformation()
            {
                EmailType = emailType, //you should make an EmailProfileInformation for each unique combination of Profile+Type
                EmailProfile = emailProfile,
                FromName = fromName, //title of sender
                FromAddress = fromAddress,
                CcAddress = ccAddress,
                BccAddress = bccAddress
            };
            emailProfileInformation.Save();
            return emailProfileInformation;
        }

        /// <summary>
        /// Creates a new EmailProfile
        /// <br/>
        /// If one is already in the database, it is fetched instead of creating a new one.
        /// <br/>
        /// EmailProfile is the Profile with which you want to add Content to your EmailTypes. EmailProfile can then be either Brand separation, shop separation etc. Remember to add Information with <see cref="CreateEmailProfileInformationIfNotExist"/>, and Content with <see cref="CreateEmailContentIfNotExist"/>
        /// <br/>
        /// var defaultEmailProfile = CreateSettingsHelper.CreateEmailProfileIfNotExist(name:"Default");
        /// </summary>
        /// <param name="name">Name of the EmailProfile.</param>
        /// <returns>The EmailProfile, either found or created</returns>
        public EmailProfile CreateEmailProfileIfNotExist(string name)
        {
            var emailProfile = EmailProfileRepository.Select().FirstOrDefault(x => x.Name == name);
            
            if (emailProfile != null)
                return emailProfile;
            
            emailProfile = new global::Ucommerce.EntitiesV2.EmailProfile()
            {
                Name = name, //eg Brand separation, shop separation or other.
                Deleted = false
            };
            emailProfile.Save();

            return emailProfile;
        }

        /// <summary>
        /// Creates a new EmailType
        /// <br/>
        /// If one is already in the database, it is fetched instead of creating a new one.
        /// <br/>
        /// EmailType is the types of emails available to the EmailProfile. Remember to add an EmailProfile with <see cref="CreateEmailProfileIfNotExist"/>
        /// <br/>
        /// var confirmationEmailType = CreateSettingsHelper.CreateEmailTypeIfNotExist(name: "OrderConfirmation", description:"E-mail which will be sent to the customer after checkout.");
        /// </summary>
        /// <param name="name">The name of the EmailType, eg. OrderConfirmation, OrderCancellation etc.</param>
        /// <param name="description">The description of the EmailType.</param>
        /// <returns>The EmailType, either found or created.</returns>
        public EmailType CreateEmailTypeIfNotExist(string name, string description)
        {
            var emailType = EmailTypeRepository.Select().FirstOrDefault(x => x.Name == name);
            
            if (emailType != null)
                return emailType;
            
            emailType = new global::Ucommerce.EntitiesV2.EmailType()
            {
                Name = name,
                Description = description,
                Deleted = false
            };
            emailType.Save();

            return emailType;
        }
        
        /// <summary>
        /// Creates a new PriceGroup
        /// <br/>
        /// If one is already in the database, it is fetched instead of creating a new one.
        /// <br/>
        /// PriceGroup is needed for storing prices on Products. Multiple PriceGroups can give you multiple Price Points.
        /// <br/>
        /// var eur15PriceGroup = CreateSettingsHelper.CreatePriceGroupIfNotExist(name:"EUR 15 pct", currency: <see cref="CreateCurrencyIfNotExist">euroCurrency</see>, vatRate:0.15m);
        /// </summary>
        /// <param name="name">The name of the PriceGroup</param>
        /// <param name="currency">The currency of the PriceGroup, find/create one with <see cref="CreateCurrencyIfNotExist"/></param>
        /// <param name="vatRate">The VATrate of the PriceGroup</param>
        /// <returns>The PriceGroup, either found or created.</returns>
        public PriceGroup CreatePriceGroupIfNotExist(string name, Currency currency, decimal vatRate)
        {
            var priceGroup = PriceGroupRepository.Select().FirstOrDefault(x => x.Name == name);
            
            if (priceGroup != null)
                return priceGroup;
            
            priceGroup = new global::Ucommerce.EntitiesV2.PriceGroup()
            {
                Name = name,
                VATRate = vatRate,
                Currency = currency,
                Deleted = false
            };
            priceGroup.Save();

            return priceGroup;
        }

        /// <summary>
        /// Creates a new Currency.
        /// <br/>
        /// If one is already in the database, it is fetched instead of creating a new one.
        /// <br/>
        /// Currency is used when creating a PriceGroup.
        /// <br/>
        /// var euroCurrency = CreateSettingsHelper.CreateCurrencyIfNotExist(ISOCode:"EUR", exchangeRate:100);
        /// </summary>
        /// <param name="ISOCode">The ISOCode of the Currency eg. "EUR", "USD", "GBP"</param>
        /// <param name="exchangeRate">The exchange rate of the Currency</param>
        /// <returns>The Currency, either found or created.</returns>
        public Currency CreateCurrencyIfNotExist(string ISOCode, int exchangeRate)
        {
            var currency = CurrencyRepository.Select().FirstOrDefault(x => x.ISOCode == ISOCode);
            
            if (currency != null)
                return currency;
            
            currency = new global::Ucommerce.EntitiesV2.Currency()
            {
                Name = ISOCode,
                ISOCode = ISOCode,
                ExchangeRate = exchangeRate,
                Deleted = false
            };
            currency.Save();

            return currency;
        }
        
        /// <summary>
        /// Creates a new ProductRelationType.
        /// <br/>
        /// If one is already in the database, it is fetched instead of creating a new one.
        /// <br/>
        /// ProductRelationType is used to create different layers of Product Relations, and is selected when adding ProductRelations in the back-office.
        /// <br/>
        /// var myProductRelationType = CreateSettingsHelper.CreateProductRelationTypeIfNotExist(name: "MyProductRelationType", description:"Goes well with");
        /// </summary>
        /// <param name="name">The name of the ProductRelationType</param>
        /// <param name="description">The description the ProductRelationType should be created with</param>
        /// <returns>The ProductRelationType, either found or created.</returns>
        public ProductRelationType CreateProductRelationTypeIfNotExist(string name, string description)
        {
            var productRelationType = ProductRelationTypeRepository.Select().FirstOrDefault(x => x.Name == name);
            
            if (productRelationType != null)
                return productRelationType;
            
            productRelationType = new global::Ucommerce.EntitiesV2.ProductRelationType()
            {
                Name = name,
                Description = description
            };
            productRelationType.Save();

            return productRelationType;
        }
        
        /// <summary>
        /// Creates a new DefinitionRelation.
        /// <br/>
        /// If one is already in the database, it is fetched instead of creating a new one.
        /// <br/>
        /// DefinitionRelation specifies a relationship between two Definitions, where the child inherits the fields of the parent.
        /// <br/>
        /// var myDefinitionRelation = CreateSettingsHelper.CreateDefinitionRelationIfNotExist(<see cref="CreateDefinitionIfNotExist">myCategoryDefinition</see>,<see cref="CreateDefinitionIfNotExist">myParentCategoryDefinition</see>);
        /// </summary>
        /// <param name="childDefinition">The Definition which should be the child of the relationship, find/create one with <see cref="CreateDefinitionIfNotExist"/></param>
        /// <param name="parentDefinition">The Definition which should be the parent of the relationship, find/create one with <see cref="CreateDefinitionIfNotExist"/></param>
        /// <returns>The DefinitionRelation, either found or created</returns>
        public DefinitionRelation CreateDefinitionRelationIfNotExist(Definition childDefinition, Definition parentDefinition)
        {
            var definitionRelation = DefinitionRelationRepository.Select().FirstOrDefault(x => x.Definition == childDefinition && x.ParentDefinition == parentDefinition);
            
            if (definitionRelation != null)
                return definitionRelation;
            
            definitionRelation = new global::Ucommerce.EntitiesV2.DefinitionRelation()
            {
                Definition = childDefinition,
                ParentDefinition = parentDefinition
            };
            definitionRelation.Save();

            return definitionRelation;
        }

        /// <summary>
        /// Creates a new ProductDefinition.
        /// <br/>
        /// If one is already in the database, it is fetched instead of creating a new one.
        /// <br/>
        /// ProductDefinition is needed when creating products, and will be where you add DefinitionFields for the specific data you want to save on the Product. Add ProductDefinitionFields with <see cref="CreateProductDefinitionFieldIfNotExist"/>
        /// <br/>
        /// var shirtDefinition = CreateSettingsHelper.CreateProductDefinitionIfNotExist("Shirt");
        /// </summary>
        /// <param name="name">The name of the ProductDefinition</param>
        /// <returns>The ProductDefinition, either found or created.</returns>
        public ProductDefinition CreateProductDefinitionIfNotExist(string name)
        {
            var productDefinition = ProductDefinitionRepository.Select().FirstOrDefault(x => x.Name == name);
            
            if (productDefinition != null)
                return productDefinition;
            
            productDefinition = new global::Ucommerce.EntitiesV2.ProductDefinition()
            {
                Name = name,
                Description = null,
                Deleted = false
            };
            productDefinition.Save();

            return productDefinition;
        }

        /// <summary>
        /// Creates a new ProductDefinitionField.
        /// <br/>
        /// If one is already in the database, it is fetched instead of creating a new one.
        /// <br/>
        /// ProductDefinitionField is needed for adding custom fields to a ProductDefinition.
        /// <br/>
        /// var collarSizeField = CreateSettingsHelper.CreateProductDefinitionFieldIfNotExist(<see cref="CreateProductDefinitionIfNotExist">shirtDefinition</see>, "CollarSize", <see cref="CreateDataTypeIfNotExist">shortTextDataType</see>, false, true, true, true);
        /// </summary>
        /// <param name="definition">The ProductDefinition that this Field should be added to. Find/create one with <see cref="CreateProductDefinitionIfNotExist"/></param>
        /// <param name="name">The name of the ProductDefinitionField</param>
        /// <param name="dataType">The DataType of the ProductDefinitionField, find/create one with <see cref="CreateDataTypeIfNotExist"/></param>
        /// <param name="multilingual">If the ProductDefinitionField should be multilingual, add multilingual values with <see cref="CreateProductDefinitionFieldDescriptionIfNotExist"/></param>
        /// <param name="displayOnWebSite">If the ProductDefinitionField should be displayed on the website.</param>
        /// <param name="isVariantProperty">If the ProductDefinitionField should be a variant property</param>
        /// <param name="renderInEditor">If the ProductDefinitionField should be rendered in the back-office view.</param>
        /// <returns>The ProductDefinitionField, either found or created</returns>
        public ProductDefinitionField CreateProductDefinitionFieldIfNotExist(ProductDefinition definition, string name, DataType dataType, bool multilingual, bool displayOnWebSite, bool isVariantProperty, bool renderInEditor)
        {
            var productDefinitionField = ProductDefinitionFieldRepository.Select().FirstOrDefault(x => x.Name == name && x.ProductDefinition.ProductDefinitionId == definition.ProductDefinitionId);
            
            if (productDefinitionField != null)
                return productDefinitionField;
            
            productDefinitionField = new global::Ucommerce.EntitiesV2.ProductDefinitionField()
            {
                Name = name,
                DataType = dataType,
                Multilingual = multilingual,
                DisplayOnSite = displayOnWebSite,
                IsVariantProperty = isVariantProperty,
                RenderInEditor = renderInEditor,
                Deleted = false
            };
            definition.AddProductDefinitionField(productDefinitionField);
            definition.Save();

            return productDefinitionField;
        }

        /// <summary>
        /// Creates a new ProductDefinitionRelation.
        /// <br/>
        /// If one is already in the database, it is fetched instead of creating a new one.
        /// <br/>
        /// ProductDefinitionRelation specifies a relationship between two ProductDefinitions, where the child inherits the fields of the parent.
        /// <br/>
        /// var shirtProductDefinitionRelation = CreateSettingsHelper.CreateProductDefinitionRelationIfNotExist(<see cref="CreateProductDefinitionIfNotExist">shirtDefinition</see>, <see cref="CreateProductDefinitionIfNotExist">allProductsParentDefinition)</see>;
        /// </summary>
        /// <param name="childDefinition">The ProductDefinition which should be the child of the relationship, find/create one with <see cref="CreateProductDefinitionIfNotExist"/></param>
        /// <param name="parentDefinition">The ProductDefinition which should be the parent of the relationship, find/create one with <see cref="CreateProductDefinitionIfNotExist"/></param>
        /// <returns>The ProductDefinitionRelation, either found or created</returns>
        public ProductDefinitionRelation CreateProductDefinitionRelationIfNotExist(ProductDefinition childDefinition, ProductDefinition parentDefinition)
        {
            var productDefinitionRelation = ProductDefinitionRelationRepository.Select().FirstOrDefault(x => x.ProductDefinition == childDefinition && x.ParentProductDefinition == parentDefinition);
            
            if (productDefinitionRelation != null)
                return productDefinitionRelation;
            
            productDefinitionRelation = new global::Ucommerce.EntitiesV2.ProductDefinitionRelation()
            {
                ProductDefinition = childDefinition,
                ParentProductDefinition = parentDefinition
            };
            productDefinitionRelation.Save();

            return productDefinitionRelation;
        }

        /// <summary>
        /// Creates a new ProductDefinitionFieldDescription.
        /// <br/>
        /// If one is already in the database, it is fetched instead of creating a new one.
        /// <br/>
        /// ProductDefinitionFieldDescription is used for adding multilingual data to a ProductDefinitionField.
        /// <br/>
        /// var colourProductDefinitionFieldDescriptionEnglish = CreateSettingsHelper.CreateProductDefinitionFieldDescriptionIfNotExist(<see cref="CreateProductDefinitionFieldIfNotExist">colourField</see>, "en-US", "Colour", "");
        /// </summary>
        /// <param name="productDefinitionField">The ProductDefinitionField which this ProductDefinitionFieldDescription is for. Find/create one with <see cref="CreateProductDefinitionFieldIfNotExist"/></param>
        /// <param name="cultureCode">The CultureCode for the ProductDefinitionFieldDescription</param>
        /// <param name="displayName">The DisplayName for the ProductDefinitionField in the specific language</param>
        /// <param name="description">The Description for the ProductDefinitionField in the specific language</param>
        /// <returns>The ProductDefinitionFieldDescription, either found or created</returns>
        public ProductDefinitionFieldDescription CreateProductDefinitionFieldDescriptionIfNotExist(ProductDefinitionField productDefinitionField, string cultureCode, string displayName, string description)
        {
            var productDefinitionFieldDescription = ProductDefinitionFieldDescriptionRepository.Select().FirstOrDefault(x => x.ProductDefinitionField == productDefinitionField && x.CultureCode == cultureCode);

            if (productDefinitionFieldDescription != null)
                return productDefinitionFieldDescription;

            productDefinitionFieldDescription = new global::Ucommerce.EntitiesV2.ProductDefinitionFieldDescription()
            {
                ProductDefinitionField = productDefinitionField,
                CultureCode = cultureCode,
                DisplayName = displayName,
                Description = description
            };
            productDefinitionFieldDescription.Save();

            return productDefinitionFieldDescription;
        }
        
        /// <summary>
        /// Creates a new DataTypeEnumDescription.
        /// <br/>
        /// If one is already in the database, it is fetched instead of creating a new one.
        /// <br/>
        /// DataTypeEnumDescription is needed for adding multilingual values to a DataTypeEnum.
        /// <br/>
        /// var blueUK = CreateSettingsHelper.CreateDataTypeEnumDescriptionIfNotExist(<see cref="CreateDataTypeEnumIfNotExist">blueColourType</see>, "en-UK", "Blue");
        /// </summary>
        /// <param name="dataTypeEnum">The DataTypeEnum which this DataTypeEnumDescription is for. Find/create one with <see cref="CreateDataTypeEnumIfNotExist"/></param>
        /// <param name="cultureCode">The CultureCode for the DataTypeEnumDescription</param>
        /// <param name="displayName">The DisplayName for the DataType in the specific language</param>
        /// <param name="description">The Description for the DataTypeEnum in the specific language</param>
        /// <returns>The DataTypeEnumDescription, either found or created</returns>
        public DataTypeEnumDescription CreateDataTypeEnumDescriptionIfNotExist(DataTypeEnum dataTypeEnum,
            string cultureCode,
            string displayName,
            string description = null)
        {
            var dataTypeEnumDescription = DataTypeEnumDescriptionRepository.Select().FirstOrDefault(x => x.DataTypeEnum == dataTypeEnum && x.CultureCode == cultureCode);

            if (dataTypeEnumDescription != null)
                return dataTypeEnumDescription;

            dataTypeEnumDescription = new global::Ucommerce.EntitiesV2.DataTypeEnumDescription()
            {
                DataTypeEnum = dataTypeEnum,
                CultureCode = cultureCode,
                DisplayName = displayName,
                Description = description
            };
            dataTypeEnumDescription.Save();

            return dataTypeEnumDescription;
        }

        /// <summary>
        /// Creates a new OrderNumberSerie.
        /// <br/>
        /// If one is already in the database, it is fetched instead of creating a new one.
        /// <br/>
        /// OrderNumberSerie is used when a basket becomes an order, and is assigned an OrderNumber.
        /// <br/>
        /// var exampleOrderNumberSerie = CreateSettingsHelper.CreateOrderNumberSerieIfNotExist("Example","TEST-");
        /// </summary>
        /// <param name="name">Name of the OrderNumberSerie</param>
        /// <param name="prefix">Prefix eg: "TEST-14"</param>
        /// <param name="increment">Size of OrderNumber increment, defaulting to 1</param>
        /// <param name="currentNumber">Specifies the starting number for this OrderNumberSerie, defaulting to 0</param>
        /// <returns>The OrderNumberSerie, either found or created</returns>
        public OrderNumberSerie CreateOrderNumberSerieIfNotExist(string name, string prefix, int increment = 1, int currentNumber = 0)
        {
            var orderNumberSerie = OrderNumberSerieRepository.Select().FirstOrDefault(x => x.OrderNumberName == name);

            if (orderNumberSerie != null)
                return orderNumberSerie;

            orderNumberSerie = new global::Ucommerce.EntitiesV2.OrderNumberSerie()
            {
                Name = name,
                OrderNumberName = name,
                Deleted = false,
                Prefix = prefix,
                Increment = increment,
                CurrentNumber = currentNumber
            };
            orderNumberSerie.Save();

            return orderNumberSerie;
        }
    }
}
