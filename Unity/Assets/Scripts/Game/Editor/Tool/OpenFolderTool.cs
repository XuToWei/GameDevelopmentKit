using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor;

namespace Game.Editor
{
    public static class OpenFolderTool
    {
        [MenuItem("Tools/Open Folder/打开Excel文件夹", false, -99)]
        public static void OpenExcelPath()
        {
            OpenFolder.Execute($"{Application.dataPath}/../../Design/Excel");
        }
        
        [MenuItem("Tools/Open Folder/打开Proto文件夹", false, -98)]
        public static void OpenProtoPath()
        {
            OpenFolder.Execute($"{Application.dataPath}/../../Design/Proto");
        }
        
        [MenuItem("Tools/Open Folder/打开Build文件夹", false, -97)]
        public static void OpenBuildPath()
        {
            OpenFolder.Execute($"{Application.dataPath}/../../Temp");
        }
    }
}
