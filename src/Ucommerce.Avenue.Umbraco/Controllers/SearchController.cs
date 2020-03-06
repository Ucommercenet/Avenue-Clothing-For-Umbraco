using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ucommerce.Api;
using UCommerce.Infrastructure;
using UCommerce.RazorStore.Models;
using UCommerce.Search;
using UCommerce.Search.Models;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace UCommerce.RazorStore.Controllers
{
    public class SearchController : RenderMvcController
    {
        public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();
        public ICatalogLibrary CatalogLibrary => ObjectFactory.Instance.Resolve<ICatalogLibrary>();
        public IIndex<Product> ProductIndex => ObjectFactory.Instance.Resolve<IIndex<Product>>();
        public IUrlService UrlService => ObjectFactory.Instance.Resolve<IUrlService>();

        // GET: Search
        public ActionResult Index(ContentModel model)
        {
            var keyword = System.Web.HttpContext.Current.Request.QueryString["search"];
            IEnumerable<Product> products = new List<Product>();
            ProductsViewModel productsViewModel = new ProductsViewModel();

            //TODO: test this
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                products = ProductIndex.Find()
                    .Where(p => p.VariantSku == null &&
                                (
                                    p.Sku.Contains(keyword)
                                    || p.Name.Contains(keyword)
                                    || p.DisplayName.Contains(keyword)
                                    || p.ShortDescription.Contains(keyword)
                                    || p.LongDescription.Contains(keyword)
                                ))
                    .ToList();
            }

            var productPriceCalculationResult = CatalogLibrary.CalculatePrices(products.Select(p => p.Guid).ToList());
            var pricesPerProductId = productPriceCalculationResult.Items.ToLookup(item => item.ProductGuid);

            foreach (var product in products)
            {
                var productPriceCalculationResultItem = pricesPerProductId[product.Guid].First();
                productsViewModel.Products.Add(new ProductViewModel
                {
                    Url = UrlService.GetUrl(CatalogContext.CurrentCatalog, new[] {product}),
                    Name = product.DisplayName,
                    Sku = product.Sku,
                    IsVariant = !string.IsNullOrWhiteSpace(product.VariantSku),
                    LongDescription = product.LongDescription,
                    PriceCalculation = new ProductPriceCalculationViewModel
                    {
                        YourPrice = productPriceCalculationResultItem.PriceInclTax.ToString("C"),
                        ListPrice = productPriceCalculationResultItem.ListPriceInclTax.ToString("C")
                    },
                    ThumbnailImageUrl = product.ThumbnailImageUrl,
                    VariantSku = product.VariantSku
                });
            }

            return View("/Views/Search.cshtml", productsViewModel);
        }
    }
}