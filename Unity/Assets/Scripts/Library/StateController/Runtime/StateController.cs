using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StateController
{
    public sealed partial class StateController : MonoBehaviour
    {
        [ValidateInput("ValidateInputName")]
        [SerializeField]
        private string m_Name;
        [ValidateInput("ValidateInputStateNames")]
        [SerializeField]
        private List<string> m_StateNames = new List<string>();
        [SerializeField]
        private List<BaseSate> m_States = new List<BaseSate>();
        [ValueDropdown("ValueDropdownSelectedIndex")]
        [SerializeField]
        private int m_SelectedIndex;

        public string Name => m_Name;
        public List<string> StateNames => m_StateNames;

        public int SelectedIndex
        {
            get
            {
                return m_SelectedIndex;
            }
            set
            {
                if (value != m_SelectedIndex)
                {
                    m_SelectedIndex = value;
                    Refresh(false);
                }
            }
        }

        private void Awake()
        {
            Refresh(true);
        }

        private void Refresh(bool isInit)
        {
            foreach (var state in m_States)
            {
                state.Refresh(isInit);
            }
        }
    }
}