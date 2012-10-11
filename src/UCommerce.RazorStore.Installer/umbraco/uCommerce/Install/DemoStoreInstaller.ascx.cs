using System;

using UCommerce.EntitiesV2;

namespace UCommerce.RazorStore.Installer.umbraco.uCommerce.Install
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

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

            var messages = new List<string>();
            if (chkDelete.Checked)
                messages.Add("You will need to delete the default uCommerce store manually.");

            if (chkPublish.Checked)
                messages.Add("You will need to publish the new content nodes manually.");

            if (messages.Any())
            {
                if (messages.Count == 1)
                {
                    litStatus.Text = String.Format("<p style=\"padding: 8px 35px 8px 14px; margin-bottom: 20px; text-shadow: 0 1px 0 rgba(255, 255, 255, 0.5); background-color: #fcf8e3; border: 1px solid #fbeed5; -webkit-border-radius: 4px; -moz-border-radius: 4px; border-radius: 4px; color: #c09853;\">{0}</p>", messages.First());
                }
                else
                {
                    var sb = messages.Aggregate(String.Empty, (current, msg) => current + ("<li>" + msg + "</li>"));
                    litStatus.Text = String.Format("<div style=\"padding: 8px 35px 8px 14px; margin-bottom: 20px; text-shadow: 0 1px 0 rgba(255, 255, 255, 0.5); background-color: #fcf8e3; border: 1px solid #fbeed5; -webkit-border-radius: 4px; -moz-border-radius: 4px; border-radius: 4px; color: #c09853;\"><p>Please note the following:</p><ul>{0}</ul></div>", sb);
                }
            }

            pnlInstall.Visible = false;
            pnlThanks.Visible = true;
        }

        protected void btnAssignPermissions_Click(object sender, EventArgs e)
        {
            var installer = new ConfigurationInstaller();
            installer.AssignAccessPermissionsToDemoStore();
            pnlThanks.Visible = false;
            pnlThanks2.Visible = true;
        }
    }
}