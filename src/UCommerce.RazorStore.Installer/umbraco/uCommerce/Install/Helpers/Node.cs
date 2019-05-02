using Umbraco.Core.Composing;
using Umbraco.Core.Models;

namespace UCommerce.RazorStore.Installer.Helpers
{
    public class Node
    {
        public static void PublishChildDocs(IContent doc)
        {
            Current.Services.ContentService.SaveAndPublishBranch(doc, true);
        }
    }
}