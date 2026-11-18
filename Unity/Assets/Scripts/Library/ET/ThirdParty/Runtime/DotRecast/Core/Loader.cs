using System.IO;

namespace DotRecast.Core
{
    public static class Loader
    {
        public static byte[] ToBytes(string filename)
        {
            var filepath = FindParentPath(filename);
            return File.ReadAllBytes(filepath);
        }

        public static string FindParentPath(string filename)
        {
            string filePath = Path.Combine("resources", filename);
            for (int i = 0; i < 10; ++i)
            {
                if (File.Exists(filePath))
                {
                    return Path.GetFullPath(filePath);
                }

                filePath = Path.Combine("..", filePath);
            }

            return Path.GetFullPath(filename);
        }
    }
}
