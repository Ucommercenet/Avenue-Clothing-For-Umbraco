using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Web;
using System.Web.Compilation;

//[assembly: PreApplicationStartMethod(typeof(UCommerce.RazorStore.Services.uCommerceApiRegistration), "Start")]

namespace UCommerce.RazorStore.Services
{
    using System;
    using System.Linq;
    using System.Reflection;

    public class uCommerceApiRegistration : umbraco.BusinessLogic.ApplicationBase
    {
        public uCommerceApiRegistration()
        {
            Start();
        }

        public static void Start()
        {
            var assemblies = GetServicesFromAssemblies();
            var host = new uCommerceApiAppHost(assemblies);
            host.Init();
        }

        private static Assembly[] GetServicesFromAssemblies()
        {
            var serviceType = typeof(IUCommerceApiService);
            var assemblies = GetAssemblies();
            return assemblies
                        .SelectMany(s => s.GetTypes())
                        .Where(serviceType.IsAssignableFrom)
                        .Select(t => t.Assembly)
                        .Distinct()
                        .ToArray();
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            return new ReadOnlyCollection<Assembly>(BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToList());
        }
    }
}