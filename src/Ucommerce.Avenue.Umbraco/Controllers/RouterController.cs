using System.Web.Mvc;
using Ucommerce.Api;
using UCommerce.Infrastructure;
using Umbraco.Web.Mvc;

namespace UCommerce.RazorStore.Controllers
{
    public class RouterController : RenderMvcController
    {
        public ICatalogContext CatalogContext => ObjectFactory.Instance.Resolve<ICatalogContext>();

        [Route("{*url}")]
        public ActionResult Index()
        {
            var product = CatalogContext.CurrentProduct;
            
            if (product != null)
            {
                return RedirectToAction("Index", "Product");
            }
            return RedirectToAction("Index", "Category");
        }
    }
}