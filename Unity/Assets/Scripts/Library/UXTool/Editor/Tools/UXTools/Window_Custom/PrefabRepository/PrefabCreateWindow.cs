#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;
using UnityEngine.SceneManagement;
using Cursor = UnityEngine.Cursor;

namespace ThunderFireUITool
{
    public class PrefabCreateWindow : EditorWindow
    {
        private static PrefabCreateWindow m_window;

        //UI
        protected IMGUIContainer widgetNameInput;

        protected VisualElement LabelDropDownContainer;
        protected PopupField<string> LabelDropDown;
        protected VisualElement PackDropDownContainer;
        protected PopupField<string> PackDropDown;

        protected Button pathSelectButton;
        protected IMGUIContainer pathInput;


        protected GUIStyle inputStyle;

        //Origin Data
        protected string originalName = null;
        protected string originalLabel = WidgetRepositoryConfig.NoneLabelText;
        protected string originalPack = WidgetRepositoryConfig.PackText;

        //window Data
        protected List<string> labelNames;
        protected string currentName = null;
        protected string componentPath;

        protected bool isPrefab;
        //isPrefab == false时 这个值有效
        protected GameObject[] selectObjList;

        //isPrefab == true时 这个值有效
        protected GameObject selectPrefabObj;


        [MenuItem("Assets/设置为组件 (Set As UXWidget)", false, -800)]
        static void SetPrefabAsComponent()
        {
            string[] guids = Selection.assetGUIDs;
            if (guids.Length != 1)
            {
                EditorUtility.DisplayDialog("messageBox",
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_选择多个PrefabTip),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));
                return;
            }

            if (!AssetDatabase.GUIDToAssetPath(guids[0]).EndsWith(".prefab"))
            {
                EditorUtility.DisplayDialog("messageBox",
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_选择文件不是PrefabTip),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));
                return;
            }

            string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            OpenWindowFromPrefab(Selection.gameObjects[0], assetPath);
        }

        //从toolbar打开
        public static void OpenWindow()
        {
            if (Selection.gameObjects.Length == 0)
            {
                string message = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_请先选中一个节点);
                EditorUtility.DisplayDialog("messageBox", message, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定), EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));
            }
            else if (Selection.gameObjects.Length == 1)
            {
                string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(Selection.gameObjects[0]);
                if (string.IsNullOrEmpty(path))
                {
                    //选中对象不是prefab
                    OpenWindowFromObjList(Selection.gameObjects);
                }
                else
                {
                    //选中对象是Prefab
                    OpenWindowFromPrefab(Selection.gameObjects[0], path);
                }
            }
            else
            {
                //选中多个节点看能否组合
                if (!CombineWidgetLogic.CanCombine(Selection.gameObjects))
                {
                    EditorUtility.DisplayDialog("messageBox",
                        EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_Prefab创建失败Tip),
                        EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                        EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));
                    return;
                }
                else
                {
                    OpenWindowFromObjList(Selection.gameObjects);
                }
            }
        }

        //从hierarchy中选中节点创建prefab并设置组件
        public static void OpenWindowFromObjList(GameObject[] objList)
        {
            int width = 400;
            int height = 300;
            m_window = GetWindow<PrefabCreateWindow>();
            m_window.minSize = new Vector2(width, height);
            m_window.titleContent.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_新建组件);

            m_window.InitObjectData(objList);
            m_window.InitWindowData();
            m_window.InitWindowUI();
        }

        //现有prefab设置为组件
        public static void OpenWindowFromPrefab(GameObject obj, string path)
        {
            if (CheckIsWidget(path))
            {
                //Hierarchy中的Gameobject和由Asset中加载出来的GameObject有所不同;
                //TODO 参数改成直接传path
                PrefabModifyWindow.OpenWindow(AssetDatabase.LoadAssetAtPath<GameObject>(path));
                return;
            }

            int width = 400;
            int height = 300;
            m_window = GetWindow<PrefabCreateWindow>();
            m_window.minSize = new Vector2(width, height);
            m_window.titleContent.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_新建组件);

            m_window.InitObjectData(obj, path);
            m_window.InitWindowData();
            m_window.InitWindowUI();
        }

        protected virtual void CloseWindow()
        {
            Close();
        }

        protected void InitObjectData(GameObject obj, string prefabPath)
        {
            if (!string.IsNullOrEmpty(prefabPath))
            {
                selectPrefabObj = obj;
                originalName = obj.name;
                currentName = obj.name;
                componentPath = Path.GetDirectoryName(prefabPath) + "/";
                isPrefab = true;
            }
        }

        protected void InitObjectData(GameObject[] selectObjs)
        {
            selectObjList = selectObjs;
            currentName = "";
            componentPath = ThunderFireUIToolConfig.RootPath;
            isPrefab = false;
        }

        protected virtual void InitWindowData()
        {
            labelNames = new List<string>();
            labelNames.Add(WidgetRepositoryConfig.NoneLabelText);
            labelNames.AddRange(JsonAssetManager.GetAssets<WidgetLabelsSettings>().labelList);
            labelNames.Add(WidgetRepositoryConfig.AddNewLabelText);

            if (WidgetRepositoryWindow.GetInstance() == null)
            {
                originalLabel = WidgetRepositoryConfig.NoneLabelText;
            }
            else
            {
                originalLabel = WidgetRepositoryWindow.GetInstance().filtration;
            }

            inputStyle = new GUIStyle();
            inputStyle.normal.textColor = Color.black;
            inputStyle.fontSize = 14;
        }

        protected virtual void InitWindowUI()
        {
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ThunderFireUIToolConfig.UIBuilderPath + "Constant/prefabModify_popup.uxml");
            VisualElement labelFromUXML = visualTree.CloneTree();

            Label widgetNameLabel = labelFromUXML.Q<Label>("widgetNameLabel");
            widgetNameInput = labelFromUXML.Q<IMGUIContainer>("widgetNameInput");

            Label widgetLabelText = labelFromUXML.Q<Label>("labelText");
            LabelDropDownContainer = labelFromUXML.Q<VisualElement>("labelPopupContainer");

            Label pathLabel = labelFromUXML.Q<Label>("pathLabel");
            pathInput = labelFromUXML.Q<IMGUIContainer>("pathInput");
            pathSelectButton = labelFromUXML.Q<Button>("pathSelectbutton");

            Label widgetPackLabel = labelFromUXML.Q<Label>("packText");
            PackDropDownContainer = labelFromUXML.Q<VisualElement>("packPopupContainer");

            VisualElement confirmBtn = labelFromUXML.Q<VisualElement>("confirm");
            VisualElement cancelBtn = labelFromUXML.Q<VisualElement>("cancel");


            string prefabNameString = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_组件名称) + " :";
            string prefabTypeString = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_组件类型) + " :";
            string prefabPathString = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_组件储存位置) + " :";
            string prefabPackString = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_组件生成模式) + " :";
            string confirmString = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定);
            string cancelString = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消);

            widgetNameLabel.text = prefabNameString;

            widgetNameInput.onGUIHandler += () => { currentName = EditorGUILayout.TextField(currentName, inputStyle); };

            pathLabel.text = prefabPathString;
            pathSelectButton.clicked += SelectComponentPath;

            pathInput.onGUIHandler += OnPathInputGUI;

            if (isPrefab)
            {
                widgetNameInput.SetEnabled(false);
                pathInput.SetEnabled(false);
                pathSelectButton.SetEnabled(false);
            }

            widgetLabelText.text = prefabTypeString;
            RefreshLabelDropdown();

            widgetPackLabel.text = prefabPackString;
            RefreshPackDropdown();

            confirmBtn.Q<Label>("text").text = confirmString;
            cancelBtn.Q<Label>("text").text = cancelString;
            confirmBtn.RegisterCallback((MouseDownEvent e) =>
            {
                Confirm();
            });
            cancelBtn.RegisterCallback((MouseDownEvent e) =>
            {
                CloseWindow();
            });

            //组件的高亮悬浮框处理
            VisualElement nameInput = labelFromUXML.Q<VisualElement>("nameinputSelector");
            SelectorItem nameInputSelector = new SelectorItem(nameInput, widgetNameInput, false);
            LabelDropDown.RegisterCallback((MouseDownEvent e) =>
            {
                nameInputSelector.UnSelected();
            });

            rootVisualElement.RegisterCallback((MouseDownEvent e) =>
            {
                nameInputSelector.UnSelected();
            });

            new SelectorItem(labelFromUXML.Q<VisualElement>("confirmSelector"), confirmBtn, false);
            new SelectorItem(labelFromUXML.Q<VisualElement>("cancelSelector"), cancelBtn);

            rootVisualElement.Add(labelFromUXML);
        }

        protected void RefreshLabelDropdown()
        {
            LabelDropDownContainer.Clear();
            LabelDropDown = new PopupField<string>(labelNames, 0);
            LabelDropDown.value = originalLabel;
            LabelDropDown.RegisterValueChangedCallback(x => OnLabelDropDownChangeValue(x.newValue));
            LabelDropDown.style.position = Position.Absolute;
            LabelDropDown.style.left = 0;
            LabelDropDown.style.right = 0;
            LabelDropDown.style.top = 0;
            LabelDropDown.style.bottom = 0;

            LabelDropDownContainer.Add(LabelDropDown);
        }

        protected void RefreshPackDropdown()
        {
            PackDropDownContainer.Clear();

            List<string> packNames = new List<string>();
            packNames.Add(WidgetRepositoryConfig.PackText);
            packNames.Add(WidgetRepositoryConfig.UnpackText);

            PackDropDown = new PopupField<string>(packNames, 0);

            PackDropDown.style.position = Position.Absolute;
            PackDropDown.style.left = 0;
            PackDropDown.style.right = 0;
            PackDropDown.style.top = 0;
            PackDropDown.style.bottom = 0;

            PackDropDown.value = originalPack;
            PackDropDownContainer.Add(PackDropDown);
        }

        protected virtual void Confirm()
        {
            string path = componentPath;
            string label = LabelDropDown.value;
            bool isPack = PackDropDown.value == WidgetRepositoryConfig.PackText;

            if (currentName == "")
            {
                EditorUtility.DisplayDialog("messageBox",
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_Prefab名称合法Tip),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));
                return;
            }

            if (Directory.Exists(path))
            {
                if (isPrefab)
                {
                    ToolUtils.CreatePrefabAsWidget(selectPrefabObj, currentName, path, isPrefab, label, isPack);
                }
                else
                {
                    if (selectObjList.Length == 1)
                    {
                        ToolUtils.CreatePrefabAsWidget(selectObjList[0], currentName, path, isPrefab, label, isPack);
                    }

                    if (selectObjList.Length > 1)
                    {
                        GameObject root = CombineWidgetLogic.GenCombineRootRect(selectObjList);
                        ToolUtils.CreatePrefabAsWidget(root, currentName, path, isPrefab, label, isPack);
                    }
                }
            }
            else
            {
                EditorUtility.DisplayDialog("messageBox",
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_Prefab存储位置不存在Tip),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));
                return;
            }

            m_window.Close();
        }

        protected void OnLabelDropDownChangeValue(string value)
        {
            if (value == WidgetRepositoryConfig.AddNewLabelText)
            {
                ShowAddLabelWindow();
            }
        }

        protected void ShowAddLabelWindow()
        {
            PrefabAddNewLabelWindow.OpenWindow(OnAddLabelSuccess, OnAddLabelCancel);
        }

        protected void OnAddLabelSuccess(string newLabel)
        {
            InitWindowData();
            RefreshLabelDropdown();
            LabelDropDown.value = newLabel;
        }

        protected void OnAddLabelCancel()
        {
            LabelDropDown.value = labelNames[0];
        }

        private void OnPathInputGUI()
        {
            componentPath = EditorGUILayout.TextField(componentPath, inputStyle);
        }

        protected void SelectComponentPath()
        {
            if (pathInput != null)
            {
                string path = SelectPath();
                if (path != null)
                {
                    componentPath = path;

                    pathInput.onGUIHandler -= OnPathInputGUI;
                    pathInput.onGUIHandler += OnPathInputGUI;
                    //GetWindow<PrefabCreateWindow>(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_新建组件), false);
                    if (SceneView.lastActiveSceneView != null)
                    {
                        SceneView.lastActiveSceneView.Focus();
                    }
                }
            }
        }

        protected string SelectPath()
        {
            string folderPath = Application.dataPath;
            string path = EditorUtility.OpenFolderPanel(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_选择路径), folderPath, "");
            if (path != "")
            {
                int index = path.IndexOf("Assets");
                if (index != -1)
                {
                    PlayerPrefs.SetString("LastParticleCheckPath", path);
                    path = path.Substring(index);
                    return path + "/";
                }
                else
                {
                    EditorUtility.DisplayDialog("messageBox",
                        EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_目录不在Assets下Tip),
                        EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                        EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));
                }
            }
            return null;
        }

        private static bool CheckIsWidget(string path)
        {
            WidgetListSetting list = JsonAssetManager.GetAssets<WidgetListSetting>();
            List<string> guidList = list.List;


            if (guidList.Contains(AssetDatabase.AssetPathToGUID(path)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
#endif