using System.Net.Mail;
using System.Web.Mvc;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace UCommerce.RazorStore.Controllers
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