using System.Web;
using System.Xml.Linq;
using Umbraco.Core.PackageActions;

namespace UCommerce.RazorStore.Installer.PackageActions
{
    public class UnloadAppDomain: IPackageAction
    {
        public bool Execute(string packageName, XElement xmlData)
        {
            HttpRuntime.UnloadAppDomain();
            return true;
        }

        public string Alias()
        {
            return "UnloadAppDomain";
        }

        public bool Undo(string packageName, XElement xmlData)
        {
            // Does not support undo
            return true;
        }
    }
}