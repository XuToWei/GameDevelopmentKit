using System.Collections.Generic;
using System.Linq;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

namespace StateController
{
    public abstract class BaseSelectableState<T> : BaseSate where T : BaseStateData
    {
        [FormerlySerializedAs("m_StateControllerData")] [SerializeField]
        internal SubStateController m_Controller;
        [OdinSerialize]
        private Dictionary<string, T> m_StateDataDict;

        private string m_CurStateName;

        internal override void Refresh()
        {
            if (m_CurStateName == m_Controller.SelectedName)
                return;
            m_CurStateName = m_Controller.SelectedName;
            m_StateDataDict[m_Controller.SelectedName].Apply();
        }

#if UNITY_EDITOR
        internal override void EditorRefresh()
        {
            if (m_Controller != null)
            {
                foreach (var sateName in m_StateDataDict.Keys.ToList())
                {
                    if (!m_Controller.StateNames.Contains(sateName))
                    {
                        m_StateDataDict.Remove(sateName);
                    }
                }
                foreach (var sateName in m_Controller.StateNames)
                {
                    m_StateDataDict.TryAdd(sateName, default);
                }
            }
        }
#endif
    }
}