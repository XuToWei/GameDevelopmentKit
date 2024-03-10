using UnityEngine;

namespace StateController
{
    public class SateActive : BaseBooleanLogicState
    {
        private GameObject m_GameObject;

        protected override void OnSateChanged(bool logicResult)
        {
            m_GameObject.SetActive(logicResult);
        }
    }
}
