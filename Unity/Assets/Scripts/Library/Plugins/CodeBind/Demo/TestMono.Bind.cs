namespace MonoTest
{
    public partial class TestMono
    {
        [UnityEngine.SerializeField] private UnityEngine.Transform _testTransform;

        [UnityEngine.SerializeField] private UnityEngine.Transform _tes2tTransform;

        public UnityEngine.Transform testTransform => _testTransform;

        public UnityEngine.Transform tes2tTransform => _tes2tTransform;

    }
}
