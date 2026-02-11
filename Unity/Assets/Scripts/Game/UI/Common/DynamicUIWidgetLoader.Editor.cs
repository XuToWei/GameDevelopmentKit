#if UNITY_EDITOR
using System;
using Game;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace ET
{
    public partial class DynamicUIWidgetLoader
    {
        [ShowInInspector, NonSerialized]
        [OnValueChanged(nameof(OnUIWidgetChanged))]
        private AUIWidget m_UIWidget;

        [HideInInspector, SerializeField]
        private string m_UIWidgetGUID;

        private void OnValidate()
        {
            string assetPath = string.Empty;
            if (string.IsNullOrEmpty(m_UIWidgetGUID))
            {
                m_UIWidget = null;
            }
            else
            {
                assetPath = AssetDatabase.GUIDToAssetPath(m_UIWidgetGUID);
                if (string.IsNullOrEmpty(assetPath))
                {
                    m_UIWidget = null;
                }
                else
                {
                    GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                    if (go == null)
                    {
                        m_UIWidget = null;
                    }
                    else
                    {
                        m_UIWidget = go.GetComponent<AUIWidget>();
                    }
                }
            }

            bool isDirty = false;
            if (m_UIWidget == null)
            {
                if (m_UIWidgetAssetPath != string.Empty)
                {
                    m_UIWidgetAssetPath = string.Empty;
                    isDirty = true;
                }
            }
            else
            {
                if (m_UIWidgetAssetPath != assetPath)
                {
                    m_UIWidgetAssetPath = assetPath;
                    isDirty = true;
                }
            }

            ShowUIWidget();

            if (isDirty)
            {
                ShowUIWidget();
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }
        }

        private void OnUIWidgetChanged()
        {
            bool isDirty = false;
            if (m_UIWidget == null)
            {
                if (m_UIWidgetGUID != string.Empty)
                {
                    isDirty = true;
                    m_UIWidgetGUID = string.Empty;
                }
                if(m_UIWidgetAssetPath != string.Empty)
                {
                    isDirty = true;
                    m_UIWidgetAssetPath = string.Empty;
                }
            }
            else
            {
                string assetPath = AssetDatabase.GetAssetPath(m_UIWidget);
                if(m_UIWidgetAssetPath != assetPath)
                {
                    isDirty = true;
                    m_UIWidgetAssetPath = assetPath;
                }
                string guid = AssetDatabase.AssetPathToGUID(assetPath);
                if (m_UIWidgetGUID != guid)
                {
                    isDirty = true;
                    m_UIWidgetGUID = guid;
                }
                RectTransform rectTransform = GetComponent<RectTransform>();
                RectTransform uiWidgetRectTransform = m_UIWidget.GetComponent<RectTransform>();
                rectTransform.localRotation = uiWidgetRectTransform.localRotation;
                rectTransform.localPosition = uiWidgetRectTransform.localPosition;
                rectTransform.localScale = uiWidgetRectTransform.localScale;
                rectTransform.anchorMin = uiWidgetRectTransform.anchorMin;
                rectTransform.anchorMax = uiWidgetRectTransform.anchorMax;
                rectTransform.pivot = uiWidgetRectTransform.pivot;
                rectTransform.anchoredPosition3D = uiWidgetRectTransform.anchoredPosition3D;
                rectTransform.sizeDelta = uiWidgetRectTransform.sizeDelta;
            }

            if (isDirty)
            {
                ShowUIWidget();
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }
        }

        private void ShowUIWidget()
        {
            if (PrefabUtility.IsPartOfPrefabAsset(transform))
                return;
            EditorApplication.delayCall += () =>
            {
                InternalShowUIWidget();
                EditorApplication.delayCall -= InternalShowUIWidget;
            };
        }

        private void InternalShowUIWidget()
        {
            if (this == null)
                return;

            AUIWidget auiWidget = GetComponentInChildren<AUIWidget>(true);
            if (m_UIWidget == null)
            {
                if (auiWidget != null)
                {
                    DestroyImmediate(auiWidget.gameObject);
                }
                return;
            }

            if (auiWidget != null)
            {
                if (auiWidget.GetType() == m_UIWidget.GetType())
                    return;

                if (auiWidget.GetType() != m_UIWidget.GetType())
                {
                    DestroyImmediate(auiWidget.gameObject);
                }
            }

            GameObject uiWidgetInstance = Instantiate(m_UIWidget.gameObject, transform);
            RectTransform uiWidgetRectTransform = uiWidgetInstance.GetComponent<RectTransform>();
            uiWidgetInstance.hideFlags = HideFlags.DontSave;
            uiWidgetRectTransform.localRotation = Quaternion.identity;
            uiWidgetRectTransform.localPosition = Vector3.zero;
            uiWidgetRectTransform.localScale = Vector3.one;
            uiWidgetRectTransform.anchorMin = Vector2.zero;
            uiWidgetRectTransform.anchorMax = Vector2.one;
            uiWidgetRectTransform.pivot = new Vector2(0.5f, 0.5f);
            uiWidgetRectTransform.anchoredPosition3D = Vector3.zero;
            uiWidgetRectTransform.sizeDelta = Vector2.zero;
        }
    }
}
#endif