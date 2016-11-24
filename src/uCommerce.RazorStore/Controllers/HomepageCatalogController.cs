using System;
using System.Linq;
using System.Web.Mvc;
using UCommerce.Api;
using UCommerce.Content;
using UCommerce.Infrastructure;
using UCommerce.Publishing.Runtime;
using UCommerce.RazorStore.Models;
using UCommerce.Runtime;
using Umbraco.Web.Mvc;
using ICatalogContext = UCommerce.Publishing.Runtime.ICatalogContext;

namespace UCommerce.RazorStore.Controllers
{
    public class HomepageCatalogController : SurfaceController
    {
        // GET: HomepageCatalog
        public ActionResult Index()
        {
            var catalogContext = ObjectFactory.Instance.Resolve<ICatalogContext>();
            var catalogLibrary = ObjectFactory.Instance.Resolve<ICatalogLibrary>();

            var currentCatalogId = catalogContext.CurrentCatalogId;
            var products = catalogLibrary.FindProductsWhere(currentCatalogId, "ShowOnHomepage", true);

            ProductsViewModel productsViewModel = new ProductsViewModel();

            foreach (var p in products)
            {
                productsViewModel.Products.Add(new ProductViewModel()
                {
                    Name = p.Name,
                    PriceCalculation = catalogLibrary.CalculatePrice(currentCatalogId, p),
                    Url = catalogLibrary.GetNiceUrlForProduct(p),
                    Sku = p.Sku,
                    IsVariant = false,
                    VariantSku = p.VariantSku,
                    ThumbnailImageUrl = p.ThumbnailImageUrl
                });
            }
        
            return View("/Views/PartialView/HomepageCatalog.cshtml", productsViewModel);
        }
    }
}