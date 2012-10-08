namespace uCommerce.RazorStore.Umbraco.ucommerce.Install
{
    using System;
    using System.Linq;

    using UCommerce.EntitiesV2;
    using UCommerce.EntitiesV2.Factories;
    using UCommerce.Infrastructure;
    using UCommerce.Infrastructure.Globalization;

    public partial class DemoStoreInstaller : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnInstall_Click(object sender, EventArgs e)
        {
            if (chkSettings.Checked)
                ConfigureSettings();

            if (chkCatalog.Checked)
                ConfigureCatalogue();
        }

        private void ConfigureSettings()
        {
            CreateDataTypes();
            CreateProductDefinitions();
        }

        private void ConfigureCatalogue()
        {
            var catalogGroup = CreateCatalogGroup();
            var catalog = CreateProductCatalog(catalogGroup);
            CreateCatalogue(catalog);
        }

        private ProductCatalogGroup CreateCatalogGroup()
        {
            var name = "avenue-clothing.com";
            return ProductCatalogGroup.SingleOrDefault(c => c.Name == name) ?? new ProductCatalogGroupFactory().NewWithDefaults(name);
        }

        private ProductCatalog CreateProductCatalog(ProductCatalogGroup catalogGroup)
        {
            var name = "Demo Store";
            return catalogGroup.ProductCatalogs.SingleOrDefault(c => c.Name == name) ?? new ProductCatalogFactory().NewWithDefaults(catalogGroup, name);
        }

        private void CreateCatalogue(ProductCatalog catalog)
        {
            var shirts = CreateCategory(catalog, "Shirts");
            catalog.AddCategory(shirts);

            var formal = CreateCategory(catalog, "Formal Shirts");
            var casual = CreateCategory(catalog, "Casual Shirts");
            shirts.AddCategory(formal);
            shirts.AddCategory(casual);
            shirts.Save();

            var shirtDefinition = ProductDefinition.SingleOrDefault(d => d.Name == "Shirt");
            var kittens = CreateProduct(formal, shirtDefinition, "Blue Kittens Mood Slim Fit Signature Shirt", 189);
            var wonderland = CreateProduct(casual, shirtDefinition, "Black & White Wonderland Mood Slim Fit Signature Shirt - Limited Edition [200 pieces]", 219);

            var shoes = CreateCategory(catalog, "Shoes");
            catalog.AddCategory(shoes);

            var shoeDefinition = ProductDefinition.SingleOrDefault(d => d.Name == "Shoe");
            CreateProduct(shoes, shoeDefinition, "Paraboot Avoriaz/Jannu Marron Brut Marron Brown Hiking Boot Shoes", 189);
            CreateProduct(shoes, shoeDefinition, "Paraboot Chambord Tex Marron Lis Marron Brown Shoes", 189);
            CreateProduct(shoes, shoeDefinition, "Paraboot Chambord Tex Marron Lis Cafe Brown Shoes", 189);

            var accessories = CreateCategory(catalog, "Accessories");
            catalog.AddCategory(accessories);

            var accessoryDefinition = ProductDefinition.SingleOrDefault(d => d.Name == "Accessory");
            CreateProduct(accessories, accessoryDefinition, "Baby Alpaca Throw", 189);

            var belts = CreateCategory(catalog, "Belts");
            var scarves = CreateCategory(catalog, "Scarves");
            accessories.AddCategory(belts);
            accessories.AddCategory(scarves);
            accessories.Save();

            CreateProduct(belts, accessoryDefinition, "Paul Smith", 189);
            CreateProduct(scarves, accessoryDefinition, "Baby Alpaca Scarf", 189);
        }

        private static Category CreateCategory(ProductCatalog catalog, string name)
        {
            var definition = Definition.SingleOrDefault(d => d.Name == "Default Category Definition");
            var category = Category.SingleOrDefault(c => c.Name == name) ?? new CategoryFactory().NewWithDefaults(catalog, definition, name);
            category.DisplayOnSite = true;

            DoForEachCulture(language => category.AddCategoryDescription(new CategoryDescription()
                {
                    CultureCode = language,
                    DisplayName = name
                }));

            category.Save();
            return category;
        }

        private Product CreateProduct(Category category, ProductDefinition productDefinition, string name, decimal price)
        {
            // ProductDefinition productDefinition = ProductDefinition.Get(Convert.ToInt32(this.DefinitionDropDown.SelectedValue));
            //var product = Product.SingleOrDefault(p => p.Name == name && p.CategoryProductRelations.Any(r => r.Category.Id == category.Id)) ?? new ProductFactory().;
            var sku = (name.Length > 30) ? name.Substring(0, 30) : name;
            var product = new Product
            {
                Name = name,
                Sku = sku,
                ProductDefinition = productDefinition,
                DisplayOnSite = true,
                AllowOrdering = true
            };

            DoForEachCulture(language => product.AddProductDescription(new ProductDescription()
                {
                    CultureCode = language,
                    DisplayName = name
                }));

            product.AddPriceGroupPrice(new PriceGroupPrice() { Price = price });

            category.AddProduct(product, 0);
            return product;
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

        private static DataType CreateDataType(string name, string dataType)
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

            DoForEachCulture(language => dataTypeEnum.AddDescription(new DataTypeEnumDescription
                    {
                        CultureCode = language,
                        DisplayName = colour,
                        Description = colour
                    }));

            return dataTypeEnum;
        }

        private void CreateProductDefinitions()
        {
            CreateShirt();
            CreateShoe();
            CreateAccessory();
        }

        private static ProductDefinition CreateProductDefinition(string name)
        {
            var productDefinition = ProductDefinition.SingleOrDefault(d => d.Name == name) ?? new ProductDefinition();

            productDefinition.Name = name;

            productDefinition.Save();
            return productDefinition;
        }

        private void CreateShirt()
        {
            var productDefinition = CreateProductDefinition("Shirt");
            productDefinition.AddProductDefinitionField(ProductDefinitionField("CollarSize", "Number", true, true, "Collar Inches"));
            productDefinition.AddProductDefinitionField(ProductDefinitionField("Colour", "Colour", true, true, "Colour"));
            productDefinition.AddProductDefinitionField(ProductDefinitionField("ShowOnHomepage", "Boolean", true, true, "Show On Homepage"));
        }

        private void CreateShoe()
        {
            var productDefinition = CreateProductDefinition("Shoe");
            productDefinition.AddProductDefinitionField(ProductDefinitionField("ShoeSize", "ShortText", true, true, "Shoe Size"));
            productDefinition.AddProductDefinitionField(ProductDefinitionField("ShowOnHomepage", "Boolean", true, true, "Show On Homepage"));
        }

        private void CreateAccessory()
        {
            var productDefinition = CreateProductDefinition("Accessory");
            productDefinition.AddProductDefinitionField(ProductDefinitionField("ShowOnHomepage", "Boolean", true, true, "Show On Homepage"));
        }

        private static ProductDefinitionField ProductDefinitionField(string name, string typeName, bool displayOnWebsite, bool variantProperty, string displayName)
        {
            var definition = new ProductDefinitionField()
                {
                    Name = name,
                    DataType = DataType.SingleOrDefault(d => d.TypeName == typeName),
                    Deleted = false,
                    Multilingual = false,
                    DisplayOnSite = displayOnWebsite,
                    IsVariantProperty = variantProperty,
                    RenderInEditor = true
                };

            DoForEachCulture(language => definition.AddProductDefinitionFieldDescription(new ProductDefinitionFieldDescription
                {
                    CultureCode = language,
                    DisplayName = displayName,
                    Description = displayName
                }));

            return definition;
        }

        private static void DoForEachCulture(Action<string> toDo)
        {
            foreach (Language language in ObjectFactory.Instance.Resolve<ILanguageService>().GetAllLanguages())
            {
                toDo(language.CultureCode);
            }
        }
    }
}