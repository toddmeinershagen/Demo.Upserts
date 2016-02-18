namespace Demo.Upserts.Command
{
    public class Message
    {
        public int AccountNumber { get; set; }
        public Version Version { get; set; }

        public bool IsNextToBeProcessed(Version currentVersion)
        {
            return (currentVersion.IsNotExisting() && Version.IsFirstVersion()) 
                   || IsNextVersion(currentVersion);
        }

        private bool IsNextVersion(Version currentVersion)
        {
            return (currentVersion == Version - 1);
        }
    }
}