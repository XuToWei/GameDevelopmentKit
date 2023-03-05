namespace ET.Client
{
    public partial class UGFUILobby : CodeBind.ICSCodeBind
    {
        public CodeBind.CSCodeBindMono mono { get; private set; }

        public UnityEngine.Transform transform { get; private set; }

        public UnityEngine.UI.Button EnterMapButton { get; private set; }

        public void InitBind(CodeBind.CSCodeBindMono mono)
        {
            this.mono = mono;
            this.transform = mono.transform;
            this.EnterMapButton = this.mono.bindComponents[0] as UnityEngine.UI.Button;
        }

        public void ClearBind()
        {
            this.mono = null;
            this.transform = null;
            this.EnterMapButton = null;
        }
    }
}
