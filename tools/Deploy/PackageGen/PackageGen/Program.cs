using System;

namespace PackageGen
{
    using System.IO;

    using NDesk.Options;

    class Program
    {
        static void Main(string[] args)
        {
            bool show_help = false;
            bool verbose = false;

            var packageName = string.Empty;
            var packageId = string.Empty;
            var sourceFolder = string.Empty;
            var installControl = string.Empty;
            var xmlStubs = string.Empty;

            var p = new OptionSet() 
                        {
                            { "n|name=", "the {NAME} of the package's zip file.", v => packageName = v },
                            { "g|guid=", "the {GUID} of the package.", v => packageId = v },
                            { "p|path=", "the source {PATH} of the files to include.", v => sourceFolder = v },
                            { "c|control:", "the installer {CONTROL} that should be included used (defaults to: \"" + installControl + "\").", v => installControl = v },
                            { "i|info:", "the path of the package information XML that should be included used (defaults to: \"" + xmlStubs + "\").", v => xmlStubs= v },
                            { "x|xml:", "the path of the XML stubs that should be included used (defaults to the source folder).", v => xmlStubs = v },
                            { "v|verbose", "output a verbose log as to what it's doing.", v => verbose = v != null },
                            { "h|?|help",  "show this message and exit", v => show_help = v != null },
                        };

            try
            {
                p.Parse(args);
            }
            catch (OptionException e)
            {
                OutputError(e.Message);
                return;
            }

            if (show_help)
            {
                ShowHelp(p);
                return;
            }

            if (String.IsNullOrWhiteSpace(packageName))
            {
                OutputError("No name for the zip file was specified (n or name parameters)");
                return;
            }

            if (!packageName.EndsWith(".zip"))
                packageName = packageName + ".zip";

            if (String.IsNullOrWhiteSpace(packageId))
            {
                OutputError("No package id was specified (g or guid parameters)");
                return;
            }

            if (String.IsNullOrWhiteSpace(sourceFolder))
            {
                OutputError("No source folder was specified (p or path parameters)");
                return;
            }

            if (String.IsNullOrWhiteSpace(xmlStubs))
                xmlStubs = sourceFolder;

            var builder = new PackageBuilder(packageName, packageId, new DirectoryInfo(sourceFolder), new DirectoryInfo(xmlStubs), installControl);
            builder.VerboseLogging = verbose;
            var savePath = builder.BuildPackage();

            Console.WriteLine("Package saved to: " + savePath);
        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: PackageGen.exe [OPTIONS]+");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }

        private static void OutputError(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
                Console.WriteLine(message);
            Console.WriteLine("Try `PackageGen.exe -?' for more information.");
        }
    }
}
