using System;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Ucommerce.EntitiesV2;
using Ucommerce.Infrastructure;
using AvenueClothing.Installer.Helpers;
using Ucommerce.Search.Indexers;
using Umbraco.Core.Services;
using Umbraco.Web.Composing;

namespace AvenueClothing.Installer.PackageActions
{

    public class InstallDefaultAcceleratorData : Umbraco.Core.PackageActions.IPackageAction
    {
        public IContentTypeService ContentTypeService => Current.Services.ContentTypeService;
        public IContentService ContentService => Current.Services.ContentService;
        
        public bool Execute(string packageName, XElement xmlData)
        {
            var installer = new ConfigurationInstaller();
            installer.Configure();

            // Install Demo store Catalog
            var installer2 = new CatalogueInstaller("avenue-clothing.com", "Demo Store");
            installer2.Configure();

            CreateMediaContent();

            DeleteOldUcommerceData();

            PublishContent();

            IndexEverything();

            return true;
        }

        private void CreateMediaContent()
        {
            var server = HttpContext.Current.Server;
            var mediaService = new MediaService(server.MapPath(Umbraco.Core.IO.SystemDirectories.Media),
                server.MapPath("~/umbraco/Ucommerce/install/files/"));

            var categories = Category.All().ToList();
            var products = Product.All().ToList();

            mediaService.InstallCategoryImages(categories);
            mediaService.InstallProductImages(products);
        }

        private void PublishContent()
        {
            var docType = ContentTypeService.GetAll().FirstOrDefault(x => x.Alias == "home");

            if (docType != null)
            {
                var root = ContentService.GetPagedOfType(docType.Id, 0, int.MaxValue, out var b, null).FirstOrDefault();
                ContentService.SaveAndPublishBranch(root, true);
            }
        }

        private void DeleteOldUcommerceData()
        {
            var group = ObjectFactory.Instance.Resolve<IRepository<ProductCatalogGroup>>().SingleOrDefault(g => g.Name == "Ucommerce.dk");
            if (group != null)
            {
                // Delete products in group
                var relations = CategoryProductRelation.All()
                    .Where(x => group.ProductCatalogs.Contains(x.Category.ProductCatalog))
                    .ToList();
                foreach (var relation in relations)
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

        private void IndexEverything()
        {
            ObjectFactory.Instance.Resolve<IScratchIndexer>().Index();
        }

        public string Alias()
        {
            return "InstallDefaultAcceleratorData";
        }

        public bool Undo(string packageName, XElement xmlData)
        {
            throw new System.NotImplementedException();
        }

    }
}
