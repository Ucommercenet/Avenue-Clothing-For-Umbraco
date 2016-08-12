using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
namespace UCommerce.MasterClass.Website.Controllers
{
	public class HomeController : RenderMvcController
	{
		public ActionResult Index()
		{
			return View("/views/frontpage.cshtml");
		}
	}
}