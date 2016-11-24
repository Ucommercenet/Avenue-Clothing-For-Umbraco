using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UCommerce.Infrastructure;
using UCommerce.Publishing.Model;
using UCommerce.Publishing.Runtime;
using UCommerce.RazorStore.Models;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace UCommerce.RazorStore.Controllers
{
    public class SearchController : RenderMvcController
    {
        // GET: Search
        public ActionResult Index(RenderModel model)
        {
            var keyword = System.Web.HttpContext.Current.Request.QueryString["search"];
            IEnumerable<Product> products = new List<Product>();
            ProductsViewModel productsViewModel = new ProductsViewModel();

            var searchLibrary = ObjectFactory.Instance.Resolve<ISearchLibrary>();
            var catalogLibrary = ObjectFactory.Instance.Resolve<ICatalogLibrary>();
            var catalogContext = ObjectFactory.Instance.Resolve<ICatalogContext>();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                products = searchLibrary.FindProducts(keyword);
            }

            foreach (var product in products.Where(x=> x.DisplayOnSite))
            {
                productsViewModel.Products.Add(new ProductViewModel()
                {
                    Url = catalogLibrary.GetNiceUrlForProduct(product),
                    Name = product.DisplayName,
                    Sku = product.Sku,
                    IsVariant = false,
                    LongDescription = product.LongDescription,
                    PriceCalculation = catalogLibrary.CalculatePrice(catalogContext.CurrentCatalogId, product),
                    ThumbnailImageUrl = product.ThumbnailImageUrl,
                    VariantSku = product.VariantSku
                });
            }
        
            return base.View("/Views/Search.cshtml", productsViewModel);
        }
    }
}