using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ucommerce.Api;
using Ucommerce.Api.PriceCalculation;
using Ucommerce.Infrastructure;
using Ucommerce.Search;
using AvenueClothing.Models;
using Ucommerce.Search.Models;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace AvenueClothing.Controllers
{
    public class ProductController : RenderMvcController
    {
        public ITransactionLibrary TransactionLibrary => ObjectFactory.Instance.Resolve<ITransactionLibrary>();
        public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
        private ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();

        [HttpGet]
        public ActionResult Index(ContentModel model)
        {
            return RenderView(false);
        }

        [HttpPost]
        public ActionResult Index(AddToBasketViewModel model)
        {
            TransactionLibrary.AddToBasket(1, model.Product, model.Variant);
            return RenderView(true);
        }

        protected virtual ActionResult RenderView(bool addedToBasket = false)
        {
            Product currentProduct = CatalogContext.CurrentProduct;

            // Price calculations
            currentProduct.UnitPrices.TryGetValue(CatalogContext.CurrentPriceGroup.Name, out decimal unitPrice);
            string currencyIsoCode = CatalogContext.CurrentPriceGroup.CurrencyISOCode;
            decimal taxRate = CatalogContext.CurrentPriceGroup.TaxRate;

            var productViewModel = new ProductViewModel
            {
                Sku = currentProduct.Sku,
                Name = currentProduct.DisplayName,
                LongDescription = currentProduct.LongDescription,
                IsOrderingAllowed = currentProduct.AllowOrdering,
                IsProductFamily = currentProduct.ProductType == ProductType.ProductFamily,
                IsVariant = false,
                Tax = unitPrice > 0 ? new Money(unitPrice * taxRate, currencyIsoCode).ToString() : "",
                Price = unitPrice > 0 ? new Money(unitPrice * (1.0M + taxRate), currencyIsoCode).ToString() : ""
            };

            if (!string.IsNullOrEmpty(currentProduct.PrimaryImageUrl))
            {
                productViewModel.ThumbnailImageUrl = currentProduct.PrimaryImageUrl;
            }

            var variants = CatalogLibrary.GetVariants(currentProduct);

            productViewModel.Properties = MapProductProperties(variants);

            if (currentProduct.ProductType == ProductType.ProductFamily)
            {
                productViewModel.Variants = MapVariants(variants);
            }

            bool isInBasket = TransactionLibrary.GetBasket(true).OrderLines.Any(x => x.Sku == currentProduct.Sku);

            var productPageViewModel = new ProductPageViewModel
            {
                ProductViewModel = productViewModel,
                AddedToBasket = addedToBasket,
                ItemAlreadyExists = isInBasket
            };

            return View("/Views/Product.cshtml", productPageViewModel);
        }

        private IList<ProductViewModel> MapVariants(IEnumerable<Product> variants)
        {
            var variantModels = new List<ProductViewModel>();
            foreach (var currentVariant in variants)
            {
                var productModel = new ProductViewModel
                {
                    Sku = currentVariant.Sku,
                    VariantSku = currentVariant.VariantSku,
                    Name = currentVariant.DisplayName,
                    LongDescription = currentVariant.LongDescription,
                    IsVariant = true
                };

                variantModels.Add(productModel);
            }

            return variantModels;
        }

        private IList<ProductPropertiesViewModel> MapProductProperties(ResultSet<Product> variants)
        {
            var productProperties = new List<ProductPropertiesViewModel>();

            var uniqueVariants =
                from v in variants.SelectMany(p => p.GetUserDefinedFields())
                group v by v.Key
                into g
                select g;

            foreach (var prop in uniqueVariants)
            {
                var productPropertiesViewModel = new ProductPropertiesViewModel();
                productPropertiesViewModel.PropertyName = prop.Key;

                foreach (var value in prop.Select(p => p.Value).Distinct())
                {
                    productPropertiesViewModel.Values.Add(value.ToString());
                }

                productProperties.Add(productPropertiesViewModel);
            }

            return productProperties;
        }
    }
}