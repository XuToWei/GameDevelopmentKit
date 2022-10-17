namespace ET
{
    public partial class FightForm
    {
        [UnityEngine.SerializeField] private ET.EnemyView m_EnemyView;
        [UnityEngine.SerializeField] private UnityEngine.UI.Image m_Weapon;
        public ET.EnemyView EnemyView => m_EnemyView;
        public UnityEngine.UI.Image Weapon => m_Weapon;
    }
}
