#if UNITY_EDITOR 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public abstract class DecoratorEditor : Editor
{
    private static readonly object[] EMPTY_ARRAY = new object[0];

    private System.Type decoratedEditorType;

    private System.Type editedObjectType;

    private Editor editorInstance;

    private static Dictionary<string, MethodInfo> decoratedMethods = new Dictionary<string, MethodInfo>();

    private static Assembly editorAssembly = Assembly.GetAssembly(typeof(Editor));

    protected Editor EditorInstance
    {
        get
        {
            if (editorInstance == null && targets != null && targets.Length > 0)
            {
                editorInstance = Editor.CreateEditor(targets, decoratedEditorType);
            }

            //检查创建是否成功
            if (editorInstance == null)
            {
                Debug.LogError("Could not create editor !");
            }

            return editorInstance;
        }
    }

    public DecoratorEditor(string editorTypeName)
    {
        decoratedEditorType = editorAssembly.GetTypes().Where(t => t.Name == editorTypeName).FirstOrDefault();

        Init();

        //确认 CustomEditor 的类型
        var originalEditedType = GetCustomEditorType(decoratedEditorType);

        if (originalEditedType != editedObjectType)
        {
            throw new System.ArgumentException(
                string.Format("Type {0} does not match the editor {1} type {2}",
                editedObjectType, editorTypeName, originalEditedType));
        }
    }

    private void OnEnable()
    {
        editorInstance = CreateEditor(targets, decoratedEditorType);
    }

    private System.Type GetCustomEditorType(System.Type type)
    {
        var flags = BindingFlags.NonPublic | BindingFlags.Instance;

        var attributes = type.GetCustomAttributes(typeof(CustomEditor), true) as CustomEditor[];
        var field = attributes.Select(editor => editor.GetType().GetField("m_InspectedType", flags)).FirstOrDefault();

        return field.GetValue(attributes[0]) as System.Type;
    }

    private void Init()
    {
        var flags = BindingFlags.NonPublic | BindingFlags.Instance;

        var attributes = this.GetType().GetCustomAttributes(typeof(CustomEditor), true) as CustomEditor[];
        var field = attributes.Select(editor => editor.GetType().GetField("m_InspectedType", flags)).FirstOrDefault();

        editedObjectType = field.GetValue(attributes[0]) as System.Type;
    }

    private void OnDisable()
    {
        if (editorInstance != null)
        {
            DestroyImmediate(editorInstance);
        }
    }

    /// <summary>
    /// 调用decorated editor instance的指定方法
    /// </summary>
    /// <param name="methodName">要调用的方法的名字</param>
    protected void CallInspectorMethod(string methodName)
    {
        MethodInfo method = null;

        if (!decoratedMethods.ContainsKey(methodName))
        {
            var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

            method = decoratedEditorType.GetMethod(methodName, flags);

            if (method != null)
            {
                decoratedMethods[methodName] = method;
            }
            else
            {
                Debug.LogError(string.Format("Could not find method {0}", methodName));
            }
        }
        else
        {
            method = decoratedMethods[methodName];
        }

        if (method != null)
        {
            method.Invoke(EditorInstance, EMPTY_ARRAY);
        }
    }

    public virtual void OnSceneGUI()
    {
        CallInspectorMethod("OnSceneGUI");
    }

    protected override void OnHeaderGUI()
    {
        CallInspectorMethod("OnHeaderGUI");
    }

    public override void OnInspectorGUI()
    {
        EditorInstance.OnInspectorGUI();
    }

    public override void DrawPreview(Rect previewArea)
    {
        EditorInstance.DrawPreview(previewArea);
    }

    public override string GetInfoString()
    {
        return EditorInstance.GetInfoString();
    }

    public override GUIContent GetPreviewTitle()
    {
        return EditorInstance.GetPreviewTitle();
    }

    public override bool HasPreviewGUI()
    {
        return EditorInstance.HasPreviewGUI();
    }

    public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
    {
        EditorInstance.OnInteractivePreviewGUI(r, background);
    }

    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        EditorInstance.OnPreviewGUI(r, background);
    }

    public override void OnPreviewSettings()
    {
        EditorInstance.OnPreviewSettings();
    }

    public override void ReloadPreviewInstances()
    {
        EditorInstance.ReloadPreviewInstances();
    }

    public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
    {
        return EditorInstance.RenderStaticPreview(assetPath, subAssets, width, height);
    }

    public override bool RequiresConstantRepaint()
    {
        return EditorInstance.RequiresConstantRepaint();
    }

    public override bool UseDefaultMargins()
    {
        return EditorInstance.UseDefaultMargins();
    }
}
#endif