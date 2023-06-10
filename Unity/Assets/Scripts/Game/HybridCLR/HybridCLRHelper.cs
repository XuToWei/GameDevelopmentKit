using Cysharp.Threading.Tasks;
using HybridCLR;
using UnityEngine;
using UnityGameFramework.Extension;
using UnityGameFramework.Runtime;

namespace Game
{
    public static class HybridCLRHelper
    {
        public static readonly string ConfigAsset = "Assets/Res/HybridCLR/HybridCLRConfig.asset";
        
        public static async UniTask LoadAsync()
        {
            HybridCLRConfig aotGroup = await GameEntry.Resource.LoadAssetAsync<HybridCLRConfig>(ConfigAsset);
            foreach (TextAsset textAsset in aotGroup.aotAssemblies)
            {
                RuntimeApi.LoadMetadataForAOTAssembly(textAsset.bytes, HomologousImageMode.Consistent);
            }
            GameEntry.Resource.UnloadAsset(aotGroup);
            Log.Info("HybridCLR load metadata.");
        }
    }
}