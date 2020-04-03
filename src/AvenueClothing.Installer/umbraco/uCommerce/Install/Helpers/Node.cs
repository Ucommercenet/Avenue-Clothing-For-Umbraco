using Umbraco.Core.Composing;
using Umbraco.Core.Models;

namespace AvenueClothing.Installer.Helpers
{
    public class Node
    {
        public static void PublishChildDocs(IContent doc)
        {
            Current.Services.ContentService.SaveAndPublishBranch(doc, true);
        }
    }

}
