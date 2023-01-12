using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CodeBind
{
    [CSCodeBind]
    public sealed class CSMonoBind : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField]
        private char m_SeparatorChar = '_';

        [SerializeField]
        private MonoScript m_BindScript;
        
        [FormerlySerializedAs("m_BindComponentTypes")]
        [SerializeField]
        private List<string> m_BindComponentNames = new List<string>();

        public List<string> BindComponentNames => this.m_BindComponentNames;
#endif

        [SerializeField]
        private List<Component> m_BindComponents = new List<Component>();

        public List<Component> BindComponents => this.m_BindComponents;
    }
}
