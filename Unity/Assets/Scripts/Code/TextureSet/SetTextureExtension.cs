using GameFramework;
using UnityEngine.UI;
using UnityGameFramework.Extension;

namespace Game
{
    public static partial class SetTextureExtension
    {
        public static void SetTextureByNetworkAsync(this RawImage rawImage, string file, string saveFilePath = null, GameFrameworkAction cancelAction = null)
        {
            GameEntry.TextureSet.SetTextureByNetworkAsync(SetRawImage.Create(rawImage, file), saveFilePath,
                cancelAction);
        }

        public static void SetTextureByResourcesAsync(this RawImage rawImage, string file, GameFrameworkAction cancelAction = null)
        {
            GameEntry.TextureSet.SetTextureByResourcesAsync(SetRawImage.Create(rawImage, file), cancelAction);
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