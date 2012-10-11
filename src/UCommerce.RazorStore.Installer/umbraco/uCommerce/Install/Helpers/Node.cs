namespace UCommerce.RazorStore.Installer
{
    using System.Linq;

    using umbraco.BusinessLogic;
    using umbraco.cms.businesslogic.web;
    using umbraco.presentation.nodeFactory;

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
