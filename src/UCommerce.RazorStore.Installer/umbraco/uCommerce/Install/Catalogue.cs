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
    public class CatalogueInstaller
    {
        private string _catalogGroupName;
        private string _catalogName;

        public CatalogueInstaller(string catalogGroupName, string catalogName)
        {
            _catalogGroupName = catalogGroupName;
            _catalogName = catalogName;
        }

        public void Configure()
        {
            var catalogGroup = CreateCatalogGroup();
            var catalog = CreateProductCatalog(catalogGroup);
            CreateCatalogue(catalog);
        }

        private ProductCatalogGroup CreateCatalogGroup()
        {
            var group = ProductCatalogGroup.SingleOrDefault(c => c.Name == _catalogGroupName) ?? new ProductCatalogGroupFactory().NewWithDefaults(_catalogGroupName);
            group.ProductReviewsRequireApproval = true;
            group.Deleted = false;
            group.CreateCustomersAsUmbracoMembers = true;
            return group;
        }

        private ProductCatalog CreateProductCatalog(ProductCatalogGroup catalogGroup)
        {
            var catalog = catalogGroup.ProductCatalogs.SingleOrDefault(c => c.Name == _catalogName) ?? new ProductCatalogFactory().NewWithDefaults(catalogGroup, _catalogName);
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

            Helpers.DoForEachCulture(language =>
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

            Helpers.DoForEachCulture(
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
    }
}