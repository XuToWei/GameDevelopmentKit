#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;

namespace ThunderFireUITool
{
    [UXInitialize(100)]
    public class SVNCustomProjectWindow
    {
        private static Texture2D svnOperationIcon;
        private static Texture2D SVNOperationIcon
        {
            get
            {
                if (svnOperationIcon == null)
                {
                    svnOperationIcon = ToolUtils.GetIcon("SVN_Added") as Texture2D;
                }
                return svnOperationIcon;
            }
        }

        static SVNCustomProjectWindow()
        {
            EditorApplication.projectWindowItemOnGUI -= DrawSVNOperationIcon;
            EditorApplication.projectWindowItemOnGUI += DrawSVNOperationIcon;
        }

        private static void DrawSVNOperationIcon(string guid, Rect selectionRect)
        {
            if (string.IsNullOrEmpty(guid) || guid.StartsWith("00000000", StringComparison.Ordinal))
                return;

            // if (Selection.assetGUIDs.Length == 1 && guid == Selection.assetGUIDs[0])
            // {
            //     string path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
            //     bool isFolder = System.IO.Directory.Exists(path);
            //     if (isFolder)
            //     {
            //         Rect rect = new Rect(selectionRect.x + selectionRect.width - 50f, selectionRect.y - 7.5f, 30f, 30f);
            //         if (GUI.Button(rect, SVNOperationIcon))
            //         {
            //             TortoiseSVNLogic.CommitSelected();
            //         }
            //     }
            // }
            foreach (string str in Selection.assetGUIDs){
                if(str.Equals(guid)){
                    string path = AssetDatabase.GUIDToAssetPath(str);
                    bool isFolder = System.IO.Directory.Exists(path);
                    if (isFolder)
                    {
                        Rect rect = new Rect(selectionRect.x + selectionRect.width - 50f, selectionRect.y - 7.5f, 30f, 30f);
                        if (GUI.Button(rect, SVNOperationIcon))
                        {
                            TortoiseSVNLogic.CommitSelected();
                        }
                    }
                }
            }
            
        }
    }
}
#endif