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
        public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
        public IIndex<Product> ProductIndex => ObjectFactory.Instance.Resolve<IIndex<Product>>();

        // GET: HomepageCatalog
        public ActionResult Index()
        {
            var products = ProductIndex.Find()
                .Where(p => (bool) p["ShowOnHomepage"] == true)
                .Take(12)
                .ToList();

            ProductsViewModel productsViewModel = new ProductsViewModel();

            foreach (var product in products)
            {
                var niceUrl = UrlService.GetUrl(CatalogContext.CurrentCatalog, new[] {product});
                var unitPrice = product.UnitPrices[CatalogContext.CurrentPriceGroup.Name];
                var currencyIsoCode = CatalogContext.CurrentPriceGroup.CurrencyISOCode;
                var taxRate = CatalogContext.CurrentPriceGroup.TaxRate;

                productsViewModel.Products.Add(new ProductViewModel()
                {
                    Sku = product.Sku,
                    Name = product.Name,
                    Url = niceUrl,
                    Price = new ApiMoney(unitPrice * (1.0M + taxRate), currencyIsoCode).ToString(),
                    Tax = new ApiMoney(unitPrice * taxRate, currencyIsoCode).ToString(),
                    IsVariant = !String.IsNullOrWhiteSpace(product.VariantSku),
                    VariantSku = product.VariantSku,
                    ThumbnailImageUrl = product.ThumbnailImageUrl
                });
            }

            return View("/Views/PartialView/HomepageCatalog.cshtml", productsViewModel);
        }
    }
}