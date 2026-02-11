#if UNITY_EDITOR
using System;
using Game;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace ET
{
    public partial class ADynamicUIWidgetLoader
    {
        [ShowInInspector, NonSerialized]
        [OnValueChanged(nameof(OnUIWidgetAssetChanged))]
        private AUIWidget m_UIWidgetAsset;

        [HideInInspector, SerializeField]
        private string m_UIWidgetAssetGUID;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(m_UIWidgetAssetGUID))
            {
                m_UIWidgetAsset = null;
                m_UIWidgetAssetPath = string.Empty;
            }
            else
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(m_UIWidgetAssetGUID);
                if (string.IsNullOrEmpty(assetPath))
                {
                    m_UIWidgetAsset = null;
                    m_UIWidgetAssetPath = string.Empty;
                }
                else
                {
                    GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                    if (go == null)
                    {
                        m_UIWidgetAsset = null;
                        m_UIWidgetAssetPath = string.Empty;
                    }
                    else
                    {
                        m_UIWidgetAsset = go.GetComponent<AUIWidget>();
                        if (m_UIWidgetAsset == null)
                        {
                            m_UIWidgetAssetPath = string.Empty;
                        }
                        else
                        {
                            m_UIWidgetAssetPath = assetPath;
                        }
                    }
                }
            }

            ShowUIWidget();
        }

        private void OnUIWidgetAssetChanged()
        {
            if (m_UIWidgetAsset == null)
            {
                m_UIWidgetAssetGUID = string.Empty;
                m_UIWidgetAssetPath = string.Empty;
            }
            else
            {
                string assetPath = AssetDatabase.GetAssetPath(m_UIWidgetAsset);
                m_UIWidgetAssetPath = assetPath;
                string guid = AssetDatabase.AssetPathToGUID(assetPath);
                m_UIWidgetAssetGUID = guid;

                RectTransform rectTransform = GetComponent<RectTransform>();
                RectTransform uiWidgetRectTransform = m_UIWidgetAsset.GetComponent<RectTransform>();
                rectTransform.localRotation = uiWidgetRectTransform.localRotation;
                rectTransform.localPosition = uiWidgetRectTransform.localPosition;
                rectTransform.localScale = uiWidgetRectTransform.localScale;
                rectTransform.anchorMin = uiWidgetRectTransform.anchorMin;
                rectTransform.anchorMax = uiWidgetRectTransform.anchorMax;
                rectTransform.pivot = uiWidgetRectTransform.pivot;
                rectTransform.anchoredPosition3D = uiWidgetRectTransform.anchoredPosition3D;
                rectTransform.sizeDelta = uiWidgetRectTransform.sizeDelta;
            }

            ShowUIWidget();
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
            if (m_UIWidgetAsset == null)
            {
                if (auiWidget != null)
                {
                    DestroyImmediate(auiWidget.gameObject);
                }
                return;
            }

            if (auiWidget != null)
            {
                if (auiWidget.GetType() == m_UIWidgetAsset.GetType())
                    return;

                if (auiWidget.GetType() != m_UIWidgetAsset.GetType())
                {
                    DestroyImmediate(auiWidget.gameObject);
                }
            }

            GameObject uiWidgetInstance = Instantiate(m_UIWidgetAsset.gameObject, transform);
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