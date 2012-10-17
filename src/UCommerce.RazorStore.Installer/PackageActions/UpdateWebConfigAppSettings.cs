using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UCommerce.RazorStore.Installer.PackageActions
{
    using System.Xml;

    using Tools.XmlConfigMerge;

    using umbraco.interfaces;

    public class UpdateWebConfigAppSettings : IPackageAction
    {
        private static string _xpathFormatString = "/configuration/appSettings/add[@key='{0}']/@value";
        private string _targetConfigFullPath;

        private string TargetConfigPath { get; set; }

        private void Initialize(XmlNode xmlData)
        {
            TargetConfigPath = PackageActionsHelpers.GetAttributeValueFromNode(xmlData, "targetConfig");

            _targetConfigFullPath = PackageActionsHelpers.GetLocalPath(TargetConfigPath);
        }

        public bool Execute(string packageName, XmlNode xmlData)
        {
            Initialize(xmlData);

            PackageActionsHelpers.BackupExistingXmlConfig(_targetConfigFullPath);

            var config = new ConfigFileManager(_targetConfigFullPath);

            var current = GetRewritingNode(config, "umbracoReservedPaths");
            if (!String.IsNullOrEmpty(current))
            {
                if (!current.Contains(",~/ucommerceapi/"))
                    AddValue(config, "umbracoReservedPaths", string.Concat(current, ",~/ucommerceapi/"));
            }
            else
            {
                throw new ArgumentOutOfRangeException("umbracoReservedPaths", "No setting for umbracoReservedPaths could be found in your web.config -are you sure this is an Umbraco install?");
            }

            config.Save();

            return true;
        }

        public string Alias()
        {
            return "UpdateWebConfigAppSettings";
        }

        public bool Undo(string packageName, XmlNode xmlData)
        {
            return true;
        }

        public XmlNode SampleXml()
        {
            string value = string.Format("<Action runat=\"install\" undo=\"true\" alias=\"{0}\" targetConfig=\"~/config/UrlRewriting.config\" />", Alias());
            return PackageActionsHelpers.ParseStringToXmlNode(value);
        }

        private string GetRewritingNode(ConfigFileManager config, string key)
        {
            var xpath = string.Format(_xpathFormatString, key);
            return config.GetXPathValue(xpath);
        }

        private void AddValue(ConfigFileManager config, string key, string value)
        {
            var xpath = string.Format(_xpathFormatString, key);
            config.ReplaceXPathValues(xpath, value);
        }
    }
}