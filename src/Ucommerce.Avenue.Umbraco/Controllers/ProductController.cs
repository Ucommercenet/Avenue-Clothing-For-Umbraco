using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ucommerce.Api;
using UCommerce.Api;
using UCommerce.Infrastructure;
using UCommerce.RazorStore.Models;
using UCommerce.Search.Models;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace UCommerce.RazorStore.Controllers
{
    public class ProductController : RenderMvcController
    {
        public ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();
        public ITransactionLibrary TransactionLibrary => ObjectFactory.Instance.Resolve<ITransactionLibrary>();
        
        [HttpGet]
        public ActionResult Index(ContentModel model)
        {
            return RenderView(false);
        }

        [HttpPost]
        public ActionResult Index(AddToBasketViewModel model)
        {
            string variant = GetVariantFromPostData(model.Sku, "variation-");
            // TODO:
            // TransactionLibrary.AddToBasket(1, model.Sku, variant);
            return RenderView(true);
        }
        
        private ActionResult RenderView(bool addedToBasket)
        {
            Product currentProduct = SiteContext.Current.CatalogContext.CurrentProduct;

            var productViewModel = new ProductViewModel();
            productViewModel.Sku = currentProduct.Sku;
            productViewModel.Name = currentProduct.DisplayName;
            productViewModel.LongDescription = currentProduct.LongDescription;
            productViewModel.IsOrderingAllowed = currentProduct.AllowOrdering;
            productViewModel.IsProductFamily = currentProduct.ProductFamily;
            productViewModel.IsVariant = false;

            // Price calculations
            var productGuids = new List<Guid>(){ currentProduct.Guid };
            var productPriceCalculationResult = CatalogLibrary.CalculatePrices(productGuids);
            var productPriceCalculationResultItem = productPriceCalculationResult.Items.FirstOrDefault();
            if (productPriceCalculationResultItem != null)
            {
                productViewModel.TaxCalculation = productPriceCalculationResultItem.ListTax.ToString();
                productViewModel.PriceCalculation = new ProductPriceCalculationViewModel()
                {
                    YourPrice = productPriceCalculationResultItem.PriceInclTax.ToString("C"),
                    ListPrice = productPriceCalculationResultItem.ListPriceInclTax.ToString("C")
                };
            }

            if (!string.IsNullOrEmpty(currentProduct.PrimaryImageUrl))
            {
                productViewModel.ThumbnailImageUrl = currentProduct.PrimaryImageUrl;
            }

            productViewModel.Properties = MapProductProperties(currentProduct);
         
            if (currentProduct.ProductFamily)
            {
                // TODO:
                // productViewModel.Variants = MapVariants(currentProduct.Variants);
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
        
        private IList<ProductViewModel> MapVariants(ICollection<Product> variants)
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

        private IList<ProductPropertiesViewModel> MapProductProperties(Product product)
        {
            var productProperties = new List<ProductPropertiesViewModel>();

            // TODO:
            // var uniqueVariants = from v in product.Variants.SelectMany(p => p.ProductProperties)
            //                      where v.ProductDefinitionField.DisplayOnSite
            //                      group v by v.ProductDefinitionField into g
            //                      select g;
            //
            // foreach (var prop in uniqueVariants)
            // {
            //     var productPropertiesViewModel = new ProductPropertiesViewModel();
            //     productPropertiesViewModel.PropertyName = prop.Key.Name;
            //
            //     foreach (var value in prop.Select(p => p.Value).Distinct())
            //     {
            //         productPropertiesViewModel.Values.Add(value);
            //     }
            //     productProperties.Add(productPropertiesViewModel);
            // }

            return productProperties;
        }
        
        private string GetVariantFromPostData(string sku, string prefix)
        {
            var request = System.Web.HttpContext.Current.Request;
            var keys = request.Form.AllKeys.Where(k => k.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase));
            var properties = keys.Select(k => new { Key = k.Replace(prefix, string.Empty), Value = Request.Form[k] }).ToList();

            Product product = SiteContext.Current.CatalogContext.CurrentProduct;
            string variantSku = null;

            // TODO:
            // // If there are variant values we'll need to find the selected variant
            // if (!product.IsVariant && properties.Any())
            // {
            //     var variant = product.Variants.FirstOrDefault(v => v.ProductProperties
            //           .Where(pp => pp.ProductDefinitionField.DisplayOnSite
            //               && pp.ProductDefinitionField.IsVariantProperty
            //               && !pp.ProductDefinitionField.Deleted)
            //           .All(p => properties.Any(kv => kv.Key.Equals(p.ProductDefinitionField.Name, StringComparison.InvariantCultureIgnoreCase) && kv.Value.Equals(p.Value, StringComparison.InvariantCultureIgnoreCase))));
            //     variantSku = variant.VariantSku;
            // }
            //
            // // Only use the current product where there are no variants
            // else if (!product.Variants.Any())
            // {
            //     variantSku = product.Sku;
            // }

            return variantSku;
        }
    }
}
