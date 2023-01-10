#if !ODIN_INSPECTOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGameFramework.Extension
{
    [Serializable]
    public class StringSpriteDictionary : Dictionary<string, Sprite>, ISerializationCallbackReceiver
    {
        [SerializeField] private string[] m_Keys;
        [SerializeField] private Sprite[] m_Values;

        public void OnBeforeSerialize()
        {
            int n = this.Count;
            m_Keys = new string[n];
            m_Values = new Sprite[n];

            int i = 0;
            foreach (var kvp in this)
            {
                m_Keys[i] = kvp.Key;
                m_Values[i] = kvp.Value;
                ++i;
            }
        }

        public void OnAfterDeserialize()
        {
            if (m_Keys != null && m_Values != null && m_Keys.Length == m_Values.Length)
            {
                this.Clear();
                int n = m_Keys.Length;
                for (int i = 0; i < n; ++i)
                {
                    this[m_Keys[i]] = m_Values[i];
                }

                m_Keys = null;
                m_Values = null;
            }
        }

        public void CopyFrom(IDictionary<string, Sprite> target)
        {
            this.Clear();
            foreach (KeyValuePair<string, Sprite> kv in target)
            {
                this.Add(kv.Key, kv.Value);
            }
        }
    }
}
#endif