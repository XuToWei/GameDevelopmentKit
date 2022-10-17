namespace ET
{
    public partial class EnemyView
    {
        [UnityEngine.SerializeField] private UnityEngine.SpriteRenderer m_Bg;
        [UnityEngine.SerializeField] private TMPro.TextMeshPro m_Count;
        [UnityEngine.SerializeField] private UnityEngine.SpriteRenderer m_Buff;
        public UnityEngine.SpriteRenderer Bg => m_Bg;
        public TMPro.TextMeshPro Count => m_Count;
        public UnityEngine.SpriteRenderer Buff => m_Buff;
    }
}
