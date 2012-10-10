using System;
using System.Collections.Generic;
using System.Linq;

using UCommerce.EntitiesV2;
using UCommerce.EntitiesV2.Factories;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Globalization;
using UCommerce.Security;


namespace UCommerce.RazorStore.Installer.umbraco.uCommerce.Install
{
    public partial class DemoStoreInstaller1 : System.Web.UI.UserControl
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

            pnlInstall.Visible = false;
            pnlthanks.Visible = true;
        }

        protected void btnAssignPermissions_Click(object sender, EventArgs e)
        {
            var name = "avenue-clothing.com";
            var group = ProductCatalogGroup.SingleOrDefault(c => c.Name == name);
            AssignAccessPermissionsToDemoStore(group);
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

        private void AssignAccessPermissionsToDemoStore(ProductCatalogGroup catalogGroup)
        {
            var userService = ObjectFactory.Instance.Resolve<IUserService>();
            var user = userService.GetCurrentUser();

            var roleService = ObjectFactory.Instance.Resolve<IRoleService>();
            var roles = roleService.GetAllRoles();

            roleService.AddUserToRoles(user, roles);
        }

        private ProductCatalogGroup CreateCatalogGroup()
        {
            var name = "avenue-clothing.com";
            var group = ProductCatalogGroup.SingleOrDefault(c => c.Name == name) ?? new ProductCatalogGroupFactory().NewWithDefaults(name);
            group.ProductReviewsRequireApproval = true;
            group.Deleted = false;
            group.CreateCustomersAsUmbracoMembers = true;
            return group;
        }

        private ProductCatalog CreateProductCatalog(ProductCatalogGroup catalogGroup)
        {
            var name = "Demo Store";
            var catalog = catalogGroup.ProductCatalogs.SingleOrDefault(c => c.Name == name) ?? new ProductCatalogFactory().NewWithDefaults(catalogGroup, name);
            catalog.DisplayOnWebSite = true;
            catalog.Deleted = false;
            catalog.ShowPricesIncludingVAT = true;
            return catalog;
        }

        private void CreateCatalogue(ProductCatalog catalog)
        {
            CreateShirts(catalog);
            CreateShoes(catalog);
            CreateAccessories(catalog);
        }

        private void CreateShirts(ProductCatalog catalog)
        {
            var shirts = CreateCatalogCategory(catalog, "Shirts");
            var shirtDefinition = GetProductDefinition("Shirt");
            CreateFormalShirts(shirts, shirtDefinition);
            CreateCasualShirts(shirts, shirtDefinition);
        }

        private void CreateCasualShirts(Category shirts, ProductDefinition shirtDefinition)
        {
            var casual = CreateChildCategory(shirts, "Casual Shirts");
            var wonderland = CreateProductOnCategory(casual, shirtDefinition, "SWWMSFSS-LE", "Black & White Wonderland Mood Slim Fit Signature Shirt - Limited Edition [200 pieces]", 219);
            AddShirtVariantsToProduct(wonderland, "Plain", new List<string>() { "15", "16", "17", "18" }, new List<string>() { "White" });
        }

        private void CreateFormalShirts(Category shirts, ProductDefinition shirtDefinition)
        {
            var formal = CreateChildCategory(shirts, "Formal Shirts");
            var kittens = CreateProductOnCategory(formal, shirtDefinition, "BKMSFSS", "Blue Kittens Mood Slim Fit Signature Shirt", 189);
            AddShirtVariantsToProduct(kittens, "Plain", new List<string>() { "15", "16", "17", "18" }, new List<string>() { "White", "Blue" });
        }

        private void CreateShoes(ProductCatalog catalog)
        {
            var shoes = CreateCatalogCategory(catalog, "Shoes");

            var shoeDefinition = GetProductDefinition("Shoe");

            var hiking = CreateProductOnCategory(shoes, shoeDefinition, "074617", "Paraboot Avoriaz/Jannu Marron Brut Marron Brown Hiking Boot Shoes", 343.85M);
            AddShoeVariantsToProduct(hiking, new List<string>() { "6", "8", "10" });

            var marron = CreateProductOnCategory(shoes, shoeDefinition, "710708", "Paraboot Chambord Tex Marron Lis Marron Brown Shoes", 281.75M);
            AddShoeVariantsToProduct(marron, new List<string>() { "6", "8", "10" });

            var brown = CreateProductOnCategory(shoes, shoeDefinition, "710707", "Paraboot Chambord Tex Marron Lis Cafe Brown Shoes", 281.75M);
            AddShoeVariantsToProduct(brown, new List<string>() { "6", "8", "10", "12" });
        }

        private void CreateAccessories(ProductCatalog catalog)
        {
            var accessories = CreateCatalogCategory(catalog, "Accessories");

            var accessoryDefinition = GetProductDefinition("Accessory");

            var alpacaThrow = CreateProductOnCategory(accessories, accessoryDefinition, "BAT", "Baby Alpaca Throw", 189);

            CreateBelts(accessories, accessoryDefinition);
            CreateScarves(accessories, accessoryDefinition);
        }

        private void CreateScarves(Category accessories, ProductDefinition accessoryDefinition)
        {
            var scarves = CreateChildCategory(accessories, "Scarves");
            var alpacaScarf = CreateProductOnCategory(scarves, accessoryDefinition, "PAS", "Baby Alpaca Scarf", 99);
        }

        private void CreateBelts(Category accessories, ProductDefinition accessoryDefinition)
        {
            var belts = CreateChildCategory(accessories, "Belts");
            var paulSmithBelt = CreateProductOnCategory(belts, accessoryDefinition, "PSB", "Paul Smith Belt", 189);
        }

        private static ProductDefinition GetProductDefinition(string name)
        {
            var definition = ProductDefinition.SingleOrDefault(d => d.Name == name);

            if (definition == null)
                throw new ArgumentOutOfRangeException("definition", string.Format("No product definition with the name \"{0}\" could be found. Please check you have installed the default settings.", name));

            return definition;
        }

        private void AddShirtVariantsToProduct(Product product, string namePrefix, IList<string> sizes, IList<string> colours)
        {
            if (!sizes.Any())
                return;

            foreach (var size in sizes)
            {
                foreach (var colour in colours)
                {
                    AddShirtVariantToProduct(product, namePrefix, size, colour);
                }
            }
        }

        private void AddShoeVariantsToProduct(Product product, IList<string> sizes)
        {
            if (!sizes.Any())
                return;

            foreach (var size in sizes)
            {
                AddShoeVariantToProduct(product, size);
            }
        }

        private void AddShoeVariantToProduct(Product product, string size)
        {
            var sku = string.Format("{0}-{1}", product.Sku, size);
            var variant = CreateVariantOnProduct(product, sku, size);
            AddProductProperty("ShoeSize", variant, string.Format("UK {0}", size));
            variant.Save();
        }

        private void AddShirtVariantToProduct(Product product, string namePrefix, string size, string colour)
        {
            var sku = string.Format("{0}-{1}-{2}", product.Sku, size, colour);
            var name = string.Format("{0} {1} {2}\"", namePrefix, size, colour);

            var variant = CreateVariantOnProduct(product, sku, name);
            AddProductProperty("CollarSize", variant, size);
            AddProductProperty("Colour", variant, colour);
            variant.Save();
        }

        private void AddProductProperty(string dataFieldName, Product variant, string value)
        {
            if (variant.ProductProperties.Any(p => p.Value == value))
                return;

            var field = GetProductDefinitionField(dataFieldName);
            variant.AddProductProperty(new ProductProperty
            {
                ProductDefinitionField = field,
                Value = value
            });
        }

        private ProductDefinitionField GetProductDefinitionField(string name)
        {
            var field = ProductDefinitionField.SingleOrDefault(d => d.Name == name);

            if (field == null)
                throw new ArgumentOutOfRangeException("field", string.Format("No product definition field with the name \"{0}\" could be found. Please check you have installed the default settings.", name));

            return field;
        }

        private Category CreateCatalogCategory(ProductCatalog catalog, string name)
        {
            var category = CreateCategory(catalog, name);
            catalog.AddCategory(category);
            return category;
        }

        private Category CreateChildCategory(Category parent, string name)
        {
            var category = CreateCategory(parent.ProductCatalog, name);
            parent.AddCategory(category);
            return category;
        }

        private static Category CreateCategory(ProductCatalog catalog, string name)
        {
            var definition = Definition.SingleOrDefault(d => d.Name == "Default Category Definition");
            var category = Category.SingleOrDefault(c => c.Name == name) ?? new CategoryFactory().NewWithDefaults(catalog, definition, name);
            category.DisplayOnSite = true;

            DoForEachCulture(language =>
            {
                if (category.GetDescription(language) == null)
                    category.AddCategoryDescription(new CategoryDescription() { CultureCode = language, DisplayName = name });
            });

            category.Save();
            return category;
        }

        private Product CreateProductOnCategory(Category category, ProductDefinition productDefinition, string sku, string name, decimal price)
        {
            var product = CreateBaseProduct(productDefinition, sku, null, name);

            DoForEachCulture(
                language =>
                {
                    if (!product.HasDescription(language))
                        product.AddProductDescription(new ProductDescription() { CultureCode = language, DisplayName = name });
                });

            if (!product.PriceGroupPrices.Any())
                product.AddPriceGroupPrice(new PriceGroupPrice { Price = price, PriceGroup = category.ProductCatalog.PriceGroup });

            product.Save();

            if (product.CategoryProductRelations.All(r => r.Category.CategoryId != category.CategoryId))
                category.AddProduct(product, 0);

            return product;
        }

        private Product CreateVariantOnProduct(Product product, string variantSku, string name)
        {
            var variant = CreateBaseProduct(product.ProductDefinition, product.Sku, variantSku, name);
            product.AddVariant(variant);
            return variant;
        }

        private Product CreateBaseProduct(ProductDefinition productDefinition, string sku, string variantSku, string name)
        {
            if (sku.Length > 30)
                sku = sku.Substring(0, 30);

            var product = Product.SingleOrDefault(p => p.Sku == sku && p.VariantSku == variantSku) ?? new Product();

            product.Sku = sku;
            product.VariantSku = variantSku;
            product.Name = name;
            product.ProductDefinition = productDefinition;
            product.DisplayOnSite = true;
            product.AllowOrdering = true;
            product.Save();

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

            DoForEachCulture(language =>
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
            AddProductDefinitionFieldIfDoesntExist(productDefinition, "ShowOnHomepage", "Boolean", true, true, "Show On Homepage");
        }

        private void CreateShoeProductDefinition()
        {
            var productDefinition = CreateProductDefinition("Shoe");
            AddProductDefinitionFieldIfDoesntExist(productDefinition, "ShoeSize", "ShortText", true, true, "Shoe Size");
            AddProductDefinitionFieldIfDoesntExist(productDefinition, "ShowOnHomepage", "Boolean", true, true, "Show On Homepage");
        }

        private void CreateAccessoryProductDefinition()
        {
            var productDefinition = CreateProductDefinition("Accessory");
            AddProductDefinitionFieldIfDoesntExist(productDefinition, "ShowOnHomepage", "Boolean", true, true, "Show On Homepage");
        }

        private static void AddProductDefinitionFieldIfDoesntExist(ProductDefinition definition, string name, string typeName, bool displayOnWebsite, bool variantProperty, string displayName)
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

            //DoForEachCulture(language =>
            //    {
            //        if (field.GetDescription(language) == null)
            //            field.AddProductDefinitionFieldDescription(new ProductDefinitionFieldDescription { CultureCode = language, DisplayName = displayName, Description = displayName });
            //    });

            definition.AddProductDefinitionField(field);
            definition.Save();
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