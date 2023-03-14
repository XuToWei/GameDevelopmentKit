using System.Collections.Generic;
using UGFUICompoent = UnityGameFramework.Runtime.UIComponent;

namespace ET.Client
{
    [ComponentOf(typeof(Scene))]
    public sealed class UIComponent : Entity, IAwake, IDestroy
    {
        [StaticField]
        public static UIComponent Instance;

        //所有打开的UIForm实体
        public readonly HashSet<UGFUIForm> AllOpenUIForms = new HashSet<UGFUIForm>();
    }
}