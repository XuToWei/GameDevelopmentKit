using System.Collections.Generic;
using UnityEngine;

namespace CodeBind
{
    [CodeBind]
    [CodeBindName("ReferenceBindMono")]
    [DisallowMultipleComponent]
    public sealed class ReferenceBindMono : MonoBehaviour
    {
        [SerializeField] private string[] m_BindNames;
        [SerializeField] private GameObject[] m_BindGameObjects;

        private readonly Dictionary<string, GameObject> m_DataDict = new Dictionary<string, GameObject>();

        private void Awake()
        {
            for (int i = 0; i < m_BindNames.Length; i++)
            {
                m_DataDict.Add(m_BindNames[i], m_BindGameObjects[i]);
            }
        }

        public GameObject Get(string key)
        {
            m_DataDict.TryGetValue(key, out GameObject go);
            return go;
        }
    }
}
