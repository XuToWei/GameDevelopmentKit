using TMPro;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;

namespace TMPro.Extension.Editor
{
    [CustomEditor(typeof(TMP_FontAsset))]
    [CanEditMultipleObjects]
    internal sealed class TMPFontAssetInspector : TMP_FontAssetEditor
    {
        private bool m_StockInspectorInitialized;
        private bool m_IsUnlocked;
        private int m_UnlockedFontAssetInstanceId;

        public new void OnEnable()
        {
            LockEditing();
        }

        public new void OnDisable()
        {
            try
            {
                if (m_StockInspectorInitialized)
                {
                    base.OnDisable();
                }
            }
            finally
            {
                m_StockInspectorInitialized = false;
                LockEditing();
            }
        }

        public override void OnInspectorGUI()
        {
            if (targets.Length != 1)
            {
                LockEditing();
                EditorGUILayout.HelpBox("TMP Font Asset 不支持批量编辑，请单独选择一个字体资源。", MessageType.Info);
                return;
            }

            EnsureLockMatchesTarget();
            if (!m_IsUnlocked)
            {
                EditorGUILayout.HelpBox(
                    "TMP Font Asset 当前已锁定，无法修改或保存。切换选择后仍会保持锁定。",
                    MessageType.Warning);

                if (GUILayout.Button("解锁编辑"))
                {
                    UnlockEditing();
                }
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "TMP Font Asset 已解锁，可以修改并保存；切换选择后将自动重新锁定。",
                    MessageType.Info);
            }

            if (!m_IsUnlocked)
            {
                return;
            }

            EnsureStockInspectorInitialized();

            TMP_FontAsset fontAsset = (TMP_FontAsset)target;
            using (new EditorGUI.DisabledScope(!TMPFontAssetDynamicData.IsClearOnBuildEnabled(fontAsset)))
            {
                if (GUILayout.Button(new GUIContent("清理动态字体数据", "勾选 Clear Dynamic Data On Build 后可手动清理；打包 AssetBundle 前也会自动清理。")))
                {
                    TMPFontAssetDynamicData.Clear(fontAsset);
                    serializedObject.Update();
                    Repaint();
                    GUIUtility.ExitGUI();
                }
            }

            EditorGUILayout.Space();
            base.OnInspectorGUI();
        }

        public override bool HasPreviewGUI()
        {
            return m_IsUnlocked && targets.Length == 1 && m_StockInspectorInitialized && base.HasPreviewGUI();
        }

        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
            if (!m_IsUnlocked || targets.Length != 1 || !m_StockInspectorInitialized)
            {
                return;
            }

            base.OnPreviewGUI(rect, background);
        }

        private void EnsureStockInspectorInitialized()
        {
            if (m_StockInspectorInitialized || !m_IsUnlocked || targets.Length != 1)
            {
                return;
            }

            base.OnEnable();
            m_StockInspectorInitialized = true;
        }

        private void EnsureLockMatchesTarget()
        {
            if (!m_IsUnlocked || target.GetInstanceID() == m_UnlockedFontAssetInstanceId)
            {
                return;
            }

            LockEditing();
        }

        private void UnlockEditing()
        {
            TMP_FontAsset fontAsset = (TMP_FontAsset)target;
            TMPFontAssetEditLock.Unlock(GetInstanceID(), fontAsset);
            m_UnlockedFontAssetInstanceId = fontAsset.GetInstanceID();
            m_IsUnlocked = true;
        }

        private void LockEditing()
        {
            TMPFontAssetEditLock.Lock(GetInstanceID());
            m_UnlockedFontAssetInstanceId = 0;
            m_IsUnlocked = false;
        }
    }
}
