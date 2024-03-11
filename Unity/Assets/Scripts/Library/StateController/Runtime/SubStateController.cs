using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StateController
{
    [Serializable]
    public sealed class SubStateController
    { 
        [SerializeField]
        private string m_ControllerName;
        [SerializeField, ReadOnly]
        private List<string> m_StateNames = new List<string>();
        [SerializeField, ReadOnly]
        private List<BaseSate> m_Sates = new List<BaseSate>();

        private string m_SelectedName;

        public string SelectedName
        {
            get
            {
                return m_SelectedName;
            }
            set
            {
                if(m_SelectedName == value)
                    return;
                m_SelectedName = value;
                foreach (var state in m_Sates)
                {
                    state.Refresh();
                }
            }
        }

        public string ControllerName
        {
            get => m_ControllerName;
            internal set => m_ControllerName = value;
        }

#if UNITY_EDITOR
        internal List<string> StateNames => m_StateNames;

        internal void AddState(BaseSate sate)
        {
            if (m_Sates.Contains(sate))
                return;
            m_Sates.Add(sate);
        }

        internal void RemoveState(BaseSate sate)
        {
            m_Sates.Remove(sate);
        }
#endif
    }
}