using Cysharp.Threading.Tasks;
using Game;
using UnityEngine;
using UnityGameFramework.Extension;

namespace ET
{
    [Invoke]
    public class RecastFileReader: AInvokeHandler<NavmeshComponent.RecastFileLoader, UniTask<byte[]>>
    {
        public override async UniTask<byte[]> Handle(NavmeshComponent.RecastFileLoader args)
        {
            TextAsset recastAsset = await GameEntry.Resource.LoadAssetAsync<TextAsset>(AssetUtility.GetETAsset($"ClientServer/Recast/{args.Name}.bytes"));
            return recastAsset.bytes;
        }
    }
}