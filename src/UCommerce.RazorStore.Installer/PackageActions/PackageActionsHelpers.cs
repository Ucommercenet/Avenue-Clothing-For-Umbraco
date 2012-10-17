using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UCommerce.RazorStore.Installer.PackageActions
{
    using System.IO;
    using System.Xml;

    public class PackageActionsHelpers
    {
        public static XmlNode ParseStringToXmlNode(string value)
        {
            var doc = new XmlDocument();
            doc.LoadXml(value);
            return doc.SelectSingleNode(".");
        }

        public static string GetAttributeValueFromNode(XmlNode node, string attributeName)
        {
            string str = string.Empty;
            if (node.Attributes[attributeName] != null)
                str = node.Attributes[attributeName].InnerText;
            return str;
        }

        public static void BackupExistingXmlConfig(string path)
        {
            var directory = Path.GetDirectoryName(path);
            var filename = Path.GetFileName(path);
            var newPath = string.Format("{0}.{1}.backup", Path.Combine(directory, filename), DateTime.Now.Ticks);
            //Log.Add(LogTypes.Debug, -1, string.Format("MergeConfigFiles Backup Existing Config: {0} to: {1}", path, newPath));
            loadTargetConfigAsXml(path).Save(newPath);
        }

        public static XmlDocument loadTargetConfigAsXml(string path)
        {
            XmlDocument _targetModeConfig = new XmlDocument();
            _targetModeConfig.Load(path);
            return _targetModeConfig;
        }

        public static string GetLocalPath(string path)
        {
            if (path.StartsWith("~") || path.StartsWith(".") || path.StartsWith("/") || path.StartsWith("\\"))
                return HttpContext.Current.Server.MapPath(path);

            return path;
        }
    }
}