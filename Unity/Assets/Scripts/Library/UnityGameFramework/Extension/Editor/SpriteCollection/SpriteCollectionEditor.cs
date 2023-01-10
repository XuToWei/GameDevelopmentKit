using System;
using System.IO;
using UnityEditor;
using UnityEditor.U2D;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

namespace UnityGameFramework.Extension
{
#if !ODIN_INSPECTOR
    [CustomEditor(typeof(SpriteCollection))]
    public class SpriteCollectionEditor : UnityEditor.Editor
    {
        private SpriteCollection Target => target as SpriteCollection;
        private SerializedProperty m_Sprites;
        private SerializedProperty m_Objects;
        private SerializedProperty m_AtlasFolder;
        private ReorderableList m_ReorderableList;
        private readonly int m_SelectorHash = "ObjectSelector".GetHashCode();
        private bool m_PackableListExpanded = true;
        private bool m_AtlasExpanded = true;
        private string m_NormalAtlasFolder = "Assets/Res/SpriteAtlas";

        private void OnEnable()
        {
            m_Sprites = serializedObject.FindProperty("m_Sprites");
            m_Objects = serializedObject.FindProperty("m_Objects");
            m_AtlasFolder = serializedObject.FindProperty("m_AtlasFolder");
            m_AtlasFolder.stringValue = string.IsNullOrEmpty(m_AtlasFolder.stringValue)
                ? m_NormalAtlasFolder
                : m_AtlasFolder.stringValue;
            serializedObject.ApplyModifiedProperties();
            m_ReorderableList = new ReorderableList(serializedObject, m_Objects, true, true, true, true)
            {
                onAddCallback = AddPackable,
                onRemoveCallback = RemovePackable,
                drawElementCallback = DrawPackableElement,
                elementHeight = EditorGUIUtility.singleLineHeight,
                headerHeight = 0f,
            };
        }

        void AddPackable(ReorderableList list)
        {
            EditorGUIUtility.ShowObjectPicker<Object>(null, false, "t:sprite t:texture2d t:folder", m_SelectorHash);
        }

        void RemovePackable(ReorderableList list)
        {
            var index = list.index;
            if (index != -1)
            {
                m_Objects.GetArrayElementAtIndex(index).objectReferenceValue = null;
                m_Objects.DeleteArrayElementAtIndex(index);
                m_ReorderableList.index = m_Objects.arraySize - 1;
                serializedObject.ApplyModifiedProperties();
                Target.Pack();
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            GUI.enabled = false;
            EditorGUILayout.PropertyField(m_Sprites);
            GUI.enabled = true;

            HandlePackableListUI();
            DrawPackUI();
            DrawCreateAtlasUI();
            serializedObject.ApplyModifiedProperties();
        }

        private void HandlePackableListUI()
        {
            var currentEvent = Event.current;
            var usedEvent = false;
            Rect rect = EditorGUILayout.GetControlRect();
            var controlID = GUIUtility.GetControlID(FocusType.Passive);
            switch (currentEvent.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (rect.Contains(currentEvent.mousePosition) && GUI.enabled)
                    {
                        // Check each single object, so we can add multiple objects in a single drag.
                        var didAcceptDrag = false;
                        var references = DragAndDrop.objectReferences;
                        foreach (var obj in references)
                        {
                            if (IsPackable(obj))
                            {
                                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                                if (currentEvent.type == EventType.DragPerform)
                                {
                                    AddObject(obj);
                                    didAcceptDrag = true;
                                    DragAndDrop.activeControlID = 0;
                                }
                                else
                                    DragAndDrop.activeControlID = controlID;
                            }
                        }

                        if (didAcceptDrag)
                        {
                            GUI.changed = true;
                            DragAndDrop.AcceptDrag();
                            usedEvent = true;
                        }
                    }

                    break;
                case EventType.ExecuteCommand:
                    if (currentEvent.commandName == "ObjectSelectorClosed" &&
                        EditorGUIUtility.GetObjectPickerControlID() == m_SelectorHash)
                    {
                        var obj = EditorGUIUtility.GetObjectPickerObject();
                        if (IsPackable(obj))
                        {
                            AddObject(obj);
                        }
                    }

                    usedEvent = true;
                    break;
            }

            if (usedEvent)
                currentEvent.Use();
            m_PackableListExpanded = EditorGUI.Foldout(rect, m_PackableListExpanded,
                EditorGUIUtility.TrTextContent("Objects for Packing",
                    "Only accept Folder, Sprite Sheet(Texture) and Sprite."), true);

            if (m_PackableListExpanded)
            {
                EditorGUI.indentLevel++;
                m_ReorderableList.DoLayoutList();
                EditorGUI.indentLevel--;
            }
        }

        private void AddObject(Object obj)
        {
            m_Objects.InsertArrayElementAtIndex(m_Objects.arraySize);
            m_Objects.GetArrayElementAtIndex(m_Objects.arraySize - 1).objectReferenceValue = obj;
            m_ReorderableList.index = m_Objects.arraySize - 1;
            serializedObject.ApplyModifiedProperties();
            Target.Pack();
        }

        void DrawPackableElement(Rect rect, int index, bool selected, bool focused)
        {
            var property = m_Objects.GetArrayElementAtIndex(index);
            var controlID = GUIUtility.GetControlID(FocusType.Passive, rect);
            EditorGUI.BeginChangeCheck();
            var changedObject = EditorGUI.ObjectField(rect, property.objectReferenceValue, typeof(Object), false);
            if (EditorGUI.EndChangeCheck())
            {
                property.objectReferenceValue = changedObject;
            }

            if (GUIUtility.keyboardControl == controlID && !selected)
                m_ReorderableList.index = index;
        }

        static bool IsPackable(Object o)
        {
            return o != null && (o is Sprite ||
                                 o is Texture2D ||
                                 o is DefaultAsset && ProjectWindowUtil.IsFolder(o.GetInstanceID()) ||
                                 o is SpriteAtlas);
        }

        private void DrawPackUI()
        {
            if (GUILayout.Button("Package", GUILayout.ExpandWidth(false)))
            {
                Target.Pack();
            }
        }

        private void DrawCreateAtlasUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            m_AtlasExpanded = EditorGUILayout.Foldout(m_AtlasExpanded,"Create Atlas", true);

            if (!m_AtlasExpanded)
            {
                return;
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(m_AtlasFolder.stringValue);
            if (GUILayout.Button("Select"))
            {
                string path = EditorUtility.SaveFolderPanel("", Application.dataPath, $"{target.name}.spriteatlas");
                if (!string.IsNullOrEmpty(path))
                {
                    int index = path.IndexOf("Assets/", StringComparison.Ordinal);
                    if (index==-1)
                    {
                        m_AtlasFolder.stringValue = m_NormalAtlasFolder;
                        EditorUtility.DisplayDialog("提示", $"图集生成文件夹必须在Assets目录下", "确定");
                        return;
                    }

                    m_AtlasFolder.stringValue = path.Substring(index);
                }
            }

            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Create Atlas"))
            {
                CreateAtlas();
            }
        }

        private void CreateAtlas()
        {
            if (string.IsNullOrEmpty(m_AtlasFolder.stringValue))
            {
                EditorUtility.DisplayDialog("提示", $"请先选择图集生成文件夹！", "确定");
                return;
            }

            if (Target.Objects.Find(_ => _ is SpriteAtlas) != null)
            {
                EditorUtility.DisplayDialog("提示", $"SpriteCollection 中存在Atlas 请检查!", "确定");
                return;
            }
            
            //创建图集
            string atlas = m_AtlasFolder.stringValue + "/" + target.name + ".spriteatlas";

            if (File.Exists(atlas))
            {
                bool result = EditorUtility.DisplayDialog("提示", $"存在同名图集,是否覆盖？", "确定", "取消");
                if (!result)
                {
                    return;
                }
            }
            SpriteAtlas sa = new SpriteAtlas();

            SpriteAtlasPackingSettings packSet = new SpriteAtlasPackingSettings()
            {
                blockOffset = 1,
                enableRotation = false,
                enableTightPacking = false,
                padding = 8,
            };
            sa.SetPackingSettings(packSet);


            SpriteAtlasTextureSettings textureSet = new SpriteAtlasTextureSettings()
            {
                readable = false,
                generateMipMaps = false,
                sRGB = true,
                filterMode = FilterMode.Bilinear,
            };
            sa.SetTextureSettings(textureSet);
            AssetDatabase.CreateAsset(sa, atlas);

            sa.Add(Target.Objects.ToArray());

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
#endif
}