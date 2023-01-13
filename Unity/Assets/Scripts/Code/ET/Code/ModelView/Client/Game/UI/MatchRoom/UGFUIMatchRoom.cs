using UnityEngine;

namespace ET.Client
{
    [ComponentOf(typeof(UGFUIForm))]
    public class UGFUIMatchRoom: Entity, IAwake<Transform>, IDestroy
    {
        /// <summary>
        /// UI的根节点
        /// </summary>
        public Transform transform;

        public UGFUIMatchRoomView view;
    }
}
