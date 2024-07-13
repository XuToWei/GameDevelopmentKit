#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class PrefabAddNewLabelWindow : EditorWindow
    {
        private static PrefabAddNewLabelWindow m_window;
        private static Action<string> OnAddSuccess;
        private static Action OnCancel;
        private bool AddSuccess = false;
        private string currentName = null;

        public static void OpenWindow(Action<string> OnAddSuccessAction = null, Action OnCancelAction = null)
        {
            InitWindowData();
            int width = 298;
            int height = 180;
            m_window = GetWindow<PrefabAddNewLabelWindow>();
            m_window.minSize = new Vector2(width, height);
            m_window.titleContent.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_组件类型);
            // m_window.position = new Rect((Screen.currentResolution.width - width) / 2, (Screen.currentResolution.height - height) / 2, width, height);
            OnAddSuccess += OnAddSuccessAction;
            OnCancel += OnCancelAction;
        }

        public static string prefabLabelDes;
        public static string OKText;
        public static string CancelText;

        public static void InitWindowData()
        {
            prefabLabelDes = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_添加组件类型) + " :";
            OKText = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定);
            CancelText = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消);
        }

        private void OnEnable()
        {
            currentName = "";
            VisualElement root = rootVisualElement;
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ThunderFireUIToolConfig.UIBuilderPath + "Constant/labelModify_popup.uxml");
            VisualElement labelFromUXML = visualTree.CloneTree();
            Label TextLabel = labelFromUXML.Q<Label>("textlabel");
            IMGUIContainer TextInput = labelFromUXML.Q<IMGUIContainer>("textinput");
            TextInput.style.backgroundColor = Color.white;
            VisualElement Confirm = labelFromUXML.Q<VisualElement>("confirm");
            VisualElement Cancel = labelFromUXML.Q<VisualElement>("cancel");
            var ConfirmSelector = labelFromUXML.Q<VisualElement>("confirmSelector");
            TextLabel.text = prefabLabelDes;
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.black;
            style.fontSize = 15;
            //TextInput.onGUIHandler += () => { currentName = GUI.TextField(new Rect(2.5f, 2.5f, 201, 20), currentName, style); };
            TextInput.onGUIHandler += () =>
            {
                currentName = EditorGUILayout.TextField(currentName, style).Trim();
                if (string.IsNullOrEmpty(currentName))
                {
                    ConfirmSelector.SetEnabled(false);
                    Confirm.SetEnabled(false);
                }
                else
                {
                    ConfirmSelector.SetEnabled(true);
                    Confirm.SetEnabled(true);
                }
            };
            SelectorItem textinputS = new SelectorItem(labelFromUXML.Q<VisualElement>("textinputSelector"), TextInput);
            //EditorUIUtil.CreateUIEButton(Confirm, Submit);
            //EditorUIUtil.CreateUIEButton(Cancel, closeWindow);
            //Confirm.text = OKText;
            //Confirm.clicked += Submit;
            Confirm.Q<Label>("text").text = OKText;
            Cancel.Q<Label>("text").text = CancelText;
            new SelectorItem(labelFromUXML.Q<VisualElement>("cancelSelector"), Cancel, false);
            new SelectorItem(labelFromUXML.Q<VisualElement>("confirmSelector"), Confirm, false);
            Confirm.RegisterCallback((MouseDownEvent e) =>
            {
                Submit();

            });
            Cancel.RegisterCallback((MouseDownEvent e) =>
            {
                closeWindow();

            });
            rootVisualElement.RegisterCallback((MouseDownEvent e) =>
            {
                textinputS.UnSelected();

            });

            root.Add(labelFromUXML);

        }

        private void closeWindow()
        {
            if (m_window != null)
            {
                m_window.Close();
            }

        }

        private void Submit()
        {
            if (currentName != null)
            {
                var labelSetting = JsonAssetManager.GetAssets<WidgetLabelsSettings>();
                var labelList = labelSetting.labelList;
                if (!labelList.Contains(currentName))
                {
                    labelSetting.AddNewLabel(currentName);
                    OnAddSuccess?.Invoke(currentName);
                    AddSuccess = true;
                    m_window.Close();
                }
                else
                {
                    EditorUtility.DisplayDialog("messageBox", EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_组件类型重复), EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定), EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));
                }
            }
            else
            {
                EditorUtility.DisplayDialog("messageBox", EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_请输入组件类型名称), EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定), EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));
            }
        }

        private void OnDisable()
        {
            if (!AddSuccess)
                OnCancel?.Invoke();
        }
    }
}
#endif