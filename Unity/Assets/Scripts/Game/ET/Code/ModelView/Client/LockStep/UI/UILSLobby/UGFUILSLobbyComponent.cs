using UnityEngine;

namespace ET.Client
{
    [EnableMethod]
    [ComponentOf(typeof(UGFUIForm))]
    public partial class UGFUILSLobbyComponent : UGFUIForm, IAwake<Transform>, IDestroy, IUGFUIFormOnInit
    {
        
    }
}
