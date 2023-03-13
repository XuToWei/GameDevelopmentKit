using System.IO;
using System.Threading.Tasks;

namespace QFSW.QC.Extras
{
    public static class FileCommands
    {
        [Command("write-file", Platform.AllPlatforms ^ Platform.WebGLPlayer)]
        [CommandDescription("Writes the provided data to a file at the provided path")]
        private static async Task WriteFile(string path, string data)
        {
            FileInfo file = new FileInfo(path);
            file.Directory?.Create();

            using (StreamWriter writer = new StreamWriter(path))
            {
                await writer.WriteAsync(data);
            }
        }

        [Command("read-file", Platform.AllPlatforms ^ Platform.WebGLPlayer)]
        [CommandDescription("Reads the contents of the file at the provided path")]
        private static string ReadFile(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
