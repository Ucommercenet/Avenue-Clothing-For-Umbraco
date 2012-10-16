namespace uCommerce.RazorStore.ServiceStack
{
    using System;
    using System.Web;

    public class ApplicationBase  : HttpApplication
	{
		protected void Application_Start(object sender, EventArgs e)
		{
            new AppHost().Init();
        }
    }
}