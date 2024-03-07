using System;
using System.Collections.Generic;

#if UNITY_EDITOR
namespace StateController
{
    public partial class StateController
    {
        private bool ValidateInputName(string controllerName, ref string errorMsg)
        {
            foreach (BaseSate state in m_States)
            {
                foreach (StateController stateController in state.StateControllers)
                {
                    if (stateController != this && stateController.Name == controllerName)
                    {
                        errorMsg = $"Duplicate StateController name : {controllerName}";
                        return false;
                    }
                }
            }
            return true;
        }

        private bool ValidateInputStateNames(List<string> stateNames, ref string errorMsg)
        {
            for (int i = 0; i < stateNames.Count; i++)
            {
                for (int j = i + 1; j < stateNames.Count; j++)
                {
                    if (stateNames[i] == stateNames[j])
                    {
                        errorMsg = $"Duplicate StateName : {stateNames[i]}";
                        return false;
                    }
                }
            }
            return true;
        }

        private List<int> ValueDropdownSelectedIndex
        {
            get
            {
                List<int> values = new List<int>();
                for (int i = 0; i < m_StateNames.Count; i++)
                {
                    values.Add(i);
                }
                return values;
            }
        }

        private void OnValidate()
        {
            m_States.Clear();
            transform.GetComponentsInChildren<BaseSate>(true, m_States);
            foreach (BaseSate state in m_States)
            {
                state.RefreshStateController();
            }
        }
    }
}
#endif