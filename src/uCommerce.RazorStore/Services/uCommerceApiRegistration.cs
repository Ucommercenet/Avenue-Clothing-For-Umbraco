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
            var assemblies = GetAllAssemblies();
            var types = GetTypesFromAssemblies(assemblies);
            return types.Where(serviceType.IsAssignableFrom)
                        .Select(t => t.Assembly)
                        .Distinct()
                        .ToArray();
        }

        private static IEnumerable<Assembly> GetAllAssemblies()
        {
            return new ReadOnlyCollection<Assembly>(BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToList());
        }

        private static IEnumerable<Type> GetTypesFromAssemblies(IEnumerable<Assembly> assemblies)
        {
            var allTypes = new List<Type>();
            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes();
                    allTypes.AddRange(types);
                }
                catch (Exception)
                {
                    umbraco.BusinessLogic.Log.Add(umbraco.BusinessLogic.LogTypes.Error, umbraco.BusinessLogic.User.GetUser(0), -1, "Could not load types from: " + assembly.Location);
                }
            }

            return allTypes;
        }
    }
}