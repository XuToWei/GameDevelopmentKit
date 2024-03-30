using System.IO;
using GameFramework;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityGameFramework.Extension
{
    public class MergeAssetToVFSWindow : EditorWindow
    {
        [MenuItem("Game Framework/Merge Asset To VFS", false, 3)]
        private static void ShowWindow()
        {
            var window = GetWindow<MergeAssetToVFSWindow>();
            window.titleContent = new GUIContent("Merge Asset To VFS");
            window.minSize = new Vector2(550, 400);
            window.Show();
        }

        private string m_MergeAssetScriptableObjectSaveFolder = "Assets/Res/Configs/";
        private MergeAssetScriptableObject m_MergeAssetScriptableObject;
        private MergeAssetEditorData m_MergeAssetEditorData;
        private ReorderableList m_ReorderableList;
        private readonly int m_SelectorHash = "ObjectSelector".GetHashCode();
        private bool m_PackableListExpanded = true;
        private string m_TypeTips;

        private void OnEnable()
        {
            m_TypeTips = MergeAssetUtility.GetTypeTips();
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            var config = EditorUtility.InstanceIDToObject(instanceID) as MergeAssetScriptableObject;
            if (config != null)
            {
                MergeAssetToVFSWindow window = GetWindow<MergeAssetToVFSWindow>(true, "Merge Asset To VFS", true);
                window.minSize = new Vector2(550, 400);
                window.m_MergeAssetScriptableObject = config;
                window.Load();
                return true;
            }

            return false; // we did not handle the open
        }

        void OnSelectionChange()
        {
            var config = Selection.activeObject as MergeAssetScriptableObject;
            if (config != null && config != m_MergeAssetScriptableObject)
            {
                m_MergeAssetScriptableObject = config;
                Load();
                GetWindow<MergeAssetToVFSWindow>().Focus();
            }
        }

        private void Load()
        {
            if (m_MergeAssetScriptableObject != null)
            {
                m_MergeAssetEditorData = m_MergeAssetScriptableObject.MergeAssetEditorData;
            }
            else
            {
                m_MergeAssetEditorData = new MergeAssetEditorData();
            }

            m_ReorderableList = null;
        }

        void InitAssetList()
        {
            m_ReorderableList = new ReorderableList(m_MergeAssetEditorData.AssetDataList, typeof(AssetData))
            {
                headerHeight = 0f,
                drawElementCallback = DrawElement,
                displayAdd = false,
            };
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var fileData = m_MergeAssetEditorData.AssetDataList[index];
            float half = rect.width / 2;
            EditorGUI.LabelField(new Rect(rect)
            {
                width = 100
            }, "AssetName:");
            EditorGUI.LabelField(new Rect(rect)
            {
                x = 100,
                width = half - 100,
            }, fileData.AssetName);
            EditorGUI.LabelField(new Rect(rect)
            {
                x = half,
                width = 40,
            }, "Asset:");
            EditorGUI.ObjectField(new Rect(rect) {x = half + 50, width = half - 50}, fileData.Asset, typeof(TextAsset),
                false);
        }

        private bool IsPickable(Object obj)
        {
            return MergeAssetUtility.GetAssetType(obj) != AssetType.None;
        }

        private void AddAssetData(Object obj)
        {
            AssetData assetData = new AssetData();
            assetData.AssetPath = AssetDatabase.GetAssetPath(obj);
            assetData.AssetName = assetData.AssetPath.Substring(assetData.AssetPath.LastIndexOf('/') + 1);
            assetData.Asset = obj;
            assetData.AssetType = MergeAssetUtility.GetAssetType(obj);
            m_ReorderableList.list.Add(assetData);
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
                        foreach (Object obj in references)
                        {
                            if (IsPickable(obj))
                            {
                                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                                if (currentEvent.type == EventType.DragPerform)
                                {
                                    AddAssetData(obj);
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
                        if (IsPickable(obj))
                        {
                            AddAssetData(obj);
                        }
                    }

                    usedEvent = true;
                    break;
            }

            if (usedEvent)
                currentEvent.Use();

            m_PackableListExpanded = EditorGUI.Foldout(rect, m_PackableListExpanded,
                EditorGUIUtility.TrTextContent("Objects for Packing",
                    $"Only accept {m_TypeTips}"), true);

            if (m_PackableListExpanded)
            {
                EditorGUI.indentLevel++;
                m_ReorderableList.DoLayoutList();
                EditorGUI.indentLevel--;
            }
        }

        private void OnGUI()
        {
            if (m_MergeAssetEditorData == null)
            {
                Load();
            }

            if (m_ReorderableList == null)
            {
                InitAssetList();
            }

            HandlePackableListUI();
            m_MergeAssetEditorData.SearchPatterns =
                EditorGUILayout.TextField("SearchPatterns", m_MergeAssetEditorData.SearchPatterns);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("FileSystemFolder", m_MergeAssetEditorData.FileSystemFolder);
            if (GUILayout.Button("Browser", GUILayout.Width(100)))
            {
                string path = MergeAssetUtility.ChooseVfsFolder();
                if (!string.IsNullOrEmpty(path))
                {
                    m_MergeAssetEditorData.FileSystemFolder = path;
                }
            }

            EditorGUILayout.EndHorizontal();

            m_MergeAssetEditorData.FileSystemName =
                EditorGUILayout.TextField("FileSystemName", m_MergeAssetEditorData.FileSystemName);

            if (GUILayout.Button("Merge"))
            {
                string path = Utility.Path.GetRegularPath(Path.Combine(m_MergeAssetEditorData.FileSystemFolder,
                    m_MergeAssetEditorData.FileSystemName + ".dat"));
                MergeAssetUtility.Merge(path, m_MergeAssetEditorData.AssetDataList,
                    m_MergeAssetEditorData.SearchPatterns);
            }

            if (GUILayout.Button("Save config as scriptableObject"))
            {
                string path = Utility.Path.GetRegularPath(Path.Combine(m_MergeAssetScriptableObjectSaveFolder,
                    m_MergeAssetEditorData.FileSystemName + ".asset"));
                if (m_MergeAssetScriptableObject == null)
                {
                    m_MergeAssetScriptableObject = ScriptableObject.CreateInstance<MergeAssetScriptableObject>();
                    m_MergeAssetScriptableObject.MergeAssetEditorData = m_MergeAssetEditorData;
                }

                MergeAssetUtility.SaveScriptableObject(m_MergeAssetScriptableObject, path);
            }
        }
    }
}