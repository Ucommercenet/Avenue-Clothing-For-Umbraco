using System;

using UCommerce.EntitiesV2;

namespace UCommerce.RazorStore.Installer.umbraco.uCommerce.Install
{
    public partial class DemoStoreInstaller1 : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnInstall_Click(object sender, EventArgs e)
        {
            if (chkSettings.Checked)
            {
                var installer = new ConfigurationInstaller();
                installer.Configure();
            }

            if (chkCatalog.Checked)
            {
                var installer = new CatalogueInstaller("avenue-clothing.com", "Demo Store");
                installer.Configure();
            }

            pnlInstall.Visible = false;
            pnlthanks.Visible = true;
        }

        protected void btnAssignPermissions_Click(object sender, EventArgs e)
        {
            var installer = new ConfigurationInstaller();
            installer.AssignAccessPermissionsToDemoStore();
        }
    }
}