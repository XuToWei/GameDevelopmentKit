using Cysharp.Threading.Tasks;
using HybridCLR;
using UnityEngine;
using UnityGameFramework.Extension;

namespace Game
{
    public static class HybridCLRHelper
    {
        public static readonly string ConfigAsset = "Assets/Res/Code/HybridCLR/HybridCLRConfig.asset";
        
        public static async UniTask LoadAsync()
        {
            HybridCLRConfig aotGroup = await GameEntry.Resource.LoadAssetAsync<HybridCLRConfig>(ConfigAsset);
            foreach (TextAsset textAsset in aotGroup.AOTAssemblies)
            {
                RuntimeApi.LoadMetadataForAOTAssembly(textAsset.bytes, HomologousImageMode.Consistent);
            }
        }
    }

    [CreateAssetMenu(menuName = "Game/Create HybridCLRConfig", fileName = "HybridCLRConfig", order = 0)]
    public class HybridCLRConfig : ScriptableObject
    {
        public TextAsset[] AOTAssemblies;
    }
}