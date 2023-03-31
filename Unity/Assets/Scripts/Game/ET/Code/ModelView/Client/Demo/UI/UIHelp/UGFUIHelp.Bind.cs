// This is an automatically generated class by CodeBind. Please do not modify it.

namespace ET.Client
{
    public partial class UGFUIHelp : CodeBind.ICSCodeBind
    {
        public CodeBind.CSCodeBindMono mono { get; private set; }

        public UnityEngine.Transform transform { get; private set; }

        public UnityEngine.UI.Text descText { get; private set; }

        public void InitBind(CodeBind.CSCodeBindMono mono)
        {
            this.mono = mono;
            this.transform = mono.transform;
            this.descText = this.mono.bindComponents[0] as UnityEngine.UI.Text;
        }

        public void ClearBind()
        {
            this.mono = null;
            this.transform = null;
            this.descText = null;
        }
    }
}
