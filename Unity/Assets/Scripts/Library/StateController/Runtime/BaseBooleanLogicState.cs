using System.Collections.Generic;
using System.Linq;
using Sirenix.Serialization;

namespace StateController
{
    public abstract class BaseBooleanLogicState : BaseSate
    {
        [OdinSerialize]
        private SubStateController m_SubStateController1;
        [OdinSerialize]
        private Dictionary<string, bool> m_StateDataDict1 = new Dictionary<string, bool>();
        [OdinSerialize]
        private BooleanLogicType m_BooleanLogicType;
        [OdinSerialize]
        private SubStateController m_SubStateController2;
        [OdinSerialize]
        private Dictionary<string, bool> m_StateDataDict2 = new Dictionary<string, bool>();

        private string m_CurStateName1;
        private string m_CurStateName2;

        internal override void Refresh()
        {
            if (m_BooleanLogicType == BooleanLogicType.None)
            {
                if (m_SubStateController1.SelectedName == m_CurStateName1)
                    return;
                m_CurStateName1 = m_SubStateController1.SelectedName;
                OnSateChanged(m_StateDataDict1[m_CurStateName1]);
            }
            else
            {
                if (m_SubStateController1.SelectedName == m_CurStateName1 && m_SubStateController2.SelectedName == m_CurStateName2)
                    return;
                m_CurStateName1 = m_SubStateController1.SelectedName;
                m_CurStateName2 = m_SubStateController2.SelectedName;
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
        internal SubStateController SubStateController1
        {
            get => m_SubStateController1;
            set => m_SubStateController1 = value;
        }

        internal SubStateController SubStateController2
        {
            get => m_SubStateController2;
            set => m_SubStateController2 = value;
        }

        internal override void EditorRefresh()
        {
            if (m_SubStateController1 != null)
            {
                foreach (var sateName in m_StateDataDict1.Keys.ToList())
                {
                    if (!m_SubStateController1.StateNames.Contains(sateName))
                    {
                        m_StateDataDict1.Remove(sateName);
                    }
                }
                foreach (var sateName in m_SubStateController1.StateNames)
                {
                    m_StateDataDict1.TryAdd(sateName, default);
                }
            }
            if (m_SubStateController2 != null)
            {
                foreach (var sateName in m_StateDataDict2.Keys.ToList())
                {
                    if (!m_SubStateController2.StateNames.Contains(sateName))
                    {
                        m_StateDataDict2.Remove(sateName);
                    }
                }
                foreach (var sateName in m_SubStateController2.StateNames)
                {
                    m_StateDataDict2.TryAdd(sateName, default);
                }
            }
        }
#endif
    }
}