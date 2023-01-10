using GameFramework;
using UnityEngine.UI;
using UnityGameFramework.Extension;

namespace Game
{
    public static partial class SetTextureExtension
    {
        public static void SetTextureByNetworkAsync(this RawImage rawImage, string file, string saveFilePath = null,
            GameFrameworkAction cancelAction = null)
        {
            GameEntry.TextureSet.SetTextureByNetworkAsync(SetRawImage.Create(rawImage, file), saveFilePath,
                cancelAction);
        }

        public static void SetTextureByResourcesAsync(this RawImage rawImage, string file,
            GameFrameworkAction cancelAction = null)
        {
            GameEntry.TextureSet.SetTextureByResourcesAsync(SetRawImage.Create(rawImage, file), cancelAction);
        }
    }
}