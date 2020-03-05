using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ucommerce.Api;
using UCommerce.Infrastructure;
using UCommerce.RazorStore.Models;
using UCommerce.Search;
using UCommerce.Search.Models;
using Umbraco.Web.Mvc;
using CatalogContext = Ucommerce.Api.CatalogContext;

namespace UCommerce.RazorStore.Controllers
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
                .Where(p => (bool) p["ShowOnHomePage"] == true).ToList();

            ProductsViewModel productsViewModel = new ProductsViewModel();

            // Price calculations
            var productGuids = products.Select(p => p.Guid).ToList();
            var productPriceCalculationResult = CatalogLibrary.CalculatePrices(productGuids);
            var pricesPerProductId = productPriceCalculationResult.Items.ToDictionary(item => item.ProductGuid);

            foreach (var product in products)
            {
                var productPriceCalculationResultItem = pricesPerProductId[product.Guid];
                productsViewModel.Products.Add(new ProductViewModel()
                {
                    Name = product.Name,
                    PriceCalculation = new ProductPriceCalculationViewModel()
                    {
                        YourPrice = productPriceCalculationResultItem.PriceInclTax,
                        ListPrice = productPriceCalculationResultItem.ListPriceInclTax
                    },
                    Url = UrlService.GetUrl(CatalogContext.CurrentCatalog, new[] {product}),
                    Sku = product.Sku,
                    IsVariant = !String.IsNullOrWhiteSpace(product.VariantSku),
                    VariantSku = product.VariantSku,
                    ThumbnailImageUrl = product.ThumbnailImageUrl
                });
            }

            return View("/Views/PartialView/HomepageCatalog.cshtml", productsViewModel);
        }
    }
}