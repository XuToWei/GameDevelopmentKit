using System.IO;
using System.Linq;

namespace FolderTag
{
    public static class FolderHelper
    {
        public static bool IsFolderEmpty(string directoryPath)
        {
            var items = Directory.EnumerateFileSystemEntries(directoryPath);
            using (var en = items.GetEnumerator())
                return en.MoveNext() == false;
        }

        public static bool IsSelected(string guid)
        {
            return UnityEditor.Selection.assetGUIDs.Contains(guid);
        }

        public static bool IsValidFolder(string path)
        {
            return UnityEditor.AssetDatabase.IsValidFolder(path) && !path.Equals("Assets");
        }

        public static bool IsValidScene(string path)
        {
            return path.EndsWith(".unity");
        }
    }
}