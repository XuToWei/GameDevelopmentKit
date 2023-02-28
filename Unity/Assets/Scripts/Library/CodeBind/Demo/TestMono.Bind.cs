namespace CodeBind.Demo
{
    public partial class TestMono
    {
        [UnityEngine.SerializeField] private UnityEngine.Transform _testTransform;

        [UnityEngine.SerializeField] private UnityEngine.Animator _testAnimator;

        public UnityEngine.Transform testTransform => _testTransform;

        public UnityEngine.Animator testAnimator => _testAnimator;

    }
}
