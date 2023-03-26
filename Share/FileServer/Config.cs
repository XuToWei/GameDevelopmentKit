namespace FileServer
{
    public class Config
    {
        public string Url { get; } = "http://*:8088";

        public string[] DirectoryPaths { get; } = new[]
        {
            "../Temp/Bundle",
            "../Bundle",
        };
    }
}
