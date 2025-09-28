using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using System;
using System.IO;
using GameFramework;
using UnityEditor;
#endif

namespace UnityGameFramework.Extension
{
    [CreateAssetMenu(fileName = "AssetCollection", menuName = "UGF/AssetCollection")]
    public sealed class AssetCollection : SerializedScriptableObject
    {
#if UNITY_EDITOR
        private void Awake()
        {
            Pack();
        }

        [SerializeField]
        private string m_CollectionPatterns;
        [SerializeField]
        [OnValueChanged("OnPathChange", IncludeChildren = true)]
        [AssetsOnly]
        private List<DefaultAsset> m_CollectionPaths = new List<DefaultAsset>();
        
        [NonSerialized]
        private readonly Dictionary<string, Object> m_AssetDictTemp = new Dictionary<string, Object>();

        private void OnPathChange()
        {
            m_CollectionPaths = m_CollectionPaths.Distinct().ToList();
            Pack();
        }

        [Button("RefreshCollection")]
        public void Pack()
        {
            bool isDirty = false;
            m_AssetDictTemp.Clear();
            string[] searchPatterns = (string.IsNullOrEmpty(m_CollectionPatterns) ? "*.*" : m_CollectionPatterns).Split(';', ',', '|');
            foreach (DefaultAsset pathAsset in m_CollectionPaths)
            {
                if(pathAsset == null || !ProjectWindowUtil.IsFolder(pathAsset.GetInstanceID()))
                    continue;
                string path = AssetDatabase.GetAssetPath(pathAsset);
                foreach (var pattern in searchPatterns)
                {
                    if(string.IsNullOrEmpty(pattern))
                        continue;
                    string[] files = Directory.GetFiles(path, pattern, SearchOption.AllDirectories)
                        .Where(filePath => !filePath.EndsWith(".meta")).Select(Utility.Path.GetRegularPath).ToArray();
                    foreach (string file in files)
                    {
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
#endif
        [OdinSerialize, Searchable]
        [DictionaryDrawerSettings(KeyLabel = "Path", ValueLabel = "Object", IsReadOnly = true)]
        private Dictionary<string, Object> m_AssetDict = new Dictionary<string, Object>();

        public T GetAsset<T>(string path) where T : Object
        {
            m_AssetDict.TryGetValue(path, out Object obj);
            return (T)obj;
        }
    }
}