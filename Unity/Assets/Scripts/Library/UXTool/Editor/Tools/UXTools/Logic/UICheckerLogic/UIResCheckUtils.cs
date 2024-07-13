#if UNITY_EDITOR && ODIN_INSPECTOR
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine.U2D;

#if UNITY_2021_3_OR_NEWER
using PrefabStageUtility = UnityEditor.SceneManagement.PrefabStageUtility;
using PrefabStage = UnityEditor.SceneManagement.PrefabStage;
#else
using PrefabStageUtility = UnityEditor.Experimental.SceneManagement.PrefabStageUtility;
using PrefabStage = UnityEditor.Experimental.SceneManagement.PrefabStage;
#endif
namespace ThunderFireUITool
{
    [InitializeOnLoad]
    internal class PrefabExtension
    {
        static PrefabExtension()
        {
            PrefabUtility.prefabInstanceUpdated += OnPrefabInstanceApply;
            PrefabStage.prefabStageClosing += OnPrefabClosing;
        }
        static void OnPrefabInstanceApply(GameObject instance)
        {
            UIAtlasCheckUserData data = AssetDatabase.LoadAssetAtPath<UIAtlasCheckUserData>(ThunderFireUIToolConfig.UICheckUserDataFullPath);
            if (data != null && data.UICheckEnable)
            {
                GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(instance);
                UISinglePrefabCheckWindow window = UISinglePrefabCheckWindow.CheckAtlasButton();
                window.CheckPrefabUI(prefab);
            }
        }

        static void OnPrefabClosing(PrefabStage prefabStage)
        {
            UIAtlasCheckUserData data = AssetDatabase.LoadAssetAtPath<UIAtlasCheckUserData>(ThunderFireUIToolConfig.UICheckUserDataFullPath);
            if (data != null && data.UICheckEnable)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabStage.GetAssetPath());
                UISinglePrefabCheckWindow window = UISinglePrefabCheckWindow.CheckAtlasButton();
                window.CheckPrefabUI(prefab);
            }
        }
    }

    public static class UIResCheckUtils
    {
        #region 资源检查调用接口
        [MenuItem("Assets/检查Prefab资源 (Check UIPrefab Res)", false, -803)]
        static void OnRightClickPrefab()
        {
            string[] guids = Selection.assetGUIDs;
            if (guids.Length != 1) return;

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            //GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            object result = Utils.InvokeMethod(typeof(PrefabStageUtility), "OpenPrefab", new object[] { path });
            PrefabStage p = result as PrefabStage;
            if (p != null)
            {
                UISinglePrefabCheckWindow window = UISinglePrefabCheckWindow.CheckAtlasButton();
                window.CheckPrefabUI(p.prefabContentsRoot);
            }
        }

        [MenuItem(ThunderFireUIToolConfig.Menu_ResourceCheck + "/Prefab 引用资源检查 (All UIPrefab ResCheck)", false, ThunderFireUIToolConfig.Menu_ResourceCheckIndex + 5)]
        static void CheckAllPrefab()
        {
            UIMultiPrefabCheckWindow window = UIMultiPrefabCheckWindow.ShowWindow();
            window.CheckImageDependencies();
        }

        [MenuItem(ThunderFireUIToolConfig.Menu_ResourceCheck + "/UI美术资源检查(UI ResCheck)", false, ThunderFireUIToolConfig.Menu_ResourceCheckIndex + 4)]
        public static UIResCheckWindow CheckAtlas()
        {
            UIResCheckWindow window = (UIResCheckWindow)EditorWindow.GetWindow(typeof(UIResCheckWindow));
            window.titleContent = new GUIContent("UI美术资源检查");
            window.minSize = new Vector2(400f, 800f);
            window.Show();
            return window;
        }

        [MenuItem(ThunderFireUIToolConfig.Menu_ResourceCheck + "/导出Prefab的图集依赖 (Export UIPrefab Atlas Ref)", false, ThunderFireUIToolConfig.Menu_ResourceCheckIndex + 7)]
        public static void ExportAtlasDependencies()
        {
            // 记录每个atlas包含sprite的guid
            Dictionary<string, HashSet<string>> atlasDic = new Dictionary<string, HashSet<string>>();
            Dictionary<string, List<string>> atlasDependencies = new Dictionary<string, List<string>>();
            foreach (var guid in AssetDatabase.FindAssets("t:spriteatlas", new[] { "Assets/Prefabs/Atlas" }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                atlasDic[path] = new HashSet<string>();
                atlasDependencies[path] = new List<string>();
                string content = File.ReadAllText(path);
                var begin = content.IndexOf("m_PackedSprites");
                var end = content.IndexOf("m_PackedSpriteNamesToIndex");
                content = content.Substring(begin, end - begin);
                string regStr = "{fileID:\\s?[a-fA-F0-9]+, guid:\\s?([a-fA-F0-9]+), type: 3}";
                Regex reg = new Regex(regStr);
                MatchCollection matches = reg.Matches(content);
                foreach (Match m in matches)
                {
                    var spriteGuid = m.Groups[1].Value;
                    atlasDic[path].Add(spriteGuid);
                }
            }
            Dictionary<string, List<string>> dependencies = new Dictionary<string, List<string>>();
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/Prefabs/UI" });
            foreach (string guid in guids)
            {
                var prefabPath = AssetDatabase.GUIDToAssetPath(guid);
                dependencies[prefabPath] = new List<string>();
                string content = File.ReadAllText(prefabPath);
                string regStr = "m_Sprite: {fileID:\\s?[a-fA-F0-9]+, guid:\\s?([a-fA-F0-9]+), type: 3}";
                Regex reg = new Regex(regStr);
                MatchCollection matches = reg.Matches(content);
                List<string> tmpGuids = new List<string>();
                foreach (Match m in matches)
                {
                    tmpGuids.Add(m.Groups[1].Value);//正则表达式内括号内匹配的guid
                }
                List<string> spriteGuids = tmpGuids.Distinct().ToList();
                foreach (var spriteGuid in spriteGuids)
                {
                    foreach (var kv in atlasDic)
                    {
                        if (kv.Value.Contains(spriteGuid))
                        {
                            if (!dependencies[prefabPath].Contains(kv.Key))
                            {
                                dependencies[prefabPath].Add(kv.Key);
                            }
                            if (!atlasDependencies[kv.Key].Contains(prefabPath))
                            {
                                atlasDependencies[kv.Key].Add(prefabPath);
                            }
                        }
                    }
                }
            }

            var prefab2AtlasPath = Application.dataPath + "/UIPrefabUsedAtlas.csv";
            var atlas2PrefabPath = Application.dataPath + "/UIAtlasUsedPrefab.csv";
            Thread thread = new Thread(() =>
            {

                Action<string, string, Dictionary<string, List<string>>> save = (path, title, depend) =>
                {
                    using (FileStream fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine(title);
                            foreach (var prefab in depend)
                            {
                                var subSb = new StringBuilder();
                                subSb.Append(Path.GetFileNameWithoutExtension(prefab.Key) + ",");
                                subSb.Append(prefab.Value.Count + ",");
                                foreach (var atlas in prefab.Value)
                                {
                                    string fileName = Path.GetFileNameWithoutExtension(atlas);
                                    subSb.Append(fileName + ",");
                                }

                                var tmpStr = subSb.ToString();
                                if (tmpStr.EndsWith(","))
                                {
                                    tmpStr = tmpStr.Substring(0, tmpStr.Length - 1);
                                }
                                sb.AppendLine(tmpStr);
                            }
                            sw.Write(sb);
                            sw.Close();
                        }
                        fs.Close();
                    }
                };
                save(prefab2AtlasPath, "prefab, atlas count, atlases", dependencies);
                save(atlas2PrefabPath, "atlas, prefab count, prefabs", atlasDependencies);
                Debug.Log("csv文件导出完毕");
            });
            thread.Start();
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="atlasSpriteDic">记录所有atlas包含的sprite的guid与atlas对应关系</param>
        public static void GetAtlasSpriteDic(ref Dictionary<string, UnityEngine.Object> atlasSpriteDic)
        {
            var checkAtlasSettings = AssetDatabase.LoadAssetAtPath<UIAtlasCheckRuleSettings>(ThunderFireUIToolConfig.UICheckSettingFullPath);
            string atlasFolder = checkAtlasSettings.atlasFolderPath;

            foreach (var guid in AssetDatabase.FindAssets("t:spriteatlas", new[] { atlasFolder }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);

                string content = File.ReadAllText(path);
                var begin = content.IndexOf("m_PackedSprites");
                var end = content.IndexOf("m_PackedSpriteNamesToIndex");
                content = content.Substring(begin, end - begin);
                string regStr = "{fileID:\\s?[a-fA-F0-9]+, guid:\\s?([a-fA-F0-9]+), type: 3}";
                Regex reg = new Regex(regStr);
                MatchCollection matches = reg.Matches(content);
                foreach (Match m in matches)
                {
                    var spriteGuid = m.Groups[1].Value;
                    atlasSpriteDic[spriteGuid] = atlas;
                }
            }
        }
    }
}
#endif