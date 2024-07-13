#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ThunderFireUITool
{
    public static class HierarchyContextMenuEx
    {
        static bool InCombineProcess = false;
        [MenuItem("GameObject/Combine", true, -1)]
        static public bool ShowCombine()
        {
            if (Selection.gameObjects.Length > 1)
            {
                InCombineProcess = false;
                return true;
            }
            return false;
        }

        [MenuItem("GameObject/Combine", false, -1)]
        static public void Combine(MenuCommand menuCommand)
        {
            if (!InCombineProcess)
            {
                List<RectTransform> rects = Utils.GetAllSelectionRectTransform();
                CombineWidgetLogic.GenCombineRootRect(rects);
                InCombineProcess = true;
            }

        }
    }
}
#endif