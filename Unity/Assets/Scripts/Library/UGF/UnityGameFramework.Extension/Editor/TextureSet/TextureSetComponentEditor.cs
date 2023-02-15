#if !ODIN_INSPECTOR


using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityGameFramework.Extension.Editor
{
    [CustomEditor(typeof(TextureSetComponent))]
    public class TextureSetComponentEditor : UnityEditor.Editor
    {
        private SerializedProperty m_FileSystemMaxFileLength;
        private SerializedProperty m_InitBufferLength;
        private SerializedProperty m_AutoReleaseInterval;
        private SerializedProperty m_CheckCanReleaseInterval;
        private bool m_IsShowFileSystemSettings;
        private TextureSetComponent Target => target as TextureSetComponent;
        private List<TextureSetComponent.LoadTextureObject> m_List;
        private List<TextureSetComponent.LoadTextureObject> m_TempList;
        private bool m_SelectList;
        private int m_Page = 1;
        private int m_PageCount = 10;

        private void OnEnable()
        {
            m_Page = 1;
            m_FileSystemMaxFileLength = serializedObject.FindProperty("m_FileSystemMaxFileLength");
            m_InitBufferLength = serializedObject.FindProperty("m_InitBufferLength");
            m_CheckCanReleaseInterval = serializedObject.FindProperty("m_CheckCanReleaseInterval");
            m_AutoReleaseInterval = serializedObject.FindProperty("m_AutoReleaseInterval");
            m_TempList = new List<TextureSetComponent.LoadTextureObject>();
            m_List = new List<TextureSetComponent.LoadTextureObject>();
        }

        private void RefreshList()
        {
            m_TempList.Clear();

            int startIndex = m_PageCount * (m_Page - 1);
            int endIndex = m_PageCount * m_Page;

            m_List = Target.LoadTextureObjectsLinkedList?.ToList() ?? m_List;
            if (m_List != null)
            {
                for (int i = startIndex; i < endIndex; ++i)
                {
                    if (i < m_List.Count)
                    {
                        m_TempList.Add(m_List[i]);
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_CheckCanReleaseInterval);
            EditorGUILayout.PropertyField(m_AutoReleaseInterval);
            m_IsShowFileSystemSettings = EditorGUILayout.Foldout(m_IsShowFileSystemSettings, "FileSystem Settings");
            if (m_IsShowFileSystemSettings)
            {
                Rect rect = EditorGUILayout.GetControlRect();
                rect.x += 10f;
                rect.width -= 10f;
                EditorGUI.PropertyField(rect, m_FileSystemMaxFileLength);
                rect = EditorGUILayout.GetControlRect();
                rect.x += 10f;
                rect.width -= 10f;
                EditorGUI.PropertyField(rect, m_InitBufferLength);
            }

            DrawLoadSpriteObjectsLinkedList();
            if (GUILayout.Button("Release Unused"))
            {
                Target.ReleaseUnused();
            }

            serializedObject.ApplyModifiedProperties();
        }


        private void DrawLoadSpriteObjectsLinkedList()
        {
            RefreshList();
            Rect rect = EditorGUILayout.GetControlRect();
            m_SelectList = EditorGUI.Foldout(rect, m_SelectList, "LoadSpriteObjectsLinkedList", true);
            if (m_SelectList)
            {
                if (m_TempList.Count != 0)
                {
                    rect.x += 12;
                    for (int i = 0; i < m_TempList.Count; i++)
                    {
                        var loadTextureObject = m_TempList[i];
                        rect.y += EditorGUIUtility.singleLineHeight;
                        loadTextureObject.IsSelect =
                            EditorGUI.Foldout(rect, loadTextureObject.IsSelect, "Element", true);
                        if (loadTextureObject.IsSelect)
                        {
                            rect.y += EditorGUIUtility.singleLineHeight;
                            rect = loadTextureObject.Texture2dObject.DrawSetTextureObject(rect);
                            rect.y += EditorGUIUtility.singleLineHeight;
                        }
                    }

                    rect.y += EditorGUIUtility.singleLineHeight * 2;
                    var lastRect = new Rect(rect.width / 2 - 70, rect.y, 50, EditorGUIUtility.singleLineHeight);
                    if (GUI.Button(lastRect, "上一页"))
                    {
                        m_Page = m_Page > 1 ? m_Page - 1 : m_Page;
                    }

                    lastRect.x += 52;
                    lastRect.width = 36;
                    m_Page = EditorGUI.IntField(lastRect, m_Page, new GUIStyle("textField")
                    {
                        alignment = TextAnchor.MiddleCenter
                    });
                    int allPage = m_List.Count / m_PageCount + ((m_List.Count % m_PageCount) > 0 ? 1 : 0);
                    if (m_Page < 1 || m_Page > allPage)
                    {
                        m_Page = m_Page > allPage ? allPage : m_Page;
                        m_Page = m_Page < 1 ? 1 : m_Page;
                    }

                    var nextRect = new Rect(rect.width / 2 + 20, rect.y, 50, EditorGUIUtility.singleLineHeight);
                    if (GUI.Button(nextRect, "下一页"))
                    {
                        m_Page = m_Page < allPage ? m_Page + 1 : m_Page;
                    }
                }
                else
                {
                    rect.y += EditorGUIUtility.singleLineHeight;
                    EditorGUI.LabelField(rect, "List is Empty");
                }
            }

            EditorGUILayout.GetControlRect(GUILayout.Height(rect.y));
        }
    }
}
#endif