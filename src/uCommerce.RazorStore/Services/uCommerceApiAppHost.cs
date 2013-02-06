namespace UCommerce.RazorStore.Services
{
    using System.Reflection;
    using ServiceStack.WebHost.Endpoints;

    public class uCommerceApiAppHost : AppHostBase
    {
        public uCommerceApiAppHost(Assembly[] assemblies)
            : base("uCommerce Web Services", assemblies)
        { }

        public override void Configure(Funq.Container container) { }
    }
}