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
        internal SubStateController m_SubStateController;
        [OdinSerialize]
        private Dictionary<string, T> m_StateDataDict;

        private string m_CurStateName;

        internal override void Refresh()
        {
            if (m_CurStateName == m_SubStateController.SelectedName)
                return;
            m_CurStateName = m_SubStateController.SelectedName;
            m_StateDataDict[m_SubStateController.SelectedName].Apply();
        }

#if UNITY_EDITOR
        internal override void EditorRefresh()
        {
            if (m_SubStateController != null)
            {
                foreach (var sateName in m_StateDataDict.Keys.ToList())
                {
                    if (!m_SubStateController.StateNames.Contains(sateName))
                    {
                        m_StateDataDict.Remove(sateName);
                    }
                }
                foreach (var sateName in m_SubStateController.StateNames)
                {
                    m_StateDataDict.TryAdd(sateName, default);
                }
            }
        }
#endif
    }
}