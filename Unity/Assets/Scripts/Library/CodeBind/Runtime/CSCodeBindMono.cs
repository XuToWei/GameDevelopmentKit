using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CodeBind
{
    [CSCodeBind]
    public sealed class CSCodeBindMono : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField]
        private char m_SeparatorChar = '_';

        [SerializeField]
        private MonoScript m_BindScript;
        
        [SerializeField]
        private List<string> m_BindComponentNames = new List<string>();

        public char SeparatorChar => this.m_SeparatorChar;
        public MonoScript BindScript => this.m_BindScript;
        public List<string> BindComponentNames => this.m_BindComponentNames;
#endif

        [SerializeField]
        private List<Component> m_BindComponents = new List<Component>();

        public List<Component> bindComponents => this.m_BindComponents;

        private ICSCodeBind m_CSCodeObject;

        public T GetCSCodeObject<T>() where T : ICSCodeBind, new()
        {
            if (m_CSCodeObject == null)
            {
                m_CSCodeObject = new T();
            }
            return (T)m_CSCodeObject;
        }
    }
}
