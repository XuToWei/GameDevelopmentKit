#if ODIN_INSPECTOR
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.U2D;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using TF_TableList;
using System;
using System.Reflection;

namespace ThunderFireUITool
{
    public class UISinglePrefabCheckWindow : OdinEditorWindow
    {
        private string prefabName;

        [HorizontalGroup("Data Group", 0.5f)]
        [BoxGroup("Data Group/ ")]
        [LabelText("当前Prefab中Atlas信息")]
        public List<AtlasData> atlasDatas = new List<AtlasData>();

        [BoxGroup("Data Group/  ")]
        [TF_TableList(AlwaysExpanded = true)]
        [LabelText("当前Prefab中Image信息")]
        public List<SingleImageRefData> textureDatas = new List<SingleImageRefData>();

        [BoxGroup("Data Group/  ")]
        [TF_TableList(AlwaysExpanded = true)]
        [LabelText("@GetTextCheckResult()")]
        public List<NodeFontData> nodeFontDatas = new List<NodeFontData>();


        [BoxGroup("Data Group/  ")]
        [TF_TableList(AlwaysExpanded = true)]
        [LabelText("@GetComponentCheckResult()")]
        public List<NodeComponentData> nodeComponentDatas = new List<NodeComponentData>();

        [System.Serializable]
        public class AtlasData
        {
            [LabelText("Atlas")]
            public UnityEngine.Object altas = null;

            [HideInInspector]
            public AtlasResolutionItem resolution;
            [LabelText("Atlas分辨率")]
            public string resolutionString;

            [LabelText("Atlas填充率")]
            [ProgressBar(0, 1)]
            public float fillRate;

            [HideInInspector]
            public WasteMemoryItem wasteMemory = new WasteMemoryItem();
            [LabelText("浪费内存")]
            public string wasteMemoryString;

            [LabelText("引用该Atlas的节点数")]
            public int refCount = 0;

            [HideInInspector, LabelText("是否是图集")]
            public bool IsAtlas = false;

            [TF_TableList(AlwaysExpanded = true)]
            [LabelText("详情")]
            public List<AtlasImageRefData> Images = new List<AtlasImageRefData>();
        }

        [System.Serializable]
        public class SingleImageRefData
        {
            [TF_TableListColumnName("节点")]
            public Image image = null;
            [TF_TableListColumnName("图片")]
            public Texture texture = null;
            [TF_TableListColumnName("图片尺寸")]
            public AtlasResolutionItem resolution;
            [TF_TableListColumnName("是否合规")]
            public bool inRule;
        }

        [System.Serializable]
        public class AtlasImageRefData
        {
            [TF_TableListColumnName("节点")]
            public Image Image = null;
            [TF_TableListColumnName("图片")]
            public Texture Texture = null;

            public AtlasImageRefData(Image img, Texture tex)
            {
                Image = img;
                Texture = tex;
            }
        }

        [System.Serializable]
        public class NodeFontData
        {
            [TF_TableListColumnName("节点")]
            public Text go = null;

            [TF_TableListColumnName("问题")]
            public string description;
        }

        [System.Serializable]
        public class NodeComponentData
        {
            [TF_TableListColumnName("节点")]
            public GameObject go = null;

            [TF_TableListColumnName("问题组件")]
            public Component component = null;

            [TF_TableListColumnName("问题")]
            public string description;
        }

        private UIAtlasCheckRuleSettings checkAtlasSettings;
        private UILegacyComponentSettings legacyComponentSettings;
        private void ClearResult()
        {
            atlasDatas.Clear();
            textureDatas.Clear();
            nodeFontDatas.Clear();
            nodeComponentDatas.Clear();
        }
        public static UISinglePrefabCheckWindow CheckAtlasButton()
        {
            UISinglePrefabCheckWindow window = (UISinglePrefabCheckWindow)EditorWindow.GetWindow(typeof(UISinglePrefabCheckWindow));
            window.titleContent = new GUIContent("Prefab检查信息");
            window.minSize = new Vector2(400f, 600f);
            window.Show();
            return window;
        }

        private string greenColorFlag = "<color=#BEEB9C>";
        private string redColorFlag = "<color=#E05B5B>";
        private string ColorFlagEnd = "</color>";
        protected override void OnImGUI()
        {
            GUIStyle titleStyle = new GUIStyle(EditorStyles.label);
            titleStyle.fontSize = 35;
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.normal.textColor = Color.white;
            string title = prefabName;
            EditorGUILayout.LabelField(title, titleStyle, new[] { GUILayout.Height(50) });

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            string atlasCountString = "引用的Atlas数量:   ";
            if (checkAtlasSettings != null)
            {
                if (atlasDatas.Count > checkAtlasSettings.atlasLimit)
                    atlasCountString = atlasCountString + redColorFlag + atlasDatas.Count + ColorFlagEnd;
                else
                    atlasCountString = atlasCountString + greenColorFlag + atlasDatas.Count + ColorFlagEnd;
            }
            else
            {
                atlasCountString = atlasCountString + atlasDatas.Count;
            }

            GUI.skin.label.richText = true;
            EditorGUILayout.LabelField(atlasCountString, GUI.skin.label);

            string imageCountString = "引用的Image数量:   ";
            if (checkAtlasSettings != null)
            {
                if (textureDatas.Count > checkAtlasSettings.imageLimit)
                    imageCountString = imageCountString + redColorFlag + textureDatas.Count + ColorFlagEnd;
                else
                    imageCountString = imageCountString + greenColorFlag + textureDatas.Count + ColorFlagEnd;
            }
            else
            {
                imageCountString = imageCountString + textureDatas.Count;
            }

            EditorGUILayout.LabelField(imageCountString, GUI.skin.label);
            EditorGUILayout.EndHorizontal();

            base.OnImGUI();
        }

        public void CheckPrefabUI(GameObject go = null)
        {
            checkAtlasSettings = AssetDatabase.LoadAssetAtPath<UIAtlasCheckRuleSettings>(ThunderFireUIToolConfig.UICheckSettingFullPath);
            legacyComponentSettings = AssetDatabase.LoadAssetAtPath<UILegacyComponentSettings>(ThunderFireUIToolConfig.UICheckLegacyComponentFullPath);
            if (legacyComponentSettings == null)
            {
                legacyComponentSettings = UILegacyComponentSettings.Create();
            }

            if (go == null)
            {
                var gos = Selection.gameObjects;
                if (gos.Length == 0)
                {
                    Debug.LogError($"要检查的Prefab为空");
                    return;
                }
                else
                {
                    go = gos[0];
                }
            }
            prefabName = go.name;

            ClearResult();

            CheckAtlas(go);
            atlasDatas = atlasDatas.OrderBy(p => p.fillRate).ToList();
            textureDatas = textureDatas.OrderByDescending(p => p.resolution).ToList();

            CheckTextNotDefaultFont(go);
            CheckTextOutOfFrame(go);

            CheckNodeDeprecate(go);
            CheckComponentLegacy(go);
            CheckComponentDuplicate(go);
        }

        #region Atlas && Image

        public void CheckAtlas(GameObject root)
        {
            // 记录每个atlas包含sprite的guid与atlas对应关系
            Dictionary<string, UnityEngine.Object> atlasSpriteDic = new Dictionary<string, UnityEngine.Object>();
            UIResCheckUtils.GetAtlasSpriteDic(ref atlasSpriteDic);

            var imageList = root.transform.GetComponentsInChildren<Image>(true);
            for (int i = 0; i < imageList.Length; i++)
            {

                if (imageList[i] == null || imageList[i].sprite == null
                    || imageList[i].sprite.texture == null) continue;

                string path = AssetDatabase.GetAssetPath(imageList[i].sprite);
                string guid = AssetDatabase.AssetPathToGUID(path);

                if (atlasSpriteDic.ContainsKey(guid))
                {
                    //一个Atlas的图素
                    SpriteAtlas atlas = atlasSpriteDic[guid] as SpriteAtlas;
                    int textureIndex = FindTextureInAtlasList(atlas);
                    if (textureIndex == -1)
                    {
                        SpriteAltasFillingInfo AltasFillingInfo = UIResCheckWindow.GetAltasFillingInfo(atlas);
                        atlasDatas.Add(new AtlasData()
                        {
                            altas = atlas,
                            resolution = AltasFillingInfo.atlasResolution,
                            fillRate = AltasFillingInfo.realFillingRate,
                            wasteMemory = AltasFillingInfo.wasteMemory,
                            IsAtlas = true,
                            resolutionString = AltasFillingInfo.atlasResolution.toString,
                            wasteMemoryString = AltasFillingInfo.wasteMemory.toString
                        });
                        textureIndex = atlasDatas.Count - 1;
                    }
                    atlasDatas[textureIndex].Images.Add(new AtlasImageRefData(imageList[i], imageList[i].sprite.texture));
                }
                else
                {

                    //一个散图
                    int textureIndex = FindTextureInImageList(imageList[i].sprite.texture);
                    if (textureIndex == -1)
                    {
                        Texture2D tex = imageList[i].sprite.texture;
                        bool inRule = UIResCheckWindow.GetImageIsInRule(tex);
                        AtlasResolutionItem atlasResolution = new AtlasResolutionItem();
                        atlasResolution.Add(tex.width, tex.height);
                        textureDatas.Add(new SingleImageRefData()
                        {
                            image = imageList[i],
                            texture = imageList[i].sprite.texture,
                            resolution = atlasResolution,
                            inRule = inRule
                        });
                    }
                }
            }

            for (int i = 0; i < atlasDatas.Count; i++)
            {
                atlasDatas[i].refCount = atlasDatas[i].Images.Count;
            }
        }

        private UnityEngine.Object GetRuntimeTexture(Sprite sprite)
        {
            if (sprite == null || sprite.texture == null)
            {
                return null;
            }

            var path = AssetDatabase.GetAssetPath(sprite.texture);
            if (string.IsNullOrEmpty(path))
            {
                return sprite.texture;
            }

            string folderName = Path.GetFileName(Path.GetDirectoryName(path));
            string fileName = Path.GetFileNameWithoutExtension(path);
            SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>($"Assets/Prefabs/Atlas/{folderName}.spriteatlas");
            if (atlas == null)
            {
                atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>($"Assets/Prefabs/Atlas/{folderName}_Atlas.spriteatlas");
            }
            if (atlas != null)
            {
                if (atlas.GetSprite(fileName) != null)
                {
                    return atlas;
                }
            }
            return sprite.texture;
        }

        private int FindTextureInAtlasList(UnityEngine.Object value)
        {

            for (int i = 0; i < atlasDatas.Count; i++)
            {
                if (atlasDatas[i].altas == value)
                    return i;
            }
            return -1;//没有找到，应该添加
        }
        private int FindTextureInImageList(UnityEngine.Object value)
        {

            for (int i = 0; i < textureDatas.Count; i++)
            {
                if (textureDatas[i].texture == value)
                    return i;
            }
            return -1;//没有找到，应该添加
        }

        #endregion

        #region Font
        private static string TextNotDefaultFont = "未使用默认字体";
        //private static string TextOutofFrameMaybe = "文本可能超框";
        //private static string TextOutofFrame = "文本超框";

        private int DefaultFontCount = 0;
        private int OutofFrameCount = 0;
        private void CheckTextNotDefaultFont(GameObject go)
        {
            var checkAtlasSettings = AssetDatabase.LoadAssetAtPath<UIAtlasCheckRuleSettings>(ThunderFireUIToolConfig.UICheckSettingFullPath);
            string fontPath = checkAtlasSettings.defaultFontPath;

            Font defaultFont = AssetDatabase.LoadAssetAtPath<Font>(fontPath);
            var txts = go.GetComponentsInChildren<Text>(true);
            if (txts != null && txts.Length > 0)
            {
                DefaultFontCount = 0;
                foreach (var txt in txts)
                {
                    if (txt.font != defaultFont)
                    {
                        NodeFontData data = new NodeFontData()
                        {
                            go = txt,
                            description = TextNotDefaultFont
                        };
                        nodeFontDatas.Add(data);
                        DefaultFontCount++;
                    }
                }
            }
        }

        private void CheckTextOutOfFrame(GameObject go)
        {
            return;
            /*
            OutofFrameCount = 0;
            foreach (var lxtext in go.GetComponentsInChildren<LXText>())
            {
                lxtext.CustomRefreshRenderedText();
                if (lxtext.HasEllipsis)
                {
                    var lxtextPath = lxtext.transform.PathFromRoot();
                    var lxtextNotAnchorParent = lxtext.rectTransform;
                    while (lxtextNotAnchorParent != null && lxtextNotAnchorParent.anchorMin != lxtextNotAnchorParent.anchorMax)
                    {
                        lxtextNotAnchorParent = lxtextNotAnchorParent.parent as RectTransform;
                    }
                    if (lxtext.GetComponent<ContentSizeFitter>())
                    {
                        continue;
                    }

                    NodeFontData data = new NodeFontData()
                    {
                        go = lxtext,
                    };


                    if (lxtextNotAnchorParent != null && lxtextNotAnchorParent.parent != null &&
                            lxtextNotAnchorParent.parent.GetComponent<ILayoutGroup>() != null)
                    {
                        data.description = TextOutofFrameMaybe;
                    }
                    else
                    {
                        data.description = TextOutofFrame;
                    }
                    nodeFontDatas.Add(data);
                    OutofFrameCount++;
                }
            }
            */
        }

        private string GetTextCheckResult()
        {
            string DefaultFontString = DefaultFontCount > 0 ? redColorFlag + DefaultFontCount + ColorFlagEnd : greenColorFlag + DefaultFontCount + ColorFlagEnd;
            string OutofFrameString = OutofFrameCount > 0 ? redColorFlag + OutofFrameCount + ColorFlagEnd : greenColorFlag + OutofFrameCount + ColorFlagEnd;

            return "未使用默认字体节点: " + DefaultFontString + "     " + "文本超框数量: " + OutofFrameString;
        }

        #endregion

        #region Component
        private static string NodeDeprecate = "节点可能已经废弃,请确认";
        private static string ComponentDuplicate = "节点上有重复组件,请删除";
        private static string ComponentLegacy = "节点上有违规组件,请删除";

        private int DeprecateCount = 0;
        private int DuplicateCount = 0;
        private int LegacyCount = 0;

        private void CheckNodeDeprecate(GameObject go)
        {
            var children = go.GetComponentsInChildren<Transform>(true);
            //children.Remove(go.transform);

            //生成component引用列表 表里是所有的被引过的Component的FileID
            //生成规则: 遍历prefab文件,排除掉所有的m_GameObject、m_PrefabAsset、m_Father、m_Children的项，取其他项的InstanceId,即为被引用的InstanceId
            Assembly asm = Assembly.GetAssembly(typeof(Editor));
            Type type = asm.GetType("UnityEditor.SerializationDebug");
            if (type == null) return;

            MethodInfo ToYAMLString_Method = type.GetMethod("ToYAMLString", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if (ToYAMLString_Method == null) return;

            List<string> assignedInstanceIdList = new List<string>();
            foreach (Transform child in children)
            {
                var components = child.GetComponents<Component>();
                foreach (Component component in components)
                {
                    string prefabYaml = (string)ToYAMLString_Method.Invoke(null, new System.Object[] { component });
                    //Debug.Log(prefabYaml);
                    PrefabYamlUtils.LoadDependInstanceIdFromComponentString(prefabYaml, ref assignedInstanceIdList);
                }
            }

            //检查所有的Disable节点及其Components
            List<GameObject> deprecateGos = new List<GameObject>();
            foreach (Transform child in children)
            {
                if (!child.gameObject.activeSelf)
                {
                    bool isAssigned = false;

                    string id = child.gameObject.GetInstanceID().ToString();
                    if (assignedInstanceIdList.Contains(id))
                    {
                        isAssigned = true;
                    }

                    if (isAssigned) continue;

                    List<Component> components = child.gameObject.GetComponents<Component>().ToList();
                    foreach (Component component in components)
                    {
                        id = component.GetInstanceID().ToString();
                        if (assignedInstanceIdList.Contains(id))
                        {
                            isAssigned = true;
                            break;
                        }
                    }
                    if (isAssigned)
                        continue;
                    else
                    {
                        deprecateGos.Add(child.gameObject);
                    }
                }
            }

            foreach (GameObject gameObject in deprecateGos)
            {
                NodeComponentData data = new NodeComponentData()
                {
                    go = gameObject,
                    description = NodeDeprecate
                };
                nodeComponentDatas.Add(data);
            }
            DeprecateCount = deprecateGos.Count;
        }

        private void CheckComponentDuplicate(GameObject go)
        {
            DuplicateCount = 0;

            var children = go.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                var components = child.GetComponents<Component>();
                List<IGrouping<Type, Component>> duplicate = components.GroupBy(com => com.GetType()).Where(g => g.Count() > 1).ToList();
                foreach (var comGroup in duplicate)
                {
                    Component com = comGroup.First();
                    NodeComponentData data = new NodeComponentData()
                    {
                        go = com.gameObject,
                        component = com,
                        description = ComponentDuplicate
                    };
                    nodeComponentDatas.Add(data);
                    DuplicateCount++;
                }
            }
        }

        private void CheckComponentLegacy(GameObject go)
        {
            var LegacyComponentTypes = new List<Type>();

            foreach (MonoScript script in legacyComponentSettings.LegacyComponents)
            {
                LegacyComponentTypes.Add(script.GetClass());
            }

            LegacyCount = 0;
            var components = go.GetComponentsInChildren<Component>();
            foreach (Component com in components)
            {
                if (LegacyComponentTypes.Contains(com.GetType()))
                {
                    NodeComponentData data = new NodeComponentData()
                    {
                        go = com.gameObject,
                        component = com,
                        description = ComponentLegacy
                    };
                    nodeComponentDatas.Add(data);
                    LegacyCount++;
                }
            }
        }

        private string GetComponentCheckResult()
        {
            string DeprecateString = DeprecateCount > 0 ? redColorFlag + DeprecateCount + ColorFlagEnd : greenColorFlag + DeprecateCount + ColorFlagEnd;
            string DuplicateString = DuplicateCount > 0 ? redColorFlag + DuplicateCount + ColorFlagEnd : greenColorFlag + DuplicateCount + ColorFlagEnd;
            string LegacyString = LegacyCount > 0 ? redColorFlag + LegacyCount + ColorFlagEnd : greenColorFlag + LegacyCount + ColorFlagEnd;

            return "废弃节点数量: " + DeprecateString + "     " + "重复组件: " + DuplicateString + "      " + "违规组件: " + LegacyString;
        }
        #endregion
    }
}
#endif