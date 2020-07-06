using System.Web.Mvc;
using System.Web.Security;
using AvenueClothing.Models;

namespace AvenueClothing.Controllers
{
 public class MembershipController : Umbraco.Web.Mvc.SurfaceController
{
    [HttpGet]
    [ActionName("MemberLogin")]
    public ActionResult Index()
    {
        return PartialView("LoginForm", new LoginViewModel());
    }

    [HttpGet]
    public ActionResult Logout()
    {
        Session.Clear();
        FormsAuthentication.SignOut();
        return Redirect("/");
    }

    [HttpPost]
    [ActionName("MemberLogin")]
    public ActionResult Validate(LoginViewModel model)
    {
        if (Membership.ValidateUser(model.Login, model.Password))
        {
            FormsAuthentication.SetAuthCookie(model.Login, model.RememberMe);
            return RedirectToCurrentUmbracoPage();
        }

        TempData["Status"] = "Invalid Log-in Credentials";
        return RedirectToCurrentUmbracoPage();
    }
}
}