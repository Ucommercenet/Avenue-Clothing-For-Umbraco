using System.Linq;
using System.Web;
using System.Xml.Linq;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.RazorStore.Installer.Helpers;
//using Umbraco.Core.Services;
//using Umbraco.Web.Composing;
//using Umbraco.Web.Routing;

namespace UCommerce.RazorStore.Installer.PackageActions
{
    public class InstallDefaultAcceleratorData
    {
//        public IContentTypeService ContentTypeService => Current.Services.ContentTypeService;
//        public IContentService ContentService => Current.Services.ContentService;
        
//        public virtual void CreateMediaContent()
//        {
//            var server = HttpContext.Current.Server;
//            var mediaService = new MediaService(server.MapPath(Umbraco.Core.IO.SystemDirectories.Media),
//                server.MapPath("~/umbraco/ucommerce/install/files/"));
//
//            var categories = Category.All().ToList();
//            var products = Product.All().ToList();
//
//            mediaService.InstallCategoryImages(categories);
//            mediaService.InstallProductImages(products);
//        }
//
//        public virtual void PublishContent()
//        {
//            var docType = ContentTypeService.GetAll().FirstOrDefault(x => x.Alias == "home");
//
//            if (docType != null)
//            {
//                var root = ContentService.GetPagedOfType(docType.Id, 0, int.MaxValue, out var b, null).FirstOrDefault();
//                ContentService.SaveAndPublishBranch(root, true);
//            }
//        }

        public virtual void DeleteOldUCommerceData()
        {
            var group = ObjectFactory.Instance.Resolve<IRepository<ProductCatalogGroup>>().SingleOrDefault(g => g.Name == "uCommerce.dk");
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
    }
}
