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
        readonly ICatalogContext _catalogContext = ObjectFactory.Instance.Resolve<ICatalogContext>();
        readonly ICatalogLibrary _catalogLibrary = ObjectFactory.Instance.Resolve<ICatalogLibrary>();
        readonly IIndex<Product> _productIndex = ObjectFactory.Instance.Resolve<IIndex<Product>>();
        readonly IUrlService _urlService = ObjectFactory.Instance.Resolve<IUrlService>();

        // GET: Search
        public ActionResult Index(ContentModel model)
        {
            var keyword = System.Web.HttpContext.Current.Request.QueryString["search"];
            IEnumerable<Product> products = new List<Product>();
            ProductsViewModel productsViewModel = new ProductsViewModel();

            //TODO: test this
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                products = _productIndex.Find()
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

            var productPriceCalculationResult = _catalogLibrary.CalculatePrices(products.Select(p => p.Guid).ToList());
            var pricesPerProductId = productPriceCalculationResult.Items.ToDictionary(item => item.ProductGuid);

            foreach (var product in products)
            {
                var productPriceCalculationResultItem = pricesPerProductId[product.Guid];
                productsViewModel.Products.Add(new ProductViewModel
                {
                    Url = _urlService.GetUrl(_catalogContext.CurrentCatalog, new[] {product}),
                    Name = product.DisplayName,
                    Sku = product.Sku,
                    IsVariant = !string.IsNullOrWhiteSpace(product.VariantSku),
                    LongDescription = product.LongDescription,
                    PriceCalculation = new ProductPriceCalculationViewModel
                    {
                        YourPrice = productPriceCalculationResultItem.PriceInclTax,
                        ListPrice = productPriceCalculationResultItem.ListPriceInclTax
                    },
                    ThumbnailImageUrl = product.ThumbnailImageUrl,
                    VariantSku = product.VariantSku
                });
            }

            return View("/Views/Search.cshtml", productsViewModel);
        }
    }
}