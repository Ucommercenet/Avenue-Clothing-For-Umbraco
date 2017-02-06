using System.Web.Mvc;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
namespace UCommerce.RazorStore.Controllers
{
	public class HomeController : RenderMvcController
	{
        [HttpGet]
        public ActionResult Index()
		{
			return View("/views/frontpage.cshtml");
		}


	}
}