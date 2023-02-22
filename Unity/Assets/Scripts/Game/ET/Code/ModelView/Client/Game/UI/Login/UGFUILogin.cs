using UnityEngine;

namespace ET.Client
{
    [ComponentOf(typeof(UGFUIForm))]
    public class UGFUILogin: Entity, IAwake<Transform>, IDestroy
    {
        /// <summary>
        /// UI的根节点
        /// </summary>
        public Transform transform;

        public UGFUILoginView view;
    }
}
