using System.Web.Mvc;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace Ucommerce.Avenue.Umbraco.Controllers
{
    public class ConfirmationController : RenderMvcController
    {
        // GET: Confirmation
        public override ActionResult Index(ContentModel model)
        {
          return View("/Views/Confirmation.cshtml");
        }
    }
}