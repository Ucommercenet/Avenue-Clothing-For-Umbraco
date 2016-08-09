using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UCommerce.MasterClass.Website.Controllers
{
	public class HomeController : System.Web.Mvc.Controller
	{
		public ActionResult Index()
		{
			return View("/views/frontpage.cshtml");
		}
	}
}