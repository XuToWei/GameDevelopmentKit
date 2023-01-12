namespace CodeBind.Demo
{
    public partial class TestCS : CodeBind.ICSCodeBind
    {
        public UnityEngine.Transform selfTransform {get; private set;}

        public void InitBind(CodeBind.CSMonoBind csMonoBind)
        {
            this.selfTransform = csMonoBind.BindComponents[0] as UnityEngine.Transform;
        }
    }
}
