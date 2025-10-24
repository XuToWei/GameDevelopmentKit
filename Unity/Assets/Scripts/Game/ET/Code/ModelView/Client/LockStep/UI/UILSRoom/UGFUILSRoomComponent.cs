using UnityEngine;

namespace ET.Client
{
    [EnableMethod]
    [ComponentOf(typeof(UGFUIForm))]
    public partial class UGFUILSRoomComponent : UGFUIForm, IUGFUIFormOnInit
    {
        public int frame;
        public int predictFrame;
    }
}
