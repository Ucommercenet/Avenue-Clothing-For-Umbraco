using UCommerce.Infrastructure.Components.Windsor;
using UCommerce.Infrastructure.Logging;
using UCommerce.Pipelines;
using UCommerce.Pipelines.Initialization;
using UCommerce.RazorStore.Installer.Helpers;
using UCommerce.RazorStore.Installer.PackageActions;

namespace UCommerce.RazorStore.Installer.Initialize
{
    public class InstallAccelerator: IPipelineTask<InitializeArgs>
    {
        [Mandatory]
        public ILoggingService LoggingService { get; set; }
        
        public PipelineExecutionResult Execute(InitializeArgs subject)
        {
            LoggingService.Log<InstallAccelerator>("Installing Avenue Clothing.");
            var installer = new ConfigurationInstaller();
            installer.Configure();

            // Install Demo store Catalog
            var installer2 = new CatalogueInstaller("avenue-clothing.com", "Demo Store");
            installer2.Configure();

            var defaultAcceleratorDataInstaller = new InstallDefaultAcceleratorData();
            defaultAcceleratorDataInstaller.CreateMediaContent();

            defaultAcceleratorDataInstaller.DeleteOldUCommerceData();

            defaultAcceleratorDataInstaller.PublishContent();

            LoggingService.Log<InstallAccelerator>("Installing Avenue Clothing finished successfully.");
            return PipelineExecutionResult.Success;
        }
    }
}