using System.Linq;
using System.Web.Mvc;
using UCommerce;
using Ucommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.RazorStore.Models;
using Umbraco.Web.Mvc;

namespace Ucommerce.Avenue.Umbraco.Controllers
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