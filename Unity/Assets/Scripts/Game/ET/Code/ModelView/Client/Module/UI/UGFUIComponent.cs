using System.Collections.Generic;

namespace ET.Client
{
    /// <summary>
    /// 管理Scene上的UI
    /// </summary>
    [ComponentOf]
    [EnableMethod]
    public sealed class UGFUIComponent : Entity, IAwake, IDestroy
    {
        //所有打开的UIForm实体
        public readonly HashSet<EntityRef<UGFUIForm>> AllOpenUIForms = new HashSet<EntityRef<UGFUIForm>>();
    }
}