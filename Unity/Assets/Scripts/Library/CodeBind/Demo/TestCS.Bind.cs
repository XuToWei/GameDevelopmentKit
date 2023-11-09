// This is an automatically generated class by CodeBind. Please do not modify it.

namespace CodeBind.Demo
{
    public partial class TestCS : CodeBind.ICSCodeBind
    {
        public CodeBind.CSCodeBindMono mono { get; private set; }
        public UnityEngine.Transform transform { get; private set; }

        public UnityEngine.Animation otherAnimation { get; private set; }
        public UnityEngine.Transform selfTransform { get; private set; }

        public UnityEngine.Animation[] listAnimationArray { get; private set; }

        public void InitBind(CodeBind.CSCodeBindMono mono)
        {
            this.mono = mono;
            this.transform = mono.transform;

            this.otherAnimation = this.mono.bindComponents[0] as UnityEngine.Animation;
            this.selfTransform = this.mono.bindComponents[1] as UnityEngine.Transform;
            this.listAnimationArray = new UnityEngine.Animation[4]
            {
                this.mono.bindComponents[2] as UnityEngine.Animation,
                this.mono.bindComponents[3] as UnityEngine.Animation,
                this.mono.bindComponents[4] as UnityEngine.Animation,
                this.mono.bindComponents[5] as UnityEngine.Animation,
            };
        }

        public void ClearBind()
        {
            this.mono = null;
            this.transform = null;
            this.otherAnimation = null;
            this.selfTransform = null;
            this.listAnimationArray = null;
        }
    }
}
