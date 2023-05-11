using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "Game/Create HybridCLRConfig", fileName = "HybridCLRConfig", order = 0)]
    public class HybridCLRConfig : ScriptableObject
    {
        [SerializeField]
        public TextAsset[] aotAssemblies;
    }
}
