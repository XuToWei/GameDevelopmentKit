using System.Collections.Generic;
using UGFUICompoent = UnityGameFramework.Runtime.UIComponent;

namespace ET.Client
{
    public sealed class UIComponent : Entity, IAwake, IDestroy
    {
        [StaticField]
        public static UIComponent Instance;

        //所有的UIForm实体
        public readonly HashSet<UGFUIForm> UIForms = new HashSet<UGFUIForm>();
    }
}