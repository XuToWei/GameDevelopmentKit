using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityGameFramework.Extension;

namespace Game
{
    public static partial class SetTextureExtension
    {
        public static UniTask SetTextureByNetworkAsync(this RawImage rawImage, string file, string saveFilePath = null)
        {
            AutoResetUniTaskCompletionSource tcs = AutoResetUniTaskCompletionSource.Create();
            GameEntry.TextureSet.SetTextureByNetwork(WaitableSetRawImage.Create(rawImage, file, tcs), saveFilePath);
            return tcs.Task;
        }

        public static UniTask SetTextureByResourcesAsync(this RawImage rawImage, string file)
        {
            AutoResetUniTaskCompletionSource tcs = AutoResetUniTaskCompletionSource.Create();
            GameEntry.TextureSet.SetTextureByResources(WaitableSetRawImage.Create(rawImage, file, tcs));
            return tcs.Task;
        }

        public static void SetTextureByFileSystem(this RawImage rawImage, string file)
        {
            GameEntry.TextureSet.SetTextureByFileSystem(WaitSetRawImage.Create(rawImage, file));
        }

        public static void SetTextureByNetwork(this RawImage rawImage, string file, string saveFilePath = null)
        {
           GameEntry.TextureSet.SetTextureByNetwork(WaitSetRawImage.Create(rawImage, file), saveFilePath);
        }

        public static void SetTextureByResources(this RawImage rawImage, string file)
        {
            GameEntry.TextureSet.SetTextureByResources(WaitSetRawImage.Create(rawImage, file));
        }
    }
}