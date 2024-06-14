using UnityEngine;

namespace ET.Client
{
    [EnableMethod]
    [ComponentOf(typeof(UGFUIWidget))]
    public partial class UGFUIWidgetTest : Entity, IAwake<Transform>, IDestroy
    {
        
    }
}