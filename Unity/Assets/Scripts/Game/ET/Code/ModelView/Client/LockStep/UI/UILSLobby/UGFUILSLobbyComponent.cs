using UnityEngine;

namespace ET.Client
{
    [EnableMethod]
    [ComponentOf(typeof(UGFUIForm))]
    public partial class UGFUILSLobbyComponent : Entity, IAwake<Transform>, IDestroy
    {
        
    }
}
