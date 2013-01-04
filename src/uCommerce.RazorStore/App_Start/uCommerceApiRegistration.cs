using System.Web;

[assembly: PreApplicationStartMethod(typeof(uCommerce.RazorStore.App_Start.uCommerceApiRegistration), "Start")]

namespace uCommerce.RazorStore.App_Start
{
    using System.ComponentModel;

    using ServiceStack.WebHost.Endpoints;

    public class uCommerceApiRegistration 
    {
        public static void Start()
        {
            new uCommerceApiAppHost().Init();
        }
    }
}