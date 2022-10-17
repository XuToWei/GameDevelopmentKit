using System.Collections.Generic;
using UGF;

namespace ET.Client
{
    /// <summary>
    /// 管理Scene上的UI
    /// </summary>
    [ComponentOf(typeof (Scene))]
    public class UIComponent: Entity, IAwake
    {
        public Dictionary<UIFormId, List<UI>> UIs { get; } = new();
    }
}