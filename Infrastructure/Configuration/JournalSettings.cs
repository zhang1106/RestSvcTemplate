namespace AiDollar.Infrastructure.Configuration
{
    public class JournalSettings : AppSettingsBase
    {
        public JournalSettings(string prefix) : base(prefix)
        {

        }

        public string Path => System.IO.Path.Combine(Directory, Filename);
        public string Filename => GetValue(nameof(Filename), Prefix, GetDefaultJournalFilename());
        public string Directory => GetValue(nameof(Directory), GetValue("JournalDirectory", null, System.IO.Directory.GetCurrentDirectory()));

        public string ArchivePath => System.IO.Path.Combine(ArchiveDirectory, Filename);
        public string ArchiveFilename => GetValue(nameof(ArchiveFilename), Prefix, GetDefaultJournalArchiveFilename());
        public string ArchiveDirectory => GetValue(nameof(ArchiveDirectory), GetValue("JournalArchiveDirectory", null, System.IO.Directory.GetCurrentDirectory()));

        protected string GetDefaultJournalFilename()
        {
            string fileName = Prefix.Substring(0, Prefix.IndexOf('.')) + ".dat";

            if (Directory != null)
            {
                if (!System.IO.Directory.Exists(Directory))
                    System.IO.Directory.CreateDirectory(Directory);
            }

            return fileName;
        }
        protected string GetDefaultJournalArchiveFilename()
        {
            string fileName = Prefix.Substring(0, Prefix.IndexOf('.')) + ".dat";

            if (ArchiveDirectory != null)
            {
                if (!System.IO.Directory.Exists(ArchiveDirectory))
                    System.IO.Directory.CreateDirectory(ArchiveDirectory);
            }

            return fileName;
        }
    }
}
