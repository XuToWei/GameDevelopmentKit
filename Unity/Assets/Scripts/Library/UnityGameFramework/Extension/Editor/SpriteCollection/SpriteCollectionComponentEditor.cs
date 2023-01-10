using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityGameFramework.Extension
{
#if !ODIN_INSPECTOR
    [CustomEditor(typeof(SpriteCollectionComponent))]
    public class SpriteCollectionComponentEditor : UnityEditor.Editor
    {
        private SerializedProperty m_AutoReleaseInterval;
        private SpriteCollectionComponent Target => target as SpriteCollectionComponent;
        private List<SpriteCollectionComponent.LoadSpriteObject> m_List;
        private List<SpriteCollectionComponent.LoadSpriteObject> m_TempList;
        private bool m_SelectList;
        private int m_Page = 1;
        private int m_PageCount = 10;

        private void OnEnable()
        {
            m_Page = 1;
            m_AutoReleaseInterval = serializedObject.FindProperty("m_AutoReleaseInterval");
            m_TempList = new List<SpriteCollectionComponent.LoadSpriteObject>();
            m_List = new List<SpriteCollectionComponent.LoadSpriteObject>();
        }

        private void RefreshList()
        {
            m_TempList.Clear();

            int startIndex = m_PageCount * (m_Page - 1);
            int endIndex = m_PageCount * m_Page;

            m_List = Target.LoadSpriteObjectsLinkedList?.ToList() ?? m_List;
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
            EditorGUILayout.PropertyField(m_AutoReleaseInterval);
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
            // m_ReorderableList.DoLayoutList();
            // EditorGUILayout.LabelField("LoadSpriteObjectsLinkedList");
            Rect rect = EditorGUILayout.GetControlRect();
            m_SelectList = EditorGUI.Foldout(rect,m_SelectList, "LoadSpriteObjectsLinkedList", true);
            if (m_SelectList)
            {
                if (m_TempList.Count != 0)
                {
                    rect.x += 12;
                    for (int i = 0; i < m_TempList.Count; i++)
                    {
                        var loadSpriteObject = m_TempList[i];
                        rect.y += EditorGUIUtility.singleLineHeight;
                        loadSpriteObject.IsSelect = EditorGUI.Foldout(rect,loadSpriteObject.IsSelect, "Element", true);
                        if (loadSpriteObject.IsSelect)
                        {
                            rect.y += EditorGUIUtility.singleLineHeight;
                            rect = loadSpriteObject.SpriteObject.DrawSetSpriteObject(rect);
                            rect.y += EditorGUIUtility.singleLineHeight;
                            EditorGUI.ObjectField(rect,"Collection", loadSpriteObject.Collection,
                                typeof(SpriteCollection), false);
                        }
                    }
                    rect.y += EditorGUIUtility.singleLineHeight*2;
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
                    EditorGUI.LabelField(rect,"List is Empty");
                }
            }

            EditorGUILayout.GetControlRect(GUILayout.Height(rect.y));
        }
    }
#endif
}