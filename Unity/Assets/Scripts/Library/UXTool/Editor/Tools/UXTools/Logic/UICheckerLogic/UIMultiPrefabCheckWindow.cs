#if ODIN_INSPECTOR
using UnityEngine;
using UnityEngine.U2D;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using TF_TableList;
using System.Text.RegularExpressions;

namespace ThunderFireUITool
{
    public class UIMultiPrefabCheckWindow : OdinEditorWindow
    {
        [TF_TableList(AlwaysExpanded = false)]
        public List<PrefabResData> resDatas = new List<PrefabResData>();

        [System.Serializable]
        public class PrefabResData
        {
            [TF_TableListColumnName("Prefab")]
            public GameObject go = null;

            [TF_TableListColumnName("Atlas Count")]
            public int atlasCount = 0;
            [TF_TableListColumnName("Atlas")]
            public List<UnityEngine.Object> atlasList;

            [TF_TableListColumnName("Texture Count")]
            public int textureCount = 0;
            [TF_TableListColumnName("Texture")]
            public List<Texture> textureList;
        }

        private void ClearResult()
        {
            resDatas.Clear();
        }
        public static UIMultiPrefabCheckWindow ShowWindow()
        {
            UIMultiPrefabCheckWindow window = (UIMultiPrefabCheckWindow)EditorWindow.GetWindow(typeof(UIMultiPrefabCheckWindow));
            window.titleContent = new GUIContent("Prefab资源引用");
            window.minSize = new Vector2(400f, 600f);
            window.Show();
            return window;
        }

        protected override void OnImGUI()
        {
            GUIStyle titleStyle = new GUIStyle(EditorStyles.label);
            titleStyle.fontSize = 35;
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.normal.textColor = Color.white;
            string title = "Prefab资源引用";
            EditorGUILayout.LabelField(title, titleStyle, new[] { GUILayout.Height(50) });

            EditorGUILayout.Space();

            base.OnImGUI();
        }

        public void CheckImageDependencies()
        {
            ClearResult();

            // 记录每个atlas包含sprite的guid与atlas对应关系
            Dictionary<string, UnityEngine.Object> atlasSpriteDic = new Dictionary<string, UnityEngine.Object>();
            UIResCheckUtils.GetAtlasSpriteDic(ref atlasSpriteDic);


            var checkAtlasSettings = AssetDatabase.LoadAssetAtPath<UIAtlasCheckRuleSettings>(ThunderFireUIToolConfig.UICheckSettingFullPath);
            string prefabFolder = checkAtlasSettings.prefabFolderPath;

            string[] prefabs = new string[] {
                "InGameCommentDisplay.prefab",
                "EmotionNew.prefab",
                "UserControl.prefab",
                "InGamePlayerNameBoards.prefab",
                "InGameShootPreinput.prefab",
                "InGameFreeStyleShowPanel.prefab",
                "InGameChat2.prefab",
                "InGameOwnScoreUI.prefab",
                "ScoreBoard.prefab",
                "InGameSwitchDefencePanel.prefab",
                "InGameBuff.prefab",
                "InGameGuideAssistant.prefab",
                "IngamePopUpUI.prefab",
                "IngameSettings.prefab",
                "InGameCelebrateShow.prefab",
                "IngameConsoleButton.prefab",
                "IngameJumpball.prefab"
            };

            List<string> guids = new List<string>();
            foreach (var name in prefabs)
            {
                string path = prefabFolder + "/" + name;
                string guid = AssetDatabase.AssetPathToGUID(path);
                if (string.IsNullOrEmpty(guid))
                {
                    Debug.Log(name + "cant find");
                }
                guids.Add(guid);
            }



            //string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[] { prefabFolder });
            //string[] guids = new string[1] { "b0f539533ec302548bed31b2ca6eba29" };
            foreach (string guid in guids)
            {
                var prefabPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

                if (prefab == null) continue;

                PrefabResData resData = new PrefabResData();
                resData.go = prefab;
                resData.atlasList = new List<UnityEngine.Object>();
                resData.textureList = new List<Texture>();

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
                    if (atlasSpriteDic.ContainsKey(spriteGuid))
                    {
                        //Atlas
                        if (!resData.atlasList.Contains(atlasSpriteDic[spriteGuid]))
                        {
                            resData.atlasList.Add(atlasSpriteDic[spriteGuid]);
                        }
                    }
                    else
                    {
                        //Texture
                        var imagePath = AssetDatabase.GUIDToAssetPath(spriteGuid);
                        Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(imagePath);
                        if (!resData.textureList.Contains(texture))
                        {
                            resData.textureList.Add(texture);
                        }
                    }
                }

                resData.atlasCount = resData.atlasList.Count;
                resData.textureCount = resData.textureList.Count;

                resDatas.Add(resData);
            }

            //统计一下输出个log
            List<List<Object>> atlasList = resDatas.Select(resData => resData.atlasList).ToList();
            HashSet<Object> atlasSet = new HashSet<Object>();
            foreach (var list in atlasList)
            {
                foreach (var atlas in list)
                {
                    atlasSet.Add(atlas);
                }
            }
            List<string> atlasNames = atlasSet.Select(atlas => atlas.name).ToList();
            string log = string.Join("\n", atlasNames.ToArray());
            Debug.Log("使用的Atlas: \n" + log);
        }
    }
}
#endif