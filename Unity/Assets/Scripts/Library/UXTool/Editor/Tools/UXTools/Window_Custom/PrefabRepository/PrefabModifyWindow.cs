#if UNITY_EDITOR
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace ThunderFireUITool
{
    //弹窗和PrefabCreateWindow样式相同,只是不允许修改路径
    public class PrefabModifyWindow : PrefabCreateWindow
    {
        private static PrefabModifyWindow m_window;

        private Action actionOnDisable = null;

        public static void OpenWindow(GameObject obj, Action action = null)
        {
            int width = 400;
            int height = 300;

            m_window = GetWindow<PrefabModifyWindow>();
            m_window.minSize = new Vector2(width, height);
            m_window.titleContent.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_修改信息);
            m_window.titleContent.image = ToolUtils.GetIcon("component_w");

            m_window.actionOnDisable = action;
            m_window.InitObjectData(obj, AssetDatabase.GetAssetPath(obj));
            m_window.InitWindowData();
            m_window.InitWindowUI();
        }

        protected override void CloseWindow()
        {
            if (m_window != null)
            {
                m_window.Close();
            }

        }

        private void OnDisable()
        {
            actionOnDisable?.Invoke();
        }

        protected override void InitWindowData()
        {
            labelNames = new List<string>();
            labelNames.Add(WidgetRepositoryConfig.NoneLabelText);
            labelNames.AddRange(JsonAssetManager.GetAssets<WidgetLabelsSettings>().labelList);
            labelNames.Add(WidgetRepositoryConfig.AddNewLabelText);


            string[] objLabels = AssetDatabase.GetLabels(selectPrefabObj);
            foreach (string label in objLabels)
            {
                if (labelNames.Contains(label))
                {
                    originalLabel = label;
                }
                else if (label == WidgetRepositoryConfig.UnpackText)
                {
                    originalPack = WidgetRepositoryConfig.UnpackText;
                }
                else if (label == WidgetRepositoryConfig.PackText)
                {
                    originalPack = WidgetRepositoryConfig.PackText;
                }
            }
            if (string.IsNullOrEmpty(originalLabel))
            {
                originalLabel = "All";
            }

            inputStyle = new GUIStyle();
            inputStyle.normal.textColor = Color.black;
            inputStyle.fontSize = 14;
        }

        protected override void InitWindowUI()
        {
            base.InitWindowUI();

            widgetNameInput.SetEnabled(true);
            pathSelectButton.SetEnabled(false);
            pathInput.SetEnabled(false);
        }
        protected override void Confirm()
        {
            if (currentName.Equals(originalName) && LabelDropDown.value.Equals(originalLabel) && PackDropDown.value.Equals(originalPack))
            {
                m_window.Close();
                return;
            }

            if (EditorUtility.DisplayDialog("messageBox", EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_是否要保存修改), EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定), EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消)))
            {
                AssetDatabase.ClearLabels(selectPrefabObj);
                AssetDatabase.SetLabels(selectPrefabObj, new string[] { PackDropDown.value, LabelDropDown.value });
                string path = AssetDatabase.GetAssetPath(selectPrefabObj);
                string message = AssetDatabase.RenameAsset(path, currentName);
                if (string.IsNullOrEmpty(message))
                {
                    //Rename Success
                    if (WidgetRepositoryWindow.GetInstance() != null)
                    {
                        WidgetRepositoryWindow.GetInstance().RefreshWindow();
                    }
                    if (PrefabRecentWindow.GetInstance() != null)
                    {
                        PrefabRecentWindow.GetInstance().RefreshWindow();
                    }
                    PrefabTabs.RefreshTabs(false, 0);
                    m_window.Close();
                }
                else
                {
                    //Rename Failed
                    EditorUtility.DisplayDialog("messageBox", message, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定));
                }
            }
        }
    }
}
#endif