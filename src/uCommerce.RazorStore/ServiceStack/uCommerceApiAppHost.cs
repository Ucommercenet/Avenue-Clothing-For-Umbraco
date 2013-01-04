using System.Web;

namespace uCommerce.RazorStore.ServiceStack
{
    using System.ComponentModel;

    using ServiceStack.WebHost.Endpoints;

    public class uCommerceApiAppHost : AppHostBase, ISupportInitialize
    {
        public uCommerceApiAppHost()
            : base("uCommerce Web Services", typeof(uCommerceApiAppHost).Assembly)
        { }

        public override void Configure(Funq.Container container)
        {
        }

        // is called when windsor resolves AppHostBase
        public void BeginInit()
        {
            new uCommerceApiAppHost().Init();
        }

        public void EndInit()
        {
        }
    }
}