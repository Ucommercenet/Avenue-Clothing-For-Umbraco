using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using Umbraco.Web.Mvc;
using UCommerce.RazorStore.Models;
using UCommerce.Runtime;

namespace UCommerce.RazorStore.Controllers
{
    public class MiniBasketController : SurfaceController
    {
// GET: MiniBasket
        public ActionResult Index()
        {
            var miniBasket = new MiniBasketViewModel();
            var currency = SiteContext.Current.CatalogContext.CurrentCatalog.PriceGroup.Currency;
            miniBasket.Total = new Money(0, currency);

            PurchaseOrder basket = null;

            if (TransactionLibrary.HasBasket())
            {
                basket = TransactionLibrary.GetBasket(false).PurchaseOrder;
                if (basket.OrderTotal.HasValue)
                {
                    miniBasket.Total = new Money(basket.OrderTotal.Value, currency);
                    miniBasket.NumberOfItems = basket.OrderLines.Sum(x => x.Quantity);
                }
            }

            return View("/views/MiniBasket/MiniBasket.cshtml", miniBasket);
        }
    }
}
    