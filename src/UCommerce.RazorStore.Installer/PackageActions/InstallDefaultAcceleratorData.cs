using System.Linq;
using System.Web;
using System.Xml;
using umbraco;
using umbraco.cms.businesslogic.web;
using umbraco.interfaces;
using UCommerce.EntitiesV2;
using UCommerce.RazorStore.Installer.Helpers;
using helper = umbraco.cms.businesslogic.packager.standardPackageActions.helper;

namespace UCommerce.RazorStore.Installer.PackageActions
{

    public class InstallDefaultAcceleratorData : IPackageAction
    {
        public bool Execute(string packageName, XmlNode xmlData)
        {
            var installer = new ConfigurationInstaller();
            installer.Configure();

            // Install Demo store Catalog
            var installer2 = new CatalogueInstaller("avenue-clothing.com", "Demo Store");
            installer2.Configure();

            CreateMediaContent();

	        DeleteOldUCommerceData();

	        PublishContent();

	        return true;
        }

	    private void CreateMediaContent()
	    {
		    var server = HttpContext.Current.Server;
		    var mediaService = new MediaService(server.MapPath(umbraco.IO.SystemDirectories.Media),
			    server.MapPath("~/umbraco/ucommerce/install/files/"));

		    var categories = Category.All().ToList();
		    var products = Product.All().ToList();

		    mediaService.InstallCategoryImages(categories);
		    mediaService.InstallProductImages(products);
	    }

	    private void PublishContent()
	    {
		    var docType = DocumentType.GetAllAsList().FirstOrDefault(t => t.Alias == "home");
		    if (docType != null)
		    {
			    var root = Document.GetDocumentsOfDocumentType(docType.Id);
			    foreach (var document in root)
			    {
				    Node.PublishChildDocs(document);
			    }
			    library.RefreshContent();
		    }
	    }

	    private void DeleteOldUCommerceData()
	    {
		    var group = ProductCatalogGroup.SingleOrDefault(g => g.Name == "uCommerce.dk");
		    if (group != null)
		    {
			    // Delete products in group
			    foreach (
				    var relation in
					    CategoryProductRelation.All()
						    .Where(x => group.ProductCatalogs.Contains(x.Category.ProductCatalog))
						    .ToList())
			    {
				    var category = relation.Category;
				    var product = relation.Product;
				    category.RemoveProduct(product);
				    product.Delete();
			    }

			    // Delete catalogs
			    foreach (var catalog in group.ProductCatalogs)
			    {
				    catalog.Deleted = true;
			    }

			    // Delete group itself
			    group.Deleted = true;
			    group.Save();
		    }
	    }


	    public string Alias()
        {
            return "InstallDefaultAcceleratorData";
        }

        public bool Undo(string packageName, XmlNode xmlData)
        {
            return true;
        }

        public XmlNode SampleXml()
        {
            string sample = string.Format("<Action runat=\"install\" undo=\"false\" alias=\"{0}\" />", Alias());
            return helper.parseStringToXmlNode(sample);
        }
    }
}
