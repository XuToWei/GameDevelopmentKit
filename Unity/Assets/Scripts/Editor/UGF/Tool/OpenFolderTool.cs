using UnityEditor;
using UnityEngine;

namespace UGF.Editor
{
    public static class OpenFolderTool
    {
        [MenuItem("Floder/打开Excel文件夹", false, -99)]
        public static void OpenExcelPath()
        {
            UnityGameFramework.Editor.OpenFolder.Execute($"{Application.dataPath}/../../Develop/Excel/");
        }
        
        [MenuItem("Floder/打开Proto文件夹", false, -98)]
        public static void OpeProtoPath()
        {
            UnityGameFramework.Editor.OpenFolder.Execute($"{Application.dataPath}/../../Develop/Proto/");
        }
    }
}
