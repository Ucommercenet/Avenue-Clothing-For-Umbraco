using System;
using System.Web.Mvc;
using Ucommerce.Api;
using Ucommerce.Api.PriceCalculation;
using Ucommerce.Infrastructure;
using AvenueClothing.Models;
using Ucommerce;
using Ucommerce.Search;
using Ucommerce.Search.Models;
using Ucommerce.Search.Slugs;
using Umbraco.Web.Mvc;

namespace AvenueClothing.Controllers
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
                string niceUrl = UrlService.GetUrl(CatalogContext.CurrentCatalog, product);
                product.PricesInclTax.TryGetValue(CatalogContext.CurrentPriceGroup.Name, out decimal price);
                product.Taxes.TryGetValue(CatalogContext.CurrentPriceGroup.Name, out decimal tax);
                string currencyIsoCode = CatalogContext.CurrentPriceGroup.CurrencyISOCode;

                productsViewModel.Products.Add(new ProductViewModel
                {
                    Sku = product.Sku,
                    Name = product.Name,
                    Url = niceUrl,
                    Price = price > 0 ? new Money(price, currencyIsoCode).ToString() : "",
                    Tax = tax > 0 ? new Money(tax, currencyIsoCode).ToString() : "",
                    IsVariant = !String.IsNullOrWhiteSpace(product.VariantSku),
                    VariantSku = product.VariantSku,
                    ThumbnailImageUrl = product.ThumbnailImageUrl
                });
            }

            return View("/Views/PartialView/HomepageCatalog.cshtml", productsViewModel);
        }
    }
}