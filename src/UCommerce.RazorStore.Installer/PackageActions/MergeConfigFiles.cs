namespace UCommerce.RazorStore.Installer.PackageActions
{
    using System;
    using System.Xml;

    using Tools.XmlConfigMerge;

    using umbraco.BusinessLogic;
    using umbraco.interfaces;

    public class MergeConfigFiles : IPackageAction
    {
        private readonly bool _inTestMode;

        private string _targetConfigFullPath;
        private string _sourceConfigFullPath;
        private string _sourceConfigIntegratedModeFullPath;

        private string TargetConfigPath { get; set; }
        private string SourceConfigPath { get; set; }
        private string SourceConfigIntegratedModePath { get; set; }

        public MergeConfigFiles()
        {
            _inTestMode = false;
        }

        public MergeConfigFiles(bool inTestMode)
        {
            _inTestMode = inTestMode;
        }

        public string Alias()
        {
            return "MergeConfigFiles";
        }

        public bool Execute(string packageName, XmlNode xmlData)
        {
            try
            {
                Initialize(xmlData);

                PackageActionsHelpers.BackupExistingXmlConfig(_targetConfigFullPath);

                if (!_inTestMode)
                    Log.Add(LogTypes.Debug, -1, string.Format("MergeConfigFiles: {0} into: {1}", _sourceConfigFullPath, _targetConfigFullPath));
                var config = new ConfigFileManager(_targetConfigFullPath, _sourceConfigFullPath);
                config.Save();

                if (!String.IsNullOrWhiteSpace(SourceConfigIntegratedModePath))
                {
                    if (!_inTestMode)
                        Log.Add(LogTypes.Debug, -1, string.Format("MergeConfigFiles (IIS7): {0} into: {1}", _sourceConfigIntegratedModeFullPath, _targetConfigFullPath));
                    config = new ConfigFileManager(_targetConfigFullPath, _sourceConfigIntegratedModeFullPath);
                    config.Save();
                }
            }
            catch (Exception ex)
            {
                if (!_inTestMode)
                    Log.Add(LogTypes.Error, -1, string.Concat(new object[] { ex.Message, "\n", ex.TargetSite, "\n", ex.StackTrace }));
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
            //    if(!_inTestMode)
            //    Log.Add(LogTypes.Notify, -1, "Cleaned uCommerce config");
            //    result = true;
            //}
            //catch (Exception ex)
            //{
            //    if(!_inTestMode)
            //    Log.Add(LogTypes.Error, -1, string.Concat(new object[]{ex.Message,"\n",ex.TargetSite,"\n",ex.StackTrace}));
            //    result = false;
            //}
            //return result;
        }

        private void Initialize(XmlNode xmlData)
        {
            TargetConfigPath = PackageActionsHelpers.GetAttributeValueFromNode(xmlData, "targetConfig");
            SourceConfigPath = PackageActionsHelpers.GetAttributeValueFromNode(xmlData, "sourceConfig");
            SourceConfigIntegratedModePath = PackageActionsHelpers.GetAttributeValueFromNode(xmlData, "sourceConfigIntegratedMode");

            _targetConfigFullPath = PackageActionsHelpers.GetLocalPath(TargetConfigPath);
            _sourceConfigFullPath = PackageActionsHelpers.GetLocalPath(SourceConfigPath);
            _sourceConfigIntegratedModeFullPath = PackageActionsHelpers.GetLocalPath(SourceConfigIntegratedModePath);
        }

        public XmlNode SampleXml()
        {
            string value = string.Format("<Action runat=\"install\" undo=\"true\" alias=\"{0}\" targetConfig=\"~/web.config\" sourceConfig=\"~/umbraco/ucommerce/install/uCommerce.config\" sourceConfigIntegratedMode=\"~/umbraco/ucommerce/install/uCommerce.IIS7.config\"/>", Alias());
            return PackageActionsHelpers.ParseStringToXmlNode(value);
        }
    }
}
