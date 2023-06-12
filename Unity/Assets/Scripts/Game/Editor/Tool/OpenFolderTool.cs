using System.IO;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor;

namespace Game.Editor
{
    public static class OpenFolderTool
    {
        [MenuItem("Game/Open Folder/打开Excel文件夹", false, -99)]
        public static void OpenExcelPath()
        {
            SafeOpenFolder($"{Application.dataPath}/../../Design/Excel");
        }
        
        [MenuItem("Game/Open Folder/打开Proto文件夹", false, -98)]
        public static void OpenProtoPath()
        {
            SafeOpenFolder($"{Application.dataPath}/../../Design/Proto");
        }
        
        [MenuItem("Game/Open Folder/打开Build文件夹", false, -97)]
        public static void OpenBuildPath()
        {
            SafeOpenFolder($"{Application.dataPath}/../../Temp");
        }

        public static void SafeOpenFolder(string folderPath)
        {
            folderPath = Path.GetFullPath(folderPath);
            if (Directory.Exists(folderPath))
            {
                OpenFolder.Execute(folderPath);
            }
            else
            {
                Debug.LogError($"Open folder fail! {folderPath} not exist!");
            }
        }
    }
}
