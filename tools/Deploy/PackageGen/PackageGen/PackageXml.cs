namespace PackageGen
{
    using System.Xml.Linq;

    /// <summary>
    /// Represents the Package.Xml manifest file used by Umbraco
    /// </summary>
    public class PackageXml
    {
        private XDocument Xml { get; set; }

        public PackageXml()
        {
            Xml = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), new XElement("umbPackage"));

        }
    }
}