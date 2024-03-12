using System;
using UnityEngine;

namespace StateController
{
    [ExecuteAlways]
    public abstract class BaseState : MonoBehaviour
    {
        internal abstract void OnInit(StateController controller);
        internal abstract void OnRefresh();

        internal virtual void Awake()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                return;
            var controller = Controller;
            if (controller == null)
            {
                throw new Exception($"State '{gameObject.name}' must require a parent component 'StateController'!");
            }
            if (!controller.States.Contains(this))
            {
                controller.Editor_Refresh();
            }
#endif
        }

#if UNITY_EDITOR
        internal StateController Controller => GetComponentInParent<StateController>(true);
        internal abstract void Editor_OnRefresh();
        internal abstract void Editor_OnDataRename(string oldDataName, string newDataName);
        internal abstract void Editor_OnDataRemoveState(string dataName, int index);
#endif
    }
}