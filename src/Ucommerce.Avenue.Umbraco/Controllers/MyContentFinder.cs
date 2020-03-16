using System.Web;
using System.Web.Routing;
using Umbraco.Core;
using Umbraco.Web.Routing;

namespace Ucommerce.Avenue.Umbraco.Controllers
{
    public class MyContentFinder : IContentFinder
    {
        public bool TryFindContent(PublishedRequest contentRequest)
        {
            RouteData routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));
            string controller = routeData.GetRequiredString("controller");

            if (controller == "Ucommerce.Avenue.Umbraco.Controllers.CategoryController")
            {
                var content = contentRequest.UmbracoContext.Content.GetById(1119);
                if (content == null)
                    return false; // not found let another IContentFinder in the collection try to find a document

                contentRequest.PublishedContent = content;
                return true;
            }

            // // handle all requests beginning /woot...
            // var path = contentRequest.Uri.GetAbsolutePathDecoded();
            // if (!path.StartsWith("/shop/"))
            //     return false; // not found
            //
            // // have we got a node with ID 1234?
            // var content = contentRequest.UmbracoContext.Content.GetById(1234);
            // if (content == null)
            //     return false; // not found let another IContentFinder in the collection try to find a document

            return false;
        }
    }
}