using System.Linq;
using System.Web.Mvc;
using Ucommerce.Api;
using Ucommerce.EntitiesV2;
using Ucommerce.Infrastructure;
using AvenueClothing.Models;
using Ucommerce;
using Umbraco.Web.Mvc;

namespace AvenueClothing.Controllers
{
    public class MiniBasketController : SurfaceController
    {
        public ITransactionLibrary TransactionLibrary => ObjectFactory.Instance.Resolve<ITransactionLibrary>();

        // GET: MiniBasket
        public ActionResult Index()
        {
            var miniBasket = new MiniBasketViewModel {IsEmpty = true};

            if (TransactionLibrary.HasBasket())
            {
                PurchaseOrder basket = TransactionLibrary.GetBasket(false);
                var numberOfItems = basket.OrderLines.Sum(x => x.Quantity);
                if (numberOfItems != 0)
                {
                    miniBasket.Total = new Money(basket.OrderTotal.GetValueOrDefault(), basket.BillingCurrency);
                    miniBasket.NumberOfItems = basket.OrderLines.Sum(x => x.Quantity);
                    miniBasket.IsEmpty = false;
                }
            }

            return View("/Views/PartialView/MiniBasket.cshtml", miniBasket);
        }
    }
}
