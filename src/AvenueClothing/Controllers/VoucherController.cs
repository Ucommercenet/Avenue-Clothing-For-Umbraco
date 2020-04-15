using System;
using System.Web.Mvc;
using Ucommerce.Api;
using Ucommerce.Infrastructure;
using Umbraco.Web.Mvc;

namespace AvenueClothing.Controllers
{
    public class VoucherController : SurfaceController
    {
        public ITransactionLibrary TransactionLibrary => ObjectFactory.Instance.Resolve<ITransactionLibrary>();
        public IMarketingLibrary MarketingLibrary => ObjectFactory.Instance.Resolve<IMarketingLibrary>();

        // GET: Voucher
        [HttpGet]
        public ActionResult Index()
        {
            return View("/Views/PartialView/Voucher.cshtml");
        }

        [HttpPost]
        public ActionResult Index(String voucher)
        {
            MarketingLibrary.AddVoucher(voucher);
            TransactionLibrary.ExecuteBasketPipeline();
            return Redirect(this.CurrentPage.Url);
        }
    }
}