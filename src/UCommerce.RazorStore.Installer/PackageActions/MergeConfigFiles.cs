namespace UCommerce.RazorStore.Installer.PackageActions
{
    using System;
    using System.IO;
    using System.Xml;

    using Tools.XmlConfigMerge;

    using umbraco.interfaces;

    public class MergeConfigFiles : IPackageAction
    {
        private string _targetConfigFullPath;
        public string _sourceConfigFullPath;
        public string _sourceConfigIntegratedModeFullPath;

        private XmlDocument _targetModeConfig;

        public string TargetConfigPath { get; set; }
        public string SourceConfigPath { get; set; }
        public string SourceConfigIntegratedModePath { get; set; }

        public string Alias()
        {
            return "MergeConfigFiles";
        }

        public bool Execute(string packageName, XmlNode xmlData)
        {
            try
            {
                Initialize(xmlData);

                BackupExistingConfig(_targetConfigFullPath);

                //Log.Add(LogTypes.Debug, -1, string.Format("MergeConfigFiles: {0} into: {1}", _sourceConfigFullPath, _targetConfigFullPath));
                var config = new ConfigFileManager(_targetConfigFullPath, _sourceConfigFullPath);
                config.Save();

                if (!String.IsNullOrWhiteSpace(SourceConfigIntegratedModePath))
                {
                    //Log.Add(LogTypes.Debug, -1, string.Format("MergeConfigFiles (IIS7): {0} into: {1}", _sourceConfigIntegratedModeFullPath, _targetConfigFullPath));
                    config = new ConfigFileManager(_targetConfigFullPath, _sourceConfigIntegratedModeFullPath);
                    config.Save();
                }
            }
            catch (Exception ex)
            {
                //Log.Add(LogTypes.Error, -1, string.Concat(new object[] { ex.Message, "\n", ex.TargetSite, "\n", ex.StackTrace }));
                return false;
            }
            return true;
        }

        public bool Undo(string packageName, XmlNode xmlData)
        {
            return true;
            // TODO: Update the undo
            //bool result;
            //try
            //{
            //    Initialize(xmlData);
            //    new ConfigInstaller().CleanConfig(TargetConfig);
            //    TargetConfig.Save(System.Web.HttpContext.Current.Server.MapPath("~/web.config"));
            //    //Log.Add(LogTypes.Notify, -1, "Cleaned uCommerce config");
            //    result = true;
            //}
            //catch (Exception ex)
            //{
            //    //Log.Add(LogTypes.Error, -1, string.Concat(new object[]{ex.Message,"\n",ex.TargetSite,"\n",ex.StackTrace}));
            //    result = false;
            //}
            //return result;
        }

        private void Initialize(XmlNode xmlData)
        {
            TargetConfigPath = getAttributeValueFromNode(xmlData, "targetConfig");
            SourceConfigPath = getAttributeValueFromNode(xmlData, "sourceConfig");
            SourceConfigIntegratedModePath = getAttributeValueFromNode(xmlData, "sourceConfigIntegratedMode");

            _targetConfigFullPath = GetLocalPath(TargetConfigPath);
            _sourceConfigFullPath = GetLocalPath(SourceConfigPath);
            _sourceConfigIntegratedModeFullPath = GetLocalPath(SourceConfigIntegratedModePath);
        }

        private void BackupExistingConfig(string path)
        {
            var directory = Path.GetDirectoryName(path);
            var filename = Path.GetFileName(path);
            var newPath = string.Format("{0}.{1}.backup", Path.Combine(directory, filename), DateTime.Now.Ticks);
            //Log.Add(LogTypes.Debug, -1, string.Format("MergeConfigFiles Backup Existing Config: {0} to: {1}", path, newPath));
            loadTargetConfigAsXml().Save(newPath);
        }

        private XmlDocument loadTargetConfigAsXml()
        {
            if (_targetModeConfig == null)
            {
                _targetModeConfig = new XmlDocument();
                _targetModeConfig.Load(_targetConfigFullPath);
            }
            return _targetModeConfig;
        }

        public string GetLocalPath(string path)
        {
            if (path.StartsWith("~") || path.StartsWith(".") || path.StartsWith("/") || path.StartsWith("\\"))
                return System.Web.HttpContext.Current.Server.MapPath(path);

            return path;
        }

        public XmlNode SampleXml()
        {
            string value = string.Format("<Action runat=\"install\" undo=\"true\" alias=\"{0}\" targetConfig=\"~/web.config\" sourceConfig=\"~/umbraco/ucommerce/install/uCommerce.config\" sourceConfigIntegratedMode=\"~/umbraco/ucommerce/install/uCommerce.IIS7.config\"/>", Alias());
            return parseStringToXmlNode(value);
        }

        private XmlNode parseStringToXmlNode(string value)
        {
            var doc = new XmlDocument();
            doc.LoadXml(value);
            return doc.SelectSingleNode(".");
        }

        private string getAttributeValueFromNode(XmlNode node, string attributeName)
        {
            string str = string.Empty;
            if (node.Attributes[attributeName] != null)
                str = node.Attributes[attributeName].InnerText;
            return str;
        }
    }
}
