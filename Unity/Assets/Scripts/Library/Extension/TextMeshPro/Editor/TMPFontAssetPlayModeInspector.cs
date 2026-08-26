using TMPro;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;

namespace TMPro.Extension.Editor
{
    [CustomEditor(typeof(TMP_FontAsset))]
    [CanEditMultipleObjects]
    internal sealed class TMPFontAssetPlayModeInspector : TMP_FontAssetEditor
    {
        private bool m_StockInspectorInitialized;

        public new void OnEnable()
        {
            EnsureStockInspectorInitialized();
        }

        public new void OnDisable()
        {
            if (!m_StockInspectorInitialized)
            {
                return;
            }

            if (!TMPFontAssetPlayModeLock.IsActive)
            {
                base.OnDisable();
            }

            m_StockInspectorInitialized = false;
        }

        public override void OnInspectorGUI()
        {
            if (TMPFontAssetPlayModeLock.IsActive)
            {
                EditorGUILayout.HelpBox(
                    "TMP Font Asset 在运行模式及模式切换期间不可修改，请退出运行模式后再编辑和保存字体资源。",
                    MessageType.Warning);
                return;
            }

            if (targets.Length != 1)
            {
                EditorGUILayout.HelpBox("TMP Font Asset 不支持批量编辑，请单独选择一个字体资源。", MessageType.Info);
                return;
            }

            EnsureStockInspectorInitialized();
            base.OnInspectorGUI();
        }

        public override bool HasPreviewGUI()
        {
            return !TMPFontAssetPlayModeLock.IsActive && targets.Length == 1 && m_StockInspectorInitialized && base.HasPreviewGUI();
        }

        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
            if (TMPFontAssetPlayModeLock.IsActive || targets.Length != 1 || !m_StockInspectorInitialized)
            {
                return;
            }

            base.OnPreviewGUI(rect, background);
        }

        private void EnsureStockInspectorInitialized()
        {
            if (m_StockInspectorInitialized || TMPFontAssetPlayModeLock.IsActive || targets.Length != 1)
            {
                return;
            }

            base.OnEnable();
            m_StockInspectorInitialized = true;
        }
    }
}
