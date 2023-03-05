using UnityEngine;

namespace ET.Client
{
    [EnableMethod]
    [ComponentOf(typeof (UGFUIForm))]
    public partial class UGFUILobby: Entity, IAwake<Transform>, IDestroy
    {
        
    }
}
