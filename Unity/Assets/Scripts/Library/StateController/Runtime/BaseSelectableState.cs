using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StateController
{
    public abstract class BaseSelectableState<T> : BaseState where T : BaseStateData
    {
        [SerializeField]
        private string m_DataName;
        [SerializeField]
        private List<T> m_StateDatas;

        private string m_CurStateName;
        private Dictionary<string, T> m_StateDataDict;
        private StateControllerData m_Data;

        internal override void OnInit(StateController controller)
        {
            m_Data = controller.GetData(m_DataName);
            if (m_Data != null)
            {
                m_StateDataDict = new Dictionary<string, T>();
                for (int i = 0; i < m_Data.StateNames.Count; i++)
                {
                    m_StateDataDict.Add(m_Data.StateNames[i], m_StateDatas[i]);
                }
            }
        }

        internal override void OnRefresh()
        {
            if (m_CurStateName == this.m_Data.SelectedName)
                return;
            m_CurStateName = this.m_Data.SelectedName;
            m_StateDataDict[this.m_Data.SelectedName].Apply();
        }

#if UNITY_EDITOR
        internal override void Editor_OnRefresh()
        {
            if (this.m_Data != null)
            {
                foreach (var sateName in m_StateDataDict.Keys.ToList())
                {
                    if (!this.m_Data.StateNames.Contains(sateName))
                    {
                        m_StateDataDict.Remove(sateName);
                    }
                }
                foreach (var sateName in this.m_Data.StateNames)
                {
                    m_StateDataDict.TryAdd(sateName, default);
                }
            }
        }

        internal override void Editor_OnDataReanme(string oldDataName, string newDataName)
        {
            if (m_DataName == oldDataName)
            {
                m_DataName = newDataName;
            }
        }
#endif
    }
}