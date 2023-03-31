// This is an automatically generated class by CodeBind. Please do not modify it.

namespace CodeBind.Demo
{
    public partial class TestCS : CodeBind.ICSCodeBind
    {
        public CodeBind.CSCodeBindMono mono { get; private set; }

        public UnityEngine.Transform transform { get; private set; }

        public UnityEngine.Transform selfTransform { get; private set; }

        public UnityEngine.Animator otherAnimator { get; private set; }

        public void InitBind(CodeBind.CSCodeBindMono mono)
        {
            this.mono = mono;
            this.transform = mono.transform;
            this.selfTransform = this.mono.bindComponents[0] as UnityEngine.Transform;
            this.otherAnimator = this.mono.bindComponents[1] as UnityEngine.Animator;
        }

        public void ClearBind()
        {
            this.mono = null;
            this.transform = null;
            this.selfTransform = null;
            this.otherAnimator = null;
        }
    }
}
