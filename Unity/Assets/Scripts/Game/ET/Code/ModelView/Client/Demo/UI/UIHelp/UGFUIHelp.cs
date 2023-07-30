using UnityEngine;

namespace ET.Client
{
    [EnableMethod]
    [ComponentOf(typeof(UGFUIForm))]
    public partial class UGFUIHelp : Entity, IAwake<Transform>, IDestroy
    {
        
    }
}
