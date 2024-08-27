using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Cysharp.Threading.Tasks;
// using UnityEngine.SceneManagement;
// using UnityEditor.SceneManagement;
using ThunderFireUITool;
using Game.Editor;
using UnityEditor;

namespace UnityEngine.UI
{
    [UXInitialize(100)]
    public class UXTextTable
    {
        static UXTextTable()
        {
            PrefabStageUtils.AddSavedEvent(CheckTextTableKey);
            // EditorSceneManager.sceneSaved += SyncTextTable;
        }

        [MenuItem(ThunderFireUIToolConfig.Menu_Localization + "/打开文本表格 (Open Text Table)", false, 54)]
        public static void OpenTextTable()
        {
            Application.OpenURL(Path.GetFullPath(ThunderFireUIToolConfig.TextTablePath));
        }

        [MenuItem(ThunderFireUIToolConfig.Menu_Localization + "/将文本表格转换 (Convert Text Table)", false, 54)]
        public static void ConvertTextTable()
        {
            ConvertTextTableAsync().Forget();
        }

        private static async UniTaskVoid ConvertTextTableAsync()
        {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            const string tools = "./Tool";
#else
            const string tools = ".\\Tool.exe";
#endif
            Stopwatch stopwatch = Stopwatch.StartNew();
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            await ShellTool.RunAsync($"{tools} --AppType=LocalizationExporter --Console=1", "../Bin/");
#else
            await ShellTool.RunAsync($"{tools} --AppType=LocalizationExporter --Console=1", "../Bin/");
#endif
            stopwatch.Stop();
            Debug.Log($"Export Localization cost {stopwatch.ElapsedMilliseconds} Milliseconds!");
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            EditorLocalizationTool.Clear();
            var activeObject = Selection.activeObject;
            if (activeObject != null)
            {
                Selection.activeObject = null;
                await UniTask.DelayFrame(2);
                Selection.activeObject = activeObject;
            }
        }

        private static string MergePath(string origin_path, string new_path)
        {
            if (new_path == "") return origin_path;
            if (origin_path == "") return new_path;
            return origin_path + " && " + new_path;
        }
        private static void AddItem(string[] item, List<string[]> list)
        {
            bool flag = false;
            foreach (string[] line in list)
            {
                if (line[0] == item[0])
                {
                    flag = true;
                    if (line[2] != item[2] && line[1] != "")
                    {
                        Debug.LogWarning($"{line[1]} and {item[1]} have the same key, but the original is different");
                    }
                    line[1] = MergePath(line[1], item[1]);
                    line[2] = item[2];
                    break;
                }
            }
            if (!flag)
            {
                list.Add(item);
            }
        }

        // [MenuItem("ThunderFireUXTool/Localization/Refresh TextLocalization Table using current Prefab")]
        public static void SyncTextTable()
        {
            var prefabStage = PrefabStageUtils.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                CheckTextTableKey(prefabStage.prefabContentsRoot);
            }
            // else
            // {
            //     CheckTextTableKey(SceneManager.GetActiveScene());
            // }
        }
        // private static void CheckTextTableKey(Scene scene)
        // {
        //     GameObject[] objs = scene.GetRootGameObjects();
        //     foreach(GameObject obj in objs)
        //     {
        //         GetAllKeyInGameObject(scene.path, obj);
        //     }
        // }
        private static void CheckTextTableKey(GameObject gameObject)
        {
            var prefabStage = PrefabStageUtils.GetCurrentPrefabStage();
            CheckTextTableKey(prefabStage.GetAssetPath(), prefabStage.prefabContentsRoot);
        }
        private static void CheckTextTableKey(string filePath, GameObject root)
        {
            List<string[]> list = GetAllKeyInGameObject(filePath, root);
            if (list.Count > 0)
            {
                foreach (string[] item in list)
                {
                    if (EditorLocalizationTool.GetString(EditorLocalizationTool.ReadyLanguageTypes[0], item[0], null) == null)
                    {
                        Debug.LogError($"请在{ThunderFireUIToolConfig.TextTablePath}中补充{item[1]}的本地化Key : {item[0]} !");
                    }
                }
            }
        }
        private static List<string[]> GetAllKeyInGameObject(string filePath, GameObject root)
        {
            List<string[]> list = new List<string[]>();
            ILocalizationText[] uxtexts = root.GetComponentsInChildren<ILocalizationText>(true);
            foreach (var uxt in uxtexts)
            {
                if (!uxt.ignoreLocalization && uxt.localizationType == LocalizationHelper.TextLocalizationType.RuntimeUse && uxt.localizationID != "")
                {
                    string[] item = new string[3];
                    for (int i = 0; i < item.Length; i++)
                    {
                        item[i] = "";
                    }
                    item[0] = uxt.localizationID;
                    item[2] = uxt.text;
                    Transform trans = uxt.transform;
                    while (trans != root.transform)
                    {
                        item[1] = "/" + trans.name + item[1];
                        trans = trans.parent;
                    }
                    item[1] = filePath + "/" + root.name + item[1];
                    AddItem(item, list);
                }
            }
            return list;
        }
        [MenuItem(ThunderFireUIToolConfig.Menu_Localization + "/检查所有文本表格Key (Check All Text Table Key)", false, 54)]
        private static void CheckAllTextTableKey()
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[]{ ThunderFireUIToolConfig.RootPath });
            foreach (var guid in guids)
            {
                string filePath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(filePath);
                CheckTextTableKey(filePath, obj);
            }
            // changed by gdk
            Debug.Log("检查完毕！");
        }
    }
}