using System.Collections.Generic;

namespace ET.Client
{
    /// <summary>
    /// 管理Scene上的UI
    /// </summary>
    [ComponentOf]
    public sealed class UIComponent : Entity, IAwake, IDestroy
    {
        //所有打开的UIForm实体
        public readonly HashSet<UGFUIForm> AllOpenUIForms = new HashSet<UGFUIForm>();
    }
}