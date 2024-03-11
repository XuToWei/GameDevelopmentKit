using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StateController
{
    [ExecuteAlways]
    public abstract class BaseBooleanLogicState : BaseSate
    {
        private const string CONTROLLER_1 = "Controller1";
        private const string CONTROLLER_2 = "Controller2";

        [SerializeField]
        [HideInInspector]
        private SubStateController m_Controller1;

        [SerializeField]
        [BoxGroup(CONTROLLER_1)]
        [LabelText("State Names")]
        [PropertyOrder(11)]
        [ShowInInspector]
        [EnableIf("IsSelectedController1")]
        [ListDrawerSettings(DefaultExpandedState = true,
            HideAddButton = true, HideRemoveButton = true,
            DraggableItems = false,
            OnBeginListElementGUI = "OnStateNameBeginGUI1",
            OnEndListElementGUI = "OnStateNameEndGUI1")]
        private List<bool> m_StateData1 = new List<bool>();

        [SerializeField, PropertyOrder(20)]
        private BooleanLogicType m_BooleanLogicType;

        [SerializeField]
        [HideInInspector]
        private SubStateController m_Controller2;

        [SerializeField]
        [BoxGroup(CONTROLLER_2)]
        [LabelText("State Names")]
        [PropertyOrder(31)]
        [ShowInInspector]
        [ShowIf("CanShowController2")]
        [EnableIf("IsSelectedController2")]
        [ListDrawerSettings(DefaultExpandedState = true,
            HideAddButton = true, HideRemoveButton = true,
            DraggableItems = false,
            OnBeginListElementGUI = "OnStateNameBeginGUI2",
            OnEndListElementGUI = "OnStateNameEndGUI2")]
        private List<bool> m_StateData2 = new List<bool>();

        private string m_CurStateName1;
        private string m_CurStateName2;
        private Dictionary<string, bool> m_StateDataDict1;
        private Dictionary<string, bool> m_StateDataDict2;

        protected virtual void Awake()
        {
            if (m_Controller1 != null)
            {
                m_StateDataDict1 = new Dictionary<string, bool>();
                for (int i = 0; i < m_Controller1.StateNames.Count; i++)
                {
                    m_StateDataDict1.Add(m_Controller1.StateNames[i], m_StateData1[i]);
                }
            }
            if (m_BooleanLogicType != BooleanLogicType.None && m_Controller2 != null)
            {
                m_StateDataDict2 = new Dictionary<string, bool>();
                for (int i = 0; i < m_Controller2.StateNames.Count; i++)
                {
                    m_StateDataDict2.Add(m_Controller2.StateNames[i], m_StateData2[i]);
                }
            }
        }

        internal override void Refresh()
        {
            if (m_BooleanLogicType == BooleanLogicType.None)
            {
                if (!string.IsNullOrEmpty(m_CurStateName1) && m_Controller1.SelectedName == m_CurStateName1)
                    return;
                m_CurStateName1 = m_Controller1.SelectedName;
                OnSateChanged(m_StateDataDict1[m_CurStateName1]);
            }
            else
            {
                if (!string.IsNullOrEmpty(m_CurStateName1) && !string.IsNullOrEmpty(m_CurStateName2) && 
                    m_Controller1.SelectedName == m_CurStateName1 && m_Controller2.SelectedName == m_CurStateName2)
                    return;
                m_CurStateName1 = m_Controller1.SelectedName;
                m_CurStateName2 = m_Controller2.SelectedName;
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
        [BoxGroup(CONTROLLER_1)]
        [LabelText("Controller Name")]
        [PropertyOrder(10)]
        [ShowInInspector]
        [ValueDropdown("GetControllerNames1")]
        [OnValueChanged("OnSelectedController1")]
        private string m_ControllerName1 = string.Empty;
        
        [BoxGroup(CONTROLLER_2)]
        [LabelText("Controller Name")]
        [PropertyOrder(30)]
        [ShowInInspector]
        [ShowIf("CanShowController2")]
        [ValueDropdown("GetControllerNames2")]
        [OnValueChanged("OnSelectedController2")]
        private string m_ControllerName2 = string.Empty;

        private bool IsSelectedController1()
        {
            return m_Controller1 != null;
        }

        private bool IsSelectedController2()
        {
            return m_Controller2 != null && m_BooleanLogicType != BooleanLogicType.None;
        }

        private bool CanShowController2()
        {
            return m_BooleanLogicType != BooleanLogicType.None;
        }

        private List<string> GetControllerNames1()
        {
            return m_StateController.GetControllerNames();
        }
        
        private List<string> GetControllerNames2()
        {
            var names = m_StateController.GetControllerNames();
            names.Remove(m_Controller1.ControllerName);
            return names;
        }

        private void RefreshController1()
        {
            if (m_Controller1 != null)
            {
                if (m_StateDataDict1 == null)
                {
                    m_StateDataDict1 = new Dictionary<string, bool>();
                }
                m_StateDataDict1.Clear();
                for (int i = 0; i < m_Controller1.StateNames.Count; i++)
                {
                    m_StateDataDict1.Add(m_Controller1.StateNames[i], m_StateData1[i]);
                }
            }
            else
            {
                if (m_StateDataDict1 != null)
                {
                    m_StateDataDict1.Clear();
                }
            }
        }
        
        private void RefreshController2()
        {
            if (m_Controller2 != null)
            {
                if (m_StateDataDict2 == null)
                {
                    m_StateDataDict2 = new Dictionary<string, bool>();
                }
                m_StateDataDict2.Clear();
                for (int i = 0; i < m_Controller2.StateNames.Count; i++)
                {
                    m_StateDataDict2.Add(m_Controller2.StateNames[i], m_StateData2[i]);
                }
            }
            else
            {
                if (m_StateDataDict2 != null)
                {
                    m_StateDataDict2.Clear();
                }
            }
        }

        private void OnSelectedController1()
        {
            SubStateController controller1 = null;
            foreach (var controller in m_StateController.Controllers)
            {
                if (controller.ControllerName == m_ControllerName1)
                {
                    controller1 = controller;
                    controller1.AddState(this);
                }
            }
            if (controller1 == null && m_Controller1 != null)
            {
                m_Controller1.RemoveState(this);
            }
            else
            {
                m_Controller1 = controller1;
            }
            if (m_ControllerName1 == m_ControllerName2)
            {
                if (m_Controller2 != null)
                {
                    m_Controller2.RemoveState(this);
                }
                m_ControllerName2 = string.Empty;
                m_Controller2 = null;
            }
            RefreshController1();
            RefreshController2();
        }

        private void OnSelectedController2()
        {
            foreach (var controller in m_StateController.Controllers)
            {
                if (controller.ControllerName != m_ControllerName1 && controller.ControllerName == m_ControllerName2)
                {
                    if (m_Controller2 != null)
                    {
                        m_Controller2.RemoveState(this);
                    }
                    m_Controller2 = controller;
                    m_Controller2.AddState(this);
                    return;
                }
            }
            RefreshController2();
        }
        
        private void OnStateNameBeginGUI1(int selectionIndex)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(m_Controller1.StateNames[selectionIndex]);
        }

        private void OnStateNameEndGUI1(int selectionIndex)
        {
            GUILayout.EndHorizontal();
        }

        private void OnStateNameBeginGUI2(int selectionIndex)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(m_Controller2.StateNames[selectionIndex]);
        }

        private void OnStateNameEndGUI2(int selectionIndex)
        {
            GUILayout.EndHorizontal();
        }

        internal override void EditorRefresh()
        {
            if (m_Controller1 != null)
            {
                for (int i = m_StateData1.Count; i < m_Controller1.StateNames.Count; i++)
                {
                    m_StateData1.Add(default);
                }
                for (int i = m_Controller1.StateNames.Count; i < m_StateData1.Count; i++)
                {
                    m_StateData1.RemoveAt(i);
                }
            }
            if (m_Controller2 != null)
            {
                for (int i = m_StateData2.Count; i < m_Controller2.StateNames.Count; i++)
                {
                    m_StateData2.Add(default);
                }
                for (int i = m_Controller2.StateNames.Count; i < m_StateData2.Count; i++)
                {
                    m_StateData2.RemoveAt(i);
                }
            }
        }
#endif
    }
}