namespace PackageGen
{
    internal class PackageVersion
    {
        private uint Major { get; set; }
        private uint Minor { get; set; }
        private uint Rev { get; set; }
        private uint Build { get; set; }
        public PackageVersion(uint major, uint minor, uint revision, uint build)
        {
            Major = major;
            Minor = minor;
            Rev = revision;
            Build = build;
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}.{3}", Major, Minor, Rev, Build);
        }
    }
}