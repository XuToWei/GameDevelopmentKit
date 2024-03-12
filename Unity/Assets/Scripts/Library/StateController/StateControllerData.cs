using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StateController
{
    [Serializable]
    public sealed class StateControllerData
    { 
        [SerializeField]
        private string m_Name;
        [SerializeField, ReadOnly]
        private List<string> m_StateNames = new List<string>();
        [SerializeField, ReadOnly]
        private List<BaseState> m_Sates = new List<BaseState>();

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
                    state.OnRefresh();
                }
            }
        }

#if UNITY_EDITOR
        internal string Name
        {
            get => m_Name;
            set => m_Name = value;
        }
        internal List<string> StateNames => m_StateNames;
        internal List<BaseState> States => m_Sates;

        internal void Editor_OnRefresh()
        {
            // for (int i = m_Sates.Count - 1; i >= 1; i--)
            // {
            //     for (int j = i - 1; j >= 0; j--)
            //     {
            //         if (m_Sates[i] == m_Sates[j])
            //         {
            //             m_Sates.RemoveAt(j);
            //         }
            //     }
            // }
        }
#endif
    }
}