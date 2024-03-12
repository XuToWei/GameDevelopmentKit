using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StateController
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public sealed class StateController : MonoBehaviour
    {
        [HideInInspector]
        [SerializeField]
        private List<StateControllerData> m_ControllerDatas = new List<StateControllerData>();

        [HideInInspector]
        [SerializeField]
        private List<BaseState> m_States = new List<BaseState>();

        private Dictionary<string, StateControllerData> m_ControllerDataDict;

        private void Awake()
        {
            m_ControllerDataDict = new Dictionary<string, StateControllerData>();
            foreach (var data in m_ControllerDatas)
            {
                m_ControllerDataDict.Add(data.Name, data);
            }
            foreach (var state in m_States)
            {
                state.OnInit(this);
            }
#if UNITY_EDITOR
            Editor_Refresh();
#endif
        }

        public void SelectedName(string dateName, string stateName)
        {
            m_ControllerDataDict[dateName].SelectedName = stateName;
        }

        public string GetSelectedName(string dateName)
        {
            return m_ControllerDataDict[dateName].SelectedName;
        }
        
        public StateControllerData GetData(string dateName)
        {
            return m_ControllerDataDict[dateName];
        }

#if UNITY_EDITOR
        internal List<BaseState> States => m_States;
        private void Editor_Refresh()
        {
            foreach (var sate in GetComponentsInChildren<BaseState>(true))
            {
                sate.Editor_OnRefresh();
            }
        }

        public StateControllerData Editor_GetData(string dateName)
        {
            foreach (var data in m_ControllerDatas)
            {
                if (data.Name == dateName)
                {
                    return data;
                }
            }
            return null;
        }

        [HorizontalGroup("Split", 0.5f, PaddingRight = 15)]
        [BoxGroup("Split/Left"), LabelWidth(15)]
        public int L;
        
        [BoxGroup("Split/Right"), LabelWidth(15)]
        public int M;
        
        [BoxGroup("Split/Left"), LabelWidth(15)]
        public int N;
        
        [BoxGroup("Split/Right"), LabelWidth(15)]
        public int O;
        
        private const string ADD_NAME = "Add Data";

        [HorizontalGroup(ADD_NAME)]
        [BoxGroup("Split/Right"), LabelWidth(15)]
        [LabelText("Data Name")]
        [PropertyOrder(10)]
        [ShowInInspector]
        [ValidateInput("ValidateInputNewDataName")]
        private string m_NewDataName;
        
        [HorizontalGroup(ADD_NAME)]
        [GUIColor(0, 1, 0)]
        [Button("Add")]
        [PropertyOrder(11)]
        [EnableIf("CheckCanAddControllerData")]
        private void AddControllerData()
        {
            if (!CheckCanAddControllerData())
                return;
            StateControllerData stateControllerData = new StateControllerData();
            stateControllerData.Name = m_NewDataName;
            m_ControllerDatas.Add(stateControllerData);
            m_NewDataName = string.Empty;
            Editor_Refresh();
        }

        private const string SELECT_NAME = "Select Data";

        [BoxGroup(SELECT_NAME)]
        [LabelText("Data Name")]
        [PropertyOrder(20)]
        [ShowInInspector]
        [ValueDropdown("GetAllDataNames")]
        [OnValueChanged("OnSelectedData")]
        private string m_SelectedDataName = string.Empty;

        private StateControllerData m_SelectedData;

        [BoxGroup(SELECT_NAME + "/State")]
        [LabelText("State Name")]
        [PropertyOrder(21)]
        [ShowInInspector]
        [EnableIf("IsSelectedController")]
        [ValidateInput("ValidateInputNewStateName")]
        private string m_NewStateName;

        [BoxGroup(SELECT_NAME + "/State")]
        [GUIColor(0,1,0)]
        [Button("Add State Name")]
        [PropertyOrder(22)]
        [EnableIf("IsSelectedController")]
        private void AddStateName()
        {
            if (string.IsNullOrEmpty(m_NewStateName))
                return;
            if(m_SelectedData.StateNames.Contains(m_NewStateName))
                return;
            m_SelectedData.StateNames.Add(m_NewStateName);
            m_NewStateName = string.Empty;
            Editor_Refresh();
        }

        private readonly List<string> m_EmptyListString = new List<string>();
        [BoxGroup(SELECT_NAME + "/State")]
        [LabelText("State Names")]
        [PropertyOrder(23)]
        [ShowInInspector]
        [ReadOnly]
        [EnableIf("IsSelectedController")]
        [ListDrawerSettings(DefaultExpandedState = true,
            OnBeginListElementGUI = "OnStateNameBeginGUI",
            OnEndListElementGUI = "OnStateNameEndGUI")]
        private List<string> m_SelectedStateNames
        {
            get
            {
                if (m_SelectedData == null)
                {
                    return m_EmptyListString;
                }
                return m_SelectedData.StateNames;
            }
        }
        
        private readonly List<BaseState> m_EmptyListState = new List<BaseState>();
        [BoxGroup(SELECT_NAME + "/State")]
        [LabelText("State Children")]
        [PropertyOrder(24)]
        [ShowInInspector]
        [ReadOnly]
        [EnableIf("IsSelectedController")]
        [ListDrawerSettings(DefaultExpandedState = true,
            OnBeginListElementGUI = "OnStateNameBeginGUI",
            OnEndListElementGUI = "OnStateNameEndGUI")]
        private List<BaseState> m_SelectedStates
        {
            get
            {
                if (m_SelectedData == null)
                {
                    return m_EmptyListState;
                }
                return m_SelectedData.States;
            }
        }

        [BoxGroup(SELECT_NAME + "/Rename Controller")]
        [LabelText("Controller Name")]
        [PropertyOrder(25)]
        [ShowInInspector]
        [EnableIf("IsSelectedController")]
        [ValidateInput("ValidateInputRenameDataName")]
        private string m_RenameDataName;

        [BoxGroup(SELECT_NAME + "/Rename Controller")]
        [GUIColor(0,1,0)]
        [Button("Rename")]
        [PropertyOrder(26)]
        [EnableIf("IsSelectedController")]
        private void RenameSelectedControllerName()
        {
            if (string.IsNullOrEmpty(m_RenameDataName))
                return;
            if (m_RenameDataName == m_SelectedData.Name)
                return;
            foreach (var data in m_ControllerDatas)
            {
                if(data == m_SelectedData)
                    continue;
                if (data.Name == m_RenameDataName)
                {
                    return;
                }
            }
            foreach (var state in m_States)
            {
                state.Editor_OnDataReanme(m_SelectedDataName, m_RenameDataName);
            }
            m_SelectedData.Name = m_RenameDataName;
            m_SelectedDataName = m_RenameDataName;
        }

        [BoxGroup(SELECT_NAME)]
        [GUIColor(1,1,0)]
        [Button("Remove Controller")]
        [PropertyOrder(30)]
        [EnableIf("IsSelectedController")]
        private void RemoveSelectedController()
        {
            m_ControllerDatas.Remove(m_SelectedData);
            m_SelectedData = null;
            m_NewDataName = string.Empty;
            m_RenameDataName = string.Empty;
            Editor_Refresh();
        }

        private bool ValidateInputNewDataName(string dataName, ref string errorMsg)
        {
            if (string.IsNullOrEmpty(dataName))
            {
                return true;
            }
            foreach (var subStateController in m_ControllerDatas)
            {
                if (subStateController.Name == dataName)
                {
                    errorMsg = $"A controller name '{dataName}' already exists!";
                    return false;
                }
            }
            return true;
        }

        private bool CheckCanAddControllerData()
        {
            if (string.IsNullOrEmpty(m_NewDataName))
                return false;
            foreach (var data in m_ControllerDatas)
            {
                if (data.Name == m_NewDataName)
                    return false;
            }
            return true;
        }

        private bool IsSelectedController()
        {
            if (string.IsNullOrEmpty(m_SelectedDataName))
                return false;
            foreach (var data in m_ControllerDatas)
            {
                if (data.Name == m_SelectedDataName)
                    return true;
            }
            return false;
        }

        private readonly List<string> m_ControllerNames = new List<string>();
        public List<string> GetAllDataNames()
        {
            m_ControllerNames.Clear();
            foreach (var controller in m_ControllerDatas)
            {
                m_ControllerNames.Add(controller.Name);
            }
            m_ControllerNames.Sort();
            return m_ControllerNames;
        }

        private void OnSelectedData()
        {
            m_SelectedData = null;
            foreach (var data in m_ControllerDatas)
            {
                if (data.Name == m_SelectedDataName)
                {
                    m_SelectedData = data;
                    m_RenameDataName = m_SelectedDataName;
                    break;
                }
            }
        }

        private bool ValidateInputNewStateName(string stateName, ref string errMsg)
        {
            if (m_SelectedData == null)
                return true;
            if (string.IsNullOrEmpty(stateName))
                return true;
            if (m_SelectedData.StateNames.Contains(stateName))
            {
                errMsg = $"A state name '{stateName}' already exists!";
                return false;
            }
            return true;
        }

        private bool ValidateInputRenameDataName(string dateName, ref string errMsg)
        {
            if (m_SelectedData == null || m_SelectedData.Name == dateName)
                return true;
            if (string.IsNullOrEmpty(dateName))
            {
                errMsg = "Controller name can't be empty!";
                return false;
            }
            foreach (var data in m_ControllerDatas)
            {
                if(data == m_SelectedData)
                    continue;
                if (data.Name == dateName)
                {
                    errMsg = $"A controller name '{dateName}' already exists!";
                    return false;
                }
            }
            return true;
        }

        private void OnStateNameBeginGUI(int selectionIndex)
        {
            GUILayout.BeginHorizontal();
        }

        private void OnStateNameEndGUI(int selectionIndex)
        {
            GUI.enabled = m_SelectedData.SelectedName != m_SelectedData.StateNames[selectionIndex];
            if (GUILayout.Button("Apply"))
            {
                m_SelectedData.SelectedName = m_SelectedData.StateNames[selectionIndex];
            }
            GUI.enabled = true;
            if (GUILayout.Button("X"))
            {
                m_SelectedData.StateNames.RemoveAt(selectionIndex);
                foreach (var state in m_SelectedData.States)
                {
                    state.Editor_OnRemoveStateAt(selectionIndex);
                }
            }
            GUILayout.EndHorizontal();
        }
#endif
    }
}