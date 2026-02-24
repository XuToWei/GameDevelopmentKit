using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityGameFramework.Runtime;
using Object = UnityEngine.Object;

namespace UnityGameFramework.Extension
{
    [CreateAssetMenu(fileName = "AssetCollection", menuName = "UGF/AssetCollection")]
    public sealed partial class AssetCollection : SerializedScriptableObject
    {
        [OdinSerialize, Searchable]
        [DictionaryDrawerSettings(KeyLabel = "Path", ValueLabel = "Object", IsReadOnly = true)]
        private Dictionary<string, Object> m_AssetDict = new Dictionary<string, Object>();

        public T GetAsset<T>(string path) where T : Object
        {
            if (!m_AssetDict.TryGetValue(path, out Object obj))
            {
                Log.Error("AssetCollection GetAsset can not find asset at path '{0}'.", path);
                return null;
            }
            T result = obj as T;
            if (result == null)
            {
                Log.Error("AssetCollection GetAsset at path '{0}' is not of type {1}.", path, typeof(T).FullName);
            }
            return result;
        }

        public Dictionary<string, Object>.KeyCollection Names
        {
            get
            {
                return m_AssetDict.Keys;
            }
        }

        public Dictionary<string, Object>.ValueCollection Assets
        {
            get
            {
                return m_AssetDict.Values;
            }
        }
    }
}