using UnityEngine;

namespace ET.Client
{
    [EnableMethod]
    [ComponentOf(typeof (UGFUIForm))]
    public partial class UGFUILogin: Entity, IAwake<Transform>, IDestroy
    {
        
    }
}
