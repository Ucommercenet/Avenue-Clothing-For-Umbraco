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
            var type = typeof(IUCommerceApiService);
            var dirs = getAssembliesFromDirectory();
            return dirs
                    .SelectMany(s => s.GetTypes())
                    .Where(type.IsAssignableFrom)
                    .Select(t => t.Assembly)
                    .Distinct()
                    .ToArray();
        }

        private static IEnumerable<Assembly> getAssembliesFromDirectory()
        {
            var path = Assembly.GetExecutingAssembly().Location;
            var fls = Directory.GetFiles(path, "*.dll");
            return fls.Select(f => Assembly.LoadFile(f));
        }
    }
}