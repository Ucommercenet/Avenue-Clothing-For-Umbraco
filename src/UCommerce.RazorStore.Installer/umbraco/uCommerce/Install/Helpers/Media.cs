namespace UCommerce.RazorStore.Installer
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Xml;

    using UCommerce.EntitiesV2;

    using umbraco.cms.businesslogic.media;

    using Media = umbraco.cms.businesslogic.media.Media;
    using User = umbraco.BusinessLogic.User;

    public class MediaService
    {
        private string _mediaFolder;
        private string _installFolder;
        private Media _rootMediaNode;
        private string _defaultFolderName;
        private MediaType _folderType;
        private MediaType _imageType;
        private List<string> _thumbnailExtensions = new List<string> { "jpeg", "jpg", "gif", "bmp", "png", "tiff", "tif" };

        public MediaService(string mediaFolder, string installFolder)
        {
            _mediaFolder = mediaFolder;
            _installFolder = installFolder;

            _defaultFolderName = "avenue-clothing.com";
            _folderType = MediaType.GetByAlias("folder");
            _imageType = MediaType.GetByAlias("image");

            _rootMediaNode = GetRootMediaFolder();
        }

        private Media GetRootMediaFolder()
        {
            return GetOrCreateMediaFolder(_defaultFolderName, -1);
        }

        private Media GetOrCreateMediaFolder(string folderName, int parentId)
        {
            var folders = Media.GetMediaOfMediaType(_folderType.Id);
            return folders.FirstOrDefault(f => f.Text == folderName && (f.ParentId == parentId || parentId == -1)) ?? CreateMediaFolder(folderName, parentId);
        }

        private Media CreateMediaFolder(string folderName, int parentId)
        {
            var media = Media.MakeNew(folderName, _folderType, new User(0), parentId);
            return media;
        }

        private Media CreateMediaImage(string imageName, int parentId)
        {
            return Media.MakeNew(imageName, _imageType, new User(0), parentId);
        }

        public IEnumerable<Media> InstallCategoryImages(IEnumerable<Category> categories)
        {
            var categoryImagesFolder = GetOrCreateMediaFolder("Category Images", _rootMediaNode.Id);

            var items = new List<Media>();
            foreach (var c in categories)
            {
                if (!String.IsNullOrWhiteSpace(c.ImageMediaId))
                    continue;

                var media = GetImageFromFolder("category", c.Name, categoryImagesFolder);

                if (media == null)
                    continue;

                c.ImageMediaId = media.Id.ToString();
                c.Save();

                items.Add(media);
            }

            return items;
        }

        public IEnumerable<Media> InstallProductImages(IEnumerable<Product> products)
        {
            var productImagesFolder = GetOrCreateMediaFolder("Product Images", _rootMediaNode.Id);

            var items = new List<Media>();
            foreach (var p in products)
            {
                if (!String.IsNullOrWhiteSpace(p.ThumbnailImageMediaId) && !String.IsNullOrWhiteSpace(p.ThumbnailImageMediaId))
                    continue;

                if (!String.IsNullOrWhiteSpace(p.VariantSku))
                    continue;

                var media = GetImageFromFolder("product", p.Sku, productImagesFolder);

                if (media == null)
                    continue;

                p.ThumbnailImageMediaId = media.Id.ToString();
                p.PrimaryImageMediaId = media.Id.ToString();
                p.Save();

                items.Add(media);
            }

            return items;
        }

        private Media GetImageFromFolder(string folder, string filename, Media parent)
        {
            var file = LookForFile(folder, filename);
            return file == null ? null : UmbracoSave(file, parent);
        }

        private FileInfo LookForFile(string folder, string imageName)
        {
            var path = Path.Combine(_installFolder, folder, String.Format("{0}.jpg", imageName));
            return File.Exists(path) ? new FileInfo(path) : null;
        }

        private Media UmbracoSave(FileInfo file, Media parent)
        {
            if (file == null || !file.Exists)
                return null;

            var ext = file.Extension.Replace(".", string.Empty).ToLower();
            var item = CreateMediaImage(file.Name, parent.Id);

            var relativePath = string.Concat("/media/", item.Id, "/" + file.Name);
            var mediaFolder = Path.Combine(_mediaFolder, item.Id.ToString());

            if (!Directory.Exists(mediaFolder))
                Directory.CreateDirectory(mediaFolder);

            var fullFilePath = Path.Combine(mediaFolder, file.Name);
            file.CopyTo(fullFilePath);

            TrySetProperty(item, "umbracoExtension", ext);
            TrySetProperty(item, "umbracoBytes", file.Length.ToString());
            TrySetProperty(item, "umbracoFile", relativePath);

            if (_thumbnailExtensions.Contains(ext))
                CreateImageProperties(new FileInfo(fullFilePath), item, ext);

            item.XmlGenerate(new XmlDocument());

            return item;
        }

        private void CreateImageProperties(FileInfo file, Media item, string ext)
        {
            using (var fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var image = Image.FromStream(fs);
                int fileWidth = image.Width;
                int fileHeight = image.Height;

                TrySetProperty(item, "umbracoWidth", fileWidth.ToString());
                TrySetProperty(item, "umbracoHeight", fileHeight.ToString());

                Mini(file, ext, image, fileWidth, fileHeight);
            }
        }

        private void Mini(FileInfo file, string ext, Image image, int fileWidth, int fileHeight)
        {
            using (var bp = CreateThumbnailOfImage(image, 100, fileWidth, fileHeight))
            {
                var thumbnailFileName = GenerateThumbnailName(file, ext);
                var codec = GetCodecByMimeType("image/jpeg");
                var parameters = GetEncoderParameters(90);
                bp.Save(thumbnailFileName, codec, parameters);
            }
        }

        private static string GenerateThumbnailName(FileInfo file, string ext)
        {
            return file.FullName.Replace("." + ext, "_thumb") + ".jpg";
        }

        private static void TrySetProperty(Media m, string propertyName, string ext)
        {
            try
            {
                m.getProperty(propertyName).Value = ext;
            }
            catch
            {
            }
        }

        private Bitmap CreateThumbnailOfImage(Image image, int maxWidthHeight, int fileWidth, int fileHeight)
        {
            var fx = (float)fileWidth / (float)maxWidthHeight;
            var fy = (float)fileHeight / (float)maxWidthHeight;

            var f = Math.Max(fx, fy);
            var widthTh = (int)Math.Round(fileWidth / f);
            var heightTh = (int)Math.Round(fileHeight / f);

            if (widthTh == 0)
                widthTh = 1;

            if (heightTh == 0)
                heightTh = 1;

            var bp = new Bitmap(widthTh, heightTh);
            using (var g = Graphics.FromImage(bp))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                var rect = new Rectangle(0, 0, widthTh, heightTh);
                g.DrawImage(image, rect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);

                return bp;
            }
        }

        private ImageCodecInfo GetCodecByMimeType(string mimeType)
        {
            var codecs = ImageCodecInfo.GetImageEncoders();
            return codecs.FirstOrDefault(t => t.MimeType.Equals(mimeType));
        }

        private EncoderParameters GetEncoderParameters(long quality)
        {
            var ep = new EncoderParameters();
            ep.Param[0] = new EncoderParameter(Encoder.Quality, quality);
            return ep;
        }
    }
}
