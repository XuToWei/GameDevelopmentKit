using UnityEngine;

namespace ET.Client
{
    [EnableMethod]
    [ComponentOf(typeof(UGFUIForm))]
    public partial class UGFUILoginComponent : Entity, IAwake<Transform>, IDestroy
    {
        public EntityRef<UGFUIWidget> uiWidgetTest;
    }
}
