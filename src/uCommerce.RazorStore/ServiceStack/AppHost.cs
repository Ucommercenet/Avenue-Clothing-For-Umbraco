using System.ComponentModel;
using ServiceStack.WebHost.Endpoints;
using Container = Funq.Container;

namespace uCommerce.RazorStore.ServiceStack
{
    public class AppHost : AppHostBase, ISupportInitialize
    {
        
        public AppHost() : base("uCommerce Web Services", typeof(AppHost).Assembly) { }

        public override void Configure(Container container)
        {
            ////register user-defined REST-ful urls
            //Routes
            //  .Add<Hello>("/hello")
            //  .Add<Hello>("/hello/{Name}");

            //SetConfig(new EndpointHostConfig { DebugMode = true });
        }

        // is called when windsor resolves AppHostBase
        public void BeginInit()
        {
            new AppHost().Init();
        }

        public void EndInit()
        {
        }
    }
}