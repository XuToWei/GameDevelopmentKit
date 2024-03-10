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

        [LabelText("Add New State Name")]
        [ValidateInput("ValidateInputNewStateName")]
        internal string m_NewStateName;

        [ShowIf("CheckCanAddStateName")]
        internal void AddStateName()
        {
            m_StateNames.Add(m_NewStateName);
            m_NewStateName = string.Empty;
        }

        internal bool ValidateInputNewStateName(string newStateName, ref string errorMsg)
        {
            if (m_StateNames.Contains(newStateName))
            {
                errorMsg = $"State name '{newStateName}' already exist!";
                return false;
            }
            return true;
        }

        internal bool CheckCanAddStateName()
        {
            if (string.IsNullOrEmpty(m_NewStateName))
                return false;
            if (m_StateNames.Contains(m_NewStateName))
                return false;
            return true;
        }

        internal void EditorRefresh()
        {
            foreach (var sate in m_Sates)
            {
                sate.EditorRefresh();
            }
        }
#endif
    }
}