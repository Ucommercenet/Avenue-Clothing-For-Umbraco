using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ucommerce.Api;
using Ucommerce.Api.PriceCalculation;
using UCommerce.Infrastructure;
using UCommerce.RazorStore.Models;
using UCommerce.Search;
using UCommerce.Search.Models;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace Ucommerce.Avenue.Umbraco.Controllers
{
    public class SearchController : RenderMvcController
    {
        public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
        public IIndex<Product> ProductIndex => ObjectFactory.Instance.Resolve<IIndex<Product>>();
        public IUrlService UrlService => ObjectFactory.Instance.Resolve<IUrlService>();

        // GET: Search
        public ActionResult Index(ContentModel model)
        {
            var keyword = System.Web.HttpContext.Current.Request.QueryString["search"];
            IEnumerable<Product> products = new List<Product>();
            ProductsViewModel productsViewModel = new ProductsViewModel();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                products = ProductIndex.Find()
                    .Where(p => p.VariantSku == null &&
                                (
                                    p.Sku.Contains(keyword)
                                    || p.Name.Contains(keyword)
                                    || p.DisplayName == Match.FullText(keyword)
                                    || p.ShortDescription == Match.FullText(keyword)
                                    || p.LongDescription == Match.FullText(keyword)
                                ))
                    .ToList();
            }

            var currencyIsoCode = CatalogContext.CurrentPriceGroup.CurrencyISOCode;
            var taxRate = CatalogContext.CurrentPriceGroup.TaxRate;

            foreach (var product in products)
            {
                var unitPrice = product.UnitPrices[CatalogContext.CurrentPriceGroup.Name];
                productsViewModel.Products.Add(new ProductViewModel
                {
                    Url = UrlService.GetUrl(CatalogContext.CurrentCatalog, new[] {product}),
                    Name = product.DisplayName,
                    Sku = product.Sku,
                    IsVariant = !string.IsNullOrWhiteSpace(product.VariantSku),
                    LongDescription = product.LongDescription,
                    Tax = new Money(unitPrice * taxRate, currencyIsoCode).ToString(),
                    Price = new Money(unitPrice * (1.0M + taxRate), currencyIsoCode).ToString(),
                    ThumbnailImageUrl = product.ThumbnailImageUrl,
                    VariantSku = product.VariantSku
                });
            }

            return View("/Views/Search.cshtml", productsViewModel);
        }
    }
}