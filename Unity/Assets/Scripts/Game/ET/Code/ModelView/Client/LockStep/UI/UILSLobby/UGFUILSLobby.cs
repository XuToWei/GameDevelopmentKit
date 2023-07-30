using UnityEngine;

namespace ET.Client
{
    [EnableMethod]
    [ComponentOf(typeof(UGFUIForm))]
    public partial class UGFUILSLobby : Entity, IAwake<Transform>, IDestroy
    {
        
    }
}
