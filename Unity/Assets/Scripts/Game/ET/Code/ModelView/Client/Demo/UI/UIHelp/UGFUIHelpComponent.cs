using UnityEngine;

namespace ET.Client
{
    [EnableMethod]
    [ComponentOf(typeof(UGFUIForm))]
    public partial class UGFUIHelpComponent : Entity, IAwake<Transform>, IDestroy
    {
        
    }
}
