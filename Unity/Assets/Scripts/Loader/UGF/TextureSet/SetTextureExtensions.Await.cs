using ET;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace UGF
{
    public static partial class SetTextureExtensions
    {
        public static void SetTextureByNetworkAsync(this RawImage rawImage, string file, string saveFilePath = null,ETCancellationToken cancellationToken =null)
        {
            GameEntry.TextureSet.SetTextureByNetworkAsync(SetRawImage.Create(rawImage, file), saveFilePath,cancellationToken);
        }

        public static void SetTextureByResourcesAsync(this RawImage rawImage, string file,ETCancellationToken cancellationToken)
        {
            GameEntry.TextureSet.SetTextureByResourcesAsync(SetRawImage.Create(rawImage, file),cancellationToken);
        }
    }
}