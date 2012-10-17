namespace uCommerce.RazorStore.ServiceStack
{
    using System;

    public class ApplicationBase
    {
        public static void Application_Start()
        {
            new AppHost().Init();
        }
    }
}