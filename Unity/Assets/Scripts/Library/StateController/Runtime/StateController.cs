using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StateController
{
    public sealed class StateController : MonoBehaviour
    {
        [SerializeField]
        [LabelText("Controller Info")]
        [PropertyOrder(99)]
        [ShowInInspector]
        [ReadOnly]
        private List<SubStateController> m_Controllers = new List<SubStateController>();
        private Dictionary<string, SubStateController> m_ControllerDict;

        private void Awake()
        {
            m_ControllerDict = new Dictionary<string, SubStateController>();
            foreach (var subStateController in m_Controllers)
            {
                m_ControllerDict.Add(subStateController.ControllerName, subStateController);
            }
        }

        public void SelectedName(string controllerName, string stateName)
        {
            m_ControllerDict[controllerName].SelectedName = stateName;
        }

        public string GetSelectedName(string controllerName)
        {
            return m_ControllerDict[controllerName].SelectedName;
        }
        
        public SubStateController GetController(string controllerName)
        {
            return m_ControllerDict[controllerName];
        }

#if UNITY_EDITOR
        internal List<SubStateController> Controllers => m_Controllers;

        private const string ADD_NAME = "Add Controller";

        [BoxGroup(ADD_NAME)]
        [LabelText("Controller Name")]
        [PropertyOrder(10)]
        [ShowInInspector]
        [ValidateInput("ValidateInputNewControllerName")]
        private string m_NewControllerName;

        [BoxGroup(ADD_NAME)]
        [GUIColor(0, 1, 0)]
        [Button("Add")]
        [PropertyOrder(11)]
        [EnableIf("CheckCanAddController")]
        private void AddController()
        {
            if (!CheckCanAddController())
                return;
            SubStateController subStateController = new SubStateController();
            subStateController.ControllerName = m_NewControllerName;
            m_Controllers.Add(subStateController);
            m_NewControllerName = string.Empty;
            EditorRefresh();
        }

        private const string SELECT_NAME = "Select Controller";

        [BoxGroup(SELECT_NAME)]
        [LabelText("Controller Name")]
        [PropertyOrder(20)]
        [ShowInInspector]
        [ValueDropdown("GetControllerNames")]
        [OnValueChanged("OnSelectedController")]
        private string m_SelectedControllerName = string.Empty;

        private SubStateController m_SelectedController;

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
            if(m_SelectedStateNames.Contains(m_NewStateName))
                return;
            m_SelectedStateNames.Add(m_NewStateName);
            m_SelectedController.StateNames.Add(m_NewStateName);
            m_NewStateName = string.Empty;
            EditorRefresh();
        }

        [BoxGroup(SELECT_NAME + "/State")]
        [LabelText("State Names")]
        [PropertyOrder(23)]
        [ShowInInspector]
        [ReadOnly]
        [EnableIf("IsSelectedController")]
        [OnValueChanged("OnSelectedStateNamesChange")]
        [ListDrawerSettings(DefaultExpandedState = true,
            OnBeginListElementGUI = "OnStateNameBeginGUI",
            OnEndListElementGUI = "OnStateNameEndGUI")]
        private readonly List<string> m_SelectedStateNames = new List<string>();

        [BoxGroup(SELECT_NAME + "/Rename Controller")]
        [LabelText("Controller Name")]
        [PropertyOrder(24)]
        [ShowInInspector]
        [EnableIf("IsSelectedController")]
        [ValidateInput("ValidateRenameControllerName")]
        private string m_RenameControllerName;

        [BoxGroup(SELECT_NAME + "/Rename Controller")]
        [GUIColor(0,1,0)]
        [Button("Rename")]
        [PropertyOrder(25)]
        [EnableIf("IsSelectedController")]
        private void RenameSelectedControllerName()
        {
            if (string.IsNullOrEmpty(m_RenameControllerName))
                return;
            if (m_RenameControllerName == m_SelectedController.ControllerName)
                return;
            foreach (var controller in m_Controllers)
            {
                if(controller == m_SelectedController)
                    continue;
                if (controller.ControllerName == m_RenameControllerName)
                {
                    return;
                }
            }
            m_SelectedController.ControllerName = m_RenameControllerName;
            m_SelectedControllerName = m_RenameControllerName;
        }

        [BoxGroup(SELECT_NAME)]
        [GUIColor(1,1,0)]
        [Button("Remove Controller")]
        [PropertyOrder(30)]
        [EnableIf("IsSelectedController")]
        private void RemoveSelectedController()
        {
            m_Controllers.Remove(m_SelectedController);
            m_SelectedController = null;
            m_SelectedControllerName = string.Empty;
            m_RenameControllerName = string.Empty;
            m_SelectedStateNames.Clear();
            EditorRefresh();
        }

        private bool ValidateInputNewControllerName(string controllerName, ref string errorMsg)
        {
            if (string.IsNullOrEmpty(controllerName))
            {
                return true;
            }
            foreach (var subStateController in m_Controllers)
            {
                if (subStateController.ControllerName == controllerName)
                {
                    errorMsg = $"A controller name '{controllerName}' already exists!";
                    return false;
                }
            }
            return true;
        }

        private bool CheckCanAddController()
        {
            if (string.IsNullOrEmpty(m_NewControllerName))
                return false;
            foreach (var controller in m_Controllers)
            {
                if (controller.ControllerName == m_NewControllerName)
                    return false;
            }
            return true;
        }

        private bool IsSelectedController()
        {
            if (string.IsNullOrEmpty(m_SelectedControllerName))
                return false;
            foreach (var controller in m_Controllers)
            {
                if (controller.ControllerName == m_SelectedControllerName)
                    return true;
            }
            return false;
        }

        private readonly List<string> m_ControllerNames = new List<string>();
        public List<string> GetControllerNames()
        {
            m_ControllerNames.Clear();
            foreach (var controller in m_Controllers)
            {
                m_ControllerNames.Add(controller.ControllerName);
            }
            m_ControllerNames.Sort();
            return m_ControllerNames;
        }

        private void OnSelectedController()
        {
            m_SelectedController = null;
            foreach (var controller in m_Controllers)
            {
                if (controller.ControllerName == m_SelectedControllerName)
                {
                    m_SelectedController = controller;
                    m_SelectedStateNames.Clear();
                    foreach (var stateName in m_SelectedController.StateNames)
                    {
                        m_SelectedStateNames.Add(stateName);
                    }
                    m_RenameControllerName = m_SelectedControllerName;
                    break;
                }
            }
        }

        private bool ValidateInputNewStateName(string stateName, ref string errMsg)
        {
            if (m_SelectedController == null)
                return true;
            if (string.IsNullOrEmpty(stateName))
                return true;
            if (m_SelectedStateNames.Contains(stateName))
            {
                errMsg = $"A state name '{stateName}' already exists!";
                return false;
            }
            return true;
        }

        private bool ValidateRenameControllerName(string controllerName, ref string errMsg)
        {
            if (m_SelectedController == null || m_SelectedController.ControllerName == controllerName)
                return true;
            if (string.IsNullOrEmpty(controllerName))
            {
                errMsg = "Controller name can't be empty!";
                return false;
            }
            foreach (var controller in m_Controllers)
            {
                if(controller == m_SelectedController)
                    continue;
                if (controller.ControllerName == controllerName)
                {
                    errMsg = $"A controller name '{controllerName}' already exists!";
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
            GUI.enabled = m_SelectedController.SelectedName != m_SelectedStateNames[selectionIndex];
            if (GUILayout.Button("Apply"))
            {
                m_SelectedController.SelectedName = m_SelectedStateNames[selectionIndex];
            }
            GUI.enabled = true;
            if (GUILayout.Button("X"))
            {
                m_SelectedStateNames.RemoveAt(selectionIndex);
                m_SelectedController.StateNames.RemoveAt(selectionIndex);
            }
            GUILayout.EndHorizontal();
        }

        private void OnSelectedStateNamesChange()
        {
            m_SelectedController.StateNames.Clear();
            foreach (var stateName in m_SelectedStateNames)
            {
                Debug.Log(stateName);
                m_SelectedController.StateNames.Add(stateName);
            }
        }

        private void EditorRefresh()
        {
            foreach (var sate in GetComponentsInChildren<BaseSate>(true))
            {
                sate.EditorRefresh();
            }
        }

        private void OnValidate()
        {
            EditorRefresh();
        }
#endif
    }
}