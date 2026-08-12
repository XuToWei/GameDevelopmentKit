using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace Game.Editor
{
    public abstract class ExLoopScrollRectInspectorBase : LoopScrollRectInspector
    {
        private const string ItemTemplatePropertyPath = "m_ItemTemplate";
        private const string NumItemsPropertyPath = "numItems";

        private PropertyTree m_PropertyTree;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_PropertyTree = PropertyTree.Create(serializedObject);
        }

        protected override void OnDisable()
        {
            m_PropertyTree = null;
            base.OnDisable();
        }

        public override void OnInspectorGUI()
        {
            bool guiEnabled = GUI.enabled;
            base.OnInspectorGUI();
            GUI.enabled = guiEnabled;

            serializedObject.UpdateIfRequiredOrScript();
            m_PropertyTree.UpdateTree();

            m_PropertyTree.BeginDraw(true);
            m_PropertyTree.GetPropertyAtPath(ItemTemplatePropertyPath).Draw();
            m_PropertyTree.GetPropertyAtPath(NumItemsPropertyPath).Draw();
            m_PropertyTree.EndDraw();
        }
    }

    [CustomEditor(typeof(ExLoopHorizontalScrollRect))]
    [CanEditMultipleObjects]
    public sealed class ExLoopHorizontalScrollRectInspector : ExLoopScrollRectInspectorBase
    {
    }

    [CustomEditor(typeof(ExLoopVerticalScrollRect))]
    [CanEditMultipleObjects]
    public sealed class ExLoopVerticalScrollRectInspector : ExLoopScrollRectInspectorBase
    {
    }
}
