#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

namespace ThunderFireUITool
{
    [UXInitialize(100)]
    [System.Serializable]
    public class QuickBackground
    {
        public static bool isOpen;
        public static string name;
        public static string childName;

        static Vector3 position = default;
        static Vector3 rotation = default;
        static Vector3 scale = Vector3.one;
        static Vector2 size = new Vector2(1920, 1080);
        static Color color = Color.white;
        static Sprite sprite = default;
        private static string backgroundString;

        private static bool inPrefabFlag = false;
        private static Texture2D saveIcon;

        private static bool expandFlag = true;

        private static Texture2D SaveIcon
        {
            get
            {
                if (saveIcon == null)
                {
                    saveIcon = ToolUtils.GetIcon("save") as Texture2D;
                }
                return saveIcon;
            }
        }

        private static Texture2D closeIcon;
        private static Texture2D CloseIcon
        {
            get
            {
                if (closeIcon == null)
                {
                    closeIcon = ToolUtils.GetIcon("close") as Texture2D;
                }
                return closeIcon;
            }
        }

        static QuickBackground()
        {
            EditorApplication.playModeStateChanged += (PlayModeStateChange obj) =>
            {
                if (!SwitchSetting.CheckValid(SwitchSetting.SwitchType.QuickBackground)) return;
                if (obj == PlayModeStateChange.EnteredEditMode)
                {
                    inPrefabFlag = PrefabStageUtils.GetCurrentPrefabStage() != null;
                    Load();
                    SetBackGround();
                }
                else if (obj == PlayModeStateChange.ExitingEditMode)
                {
                    Serialize();
                    var go = GameObject.Find("/UXQuickBackground");
                    Object.DestroyImmediate(go);
                }
            };

            EditorSceneManager.sceneClosing += (scene, removingScene) =>
            {
                if (!SwitchSetting.CheckValid(SwitchSetting.SwitchType.QuickBackground)) return;
                Serialize();
                var go = GameObject.Find("/UXQuickBackground");
                Object.DestroyImmediate(go);
            };

            PrefabStageUtils.AddOpenedEvent(s =>
            {
                if (!SwitchSetting.CheckValid(SwitchSetting.SwitchType.QuickBackground)) return;
                inPrefabFlag = PrefabStageUtils.GetCurrentPrefabStage() != null;
                Load();
                SetBackGround();
            });

            PrefabStageUtils.AddClosingEvent(s =>
            {
                if (!SwitchSetting.CheckValid(SwitchSetting.SwitchType.QuickBackground)) return;
                var go = s.prefabContentsRoot.transform.Find("UXQuickBackground")?.gameObject;

                if (go == null) return;

                var dataAll = JsonAssetManager.GetAssets<QuickBackgroundData>();
                if (dataAll == null)
                {
                    dataAll = JsonAssetManager.CreateAssets<QuickBackgroundData>(ThunderFireUIToolConfig.QuickBackgroundDataPath);
                }
                QuickBackgroundDetail dataSingle;
                var path = s.GetAssetPath();
                dataSingle = GetDicData(dataAll, AssetDatabase.AssetPathToGUID(path));
                if (dataSingle == null)
                {
                    var result = new QuickBackgroundDataSingle()
                    {
                        name = Path.GetFileNameWithoutExtension(path),
                        guid = AssetDatabase.AssetPathToGUID(path),
                        detail = new QuickBackgroundDetail(),
                    };
                    dataAll.list.Add(result);
                    dataSingle = result.detail;
                }


                dataSingle.isOpen = isOpen;
                dataSingle.position = go.transform.GetChild(0).localPosition;
                dataSingle.rotation = go.transform.GetChild(0).rotation.eulerAngles;
                dataSingle.scale = go.transform.GetChild(0).localScale;
                dataSingle.size = go.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta;
                Image img = go.transform.GetChild(0).GetComponent<Image>();
                dataSingle.color = new Color(img.color.r, img.color.g,
                    img.color.b, img.color.a);
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(img.sprite.texture, out dataSingle.spriteId, out long localId);

                JsonAssetManager.SaveAssets(dataAll);
                Object.DestroyImmediate(go);
            });

        }

        [MenuItem(ThunderFireUIToolConfig.Menu_BackGround, false, 151)]
        public static void CreateBackGround()
        {
            if (!SwitchSetting.CheckValid(SwitchSetting.SwitchType.QuickBackground)) return;
            inPrefabFlag = PrefabStageUtils.GetCurrentPrefabStage() != null;
            Load();
            isOpen = true;
            backgroundString = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_背景图在Hierarchy中显示名称);
            SetBackGround();
        }

        [UnityEditor.Callbacks.DidReloadScripts(0)]
        private static void OnScriptReload()
        {
            if (!SwitchSetting.CheckValid(SwitchSetting.SwitchType.QuickBackground)) return;
            inPrefabFlag = PrefabStageUtils.GetCurrentPrefabStage() != null;
            name = "UXQuickBackground";
            childName = "UXQuickBackgroundImage";
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyGUI;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }

        public static void SetBackGround()
        {
            if (!SwitchSetting.CheckValid(SwitchSetting.SwitchType.QuickBackground)) return;
            if (!isOpen) return;
            if (GameObject.Find("/UXQuickBackground") != null) return;
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(ThunderFireUIToolConfig.UXEditorToolsPath + "Res/UXQuickBackground.prefab");
            GameObject go;
            if (!inPrefabFlag)
                go = GameObject.Instantiate(prefab);
            else
            {
                var prefabStage = PrefabStageUtils.GetCurrentPrefabStage();
                if (prefabStage.prefabContentsRoot.transform.Find("UXQuickBackground") != null) return;
                go = GameObject.Instantiate(prefab);
                go.transform.SetParent(prefabStage.prefabContentsRoot.transform);
            }
            go.name = "UXQuickBackground";
            go.transform.SetSiblingIndex(0);

            go.transform.localScale = Vector3.one;
            go.transform.GetChild(0).localPosition = position;
            go.transform.GetChild(0).eulerAngles = rotation;
            go.transform.GetChild(0).localScale = scale;
            go.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = size;
            go.transform.GetChild(0).GetComponent<Image>().color = color;
            go.transform.GetChild(0).GetComponent<Image>().sprite = sprite;

            go.hideFlags = HideFlags.DontSave;
            go.transform.GetChild(0).gameObject.hideFlags = HideFlags.DontSave;
            SceneVisibilityManager.instance.DisablePicking(go, true);

            SceneHierarchyUtility.SetExpanded(go, true);
            if (inPrefabFlag)
                SceneHierarchyUtility.SetExpanded(go.transform.parent.gameObject, true);

            name = go.name;
            childName = "UXQuickBackgroundImage";

            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyGUI;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }
        private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go == null) return;

            //子节点(也就是各个面板根节点)有特殊显示样式

            if (go.name == childName)
            {
                Utils.DrawGreenRect(instanceID, selectionRect,
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_参考背景图片));
                Rect rectSave = new Rect(selectionRect.x + selectionRect.width - 50f,
                    selectionRect.y + selectionRect.height - 25, 25,
                    25);
                if (GUI.Button(rectSave, SaveIcon))
                {
                    Serialize();
                }
                Rect rectClose = new Rect(selectionRect.x + selectionRect.width - 25f,
                    selectionRect.y + selectionRect.height - 25, 25,
                    25);
                if (GUI.Button(rectClose, CloseIcon))
                    Close();
            }
            else if (go.name == name)
            {
                if (expandFlag)
                {
                    expandFlag = false;
                    if (inPrefabFlag && go.transform.parent != null)
                    {
                        SceneHierarchyUtility.SetExpanded(go.transform.parent.gameObject, true);
                    }
                    SceneHierarchyUtility.SetExpanded(go, true);
                    if (inPrefabFlag && go.transform.parent != null)
                        SceneHierarchyUtility.SetExpanded(go.transform.parent.gameObject, true);
                }
                Utils.DrawGreenRect(instanceID, selectionRect,
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_背景图在Hierarchy中显示名称));
            }

        }

        public static void Serialize()
        {
            GameObject obj;
            if (!inPrefabFlag)
                obj = GameObject.Find("/UXQuickBackground");
            else
            {
                var prefabStage = PrefabStageUtils.GetCurrentPrefabStage();
                if (prefabStage == null) return;
                obj = prefabStage.prefabContentsRoot.transform.Find("UXQuickBackground")?.gameObject;
            }
            if (obj == null)
            {
                isOpen = false;
                var data = JsonAssetManager.GetAssets<QuickBackgroundData>();
                if (data == null)
                {
                    data = JsonAssetManager.CreateAssets<QuickBackgroundData>(ThunderFireUIToolConfig.QuickBackgroundDataPath);
                }
                if (!inPrefabFlag)
                    GetDicData(data, "-1").isOpen = isOpen;
                else
                {
                    var path = PrefabStageUtils.GetCurrentPrefabStage().GetAssetPath();
                    QuickBackgroundDetail detail = GetDicData(data, AssetDatabase.AssetPathToGUID(path));
                    if (detail != null)
                    {
                        detail.isOpen = isOpen;
                    }
                }
                JsonAssetManager.SaveAssets(data);
                return;
            }
            var dataAll = JsonAssetManager.GetAssets<QuickBackgroundData>();
            if (dataAll == null)
            {
                dataAll = JsonAssetManager.CreateAssets<QuickBackgroundData>(ThunderFireUIToolConfig.QuickBackgroundDataPath);
            }
            QuickBackgroundDetail dataSingle;
            // 直接在场景里
            if (!inPrefabFlag)
                dataSingle = GetDicData(dataAll, "-1");
            // 在prefab里
            else
            {
                var path = PrefabStageUtils.GetCurrentPrefabStage().GetAssetPath();
                dataSingle = GetDicData(dataAll, AssetDatabase.AssetPathToGUID(path));
                if (dataSingle == null)
                {
                    var result = new QuickBackgroundDataSingle()
                    {
                        name = Path.GetFileNameWithoutExtension(path),
                        guid = AssetDatabase.AssetPathToGUID(path),
                        detail = new QuickBackgroundDetail(),
                    };
                    dataAll.list.Add(result);
                    dataSingle = result.detail;
                }
            }

            dataSingle.isOpen = isOpen;
            dataSingle.position = obj.transform.GetChild(0).localPosition;
            dataSingle.rotation = obj.transform.GetChild(0).rotation.eulerAngles;
            dataSingle.scale = obj.transform.GetChild(0).localScale;
            dataSingle.size = obj.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta;
            Image img = obj.transform.GetChild(0).GetComponent<Image>();
            dataSingle.color = new Color(img.color.r, img.color.g,
                img.color.b, img.color.a);
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(img.sprite.texture, out dataSingle.spriteId, out long localId);

            JsonAssetManager.SaveAssets(dataAll);

        }

        public static void Close()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyGUI;
            isOpen = false;
            Serialize();

            GameObject go;
            if (!inPrefabFlag)
                go = GameObject.Find("/UXQuickBackground");
            else
            {
                var prefabStage = PrefabStageUtils.GetCurrentPrefabStage();
                go = prefabStage.prefabContentsRoot.transform.Find("UXQuickBackground")?.gameObject;
            }
            if (go != null)
                Object.DestroyImmediate(go);
        }

        public static void Load()
        {
            expandFlag = true;
            var gameObject = GameObject.Find("/UXQuickBackground");
            if (gameObject != null)
            {
                Object.DestroyImmediate(gameObject);
            }
            var dataAll = JsonAssetManager.GetAssets<QuickBackgroundData>();
            if (dataAll == null)
            {
                dataAll = JsonAssetManager.CreateAssets<QuickBackgroundData>(ThunderFireUIToolConfig.QuickBackgroundDataPath);
            }
            QuickBackgroundDetail data;
            // 直接在场景里
            if (!inPrefabFlag)
                data = GetDicData(dataAll, "-1");
            else
            {
                var path = PrefabStageUtils.GetCurrentPrefabStage().GetAssetPath();
                data = GetDicData(dataAll, AssetDatabase.AssetPathToGUID(path));
            }

            if (data == null)
            {
                data = new QuickBackgroundDetail();
                data.spriteId = AssetDatabase.AssetPathToGUID($"{ThunderFireUIToolConfig.IconPath}QuickBackgroundDefault.png");
            }

            isOpen = data.isOpen;
            position = data.position;
            rotation = data.rotation;
            scale = data.scale;
            size = data.size;
            color = data.color;
            string spritePath = AssetDatabase.GUIDToAssetPath(data.spriteId);
            sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        }

        private static QuickBackgroundDetail GetDicData(QuickBackgroundData dic, string guid)
        {
            var result = dic.list.Find(s => s.guid == guid);
            if (result == null && guid == "-1")
            {
                result = new QuickBackgroundDataSingle()
                {
                    name = "-1",
                    guid = guid,
                    detail = new QuickBackgroundDetail(),
                };
                result.detail.spriteId = AssetDatabase.AssetPathToGUID($"{ThunderFireUIToolConfig.IconPath}QuickBackgroundDefault.png");

                dic.list.Add(result);

                JsonAssetManager.SaveAssets(dic);
            }

            return result?.detail;
        }

    }
}
#endif