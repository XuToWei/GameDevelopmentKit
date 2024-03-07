using UnityEngine;

namespace StateController
{
    public class SateActive : BaseSate
    {
        private GameObject m_GameObject;
        private void Awake()
        {
            m_GameObject = gameObject;
        }

        protected override void OnSateChanged(bool logicResult)
        {
            m_GameObject.SetActive(logicResult);
        }
    }
}
