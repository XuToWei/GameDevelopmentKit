#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameFramework;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityGameFramework.Extension
{
    public partial class AssetCollection
    {
        private void Awake()
        {
            if (EditorApplication.isUpdating ||
                EditorApplication.isCompiling ||
                !EditorApplication.isPlaying ||
                string.IsNullOrEmpty(AssetDatabase.GetAssetPath(this)))
            {
                return;
            }
            Pack();
        }

        [SerializeField]
        private string m_CollectionPatterns;

        [ShowInInspector, AssetsOnly, NonSerialized]
        [OnValueChanged(nameof(OnPathChange), IncludeChildren = true)]
        [ListDrawerSettings(DraggableItems = false, IsReadOnly = false, HideAddButton = true)]
        [Tooltip("收集资源的目录列表")]
        private List<DefaultAsset> m_CollectionPaths = new List<DefaultAsset>();

        [HideInInspector, SerializeField]
        private List<string> m_CollectionPathGUIDs = new List<string>();

        [NonSerialized]
        private readonly Dictionary<string, Object> m_AssetDictTemp = new Dictionary<string, Object>();

        private void OnValidate()
        {
            bool isDirty = RefreshCollectionPaths();
            if (isDirty)
            {
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }
        }

        private bool RefreshCollectionPaths()
        {
            bool isDirty = false;
            m_CollectionPaths.Clear();
            for (int i = m_CollectionPathGUIDs.Count - 1; i >= 0; i--)
            {
                string guid = m_CollectionPathGUIDs[i];
                if (string.IsNullOrEmpty(guid))
                {
                    isDirty = true;
                    m_CollectionPathGUIDs.RemoveAt(i);
                    continue;
                }
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(assetPath))
                {
                    isDirty = true;
                    m_CollectionPathGUIDs.RemoveAt(i);
                    continue;
                }
                DefaultAsset defaultAsset = AssetDatabase.LoadAssetAtPath<DefaultAsset>(assetPath);
                if (defaultAsset == null || !ProjectWindowUtil.IsFolder(defaultAsset.GetInstanceID()))
                {
                    isDirty = true;
                    m_CollectionPathGUIDs.RemoveAt(i);
                    continue;
                }
                bool found = false;
                for (int j = i + 1; j < m_CollectionPathGUIDs.Count; j++)
                {
                    if(guid == m_CollectionPathGUIDs[j])
                    {
                        found = true;
                        isDirty = true;
                        m_CollectionPathGUIDs.RemoveAt(i);
                        break;
                    }
                }
                if (found)
                {
                    continue;
                }
                m_CollectionPaths.Add(defaultAsset);
            }
            return isDirty;
        }

        private void OnPathChange()
        {
            bool isDirty = false;
            m_CollectionPathGUIDs.Clear();
            for (int i = m_CollectionPaths.Count - 1; i >= 0; i--)
            {
                DefaultAsset defaultAsset = m_CollectionPaths[i];
                if (defaultAsset == null || !ProjectWindowUtil.IsFolder(defaultAsset.GetInstanceID()))
                {
                    isDirty = true;
                    m_CollectionPaths.RemoveAt(i);
                    continue;
                }
                string assetPath = AssetDatabase.GetAssetPath(defaultAsset);
                if (string.IsNullOrEmpty(assetPath))
                {
                    isDirty = true;
                    m_CollectionPaths.RemoveAt(i);
                    continue;
                }
                string guid = AssetDatabase.AssetPathToGUID(assetPath);
                if (string.IsNullOrEmpty(guid))
                {
                    isDirty = true;
                    m_CollectionPaths.RemoveAt(i);
                    continue;
                }
                bool found = false;
                for (int j = i + 1; j < m_CollectionPaths.Count; j++)
                {
                    if (defaultAsset == m_CollectionPaths[j])
                    {
                        found = true;
                        isDirty = true;
                        m_CollectionPaths.RemoveAt(i);
                    }
                }
                if (found)
                {
                    continue;
                }
                m_CollectionPathGUIDs.Add(guid);
            }
            if(isDirty)
            {
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }
            Pack();
        }

        [Button("RefreshCollection")]
        public void Pack()
        {
            bool isDirty = RefreshCollectionPaths();
            m_AssetDictTemp.Clear();
            string[] searchPatterns = (string.IsNullOrEmpty(m_CollectionPatterns) ? "*.*" : m_CollectionPatterns).Split(';', ',', '|');
            foreach (DefaultAsset defaultAsset in m_CollectionPaths)
            {
                if(defaultAsset == null || !ProjectWindowUtil.IsFolder(defaultAsset.GetInstanceID()))
                    continue;
                string path = AssetDatabase.GetAssetPath(defaultAsset);
                foreach (var pattern in searchPatterns)
                {
                    if(string.IsNullOrEmpty(pattern))
                        continue;
                    string[] files = Directory.GetFiles(path, pattern, SearchOption.AllDirectories)
                        .Where(filePath => !filePath.EndsWith(".meta")).Select(Utility.Path.GetRegularPath).ToArray();
                    foreach (string file in files)
                    {
                        if(m_AssetDictTemp.ContainsKey(file))
                            continue;
                        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(file);
                        if (sprite != null)
                        {
                            m_AssetDictTemp.Add(file, sprite);
                        }
                        else
                        {
                            Object obj = AssetDatabase.LoadMainAssetAtPath(file);
                            m_AssetDictTemp.Add(file, obj);
                        }
                    }
                }
            }
            if(m_AssetDictTemp.Count != m_AssetDict.Count)
            {
                isDirty = true;
            }
            else
            {
                foreach (KeyValuePair<string, Object> item in m_AssetDict)
                {
                    if(!m_AssetDictTemp.TryGetValue(item.Key, out Object v) || v != item.Value)
                    {
                        isDirty = true;
                        break;
                    }
                }
            }
            if (isDirty)
            {
                m_AssetDict.Clear();
                foreach (KeyValuePair<string, Object> item in m_AssetDictTemp)
                {
                    m_AssetDict.Add(item.Key, item.Value);
                }
                m_AssetDictTemp.Clear();
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }
        }
    }
}
#endif