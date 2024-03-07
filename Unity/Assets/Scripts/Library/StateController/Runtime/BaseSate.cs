using UnityEngine;

namespace StateController
{
    public abstract partial class BaseSate : MonoBehaviour
    {
        [SerializeField]
        private StateController m_StateController1;
        [SerializeField]
        public int m_StateIndex1;
        [SerializeField]
        public BooleanLogicType m_BooleanLogicType;
        [SerializeField]
        private StateController m_StateController2;
        [SerializeField]
        public int m_StateIndex2;

        private bool m_LogicResult;
        internal void Refresh(bool isInit)
        {
            if (m_StateController1 == null)
                return;
            bool logicResult = false;
            switch (m_BooleanLogicType)
            {
                case BooleanLogicType.None:
                    logicResult = m_StateController1.SelectedIndex == m_StateIndex1;
                    break;
                case BooleanLogicType.And:
                    logicResult = m_StateController1.SelectedIndex == m_StateIndex1 &&
                                  m_StateController2.SelectedIndex == m_StateIndex2;
                    break;
                case BooleanLogicType.Or:
                    logicResult = m_StateController1.SelectedIndex == m_StateIndex1 ||
                                  m_StateController2.SelectedIndex == m_StateIndex2;
                    break;
            }
            if (isInit || m_LogicResult != logicResult)
            {
                m_LogicResult = logicResult;
                OnSateChanged(m_LogicResult);
            }
        }

        protected abstract void OnSateChanged(bool logicResult);
    }
}