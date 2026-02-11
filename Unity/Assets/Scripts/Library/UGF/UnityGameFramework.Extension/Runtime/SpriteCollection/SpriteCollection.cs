using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    [CreateAssetMenu(fileName = "SpriteCollection", menuName = "UGF/SpriteCollection")]
    public sealed partial class SpriteCollection : SerializedScriptableObject
    {
        [OdinSerialize, Searchable] [DictionaryDrawerSettings(KeyLabel = "Path", ValueLabel = "Sprite", IsReadOnly = true)]
        private Dictionary<string, Sprite> m_Sprites = new Dictionary<string, Sprite>();

        public Sprite GetSprite(string path)
        {
            if (!m_Sprites.TryGetValue(path, out Sprite sprite))
            {
                Log.Error("SpriteCollection GetSprite can not find sprite at path '{0}'", path);
                return null;
            }
            return sprite;
        }

        public Dictionary<string, Sprite>.KeyCollection Names
        {
            get
            {
                return m_Sprites.Keys;
            }
        }

        public Dictionary<string, Sprite>.ValueCollection Sprites
        {
            get
            {
                return m_Sprites.Values;
            }
        }
    }
}