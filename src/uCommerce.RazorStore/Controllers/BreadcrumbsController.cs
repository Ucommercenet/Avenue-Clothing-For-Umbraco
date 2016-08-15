using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using UCommerce.Api;
using UCommerce.EntitiesV2;
using UCommerce.Extensions;
using UCommerce.Runtime;
namespace UCommerce.RazorStore.Controllers
{
    public class BreadcrumbsController : SurfaceController
    {
        public ActionResult Index()
        {
            return View("/Views/PartialView/Breadcrumbs.cshtml");
        }
    }
}