using System;
using System.Collections.Generic;
using System.Linq;
using UCommerce.EntitiesV2;
using UCommerce.EntitiesV2.Factories;

namespace UCommerce.RazorStore.Installer.Helpers
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
            EnablePaymentMethodForCatalog(catalogGroup);
            EnableShippingMethodForCatalog(catalogGroup);
            CreateCatalogue(catalog);
        }

        private ProductCatalogGroup CreateCatalogGroup()
        {
            var group = ProductCatalogGroup.SingleOrDefault(c => c.Name == _catalogGroupName) ?? new ProductCatalogGroupFactory().NewWithDefaults(_catalogGroupName);
            group.ProductReviewsRequireApproval = true;
            group.Deleted = false;
            group.CreateCustomersAsUmbracoMembers = false;
            group.DomainId = null;
            group.Save();
            group.OrderNumberSerie = GetDefaultOrderNumberSeries();
            group.EmailProfile = GetDefaultEmailProfile();
            group.Save();
            return group;
        }

        private EmailProfile GetDefaultEmailProfile()
        {
            var emailProfile = EmailProfile.SingleOrDefault(o => o.Name == "Default");
            if (emailProfile == null)
                throw new ArgumentOutOfRangeException("emailProfile", "The Default email profile could not be found. Have you run the installer?");

            return emailProfile;
        }

        private OrderNumberSerie GetDefaultOrderNumberSeries()
        {
            var orderNumberSeries = OrderNumberSerie.SingleOrDefault(o => o.OrderNumberName == "Example");
            if(orderNumberSeries == null)
                throw new ArgumentOutOfRangeException("orderNumberSeries", "The Example order number series could not be found. Have you run the installer?");

            return orderNumberSeries;
        }

        private ProductCatalog CreateProductCatalog(ProductCatalogGroup catalogGroup)
        {
            var catalog = catalogGroup.ProductCatalogs.SingleOrDefault(c => c.Name == _catalogName) ?? new ProductCatalogFactory().NewWithDefaults(catalogGroup, _catalogName);

            catalog.DisplayOnWebSite = true;
            catalog.Deleted = false;
            catalog.ShowPricesIncludingVAT = true;
			
			// Versions of CatalogFactory prior to 3.6 did not
			// add catalog to catalog group. Need to do it
			// if not already done to make sure roles and 
			// permissions are created properly.
			if (!catalogGroup.ProductCatalogs.Contains(catalog))
				catalogGroup.ProductCatalogs.Add(catalog);
            
			catalog.Save();
            
            var priceGroup = PriceGroup.SingleOrDefault(p => p.Name == "EUR 15 pct");
            if (priceGroup != null)
                catalog.PriceGroup = priceGroup;

            catalog.Save();
            return catalog;
        }

        private void EnableShippingMethodForCatalog(ProductCatalogGroup catalogGroup)
        {
            var shippingMethods = ShippingMethod.All();
            foreach (var method in shippingMethods)
            {
                method.ClearEligibleProductCatalogGroups();
                method.AddEligibleProductCatalogGroup(catalogGroup);
                method.Save();
            }
        }

        private void EnablePaymentMethodForCatalog(ProductCatalogGroup catalogGroup)
        {
            var paymentMethods = PaymentMethod.All();
            foreach (var method in paymentMethods)
            {
                method.ClearEligibleProductCatalogGroups();
                method.AddEligibleProductCatalogGroup(catalogGroup);
                method.Save();
            }
        }

        private void CreateCatalogue(ProductCatalog catalog)
        {
            CreateShirts(catalog);
            CreateShoes(catalog);
            CreateAccessories(catalog);
        }

        private void CreateShirts(ProductCatalog catalog)
        {
            var tops = CreateCatalogCategory(catalog, "Tops");
            var shirtDefinition = GetProductDefinition("Shirt");
            CreateFormalShirts(tops, shirtDefinition);
            CreateCasualShirts(tops, shirtDefinition);
        }

        private void CreateCasualShirts(Category shirts, ProductDefinition shirtDefinition)
        {
            var casual = CreateChildCategory(shirts, "Casual");

            var prettyGreen = CreateProductOnCategory(casual, shirtDefinition, "GHXG-4044-7604-1", "Pretty Green White Pinstripe Jersey Short Sleeve Polo Shirt", 65, "", "<ul><li>Three button placket and classic polo collar</li><li>Short sleeved</li><li>Signature embroidered Pretty Green chest badge</li><li>White and black pin stripe</li><li>100% jersey cotton</li><li>Style number : GHXG/4044/7604/1</li></ul><p>As featured on <a href=\"http://www.pritchards.co.uk/polo-shirts-7/pretty-green-white-pinstripe-jersey-short-19759.htm\">Pritchards.co.uk</a></p>");
            AddShirtVariantsToProduct(prettyGreen, "Striped", new List<string>() { "SML", "MED", "LAR", "X.L" }, new List<string>() { "White/Red" });
        }

        private void CreateFormalShirts(Category shirts, ProductDefinition shirtDefinition)
        {
            var formal = CreateChildCategory(shirts, "Formal");

            var smDesc = "<p>100% high quality cotton fabric from Austria &amp; Italy</p> <p>European made</p> <p>Unique and exclusive patterns - on the inside of the collar, placket and cuffs</p> <p>Non-iron fabric</p> <p>Machine washable to 40<span>°</span>&nbsp;C</p> <p>Slim cut tapered shirt</p> <p>Cuffs close with or without cufflinks</p> <p>Permanent collar stays</p> <p>Versatility - smart for the office and exclusive for after work</p>";

            var wonderland = CreateProductOnCategory(formal, shirtDefinition, "BWWMSFSS-LE", "Black & White Wonderland Mood Slim Fit Signature Shirt - Limited Edition [200 pieces]", 219, "", smDesc + "<p>As featured on <a href=\"https://stauntonmoods.com/shop/all-moods/black--white-wonderland-mood-slim-fit-signature-shirt---limited-edition-%5b200-pieces%5d/c-23/c-85/p-235\">stauntonmoods.com</a></p>");
            AddShirtVariantsToProduct(wonderland, "Plain", new List<string>() { "15", "16", "17", "18" }, new List<string>() { "White" });
            AddProductProperty(wonderland, "ShowOnHomepage", "true");
            wonderland.Save();

            var kittens = CreateProductOnCategory(formal, shirtDefinition, "BKMSFSS", "Blue Kittens Mood Slim Fit Signature Shirt", 179, "", smDesc + "<p>As featured on <a href=\"https://stauntonmoods.com/shop/all-moods/blue-kittens-mood-slim-fit-signature-shirt/c-23/c-85/p-119\">stauntonmoods.com</a></p>");
            AddShirtVariantsToProduct(kittens, "Plain", new List<string>() { "15", "16", "17", "18" }, new List<string>() { "White", "Blue" });

            var pink = CreateProductOnCategory(formal, shirtDefinition, "PMMSFSS-LE", "Pink Manga Mood Slim Fit Signature Shirt - Limited Edition [100 pieces]", 279, "", smDesc + "<p>As featured on <a href=\"https://stauntonmoods.com/shop/all-moods/pink-manga-mood-slim-fit-signature-shirt---limited-edition-%5b100-pieces%5d/c-23/c-85/p-156\">stauntonmoods.com</a></p>");
            AddShirtVariantsToProduct(pink, "Plain", new List<string>() { "15", "16", "17", "18" }, new List<string>() { "White", "Blue" });

            var jungle = CreateProductOnCategory(formal, shirtDefinition, "JNMSFSS", "Jungle by Night Mood Slim Fit Signature Shirt", 179, "", smDesc + "<p>As featured on <a href=\"https://stauntonmoods.com/shop/all-moods/jungle-by-night-mood-slim-fit-signature-shirt/c-23/c-85/p-125\">stauntonmoods.com</a></p>");
            AddShirtVariantsToProduct(jungle, "Plain", new List<string>() { "15", "16", "17", "18" }, new List<string>() { "White", "Blue" });

            var ghoulies = CreateProductOnCategory(formal, shirtDefinition, "GGMSFSS-LE", "Green Ghoulies Mood Slim Fit Signature Shirt - Limited Edition [200 pieces]", 219, "", smDesc + "<p>As featured on <a href=\"https://stauntonmoods.com/shop/all-moods/green-ghoulies-mood-slim-fit-signature-shirt---limited-edition-%5b200-pieces%5d/c-23/c-85/p-124\">stauntonmoods.com</a></p>");
            AddShirtVariantsToProduct(ghoulies, "Plain", new List<string>() { "15", "16", "17", "18" }, new List<string>() { "White", "Blue" });

            var comic = CreateProductOnCategory(formal, shirtDefinition, "CMSFSS", "Comic Mood Slim Fit Signature Shirt", 179, "", smDesc + "<p>As featured on <a href=\"https:///shop/all-moods/comic-mood-slim-fit-signature-shirt/c-23/c-85/p-151\">stauntonmoods.com</a></p>");
            AddShirtVariantsToProduct(comic, "Plain", new List<string>() { "15", "16", "17", "18" }, new List<string>() { "White", "Blue" });

            var eton = CreateProductOnCategory(formal, shirtDefinition, "2285794682539", "Eton Purple & White Stripe Contemporary Fit Formal Dress Shirt", 135, "", "<ul><li>Contemporary Fit</li><li>Single button cuffs with double button holes for cuff links</li><li>Pointed collar with bone inserts</li><li>Purple and white stipe</li><li>100% cotton</li><li>Style number : 2285794682539</li></ul><p>As featured on <a href=\"http://www.pritchards.co.uk/shirts-6/eton-purple-white-stripe-contemporary-formal-20625.htm\">Pritchards.co.uk</a></p>");
            AddShirtVariantsToProduct(eton, "Striped", new List<string>() { "15", "15.5", "16", "16.5" }, new List<string>() { "White" });
            AddProductProperty(eton, "ShowOnHomepage", "true");
            eton.Save();

            var rlA02 = CreateProductOnCategory(formal, shirtDefinition, "A02 WCJNK C0223 C421A", "Polo Ralph Lauren Blue & White Stripe Custom Fit Regent Poplin Dress Shirt", 85, "", "<ul><li>Custom fit dress shirt</li><li>Classic single button collar</li><li>Long sleeve with single button cuff</li><li>Blue and white stripe with newport navy polo player</li><li>100% poplin cotton</li><li>Style number : A02 WCJNK C0223 C421A</li></ul><p>As featured on <a href=\"http://www.pritchards.co.uk/shirts-6/polo-ralph-lauren-blue-white-stripe-18999.htm\">Pritchards.co.uk</a></p>");
            AddShirtVariantsToProduct(rlA02, "Striped", new List<string>() { "SML", "MED", "LAR", "X.L", "XXL", "XXXL" }, new List<string>() { "Blue" });

            var rlA04 = CreateProductOnCategory(formal, shirtDefinition, "A04 WCBPS C4572 G4562", "Polo Ralph Lauren Blue & Navy Stripe Custom Fit Broadcloth Long Sleeve Shirt", 105, "", "<ul><li>Custom Fit</li><li>Classic button down collar</li><li>Long sleeve with single button cuff</li><li>Blue, navy and white stripes</li><li>100% cotton</li><li>Style number : A04 WCBPS C4572 G4562</li></ul><p>As featured on <a href=\"http://www.pritchards.co.uk/shirts-6/polo-ralph-lauren-blue-navy-stripe-18980.htm\">Pritchards.co.uk</a></p>");
            AddShirtVariantsToProduct(rlA04, "Striped", new List<string>() { "SML", "MED", "LAR", "X.L", "XXL" }, new List<string>() { "Blue" });
        }

        private void CreateShoes(ProductCatalog catalog)
        {
            var shoes = CreateCatalogCategory(catalog, "Shoes");

            var shoeDefinition = GetProductDefinition("Shoe");

            var hiking = CreateProductOnCategory(shoes, shoeDefinition, "074617", "Paraboot Avoriaz/Jannu Marron Brut Marron Brown Hiking Boot Shoes", 343.85M, "", "<ul><li>Paraboot Avoriaz Mountaineering Boots</li><li>Marron Brut Marron (Brown)</li><li>Full leather inners and uppers</li><li>Norwegien Welted Commando Sole</li><li>Hand made in France</li><li>Style number : 074617</li></ul><p>As featured on <a href=\"http://www.pritchards.co.uk/shoes-trainers-11/paraboot-avoriaz-jannu-marron-brut-brown-20879.htm\">Pritchards.co.uk</a></p>");
            AddShoeVariantsToProduct(hiking, new List<string>() { "6", "8", "10" });
            AddProductProperty(hiking, "ShowOnHomepage", "true");
            hiking.Save();

            var marron = CreateProductOnCategory(shoes, shoeDefinition, "710708", "Paraboot Chambord Tex Marron Lis Marron Brown Shoes", 281.75M, "", "<ul><li>Style : Chambord Tex</li><li>Colour : Marron Lis Marron</li><li>Paraboot style code : 710708</li><li>Estimated delivery time : 1 - 4 weeks</li><li>Customers ordering from outside the EU will receive a 20% VAT discount on their order. This is applied at checkout once you have given your delivery details.</li></ul><p>As featured on <a href=\"http://www.pritchards.co.uk/shoes-trainers-11/paraboot-order-chambord-marron-brown-shoes-20709.htm\">Pritchards.co.uk</a></p>");
            AddShoeVariantsToProduct(marron, new List<string>() { "6", "8", "10" });

            var brown = CreateProductOnCategory(shoes, shoeDefinition, "710707", "Paraboot Chambord Tex Marron Lis Cafe Brown Shoes", 281.75M, "", "<ul><li>Style : Chambord Tex</li><li>Colour : Marron Lis Cafe</li><li>Paraboot style code : 710707</li><li>Estimated delivery time : 1 - 4 weeks</li><li>Customers ordering from outside the EU will receive a 20% VAT discount on their order. This is applied at checkout once you have given your delivery details.</li></ul><p>As featured on <a href=\"http://www.pritchards.co.uk/shoes-trainers-11/paraboot-order-chambord-marron-cafe-brown-18606.htm\">Pritchards.co.uk</a></p>");
            AddShoeVariantsToProduct(brown, new List<string>() { "6", "8", "10", "12" });
        }

        private void CreateAccessories(ProductCatalog catalog)
        {
            var accessories = CreateCatalogCategory(catalog, "Accessories");

            var accessoryDefinition = GetProductDefinition("Accessory");

            CreateProductOnCategory(accessories, accessoryDefinition, "LBAT", "Luxury Baby Alpaca Throw", 125, "", "<ul><li>100% Baby Alpaca Wool</li><li>180cm x 130cm (plus tassel fringe)</li><li>Fair Trade</li><li>Hypo-allergenic</li></ul><p>As featured on <a href=\"http://www.amazon.co.uk/Luxury-Threads-100%25-Alpaca-Throw/dp/B008WMLGCO/ref=sr_1_5?s=kitchen&ie=UTF8&qid=1349966612&sr=1-5\">Luxury Threads</a></p>");

            CreateTies(accessories, accessoryDefinition);
            CreateScarves(accessories, accessoryDefinition);
        }

        private void CreateScarves(Category accessories, ProductDefinition accessoryDefinition)
        {
            var scarves = CreateChildCategory(accessories, "Scarves");
            CreateProductOnCategory(scarves, accessoryDefinition, "LBAS", "Luxury Baby Alpaca Scarf", 99, "", "<ul><li>100% Baby Alpaca Wool</li><li>180cm x 130cm (plus tassel fringe)</li><li>Fair Trade</li><li>Hypo-allergenic</li></ul><p>As featured on <a href=\"http://www.amazon.co.uk/Luxury-Threads-100%25-Alpaca-Throw/dp/B008WMLGCO/ref=sr_1_5?s=kitchen&ie=UTF8&qid=1349966612&sr=1-5\">Luxury Threads</a></p>");
            CreateProductOnCategory(scarves, accessoryDefinition, "20178", "Hugo Boss Black Frando Multi Colour Square Check Pattern Wool Scarf", 55, "", "<ul><li>Embroidered Hugo Boss Logo</li><li>Tassled Ends</li><li>Size 180 cm x 29 cm (including tassels)</li><li>Multi Colour Square Check Pattern</li><li>100% Virgin Wool</li><li>Style Number : 50230552 10159101 01 641</li></ul><p>As featured on <a href=\"http://www.pritchards.co.uk/accessories-14/scarves-32/hugo-boss-black-frando-multi-colour-20178.htm\">Pritchards.co.uk</a></p>");
            CreateProductOnCategory(scarves, accessoryDefinition, "20180", "Hugo Boss Black Farion Multi Colour Stripe ", 55, "", "<ul><li>Embroidered Hugo Boss logo</li><li>Tasselled ends</li><li>Size 180 cm x 25 cm (including tassels)</li><li>Multi Colour Stripe</li><li>100% Knitted Wool</li><li>Style Number : 50226493 10157685 01 960</li></ul><p>As featured on <a href=\"http://www.pritchards.co.uk/accessories-14/scarves-32/hugo-boss-black-farion-multi-colour-20180.htm\">Pritchards.co.uk</a></p>");
        }

        private void CreateTies(Category accessories, ProductDefinition accessoryDefinition)
        {
            var ties = CreateChildCategory(accessories, "Ties");
            CreateProductOnCategory(ties, accessoryDefinition, "19849", "Paul Smith Accessories Classic Blue with Brown & Pink Stripe Silk Woven Tie", 69.50M, "", "<ul><li>Classic Stripe Tie</li><li>Tie length : 140cm</li><li>Blade width : 9cm </li><li>Blue with brown and pink stripes</li><li>100% silk (Made in Italy)</li><li>Style Number : AGXA/764L/R38/VP</li></ul><p>As featured on <a href=\"http://www.pritchards.co.uk/accessories-14/ties-25/paul-smith-accessories-classic-blue-with-19849.htm\">Pritchards.co.uk</a></p>");
            CreateProductOnCategory(ties, accessoryDefinition, "19324", "Hugo Boss Black Dark Red Ultra Slim Knitted Tie", 65, "", "<ul><li>Ultra slim knitted tie</li><li>Length: 145 cm </li><li> Width: 5 cm</li><li>Squared end</li><li>100% High-quality wool</li><li>Style Number : 5023476 10156986 01 504</li></ul><p>As featured on <a href=\"http://www.pritchards.co.uk/accessories-14/ties-25/hugo-boss-black-burgundy-ultra-slim-19324.htm\">Pritchards.co.uk</a></p>");
            CreateProductOnCategory(ties, accessoryDefinition, "20301", "Hugo Boss Black Pink & White Flower Pattern Silk Woven Tie", 65, "Silky soft tie from Hugo Boss", "<ul><li>Flower Pattern Silk Tie</li><li>Tie Length : 145cm</li><li>Blade Width : 7.5cm</li><li>Black With Pink &amp; White Flowers</li><li>100% Silk (Made in Italy)</li><li>Style Number : 50227733 10156957 01 001</li></ul><p>As featured on <a href=\"http://www.pritchards.co.uk/accessories-14/ties-25/hugo-boss-black-pink-white-flower-20301.htm\">Pritchards.co.uk</a></p>");
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
            AddProductProperty(variant, "ShoeSize", string.Format("UK {0}", size));
            variant.Save();
        }

        private void AddShirtVariantToProduct(Product product, string namePrefix, string size, string colour)
        {
            var sku = string.Format("{0}-{1}-{2}", product.Sku, size, colour);
            var name = string.Format("{0} {1}\" {2}", namePrefix, size, colour);

            var variant = CreateVariantOnProduct(product, sku, name);
            AddProductProperty(variant, "CollarSize", size);
            AddProductProperty(variant, "Colour", colour);
            variant.Save();
        }

        private void AddProductProperty(Product product, string dataFieldName, string value)
        {
            if (product.ProductProperties.Any(p => p.ProductDefinitionField.Name == dataFieldName && p.Value == value))
                return;

            var field = GetProductDefinitionField(product, dataFieldName);
            product.AddProductProperty(new ProductProperty
            {
                ProductDefinitionField = field,
                Value = value
            });
        }

        private ProductDefinitionField GetProductDefinitionField(Product product, string name)
        {
            var field = ProductDefinitionField.SingleOrDefault(d => product.ProductDefinition.Name == d.ProductDefinition.Name && d.Name == name);

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

            GenericHelpers.DoForEachCulture(language =>
            {
                if (category.GetDescription(language) == null)
                    category.AddCategoryDescription(new CategoryDescription() { CultureCode = language, DisplayName = name });
            });

            category.Save();
            return category;
        }

        private Product CreateProductOnCategory(Category category, ProductDefinition productDefinition, string sku, string name, decimal price, string shortDescription = "", string longDescription = "")
        {
            var product = CreateBaseProduct(productDefinition, sku, null, name);

            GenericHelpers.DoForEachCulture(
                language =>
                {
                    if (!product.HasDescription(language))
                        product.AddProductDescription(new ProductDescription()
                            {
                                CultureCode = language,
                                DisplayName = name,
                                ShortDescription = shortDescription,
                                LongDescription = longDescription
                            });
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

            if (!String.IsNullOrWhiteSpace(variantSku) && variantSku.Length > 30)
                variantSku = variantSku.Substring(0, 30);

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