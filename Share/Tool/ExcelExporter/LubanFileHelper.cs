using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ET
{
    public static class LubanFileHelper
    {
        public static void ClearSubEmptyDirectory(string directory)
        {
            if (string.IsNullOrEmpty(directory))
                return;
            var subDirs = Directory.GetDirectories(directory);
            if (subDirs.Length < 1)
                return;
            foreach (var subDir in subDirs)
            {
                if (Directory.GetDirectories(subDir).Length > 0)
                    continue;
                if (Directory.GetFiles(subDir).Length > 0)
                    continue;
                Directory.Delete(subDir);
                string subDirMeta = $"{subDir}.meta";
                if (File.Exists(subDirMeta))
                {
                    File.Delete(subDirMeta);
                }
            }
        }

        public static void CopyDirectory(string sourDir, string desDir)
        {
            if (string.IsNullOrEmpty(sourDir))
                return;
            if (string.IsNullOrEmpty(desDir))
                return;
            sourDir = sourDir.Replace('\\', '/');
            desDir = desDir.Replace('\\', '/');
            ClearOrCreateDirectory(desDir);
            foreach (var file in GetDirectoryFiles(sourDir))
            {
                string filePath = file.Replace('\\', '/');
                string subName = filePath.Substring(sourDir.Length + 1, filePath.Length - sourDir.Length - 1);
                string desFile = Path.Combine(desDir, subName).Replace('\\', '/');
                string desSubDir = Path.GetDirectoryName(desFile);
                if (!string.IsNullOrEmpty(desSubDir) && !Directory.Exists(desSubDir))
                {
                    Directory.CreateDirectory(desSubDir);
                }
                File.Copy(file, desFile);
            }
        }

        private static void ClearOrCreateDirectory(string directory)
        {
            if (string.IsNullOrEmpty(directory))
                return;
            if (Directory.Exists(directory))
            {
                foreach (var file in GetDirectoryFiles(directory))
                {
                    File.Delete(file);
                }
            }
            else
            {
                Directory.CreateDirectory(directory);
            }
        }

        private static List<string> GetDirectoryFiles(string directory)
        {
            var files = new List<string>();
            if (Directory.Exists(directory))
            {
                foreach (var file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
                {
                    if (file.Substring(directory.Length).Split('\\', '/').Any(fileName =>
                                fileName.StartsWith(".") || fileName.StartsWith("_") || fileName.StartsWith("~")))
                    {
                        continue;
                    }
                    if (file.EndsWith(".meta", StringComparison.Ordinal))
                    {
                        continue;
                    }
                    files.Add(file.Replace('\\', '/'));
                }
                // must sort files for making generation stable.
                files.Sort(string.CompareOrdinal);
            }
            return files;
        }
    }
}