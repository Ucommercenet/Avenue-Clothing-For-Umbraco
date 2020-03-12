using System;
using System.Linq;
using System.Web.Mvc;
using Ucommerce.Api;
using Ucommerce.Api.PriceCalculation;
using UCommerce.Catalog.Models;
using UCommerce.Infrastructure;
using UCommerce.RazorStore.Models;
using UCommerce.Search;
using UCommerce.Search.Models;
using Umbraco.Web.Mvc;

namespace Ucommerce.Avenue.Umbraco.Controllers
{
    public class HomepageCatalogController : SurfaceController
    {
        public IUrlService UrlService => ObjectFactory.Instance.Resolve<IUrlService>();
        public ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();
        public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
        public IIndex<Product> ProductIndex => ObjectFactory.Instance.Resolve<IIndex<Product>>();

        // GET: HomepageCatalog
        public ActionResult Index()
        {
            var products = ProductIndex.Find()
                .Where(p => (bool)p["ShowOnHomepage"] == true)
                .ToList();

            ProductsViewModel productsViewModel = new ProductsViewModel();

            // Price calculations
            var productGuids = products.Select(p => p.Guid).ToList();
            var productPriceCalculationResult = CatalogLibrary.CalculatePrices(productGuids);
            var pricesPerProductId = productPriceCalculationResult.Items.ToLookup(item => item.ProductGuid);

            foreach (var product in products)
            {
                var niceUrl = UrlService.GetUrl(CatalogContext.CurrentCatalog, new[] {product});
                var productPriceCalculationResultItem = pricesPerProductId[product.Guid].First();
                productsViewModel.Products.Add(new ProductViewModel()
                {
                    Sku = product.Sku,
                    Name = product.Name,
                    Url = niceUrl,
                    PriceCalculation = new ProductPriceCalculationViewModel()
                    {
                        YourPrice = new ApiMoney(productPriceCalculationResultItem.PriceInclTax, productPriceCalculationResultItem.CurrencyISOCode).ToString(),
                        ListPrice = new ApiMoney(productPriceCalculationResultItem.ListPriceInclTax, productPriceCalculationResultItem.CurrencyISOCode).ToString()
                    },
                    IsVariant = !String.IsNullOrWhiteSpace(product.VariantSku),
                    VariantSku = product.VariantSku,
                    ThumbnailImageUrl = product.ThumbnailImageUrl
                });
            }

            return View("/Views/PartialView/HomepageCatalog.cshtml", productsViewModel);
        }
    }
}