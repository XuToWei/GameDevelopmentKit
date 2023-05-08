using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityGameFramework.Extension;

namespace Game
{
    public static partial class SetTextureExtension
    {
        public static async UniTask<bool> SetTextureByNetworkAsync(this RawImage rawImage, string file, string saveFilePath = null, CancellationToken cancellationToken = default)
        {
            return await GameEntry.TextureSet.SetTextureByNetworkAsync(SetRawImage.Create(rawImage, file), saveFilePath, cancellationToken);
        }

        public static async UniTask SetTextureByResourcesAsync(this RawImage rawImage, string file, CancellationToken cancellationToken = default)
        {
            await GameEntry.TextureSet.SetTextureByResourcesAsync(SetRawImage.Create(rawImage, file), cancellationToken);
        }

        public static void SetTextureByFileSystem(this RawImage rawImage, string file)
        {
            GameEntry.TextureSet.SetTextureByFileSystem(SetRawImage.Create(rawImage, file));
        }

        public static int SetTextureByNetwork(this RawImage rawImage, string file, string saveFilePath = null)
        {
            return GameEntry.TextureSet.SetTextureByNetwork(SetRawImage.Create(rawImage, file), saveFilePath);
        }

        public static int SetTextureByResources(this RawImage rawImage, string file)
        {
            return GameEntry.TextureSet.SetTextureByResources(SetRawImage.Create(rawImage, file));
        }
    }
}