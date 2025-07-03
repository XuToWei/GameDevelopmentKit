using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "Game/Create HybridCLRConfig", fileName = "HybridCLRConfig", order = 0)]
    public class HybridCLRConfig : ScriptableObject
    {
        [SerializeField]
        private TextAsset[] m_AotAssemblies;

        public TextAsset[] AotAssemblies => m_AotAssemblies;

        public void SetAotAssemblies(TextAsset[] aotAssemblies)
        {
            m_AotAssemblies = aotAssemblies;
        }

        public void SetAotAssemblies(List<TextAsset> aotAssemblies)
        {
            m_AotAssemblies = aotAssemblies.ToArray();
        }
    }
}
