using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace UGF
{
    /// <summary>
    /// 设置图片扩展
    /// </summary>
    public static partial class SetTextureExtensions
    {
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