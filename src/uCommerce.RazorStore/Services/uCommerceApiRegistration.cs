using System.Collections.Generic;
using System.IO;
using System.Web;

[assembly: PreApplicationStartMethod(typeof(UCommerce.RazorStore.Services.uCommerceApiRegistration), "Start")]

namespace UCommerce.RazorStore.Services
{
    using System;
    using System.Linq;
    using System.Reflection;

    public class uCommerceApiRegistration
    {
        public static void Start()
        {
            var assemblies = GetServicesFromAssemblies();
            var host = new uCommerceApiAppHost(assemblies);
            host.Init();
        }

        private static Assembly[] GetServicesFromAssemblies()
        {
            var serviceType = typeof(IUCommerceApiService);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            return assemblies
                        .SelectMany(s => s.GetTypes())
                        .Where(serviceType.IsAssignableFrom)
                        .Select(t => t.Assembly)
                        .Distinct()
                        .ToArray();
        }
    }
}