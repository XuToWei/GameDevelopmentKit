using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    /// <summary>
    /// 设置图片扩展
    /// </summary>
    public static partial class SetTextureExtensions
    {
        public static void SetTextureByFileSystem(this RawImage rawImage, string file)
        {
            GameEntry.GetComponent<TextureSetComponent>().SetTextureByFileSystem(SetRawImage.Create(rawImage, file));
        }

        public static int SetTextureByNetwork(this RawImage rawImage, string file, string saveFilePath = null)
        {
            return GameEntry.GetComponent<TextureSetComponent>().SetTextureByNetwork(SetRawImage.Create(rawImage, file), saveFilePath);
        }

        public static int SetTextureByResources(this RawImage rawImage, string file)
        {
           return GameEntry.GetComponent<TextureSetComponent>().SetTextureByResources(SetRawImage.Create(rawImage, file));
        }
    }
}