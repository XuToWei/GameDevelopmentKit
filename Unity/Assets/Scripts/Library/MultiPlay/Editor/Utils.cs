using System.IO;
using UnityEngine;

namespace MultiPlay
{
    public static class Utils
    {
        private static string GetAppFolder()
        {
            int dirDepth = Application.dataPath.Split('/').Length;
            return Application.dataPath.Split('/')[dirDepth - 2];
        }
        public static int GetCurrentCloneIndex()
        {
            int clientIndex = 0;
            string appFolderName = GetAppFolder();
            if (IsClone())
            {
                clientIndex = 1;

                if (appFolderName.IndexOf('[') > 0)
                {
                    int.TryParse(appFolderName.Substring(
                    appFolderName.IndexOf('[') + 1, 1), out clientIndex);
                }
            }
            return clientIndex;
        }

        /// <summary>
        /// Checks whether or not the project is a Clone
        /// </summary>
        /// <param name="path">Path to Project directory</param>
        /// <returns></returns>
        public static bool IsClone()
        {
            var assetsPath =  Application.dataPath;
            if (!Directory.Exists(assetsPath)) return false;
            
            FileInfo pathInfo = new FileInfo(assetsPath);
            return pathInfo.Attributes.HasFlag(FileAttributes.ReparsePoint);
        }
        
        public static bool IsLibraryLinked()
        {
            var libraryPath =  $"{Application.dataPath}/../Library";
            if (!Directory.Exists(libraryPath)) return false;
            
            FileInfo pathInfo = new FileInfo(libraryPath);
            return pathInfo.Attributes.HasFlag(FileAttributes.ReparsePoint);
        }
    }
}