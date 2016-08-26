using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UCommerce.Api;
using UCommerce.Content;
using UCommerce.Infrastructure;
using UCommerce.RazorStore.Models;
using UCommerce.Runtime;
using Umbraco.Web.Mvc;

namespace UCommerce.RazorStore.Controllers
{
    public class HomepageCatalogController : SurfaceController
    {
        // GET: HomepageCatalog
        public ActionResult Index()
        {
            var products = SiteContext.Current.CatalogContext.CurrentCatalog.Categories.SelectMany(c => c.Products.Where(p => p.ProductProperties.Any(pp => pp.ProductDefinitionField.Name == "ShowOnHomepage" && Convert.ToBoolean(pp.Value))));
            ProductsViewModel productsViewModel = new ProductsViewModel();

            foreach (var p in products)
            {
                productsViewModel.Products.Add(new ProductViewModel()
                {
                    Name = p.Name,
                    PriceCalculation = CatalogLibrary.CalculatePrice(p),
                    Url = CatalogLibrary.GetNiceUrlForProduct(p),
                    Sku = p.Sku,
                    IsVariant = p.IsVariant,
                    VariantSku = p.VariantSku,
                    ThumbnailImageUrl = ObjectFactory.Instance.Resolve<IImageService>().GetImage(p.ThumbnailImageMediaId).Url
                });
            }
        
            return View("/Views/PartialView/HomepageCatalog.cshtml", productsViewModel);
        }
    }
}