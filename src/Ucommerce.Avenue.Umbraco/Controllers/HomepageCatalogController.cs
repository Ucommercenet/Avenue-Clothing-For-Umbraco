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
        public ISlugService UrlService => ObjectFactory.Instance.Resolve<ISlugService>();
        public CatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<CatalogLibrary>();
        public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
        public IIndex<Product> ProductIndex => ObjectFactory.Instance.Resolve<IIndex<Product>>();

        // GET: HomepageCatalog
        public ActionResult Index()
        {
            var productIds = CatalogContext.CurrentCategories.SelectMany(c => c.Products).ToList();

            var products = ProductIndex.Find()
                .Where(p => productIds.Contains(p.Guid) && (bool) p["ShowOnHomePage"]).ToList();

            ProductsViewModel productsViewModel = new ProductsViewModel();

            foreach (var p in products)
            {
                productsViewModel.Products.Add(new ProductViewModel()
                {
                    Name = p.Name,
                    PriceCalculation = CatalogLibrary.CalculatePrices(new List<Guid> {p.Guid}).Items.FirstOrDefault(),
                    Url = UrlService.GetUrl(CatalogContext.CurrentCatalog, new[] {p}),
                    Sku = p.Sku,
                    IsVariant = !String.IsNullOrWhiteSpace(p.VariantSku),
                    VariantSku = p.VariantSku,
                    ThumbnailImageUrl = p.ThumbnailImageUrl
                });
            }

            return View("/Views/PartialView/HomepageCatalog.cshtml", productsViewModel);
        }
    }
}