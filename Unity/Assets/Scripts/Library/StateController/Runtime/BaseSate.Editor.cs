#if UNITY_EDITOR
using System.Collections.Generic;

namespace StateController
{
    public partial class BaseSate
    {
        internal List<StateController> StateControllers { get; private set; } = new List<StateController>();

        private void OnValidate()
        {
            
        }

        internal void RefreshStateController()
        {
            StateControllers.Clear();
            transform.GetComponentsInParent<StateController>(true, StateControllers);
        }
    }
}
#endif