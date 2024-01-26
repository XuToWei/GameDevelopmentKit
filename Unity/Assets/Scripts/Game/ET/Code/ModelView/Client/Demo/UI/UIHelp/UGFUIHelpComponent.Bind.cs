// This is an automatically generated class by CodeBind. Please do not modify it.

namespace ET.Client
{
    public partial class UGFUIHelpComponent : CodeBind.ICSCodeBind
    {
        public CodeBind.CSCodeBindMono Mono { get; private set; }
        public UnityEngine.Transform Transform { get; private set; }

        public UnityEngine.UI.Text DescText { get; private set; }


        public void InitBind(CodeBind.CSCodeBindMono mono)
        {
            Mono = mono;
            Transform = mono.transform;
            DescText = Mono.BindComponents[0] as UnityEngine.UI.Text;
        }

        public void ClearBind()
        {
            Mono = null;
            Transform = null;
            DescText = null;
        }
    }
}
