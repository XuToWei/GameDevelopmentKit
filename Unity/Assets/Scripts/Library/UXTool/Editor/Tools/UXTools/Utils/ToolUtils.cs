#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Reflection;
using System;

using Object = UnityEngine.Object;
using System.Linq;

namespace ThunderFireUITool
{
    public static class ToolUtils
    {
        #region Prefab
        public static GameObject CreatePrefab(GameObject go, string name, string path)
        {
            string assetpath = $"{path}{name}.prefab";
            GameObject prefab = CreatePrefab(go, assetpath);
            return prefab;
        }
        public static GameObject CreatePrefab(GameObject go, string assetpath)
        {
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetpath);
            if (asset != null)
            {
                if (EditorUtility.DisplayDialog(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_警告), EditorLocalization.GetLocalization(EditorLocalizationStorage.是否覆盖), EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_覆盖), EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消)))
                {
                    //覆盖
                    asset = PrefabUtility.SaveAsPrefabAssetAndConnect(go, assetpath, InteractionMode.UserAction);
                    AssetDatabase.Refresh();
                    return asset;
                }
                else
                {
                    //取消
                    return null;
                }
            }
            else
            {
                asset = PrefabUtility.SaveAsPrefabAssetAndConnect(go, assetpath, InteractionMode.UserAction);
                AssetDatabase.Refresh();
                return asset;
            }
        }

        public static GameObject CreatePrefabAsWidget(GameObject go, string name, string path, bool isPrefab = false, string label = null, bool isPack = false)
        {
            List<string> labelList = new List<string>();
            if (isPack)
            {
                labelList.Add(WidgetRepositoryConfig.PackText);
            }
            else
            {
                labelList.Add(WidgetRepositoryConfig.UnpackText);
            }

            if (!string.IsNullOrEmpty(label))
            {
                labelList.Add(label);
            }

            string assetpath = $"{path}{name}.prefab";
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetpath);
            if (asset != null)
            {
                if (isPrefab)
                {
                    AssetDatabase.SetLabels(asset, labelList.ToArray());
                    AddGUIDToWidgetList(assetpath);
                    return asset;
                }
                if (EditorUtility.DisplayDialog(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_警告), EditorLocalization.GetLocalization(EditorLocalizationStorage.是否覆盖), EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_覆盖), EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消)))
                {
                    //覆盖
                    asset = PrefabUtility.SaveAsPrefabAssetAndConnect(go, assetpath, InteractionMode.UserAction);
                    AssetDatabase.SetLabels(asset, labelList.ToArray());
                    AddGUIDToWidgetList(assetpath);
                    AssetDatabase.Refresh();
                    return asset;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                asset = PrefabUtility.SaveAsPrefabAssetAndConnect(go, assetpath, InteractionMode.UserAction);
                AssetDatabase.SetLabels(asset, labelList.ToArray());
                AddGUIDToWidgetList(assetpath);
                AssetDatabase.Refresh();
                return asset;
            }
        }

        public static void AddGUIDToWidgetList(string path)
        {
            string guid = AssetDatabase.AssetPathToGUID(path);
            WidgetListSetting widgetListSetting = JsonAssetManager.GetAssets<WidgetListSetting>();
            var widgetList = widgetListSetting.List;
            if (!widgetList.Contains(guid))
            {
                widgetListSetting.Add(guid);
            }
            else
            {
                widgetListSetting.ResortLast(guid);
            }
        }
        #endregion

        public static GameObject CreatePanel(string name)
        {
            Type MenuOptionsType = typeof(UnityEditor.UI.ImageEditor).Assembly.GetType("UnityEditor.UI.MenuOptions");
            Utils.InvokeMethod(MenuOptionsType, "AddPanel", new object[] { });
            GameObject obj = Selection.activeGameObject;
            obj.name = name;
            return obj;
        }

        #region Texutre&Icon
        //读取和缓存Icon图片
        static Dictionary<string, Texture> m_IconDict = new Dictionary<string, Texture>();
        public static Texture GetIcon(string name)
        {
            if (!m_IconDict.TryGetValue(name, out var tex))
            {
                tex = AssetDatabase.LoadAssetAtPath<Texture>($"{ThunderFireUIToolConfig.IconPath}{name}.png");
                if (tex == null)
                {
                    tex = AssetDatabase.LoadAssetAtPath<Texture>($"{ThunderFireUIToolConfig.IconCursorPath}{name}.png");
                }
                m_IconDict[name] = tex;
            }
            return tex;
        }
        #endregion
    }
}
#endif