namespace FileServer
{
    public class Config
    {
        public string Url { get; } = "http://*:8088";

        public string[] DirectoryPaths { get; } = new[]
        {
            "../Temp/Bundle/Full",//本地
            "../Temp/Version",//本地
            "../Bundle",//服务器目录
            "../Version",//服务器目录
        };
    }
}
