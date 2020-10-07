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
using System.Collections.Generic;
using System.Linq;

namespace AvenueClothing.Controllers
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
                .Where(p => (bool) p["ShowOnHomepage"] == true)
                .Take(12)
                .ToList();

            ProductsViewModel productsViewModel = new ProductsViewModel();

            foreach (var product in products)
            {
                string niceUrl = UrlService.GetUrl(CatalogContext.CurrentCatalog, product);
                product.UnitPrices.TryGetValue(CatalogContext.CurrentPriceGroup.Name, out decimal unitPrice);
                string currencyIsoCode = CatalogContext.CurrentPriceGroup.CurrencyISOCode;
                decimal taxRate = CatalogContext.CurrentPriceGroup.TaxRate;

                productsViewModel.Products.Add(new ProductViewModel
                {
                    Sku = product.Sku,
                    Name = product.Name,
                    Url = niceUrl,
                    Price = unitPrice > 0 ? new Money(unitPrice * (1.0M + taxRate), currencyIsoCode).ToString() : "",
                    Tax = unitPrice > 0 ? new Money(unitPrice * taxRate, currencyIsoCode).ToString() : "",
                    IsVariant = !String.IsNullOrWhiteSpace(product.VariantSku),
                    VariantSku = product.VariantSku, 
                    ThumbnailImageUrl = product.ThumbnailImageUrl,
                    Categories = CatalogLibrary.GetCategories(product.Categories).ToList(),
                    Rating = product.Rating
                });
            }

            return View("/Views/PartialView/HomepageCatalog.cshtml", productsViewModel);
        }


    }
}