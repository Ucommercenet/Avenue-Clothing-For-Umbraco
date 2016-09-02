using System;
using System.Web.Mvc;
using UCommerce.Api;
using Umbraco.Web.Mvc;

namespace UCommerce.RazorStore.Controllers
{
    public class VoucherController : SurfaceController
    {
        // GET: Voucher
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