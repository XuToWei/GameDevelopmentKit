using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StateController
{
    [ExecuteAlways]
    public abstract class BaseBooleanLogicState : BaseState
    {
        private const string DATA_1 = "Data1";
        private const string DATA_2 = "Data2";

        [SerializeField]
        [HorizontalGroup(DATA_1)]
        [LabelText("Data Name")]
        [PropertyOrder(10)]
        [ValueDropdown("GetDataNames1")]
        [OnValueChanged("OnSelectedData1")]
        private string m_DataName1;

        [SerializeField]
        [HorizontalGroup(DATA_1)]
        [LabelText("State Names")]
        [PropertyOrder(11)]
        [ShowIf("IsSelectedData1")]
        [ListDrawerSettings(DefaultExpandedState = true,
            HideAddButton = true, HideRemoveButton = true,
            DraggableItems = false,
            OnBeginListElementGUI = "OnStateDataBeginGUI1",
            OnEndListElementGUI = "OnStateDataEndGUI1")]
        private List<bool> m_StateDatas1 = new List<bool>();

        [SerializeField, PropertyOrder(20)]
        private BooleanLogicType m_BooleanLogicType;

        [SerializeField]
        [BoxGroup(DATA_2)]
        [LabelText("Data Name")]
        [PropertyOrder(30)]
        [ValueDropdown("GetDataNames2")]
        [OnValueChanged("OnSelectedData2")]
        private string m_DataName2;

        [SerializeField]
        [BoxGroup(DATA_2)]
        [LabelText("State Names")]
        [PropertyOrder(31)]
        [ShowIf("CanShowData2")]
        [EnableIf("IsSelectedData2")]
        [ListDrawerSettings(DefaultExpandedState = true,
            HideAddButton = true, HideRemoveButton = true,
            DraggableItems = false,
            OnBeginListElementGUI = "OnStateDataBeginGUI2",
            OnEndListElementGUI = "OnStateDataEndGUI2")]
        private List<bool> m_StateDatas2 = new List<bool>();

        private string m_CurStateName1;
        private string m_CurStateName2;
        private Dictionary<string, bool> m_StateDataDict1;
        private Dictionary<string, bool> m_StateDataDict2;
        private StateControllerData m_Data1;
        private StateControllerData m_Data2;

        internal override void OnInit(StateController controller)
        {
            m_Data1 = controller.GetData(m_DataName1);
            if (m_Data1 != null)
            {
                m_StateDataDict1 = new Dictionary<string, bool>();
                for (int i = 0; i <m_Data1.StateNames.Count; i++)
                {
                    m_StateDataDict1.Add(m_Data1.StateNames[i], m_StateDatas1[i]);
                }
            }
            if (m_BooleanLogicType != BooleanLogicType.None)
            {
                m_Data2 = controller.GetData(m_DataName2);
                if (m_Data2 != null)
                {
                    m_StateDataDict2 = new Dictionary<string, bool>();
                    for (int i = 0; i < m_Data2.StateNames.Count; i++)
                    {
                        m_StateDataDict2.Add(m_Data2.StateNames[i], m_StateDatas2[i]);
                    }
                }
            }
        }

        internal override void OnRefresh()
        {
            if (m_BooleanLogicType == BooleanLogicType.None)
            {
                if (!string.IsNullOrEmpty(m_CurStateName1) && m_Data1.SelectedName == m_CurStateName1)
                    return;
                m_CurStateName1 = m_Data1.SelectedName;
                OnSateChanged(m_StateDataDict1[m_CurStateName1]);
            }
            else
            {
                if (!string.IsNullOrEmpty(m_CurStateName1) && !string.IsNullOrEmpty(m_CurStateName2) && 
                    m_Data1.SelectedName == m_CurStateName1 && m_Data2.SelectedName == m_CurStateName2)
                    return;
                m_CurStateName1 = m_Data1.SelectedName;
                m_CurStateName2 = m_Data2.SelectedName;
                bool logicResult = false;
                switch (m_BooleanLogicType)
                {
                    case BooleanLogicType.And:
                        logicResult = m_StateDataDict1[m_CurStateName1] && m_StateDataDict2[m_CurStateName2];
                        break;
                    case BooleanLogicType.Or:
                        logicResult = m_StateDataDict1[m_CurStateName1] || m_StateDataDict2[m_CurStateName2];
                        break;
                }
                OnSateChanged(logicResult);
            }
        }

        protected abstract void OnSateChanged(bool logicResult);

#if UNITY_EDITOR
        internal override void Editor_OnRefresh()
        {
            RefreshData1();
            RefreshData2();
        }

        internal override void Editor_OnDataReanme(string oldDataName, string newDataName)
        {
            if (m_DataName1 == oldDataName)
            {
                m_DataName1 = newDataName;
            }
            else if (m_DataName1 == oldDataName)
            {
                m_DataName2 = newDataName;
            }
        }

        internal override void Editor_OnRemoveStateAt(int index)
        {
            
        }

        private bool IsSelectedData1()
        {
            return m_Data1 != null;
        }

        private bool IsSelectedData2()
        {
            return m_Data2 != null && m_BooleanLogicType != BooleanLogicType.None;
        }

        private bool CanShowData2()
        {
            return m_BooleanLogicType != BooleanLogicType.None;
        }

        private List<string> GetDataNames1()
        {
            return Controller.GetAllDataNames();
        }
        
        private List<string> GetDataNames2()
        {
            var names = Controller.GetAllDataNames();
            names.Remove(m_DataName1);
            return names;
        }

        private void RefreshData1()
        {
            m_Data1 = Controller.Editor_GetData(m_DataName1);
            if (m_Data1 != null)
            {
                for (int i = m_StateDatas1.Count; i < m_Data1.StateNames.Count; i++)
                {
                    m_StateDatas1.Add(default);
                }
                for (int i = m_Data1.States.Count - 1; i >=0; i--)
                {
                    if (m_Data1.States[i] == null)
                    {
                        m_Data1.States.RemoveAt(i);
                    }
                }
            }
            else
            {
                m_StateDatas1.Clear();
            }
        }
        
        private void RefreshData2()
        {
            m_Data2 = Controller.Editor_GetData(m_DataName2);
            if (m_Data2 != null)
            {
                for (int i = m_StateDatas2.Count; i < m_Data2.StateNames.Count; i++)
                {
                    m_StateDatas2.Add(default);
                }
                for (int i = m_Data2.States.Count - 1; i >=0; i--)
                {
                    if (m_Data2.States[i] == null)
                    {
                        m_Data2.States.RemoveAt(i);
                    }
                }
            }
            else
            {
                m_StateDatas2.Clear();
            }
        }

        private void OnSelectedData1()
        {
            var controller = Controller;
            if (m_Data1 != null)
            {
                m_Data1.States.Remove(this);
                controller.States.Remove(this);
            }
            m_Data1 = controller.Editor_GetData(m_DataName1);
            if (m_Data1 != null)
            {
                m_Data1.States.Add(this);
                controller.States.Remove(this);
            }
            if (m_DataName1 == m_DataName2)
            {
                if (m_Data2 != null)
                {
                    m_Data2.States.Remove(this);
                    controller.States.Remove(this);
                }
                m_DataName2 = string.Empty;
                m_Data2 = null;
                RefreshData2();
            }
            RefreshData1();
        }

        private void OnSelectedData2()
        {
            foreach (var dateName in Controller.GetAllDataNames())
            {
                if (dateName != m_DataName1 && dateName == m_DataName2)
                {
                    if (m_Data2 != null)
                    {
                        m_Data2.States.Remove(this);
                    }
                    m_Data2 = Controller.Editor_GetData(dateName);
                    if (m_Data2 != null)
                    {
                        m_Data2.States.Add(this);
                        Controller.States.Remove(this);
                    }
                    RefreshData2();
                    return;
                }
            }
        }
        
        private void OnStateDataBeginGUI1(int selectionIndex)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(m_Data1.StateNames[selectionIndex]);
        }

        private void OnStateDataEndGUI1(int selectionIndex)
        {
            GUILayout.EndHorizontal();
        }

        private void OnStateDataBeginGUI2(int selectionIndex)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(m_Data2.StateNames[selectionIndex]);
        }

        private void OnStateDataEndGUI2(int selectionIndex)
        {
            GUILayout.EndHorizontal();
        }
#endif
    }
}