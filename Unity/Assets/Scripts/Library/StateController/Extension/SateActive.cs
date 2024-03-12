using UnityEngine;

namespace StateController
{
    [DisallowMultipleComponent]
    public class SateActive : BaseBooleanLogicState
    {
        protected override void OnStateChanged(bool logicResult)
        {
            gameObject.SetActive(logicResult);
        }
    }
}
