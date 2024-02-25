#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
#if UNITY_2021_2_OR_NEWER
using PrefabStageUtility = UnityEditor.SceneManagement.PrefabStageUtility;
using PrefabStage = UnityEditor.SceneManagement.PrefabStage;
#else
using PrefabStageUtility = UnityEditor.Experimental.SceneManagement.PrefabStageUtility;
using PrefabStage = UnityEditor.Experimental.SceneManagement.PrefabStage;
#endif
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using Text = UnityEngine.UI.Text;
using ThunderFireUnityEx;
using System.Reflection;
using System.Threading.Tasks;

namespace ThunderFireUITool
{
    [UXInitialize(100)]
    [System.Serializable]
    public class UIBeginnerGuideEditor : UXSingleton<UIBeginnerGuideEditor>
    {
        private GameObject beginnerGuidePrefab;
        private UIBeginnerGuide beginnerGuide;

        public UIBeginnerGuideData guideData;
        private GuideTextData guideTextData;
        private GuideGestureData gestureData;
        private GuideGamePadData gamePadData;
        private GuideTargetStrokeData targetStrokeData;
        private GuideArrowLineData guideArrowLineData;
        private GuideHighLightData guideHighLightData;

        private VisualElement EditorPanel;
        private VisualElement SavePanel;

        private GameObject _gameObjectForDiff;

        private static Dictionary<string, string> customGuideRootNames = new Dictionary<string, string>();


        private GameObject guideRoot;
        private struct GuideRootInfo
        {
            public string name;
            public string displayName;
        }

        /// <summary>
        /// key:instanceID, value: info
        /// </summary>
        private Dictionary<int, GuideRootInfo> GuideRootInfos = new Dictionary<int, GuideRootInfo>();

        private List<Transform> allChilds;
        private List<Transform> allChildsForDiff;
        private List<Transform> sons;
        private List<Transform> grandsons;
        public void SceneCloseCallBack(Scene scene, bool ok)
        {
            CloseEditor();
        }

        public void OpenEditor(UIBeginnerGuideData data, GameObject root)
        {
            guideRoot = root;
            InitData(data);
            bool success = InitEditableBeginnerGuide();

            if (!success)
            {
                CloseEditor();
                return;
            }

            InitToolBar();

            SceneViewContextMenu.addContextMenuFunc += OnRightClickInScene;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
            PrefabStageUtils.AddClosingEvent(PrefabStageClosing);
            PrefabStageUtils.AddOpenedEvent(PrefabStageClosing);
            EditorApplication.update += UpdateTransform;
            EditorSceneManager.sceneClosing += SceneCloseCallBack;
            SceneHierarchyUtility.SetExpanded(root, true);
            Transform guideroot = root.transform.GetChild(root.transform.childCount - 1);
            SceneHierarchyUtility.SetExpanded(guideroot.gameObject, true);
            foreach (Transform child in guideroot.transform)
            {
                //Debug.Log(child.name);
                if (child.GetComponent<GuideHighLight>() != null)
                {
                    SceneHierarchyUtility.SetExpanded(child.gameObject, true);
                }
                else if (child.GetComponent<GuideText>() != null)
                {
                    SceneHierarchyUtility.SetExpandedRecursive(child.gameObject, true);
                }
                else if (child.GetComponent<GuideGestureData>() != null)
                {
                    SceneHierarchyUtility.SetExpanded(child.gameObject, true);
                }
            }
        }
        private void UpdateTransform()
        {
            if (gestureData != null)
            {
                if (gestureData.objectSelectType == ObjectSelectType.select && gestureData.selectedObject != null)
                {
                    gestureData.gameObject.transform.SetPositionAndRotation
                        (gestureData.selectedObject.transform.position, gestureData.selectedObject.transform.rotation);
                }
            }
            if (targetStrokeData != null)
            {
                if (targetStrokeData.targetType == TargetType.Target && targetStrokeData.targetGameObject != null)
                {
                    targetStrokeData.gameObject.transform.GetComponent<RectTransform>().sizeDelta = targetStrokeData.targetGameObject.transform.GetComponent<RectTransform>().sizeDelta;
                    targetStrokeData.gameObject.transform.localScale = targetStrokeData.targetGameObject.transform.localScale;
                    targetStrokeData.gameObject.transform.position = targetStrokeData.targetGameObject.transform.position;
                    targetStrokeData.gameObject.transform.eulerAngles = targetStrokeData.targetGameObject.transform.eulerAngles;
                    // targetStrokeData.gameObject.transform.SetPositionAndRotation
                    //    (targetStrokeData.targetGameObject.transform.position, targetStrokeData.targetGameObject.transform.rotation);
                }
            }

            if(guideHighLightData!=null)
            {
                if(guideHighLightData.UseCustomTarget)
                {
                    beginnerGuide.highLightWidget.SetTarget(guideData.highLightTarget);
                }
                else
                {
                    beginnerGuide.highLightWidget.SetTarget(null);
                }
            }
        }

        public void CloseEditor()
        {
            if (EditorPanel != null)
            {
                EditorPanel.parent.Remove(EditorPanel);
            }
            if (beginnerGuide != null)
            {
                Object.DestroyImmediate(beginnerGuide.gameObject);
            }

            SceneViewContextMenu.addContextMenuFunc -= OnRightClickInScene;
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyGUI;
            PrefabStageUtils.RemoveClosingEvent(PrefabStageClosing);
            PrefabStageUtils.RemoveOpenedEvent(PrefabStageClosing);
            EditorApplication.update -= UpdateTransform;
            EditorSceneManager.sceneClosing -= SceneCloseCallBack;
            Release();
        }

        private void InitData(UIBeginnerGuideData data)
        {
            guideData = data;
            beginnerGuidePrefab = guideData.guideTemplatePrefab;

            customGuideRootNames["GuideTemplate"] = EditorLocalization.GetLocalization("UIBeginnerGuide", "GuideTemplate");
            customGuideRootNames["TextWidget"] = EditorLocalization.GetLocalization("UIBeginnerGuide", "TextWidget");
            customGuideRootNames["GestureWidget"] = EditorLocalization.GetLocalization("UIBeginnerGuide", "GestureWidget");
            customGuideRootNames["GamePadWidget"] = EditorLocalization.GetLocalization("UIBeginnerGuide", "GamePadWidget");
            customGuideRootNames["HighLightWidget"] = EditorLocalization.GetLocalization("UIBeginnerGuide", "HighLightWidget");
            customGuideRootNames["TargetStrokeWidget"] = EditorLocalization.GetLocalization("UIBeginnerGuide", "TargetStrokeWidget");
            customGuideRootNames["ArrowLineWidget"] = EditorLocalization.GetLocalization("UIBeginnerGuide", "ArrowLineWidget");
        }

        private void InitToolBar()
        {
            SceneView sceneView = SceneView.lastActiveSceneView;

            VisualTreeAsset editorTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ThunderFireUIToolConfig.UIBuilderPath + "UIBeginnerEditor.uxml");


            EditorPanel = editorTreeAsset.CloneTree().Children().First();
#if UNITY_2021_1_OR_NEWER
            EditorPanel.style.top = SwitchSetting.CheckValid(SwitchSetting.SwitchType.PrefabMultiOpen) ? 25 : 0;
#else
            EditorPanel.style.top = SwitchSetting.CheckValid(SwitchSetting.SwitchType.PrefabMultiOpen) ? 47 : 22;
#endif
            sceneView.rootVisualElement.Add(EditorPanel);

            Label label = EditorPanel.Q<Label>("Label");
            label.text = guideData.guideID;
            Button previewBtn = EditorPanel.Q<Button>("PreviewBtn");
            previewBtn.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_预览);
            Button saveBtn = EditorPanel.Q<Button>("SaveBtn");
            saveBtn.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_保存);
            Button closeBtn = EditorPanel.Q<Button>("CloseBtn");
            closeBtn.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_关闭);

            previewBtn.clicked += Preview;
            saveBtn.clicked += Save;
            closeBtn.clicked += CloseEditor;

        }
        private bool InitEditableBeginnerGuide()
        {
            //先获取Prefab中的Canvas(Environment)作为父节点,初始化出正确的引导遮罩大小
            GameObject go = GameObject.Instantiate(beginnerGuidePrefab, guideRoot.transform);
            //将父节点改为Prefab的根节点,因为prefab模式下Canvas(Environment)只能有一个子节点,不改的话创建的新手引导就会被刷掉了
            //go.transform.SetParent(guideRoot.transform);
            allChildsForDiff = go.GetComponentsInChildren<Transform>(true).ToList();

            go.gameObject.hideFlags = HideFlags.DontSave;  //| HideFlags.NotEditable;
            beginnerGuide = go.GetComponent<UIBeginnerGuide>();
            if (beginnerGuide == null)
            {
                EditorUtility.DisplayDialog("messageBox",
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_引导模板中不存在UIBeginnerGuide组件),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消)
                    );
                Object.DestroyImmediate(go);
                return false;
            }

            beginnerGuide.Init(guideData);
            beginnerGuide.EditorInit();

            _gameObjectForDiff = go;

            allChilds = go.GetComponentsInChildren<Transform>(true).ToList();

            sons = new List<Transform>();

            //把自己也加入son队列中,因为在hierarchy中表现是一样的
            sons.Add(go.transform);
            string originName = GetNameWithoutBrackets(go.name);
            string formatRootName = originName.Split('_')[0];
            GuideRootInfo rootInfo = new GuideRootInfo()
            {
                name = go.name,
                displayName = customGuideRootNames.ContainsKey(formatRootName) ? customGuideRootNames[formatRootName] : originName
            };
            GuideRootInfos.Add(go.GetInstanceID(), rootInfo);

            foreach (Transform child in go.transform)
            {
                //Debug.Log(child.name);
                sons.Add(child);
                if (child.gameObject.GetComponent<GuideHighLight>() != null)
                {
                    Transform highlightchild = child.GetChild(0);
                    sons.Add(highlightchild);
                    string formatname1 = highlightchild.gameObject.name.Split('_')[0];

                    GuideRootInfo Info1 = new GuideRootInfo()
                    {
                        name = highlightchild.gameObject.name,
                        displayName = EditorLocalization.GetLocalization("UIBeginnerGuide", "highlightchild")
                    };
                    GuideRootInfos.Add(highlightchild.gameObject.GetInstanceID(), Info1);
                }
                string formatname = child.gameObject.name.Split('_')[0];

                GuideRootInfo Info = new GuideRootInfo()
                {
                    name = child.gameObject.name,
                    displayName = customGuideRootNames.ContainsKey(formatname) ? customGuideRootNames[formatname] : child.gameObject.name
                };
                GuideRootInfos.Add(child.gameObject.GetInstanceID(), Info);
            }
            grandsons = allChilds.Except(sons).ToList();

            guideTextData = go.GetComponentInChildren<GuideTextData>(true);
            gestureData = go.GetComponentInChildren<GuideGestureData>(true);
            gamePadData = go.GetComponentInChildren<GuideGamePadData>(true);
            targetStrokeData = go.GetComponentInChildren<GuideTargetStrokeData>(true);
            guideArrowLineData = go.GetComponentInChildren<GuideArrowLineData>(true);
            guideHighLightData = go.GetComponentInChildren<GuideHighLightData>(true);

            return true;
        }
        public void Save()
        {
            var guideDataList = guideRoot.GetComponent<UIBeginnerGuideDataList>();

            UIBeginnerGuideData index = guideDataList.guideDataList.Where(data => data.guideID == guideData.guideID).ToList().First();

            var so = new SerializedObject(guideDataList);
            //SerializedProperty guideDataListSp = so.FindProperty("guideDataList");
            //SerializedProperty guideDataSp = guideDataListSp.GetArrayElementAtIndex(index);

            if (guideTextData != null)
            {
                guideTextData.Saved = true;
                index.guideTextPanelData = guideTextData.Serialize();
            }

            if (gamePadData != null)
            {
                gamePadData.Saved = true;
                index.gamePadPanelData = gamePadData.Serialize();
            }

            if (guideArrowLineData != null)
            {
                guideArrowLineData.Saved = true;
                index.guideArrowLineData = guideArrowLineData.Serialize();
            }

            if (gestureData != null)
            {
                gestureData.Saved = true;
                index.guideGesturePanelData = gestureData.Serialize();
                index.GestureObject = gestureData.GestureObject;
                index.selectedObject = gestureData.selectedObject;
            }

            if (targetStrokeData != null)
            {
                targetStrokeData.Saved = true;
                index.targetStrokeData = targetStrokeData.Serialize();
                if (targetStrokeData.targetType == TargetType.Target && targetStrokeData.targetGameObject != null)
                {
                    index.strokeTarget = targetStrokeData.targetGameObject;
                }
            }

            if (guideHighLightData != null)
            {
                guideHighLightData.Saved = true;
                index.guideHighLightData = guideHighLightData.Serialize();
                if (guideHighLightData.target != null)
                {
                    index.highLightTarget = guideHighLightData.target.gameObject;
                }
                else
                {
                    index.highLightTarget = null;
                }
            }

            so.ApplyModifiedProperties();


            SaveCustomObject();
            FindNewObject();

            Debug.Log("Save Successfully!");

            SceneView sceneView = SceneView.lastActiveSceneView;

            VisualTreeAsset editorTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ThunderFireUIToolConfig.UIBuilderPath + "UIBeginnerSave.uxml");

            SavePanel = editorTreeAsset.CloneTree().Children().First();
            sceneView.rootVisualElement.Add(SavePanel);
            EditorUtility.SetDirty(guideRoot);
            PrefabUtility.RecordPrefabInstancePropertyModifications(guideDataList);
            Task.Delay(2000).ContinueWith(CloseSave);
        }
        public bool needSave()
        {
            var guideDataList = guideRoot.GetComponent<UIBeginnerGuideDataList>();
            UIBeginnerGuideData index = guideDataList.guideDataList.Where(data => data.guideID == guideData.guideID).ToList().First();

            var so = new SerializedObject(guideDataList);
            //SerializedProperty guideDataListSp = so.FindProperty("guideDataList");
            //SerializedProperty guideDataSp = guideDataListSp.GetArrayElementAtIndex(index);

            if (guideTextData != null)
            {
                string tmpstr = guideTextData.Serialize();
                if (index.guideTextPanelData != tmpstr)//(guideDataSp.FindPropertyRelative("guideTextPanelData").stringValue != tmpstr)
                {
                    return true;
                }
            }

            if (gamePadData != null)
            {
                string tmpstr = gamePadData.Serialize();
                if (index.gamePadPanelData != tmpstr)//(guideDataSp.FindPropertyRelative("gamePadPanelData").stringValue != tmpstr)
                {
                    return true;
                }
            }

            if (guideArrowLineData != null)
            {
                string tmpstr = guideArrowLineData.Serialize();
                if (index.guideArrowLineData != tmpstr)//(guideDataSp.FindPropertyRelative("guideArrowLineData").stringValue != tmpstr)
                {
                    return true;
                }
            }

            if (gestureData != null)
            {
                string tmpstr = gestureData.Serialize();
                if (index.guideGesturePanelData != tmpstr)//(guideDataSp.FindPropertyRelative("guideGesturePanelData").stringValue != tmpstr)
                {
                    return true;
                }
                GameObject tmpobj = gestureData.GestureObject;
                if (index.GestureObject != tmpobj)//(guideDataSp.FindPropertyRelative("GestureObject").objectReferenceValue != tmpobj)
                {
                    return true;
                }
            }

            if (targetStrokeData != null)
            {
                string tmpstr = targetStrokeData.Serialize();
                if (index.targetStrokeData != tmpstr)//(guideDataSp.FindPropertyRelative("targetStrokeData").stringValue != tmpstr)
                {
                    return true;
                }
                if (targetStrokeData.targetType == TargetType.Target && targetStrokeData.targetGameObject != null)
                {
                    GameObject tmpobj = targetStrokeData.targetGameObject;
                    if (index.strokeTarget != tmpobj)//(guideDataSp.FindPropertyRelative("strokeTarget").objectReferenceValue != tmpobj)
                    {
                        return true;
                    }
                }
            }


            if (guideHighLightData != null)
            {
                if (guideHighLightData.target != null)
                {
                    string tmpstr = guideHighLightData.Serialize();
                    if (index.guideHighLightData != tmpstr)//(guideDataSp.FindPropertyRelative("guideHighLightData").stringValue != tmpstr)
                    {
                        return true;
                    }
                    GameObject tmpobj = guideHighLightData.target.gameObject;
                    if (index.highLightTarget != tmpobj)//(guideDataSp.FindPropertyRelative("highLightTarget").objectReferenceValue != tmpobj)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private void CloseSave(Task tsk)
        {
            SavePanel.parent.Remove(SavePanel);
        }
        private void OnRightClickInScene()
        {
            AddGuideTargetMenuItem();
        }
        private void AddGuideTargetMenuItem()
        {
            ContextMenu.AddItem(EditorLocalization.GetLocalization("UIBeginnerGuide", "SetAsHighLight"), false, () =>
            {
                guideData.highLightTarget = Selection.activeGameObject;
                beginnerGuide.HighLightAreaPreview(guideData.highLightTarget.GetComponent<RectTransform>());
            });
        }
        private void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go == null) return;
            //Debug.Log(go.transform);
            //子节点(也就是各个面板根节点)有特殊显示样式
            if (sons.Contains(go.transform))
            {

                Utils.DrawGreenRect(instanceID, selectionRect, GuideRootInfos[instanceID].displayName);
                // EditorGUI.DrawRect(selectionRect, new Color(0.157f, 0.157f, 0.157f, 1f));
                // var icon = EditorGUIUtility.ObjectContent(null, typeof(GameObject)).image;
                // GUI.Label(selectionRect, icon);
                // GUIStyle style = LabelStyle(Color.green);

                // var rect = new Rect(selectionRect);
                // rect.x += 20;

                // GUI.Label(rect, GuideRootInfos[instanceID].displayName, style);
            }
            else
            {
                Transform par = go.transform.parent;
                if (par != null)
                {
                    if (par.GetComponent<GuideText>() != null)
                    {
                        if (go.name == "Default")
                        {
                            go.name = EditorLocalization.GetLocalization("GuideTextData", "default");
                        }
                        else if (go.name == "WithTitle")
                        {
                            go.name = EditorLocalization.GetLocalization("GuideTextData", "withTitle");
                        }
                        if (go.activeInHierarchy == true)
                        {
                            Utils.DrawGreenRect(instanceID, selectionRect, go.name);
                            // EditorGUI.DrawRect(selectionRect, new Color(0.157f, 0.157f, 0.157f, 1f));
                            // var icon = EditorGUIUtility.ObjectContent(null, typeof(GameObject)).image;
                            // GUI.Label(selectionRect, icon);
                            // GUIStyle style = LabelStyle(Color.green);

                            // var rect = new Rect(selectionRect);
                            // rect.x += 20;

                            // GUI.Label(rect, go.name, style);
                        }
                    }
                    else if (par.GetComponent<GuideGesture>() != null && go.transform.childCount == 0)
                    {
                        Utils.DrawGreenRect(instanceID, selectionRect, go.name);
                        // EditorGUI.DrawRect(selectionRect, new Color(0.157f, 0.157f, 0.157f, 1f));
                        // var icon = EditorGUIUtility.ObjectContent(null, typeof(GameObject)).image;
                        // GUI.Label(selectionRect, icon);
                        // GUIStyle style = LabelStyle(Color.green);

                        // var rect = new Rect(selectionRect);
                        // rect.x += 20;

                        // GUI.Label(rect, go.name, style);
                    }
                }

            }


            //孙节点不能被选中编辑
            if (Event.current.button == 0 && Event.current.type <= EventType.MouseUp)
            {
                if (grandsons.Contains(go.transform))
                {
                    //Event.current.type = EventType.Ignore;//该语句的作用为限制允许编辑与否
                }
            }
        }
        private static GUIStyle LabelStyle(Color color)
        {
            var style = new GUIStyle(((GUIStyle)"Label"))
            {
                padding =
            {
                left = EditorStyles.label.padding.left,
                top = EditorStyles.label.padding.top + 1
            },
                normal =
            {
                textColor = color
            }
            };
            return style;
        }
        private string GetNameWithoutBrackets(string name)
        {
            return Regex.Replace(name, @"\([^\(]*\)", "");
        }
        private void PrefabStageClosing(PrefabStage p)
        {
            CloseEditor();
        }

        public string GetTransformRoute(Transform transform)
        {
            var result = transform.name;
            var parent = transform.parent;
            while (parent != null && parent.gameObject != beginnerGuide.gameObject && transform.gameObject != beginnerGuide.gameObject)
            {
                result = $"{parent.name}/{result}";
                parent = parent.parent;
            }
            return result;
        }

        private void SaveCustomObject()
        {
            List<Transform> customObject = new List<Transform>();

            var stack = new Stack<Transform>();
            stack.Push(beginnerGuide.transform);

            while (stack.Any())
            {
                Transform transform = stack.Pop();
                if (transform.GetComponent<GuideWidgetData>() != null) continue;
                customObject.Add(transform);
                foreach (Transform child in transform)
                {
                    stack.Push(child);
                }
            }

            beginnerGuide.customObjects.Clear();
            beginnerGuide.customObjects.AddRange(customObject);

            Dictionary<string, string> transformDatas = new Dictionary<string, string>();
            Dictionary<string, string> textDatas = new Dictionary<string, string>();
            Dictionary<string, string> imageDatas = new Dictionary<string, string>();
            Dictionary<string, string> imageSprite = new Dictionary<string, string>();
            Dictionary<string, string> textfont = new Dictionary<string, string>();
            foreach (Transform trans in customObject)
            {
                GuideTransformData data = trans.gameObject.AddComponent<GuideTransformData>();
                data.Saved = true;
                string transData = data.Serialize();
                transformDatas[GetTransformRoute(trans)] = transData;
                Object.DestroyImmediate(data);

                Text text = trans.GetComponent<Text>();
                if (text != null)
                {
                    string textData = JsonUtility.ToJson(text);
                    textDatas[GetTransformRoute(trans)] = textData;
                    if(text.font != null) 
                    {
                        textfont[GetTransformRoute(trans)] = AssetDatabase.GetAssetPath(text.font.GetInstanceID());
                    }
                }

                Image image = trans.GetComponent<Image>();
                if (image != null)
                {
                    string imageData = JsonUtility.ToJson(image);
                    imageDatas[GetTransformRoute(trans)] = imageData;
                    if (image.sprite != null)
                    {
                        imageSprite[GetTransformRoute(trans)] = AssetDatabase.GetAssetPath(image.sprite.GetInstanceID());
                    }
                }
            }

            var guideDataList = guideRoot.GetComponent<UIBeginnerGuideDataList>();
            //int index = guideDataList.guideDataList.IndexOf(guideData);
            var index = guideDataList.guideDataList.Where(data => data.guideID == guideData.guideID).ToList().First();

            var so = new SerializedObject(guideDataList);
            //SerializedProperty guideDataListSp = so.FindProperty("guideDataList");
            //SerializedProperty guideDataSp = guideDataListSp.GetArrayElementAtIndex(index);

            index.CustomTransformDatas = JsonUtilityEx.ToJson(transformDatas);
            index.CustomTextDatas = JsonUtilityEx.ToJson(textDatas);
            index.CustomImageDatas = JsonUtilityEx.ToJson(imageDatas);
            index.CustomImagesprite = JsonUtilityEx.ToJson(imageSprite);
            index.CustomTextFont = JsonUtilityEx.ToJson(textfont);

            so.ApplyModifiedProperties();
        }

        private void FindNewObject()
        {
            Transform[] allChild = _gameObjectForDiff.GetComponentsInChildren<Transform>(true);
            List<int> list = new List<int>();
            List<Transform> transforms = new List<Transform>();

            GuideWidgetBase[] idsList = _gameObjectForDiff.GetComponentsInChildren<GuideWidgetBase>(true);
            foreach (Transform item in allChildsForDiff)
            {
                list.Add(item.GetInstanceID());
            }
            foreach (var ids in idsList)
            {
                list = list.Union(ids.GetControlledInstanceIds()).ToList<int>();
            }
            // list.Add(_gameObjectForDiff.transform.GetInstanceID());

            guideData.GuideSelfDefinedData.Clear();
            foreach (Transform child in allChild)
            {
                if (!list.Contains(child.GetInstanceID()))
                {
                    child.gameObject.AddComponent<GuideSelfDefinedData>();
                    transforms.Add(child);
                    string str = "";
                    var item = child;
                    while (item.parent.name != _gameObjectForDiff.name)
                    {
                        str = "/" + item.parent.name + str;
                        item = item.parent;
                    }

                    GuideSelfDefinedData guideSelfDefinedData = child.gameObject.GetComponent<GuideSelfDefinedData>();
                    guideSelfDefinedData.parentPath = str;
                    guideSelfDefinedData.name = child.name;
                    guideSelfDefinedData.active = child.gameObject.activeInHierarchy;

                    guideData.GuideSelfDefinedData.Add(guideSelfDefinedData.Serialize());
                }
            }
        }
        private void Preview()
        {
            UIBeginnerGuidePreview.PreviewGuide(guideRoot, guideData.guideID);
        }
    }
}

#endif