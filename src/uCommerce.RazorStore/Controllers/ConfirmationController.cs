using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace UCommerce.RazorStore.Controllers
{
    public class ConfirmationController : RenderMvcController
    {
        // GET: Confirmation
        public ActionResult Index()
        {
            return View("/Views/Confirmation.cshtml");
        }
    }
}