using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using UCommerce.EntitiesV2;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using File = System.IO.File;

namespace UCommerce.RazorStore.Installer.Helpers
{
    public class MediaService
    {
        public IMediaService UmbracoMediaService => Current.Services.MediaService;

        private string _mediaFolder;
        private string _installFolder;
        private IMedia _rootMediaNode;
        private string _defaultFolderName;
        private IMediaType _folderType;
        private IMediaType _imageType;
        private List<string> _thumbnailExtensions = new List<string> { "jpeg", "jpg", "gif", "bmp", "png", "tiff", "tif" };

        public MediaService(string mediaFolder, string installFolder)
        {
            _mediaFolder = mediaFolder;
            _installFolder = installFolder;

            _defaultFolderName = "avenue-clothing.com";
            _folderType = Current.Services.MediaTypeService.Get("folder");
            _imageType = Current.Services.MediaTypeService.Get("image");

            _rootMediaNode = GetRootMediaFolder();
        }

        private IMedia GetRootMediaFolder()
        {
            return GetOrCreateMediaFolder(_defaultFolderName, -1);
        }

        private IMedia GetOrCreateMediaFolder(string folderName, int parentId)
        {
            //var folders = Media.GetMediaOfMediaType(_folderType.Id);
            var folders = Current.Services.MediaService.GetPagedOfType(_folderType.Id, 0, int.MaxValue,
                out var size, null);
            return folders.FirstOrDefault(f => f.Name == folderName && (f.ParentId == parentId || parentId == -1)) ?? CreateMediaFolder(folderName, parentId);
        }

        private IMedia CreateMediaFolder(string folderName, int parentId)
        {
            var media = UmbracoMediaService.CreateMedia(folderName, parentId, "Folder");
            UmbracoMediaService.Save(media);
            return media;
        }

        private IMedia CreateMediaImage(string imageName, int parentId)
        {
            //return Media.MakeNew(imageName, _imageType, new User(0), parentId);
            var media = Current.Services.MediaService.CreateMedia(imageName, parentId, _imageType.Alias);
            Current.Services.MediaService.Save(media, -1);
            return media;
        }

        public IEnumerable<IMedia> InstallCategoryImages(IEnumerable<Category> categories)
        {
            var categoryImagesFolder = GetOrCreateMediaFolder("Category Images", _rootMediaNode.Id);

            var items = new List<IMedia>();
            foreach (var c in categories)
            {
                if (!String.IsNullOrWhiteSpace(c.ImageMediaId))
                    continue;

                var media = GetImageFromFolder("category", c.Name, categoryImagesFolder);

                if (media == null)
                    continue;

                c.ImageMediaId = media.GetUdi().Guid.ToString();
                c.Save();

                items.Add(media);
            }

            return items;
        }

        public IEnumerable<IMedia> InstallProductImages(IEnumerable<Product> products)
        {
            var productImagesFolder = GetOrCreateMediaFolder("Product Images", _rootMediaNode.Id);

            var items = new List<IMedia>();
            foreach (var p in products)
            {
                if (!String.IsNullOrWhiteSpace(p.ThumbnailImageMediaId) && !String.IsNullOrWhiteSpace(p.ThumbnailImageMediaId))
                    continue;

                if (!String.IsNullOrWhiteSpace(p.VariantSku))
                    continue;

                var media = GetImageFromFolder("product", p.Sku, productImagesFolder);

                if (media == null)
                    continue;

                p.ThumbnailImageMediaId = media.GetUdi().Guid.ToString();
                p.PrimaryImageMediaId = media.GetUdi().Guid.ToString();
                p.Save();

                items.Add(media);
            }

            return items;
        }

        private IMedia GetImageFromFolder(string folder, string filename, IMedia parent)
        {
            var file = LookForFile(folder, filename);
            return file == null ? null : UmbracoSave(file, parent);
        }

        private FileInfo LookForFile(string folder, string imageName)
        {
            var path = Path.Combine(_installFolder, folder, String.Format("{0}.jpg", imageName));
            return File.Exists(path) ? new FileInfo(path) : null;
        }

        private IMedia UmbracoSave(FileInfo file, IMedia parent)
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
			//item.Save();
            Current.Services.MediaService.Save(item);

            if (_thumbnailExtensions.Contains(ext))
                CreateImageProperties(new FileInfo(fullFilePath), item, ext);

            return item;
        }

        private void CreateImageProperties(FileInfo file, IMedia item, string ext)
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

        private static void TrySetProperty(IMedia m, string propertyName, string ext)
        {
            try
            {
                m.Properties.FirstOrDefault(x => x.Alias == propertyName).SetValue(ext);
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
