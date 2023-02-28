using System;
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
        private static readonly CSCodeBindPool s_Pool = new CSCodeBindPool();
#if UNITY_EDITOR
        [SerializeField]
        private char m_SeparatorChar = '_';

        [SerializeField]
        private MonoScript m_BindScript;
        
        [SerializeField]
        private List<string> m_BindComponentNames = new List<string>();

        public char separatorChar => this.m_SeparatorChar;
        public MonoScript bindScript => this.m_BindScript;
        public List<string> bindComponentNames => this.m_BindComponentNames;
#endif

        [SerializeField]
        private List<Component> m_BindComponents = new List<Component>();

        public List<Component> bindComponents => this.m_BindComponents;

        private ICSCodeBind m_CSCodeBindObject;

        /// <summary>
        /// 获取绑定代码的的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetCSCodeBindObject<T>() where T : ICSCodeBind, new()
        {
            if (m_CSCodeBindObject == null)
            {
                m_CSCodeBindObject = s_Pool.Fetch<T>(this);
            }
            else
            {
                if (m_CSCodeBindObject is not T)
                {
                    s_Pool.Recycle(m_CSCodeBindObject);
                    m_CSCodeBindObject = s_Pool.Fetch<T>(this);
                }
            }
            return (T)m_CSCodeBindObject;
        }

        private void OnDestroy()
        {
            if (m_CSCodeBindObject != null)
            {
                var obj = m_CSCodeBindObject;
                m_CSCodeBindObject = null;
                s_Pool.Recycle(obj);
            }
        }
    }
}
