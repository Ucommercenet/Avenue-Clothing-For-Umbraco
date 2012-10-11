namespace PackageGen
{
    internal class UmbracoVersion
    {
        public uint Major { get; set; }
        public uint Minor { get; set; }
        public uint Patch { get; set; }

        public UmbracoVersion(uint major, uint minor, uint patch)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}", Major, Minor, Patch);
        }
    }
}