using System;
using UnityEngine;

namespace StateController
{
    [ExecuteAlways]
    public abstract class BaseState : MonoBehaviour
    {
        internal abstract void OnInit(StateController controller);
        internal abstract void OnRefresh();

        internal virtual void OnEnable()
        {
#if UNITY_EDITOR
            if (Controller == null)
            {
                throw new Exception($"State '{gameObject.name}' must require a parent component 'StateController'!");
            }
            if (!Controller.States.Contains(this))
            {
                Controller.States.Add(this);
            }
#endif
        }

#if UNITY_EDITOR
        internal StateController Controller => GetComponentInParent<StateController>(true);
        internal abstract void Editor_OnRefresh();
        internal abstract void Editor_OnDataReanme(string oldDataName, string newDataName);
        internal abstract void Editor_OnRemoveStateAt(int index);
#endif
    }
}