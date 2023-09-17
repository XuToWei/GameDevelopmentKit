using UnityEngine;

namespace ET.Client
{
    [EnableMethod]
    [ComponentOf(typeof(UGFUIForm))]
    public partial class UGFUILobbyComponent : Entity, IAwake<Transform>, IDestroy
    {
        
    }
}
