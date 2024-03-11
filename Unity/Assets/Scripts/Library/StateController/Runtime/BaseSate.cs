using UnityEngine;

namespace StateController
{
    public abstract class BaseSate : MonoBehaviour
    {
        internal abstract void Refresh();

#if UNITY_EDITOR
        protected StateController m_StateController => GetComponentInParent<StateController>(true);

        internal abstract void EditorRefresh();

        private void OnValidate()
        {
            EditorRefresh();
        }
#endif
    }
}