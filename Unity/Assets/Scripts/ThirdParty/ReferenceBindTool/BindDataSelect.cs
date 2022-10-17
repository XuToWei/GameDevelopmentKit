using UnityEngine;

namespace ReferenceBindTool.Runtime
{
    public class BindDataSelect : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private Component[] m_BindComponents;

        public Component[] BindComponents
        {
            get => m_BindComponents;
        }
#endif
    }
}
