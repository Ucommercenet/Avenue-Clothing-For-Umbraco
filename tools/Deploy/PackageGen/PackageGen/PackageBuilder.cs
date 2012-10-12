using System.Text.RegularExpressions;

namespace PackageGen
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    using Ionic.Zip;

    public class PackageBuilder
    {
        private int _fileCounterInternal = 0;

        private readonly string _packageName;
        private readonly string _packageId;
        private readonly DirectoryInfo _sourceFolder;
        private readonly DirectoryInfo _filesFolder;
        private readonly string _installControl;
        private readonly DirectoryInfo _xmlStubsPath;

        public bool VerboseLogging { get; set; }

        private DirectoryInfo _tempFolder;

        public PackageBuilder(string packageName, string packageId, DirectoryInfo sourceFolder, DirectoryInfo xmlStubsPath, string installControl)
        {
            _packageName = packageName;
            _packageId = packageId;
            _sourceFolder = sourceFolder;
            _filesFolder = new DirectoryInfo(Path.Combine(_sourceFolder.FullName, "files"));
            _installControl = installControl;
            _xmlStubsPath = xmlStubsPath;
        }

        private void InitialiseTempFolder()
        {
            var tempFolderPath = Path.GetTempPath();

            _tempFolder = new DirectoryInfo(Path.Combine(tempFolderPath, _packageId));

            if (_tempFolder.Exists)
                _tempFolder.Delete(true);

            _tempFolder.Create();

            WriteMessage(String.Format("Temporary Working Folder: {0}", _tempFolder.FullName));
        }

        public string BuildPackage()
        {
            InitialiseTempFolder();
            CreatePackageXml();
            ZipTempFolder();
            CleanUp();
            return _packageName;
        }

        private void CleanUp()
        {
            WriteMessage(String.Format("Deleteing: {0}", _sourceFolder.FullName));
            _sourceFolder.Delete(true);

            WriteMessage(String.Format("Deleteing: {0}", _tempFolder.FullName));
            _tempFolder.Delete(true);
        }

        private void ZipTempFolder()
        {
            if (File.Exists(_packageName))
            {
                WriteMessage(String.Format("Package file already exists, deleteing: {0}", _packageName));
                File.Delete(_packageName);
            }

            using (var zip = new ZipFile())
            {
                zip.AddDirectory(_tempFolder.FullName, _packageId);
                zip.Save(_packageName);
            }
        }

        private void CreatePackageXml()
        {
            WriteMessage("Creating package file");

            var packageRoot = new XElement(
                "umbPackage",
                CreateFilesStub(),
                GetXmlStub("Info"),
                GetXmlStub("Documents"),
                GetXmlStub("DocumentTypes"),
                CreateTemplatesStub(),
                CreateStylesheetsStub(),
                GetXmlStub("Macros"),
                GetXmlStub("DictionaryItems"),
                GetXmlStub("Languages"),
                GetXmlStub("DataTypes"),
                GetXmlStub("Actions"),
                new XElement("control", _installControl));

            var savePath = Path.Combine(_tempFolder.FullName, "package.xml");
            packageRoot.Save(savePath);

            WriteMessage(String.Format("Package file saved to: {0}", savePath));
        }

        private XElement CreateFilesStub()
        {
            var extensionsToIgnore = new List<string>() { ".css", ".master" };
            var items = CreateXmlFromFileType("*.*", (file, contents) =>
                {
                    if (extensionsToIgnore.Contains(file.Extension))
                        return null;

                    WriteMessage(String.Format("Adding file: {0}", file.FullName));

                    var original = new FileInfo(file.FullName);
                    var newFileName = MoveFileToPackageFolder(file);
                    var relativePath = original.DirectoryName.Replace(_filesFolder.FullName, String.Empty);
                    return new XElement("file",
                            new XElement("guid", newFileName),
                            new XElement("orgPath", relativePath),
                            new XElement("orgName", original.Name)
                        );
                });

            var templateRoot = new XElement("files");
            foreach (var item in items.Where(item => item != null))
            {
                templateRoot.Add(item);
            }

            return templateRoot;
        }

        private string MoveFileToPackageFolder(FileInfo file)
        {
            var newFileName = string.Format("{0}.{1}", file.Name, _fileCounterInternal++);
            file.MoveTo(Path.Combine(_tempFolder.FullName, newFileName));
            return newFileName;
        }

        private static Regex parentMaster = new Regex("MasterPageFile=\"~/masterpages/(?<filename>.+)\\.master", RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.Singleline);

        private XElement CreateTemplatesStub()
        {
            var items = CreateXmlFromFileType("*.master", (file, contents) =>
                {
                    WriteMessage(String.Format("Adding template: {0}", file.FullName));

                    var match = parentMaster.Match(contents);
                    var parent = string.Empty;

                    var val = match.Groups["filename"].Value;
                    if (!string.IsNullOrWhiteSpace(val))
                        parent = match.Groups["filename"].Value;

                    var nameWithoutExt = Path.GetFileNameWithoutExtension(file.FullName);
                    return new XElement("Template",
                            new XElement("Name", nameWithoutExt),
                            new XElement("Alias", nameWithoutExt),
                            new XElement("Master", parent),
                            new XElement("Design", new XCData(contents))
                        );
                });

            var templateRoot = new XElement("Templates");
            foreach (var item in items)
            {
                templateRoot.Add(item);
            }
            return templateRoot;
        }

        private XElement CreateStylesheetsStub()
        {
            var items = CreateXmlFromFileType("*.css", (file, contents) =>
            {
                WriteMessage(String.Format("Adding stylesheet: {0}", file.FullName));

                var nameWithoutExt = Path.GetFileNameWithoutExtension(file.FullName);
                return new XElement("Stylesheet",
                        new XElement("Name", nameWithoutExt),
                        new XElement("FileName", nameWithoutExt),
                        new XElement("Content", new XCData(contents))
                    );
            });

            var stylesheetsRoot = new XElement("Stylesheets");
            foreach (var item in items)
            {
                stylesheetsRoot.Add(item);
            }

            return stylesheetsRoot;
        }

        private IEnumerable<XElement> CreateXmlFromFileType(string extension, Func<FileInfo, string, XElement> fileProcessor)
        {
            var files = GetFilesOfTypeFromFolder(extension);
            return from file in files
                   let contents = ReadFileContents(file)
                   select fileProcessor(file, contents);
        }

        private XElement GetXmlStub(string nodename)
        {
            var filename = Path.Combine(_xmlStubsPath.FullName, string.Format("{0}.xml", nodename));

            if (File.Exists(filename))
            {
                WriteMessage(String.Format("Adding {0} from file: {1}", nodename, filename));

                try
                {
                    return XElement.Load(filename);
                }
                catch { }
            }

            WriteMessage(String.Format("Couldn't find a file for: {0} -adding empty stub", nodename));
            return new XElement(nodename);
        }

        private IEnumerable<FileInfo> GetFilesOfTypeFromFolder(string extension)
        {
            return _filesFolder.GetFiles(extension, SearchOption.AllDirectories);
        }

        private string ReadFileContents(FileSystemInfo file)
        {
            using (var sr = new StreamReader(file.FullName))
            {
                return sr.ReadToEnd();
            }
        }

        private void WriteMessage(string message)
        {
            if (VerboseLogging)
                Console.WriteLine(message);
        }
    }
}