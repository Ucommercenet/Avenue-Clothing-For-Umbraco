using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UCommerce.Api;
using UCommerce.Infrastructure;
using UCommerce.Publishing.Model;
using UCommerce.Publishing.Runtime;
using UCommerce.RazorStore.Models;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace UCommerce.RazorStore.Controllers
{
    public class ProductController : RenderMvcController
    {
        public ActionResult Index(RenderModel model)
        {
            return RenderView(false);
        }

        [HttpPost]
        public ActionResult Index(AddToBasketViewModel model)
        {
            string variant = GetVariantFromPostData("variation-");
            TransactionLibrary.AddToBasket(1, model.Sku, variant);

            return RenderView(true);
        }
        
        private ActionResult RenderView(bool addedToBasket)
        {
            var catalogContext = ObjectFactory.Instance.Resolve<ICatalogContext>();
            var catalogLibrary = ObjectFactory.Instance.Resolve<ICatalogLibrary>();

            Product<Variant> currentProduct = catalogContext.CurrentProduct;

            var priceCalculation = catalogLibrary.CalculatePrice(catalogContext.CurrentCatalogId, currentProduct);

            var productViewModel = new ProductViewModel
            {
                Sku = currentProduct.Sku,
                PriceCalculation = priceCalculation,
                Name = currentProduct.DisplayName,
                LongDescription = currentProduct.LongDescription,
                IsVariant = false,
                IsOrderingAllowed = currentProduct.AllowOrdering,
                TaxCalculation = priceCalculation.YourTax.ToString(),
                ThumbnailImageUrl = currentProduct.ThumbnailImageUrl,
                Properties = MapProductProperties(currentProduct)
            };


            if (currentProduct.Variants.Any())
            {
                productViewModel.Variants = MapVariants(currentProduct.Variants);
            }

            bool isInBasket = TransactionLibrary.GetBasket(true).PurchaseOrder.OrderLines.Any(x => x.Sku == currentProduct.Sku);
            
            ProductPageViewModel productPageViewModel = new ProductPageViewModel()
            {
                ProductViewModel = productViewModel,
                AddedToBasket = addedToBasket,
                ItemAlreadyExists = isInBasket
            };

            return View("/Views/Product.cshtml", productPageViewModel);
        }

        private IList<ProductViewModel> MapVariants(ICollection<Variant> variants)
        {
            var variantModels = new List<ProductViewModel>();
            foreach (var currentVariant in variants)
            {
                ProductViewModel productModel = new ProductViewModel();
                productModel.Sku = currentVariant.Sku;
                productModel.VariantSku = currentVariant.VariantSku;
                productModel.Name = currentVariant.DisplayName;
                productModel.LongDescription = currentVariant.LongDescription;
                productModel.IsVariant = true;
                
                variantModels.Add(productModel);
            }

            return variantModels;
        }

        private IList<ProductPropertiesViewModel> MapProductProperties(Product<Variant> product)
        {
            var productProperties = new List<ProductPropertiesViewModel>();

            var uniqueVariants = from v in product.Variants.SelectMany(p => p.Properties)
                                 group v by v.Name into g
                                 select g;

            foreach (var prop in uniqueVariants)
            {
                var productPropertiesViewModel = new ProductPropertiesViewModel();
                productPropertiesViewModel.PropertyName = prop.Key;

                foreach (var value in prop.Select(p => p.Value).Distinct())
                {
                    productPropertiesViewModel.Values.Add(value);
                }
                productProperties.Add(productPropertiesViewModel);
            }

            return productProperties;
        }
        
        private string GetVariantFromPostData(string prefix)
        {
            var catalogContext = ObjectFactory.Instance.Resolve<ICatalogContext>();

            var request = System.Web.HttpContext.Current.Request;
            var keys = request.Form.AllKeys.Where(k => k.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase));
            var properties = keys.Select(k => new { Key = k.Replace(prefix, string.Empty), Value = Request.Form[k] }).ToList();

            Product<Variant> product = catalogContext.CurrentProduct;
            string variantSku = null;

            // If there are variant values we'll need to find the selected variant
            if (product.Variants.Any())
            {
                var variant = product.Variants.FirstOrDefault(v => v.Properties
                      .All(p => properties.Any(kv => kv.Key.Equals(p.Name, StringComparison.InvariantCultureIgnoreCase) && kv.Value.Equals(p.Value, StringComparison.InvariantCultureIgnoreCase))));

                if (variant == null) { throw new InvalidOperationException("Could not find the correct variant"); }
                variantSku = variant.VariantSku;
            }
           
            // Only use the current product where there are no variants
            else if (!product.Variants.Any())
            {
                variantSku = product.Sku;
            }

            return variantSku;
        }
    }
}
