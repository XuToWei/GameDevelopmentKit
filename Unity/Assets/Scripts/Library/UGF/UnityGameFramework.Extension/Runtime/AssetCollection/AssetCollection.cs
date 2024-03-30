using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using GameFramework;
using UnityEditor;
#endif

namespace UnityGameFramework.Extension
{
    [CreateAssetMenu(fileName = "AssetCollection", menuName = "UGF/AssetCollection")]
    public class AssetCollection : SerializedScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField]
        private string m_CollectionPatterns;
        [SerializeField]
        [OnValueChanged("OnPathChange", IncludeChildren = true)]
        [AssetsOnly]
        private List<DefaultAsset> m_CollectionPaths = new List<DefaultAsset>();

        private void OnPathChange()
        {
            m_CollectionPaths = m_CollectionPaths.Distinct().ToList();
        }

        [Button("RefreshCollection")]
        public void Pack()
        {
            var searchPatterns = (string.IsNullOrEmpty(m_CollectionPatterns) ? "*.*" : m_CollectionPatterns).Split(';', ',', '|');
            m_AssetDict.Clear();
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
                            m_AssetDict.Add(file, sprite);
                        }
                        else
                        {
                            Object obj = AssetDatabase.LoadMainAssetAtPath(file);
                            m_AssetDict.Add(file, obj);
                        }
                    }
                }
            }
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
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