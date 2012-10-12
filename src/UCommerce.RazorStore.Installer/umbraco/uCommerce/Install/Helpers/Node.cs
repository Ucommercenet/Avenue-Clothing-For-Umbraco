using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.web;

namespace UCommerce.RazorStore.Installer.Helpers
{
    public class Node
    {
        public static void PublishChildDocs(Document doc)
        {
            doc.Publish(User.GetUser(0));

            foreach (var childDoc in doc.Children)
            {
                PublishChildDocs(childDoc);
            }
        }
    }

}
