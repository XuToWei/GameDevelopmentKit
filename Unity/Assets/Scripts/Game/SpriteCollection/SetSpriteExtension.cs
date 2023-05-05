using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityGameFramework.Extension;

namespace Game
{
    public static partial class SetSpriteExtension
    {
        /// <summary>
        /// 设置精灵
        /// </summary>
        /// <param name="image"></param>
        /// <param name="collectionPath">精灵所在收集器地址</param>
        /// <param name="spritePath">精灵名称</param>
        public static UniTask SetSpriteAsync(this Image image, string collectionPath, string spritePath)
        {
            return GameEntry.SpriteCollection.SetSpriteAsync(WaitSetImage.Create(image, collectionPath, spritePath));
        }
        
        /// <summary>
        /// 设置精灵
        /// </summary>
        /// <param name="image"></param>
        /// <param name="collectionPath">精灵所在收集器地址</param>
        /// <param name="spritePath">精灵名称</param>
        public static void SetSprite(this Image image, string collectionPath, string spritePath)
        {
            GameEntry.SpriteCollection.SetSprite(WaitSetImage.Create(image,collectionPath,spritePath));
        }
    }
}