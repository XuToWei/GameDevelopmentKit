using System.IO;
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
            AutoResetUniTaskCompletionSource tcs = AutoResetUniTaskCompletionSource.Create();
            GameEntry.SpriteCollection.SetSprite(WaitableSetImage.Create(image, collectionPath, spritePath, tcs));
            return tcs.Task;
        }

        public static UniTask SetSpriteAsync(this UXImage uxImage, string collectionPath, string spritePath)
        {
            AutoResetUniTaskCompletionSource tcs = AutoResetUniTaskCompletionSource.Create();
            GameEntry.SpriteCollection.SetSprite(WaitableSetUXImage.Create(uxImage, collectionPath, spritePath, tcs));
            return tcs.Task;
        }
        
        /// <summary>
        /// 设置精灵，collectionPath与spritePath相同的路径和文件名时候使用此方法
        /// </summary>
        /// <param name="image"></param>
        /// <param name="spritePath">精灵名称</param>
        public static UniTask SetSpriteAsync(this Image image, string spritePath)
        {
            AutoResetUniTaskCompletionSource tcs = AutoResetUniTaskCompletionSource.Create();
            GameEntry.SpriteCollection.SetSprite(WaitableSetImage.Create(image, Path.ChangeExtension(spritePath, ".asset"), spritePath, tcs));
            return tcs.Task;
        }
        
        public static UniTask SetSpriteAsync(this UXImage uxImage, string spritePath)
        {
            AutoResetUniTaskCompletionSource tcs = AutoResetUniTaskCompletionSource.Create();
            GameEntry.SpriteCollection.SetSprite(WaitableSetUXImage.Create(uxImage, Path.ChangeExtension(spritePath, ".asset"), spritePath, tcs));
            return tcs.Task;
        }
        
        /// <summary>
        /// 设置精灵
        /// </summary>
        /// <param name="image"></param>
        /// <param name="collectionPath">精灵所在收集器地址</param>
        /// <param name="spritePath">精灵名称</param>
        public static void SetSprite(this Image image, string collectionPath, string spritePath)
        {
            GameEntry.SpriteCollection.SetSprite(WaitSetImage.Create(image, collectionPath,spritePath));
        }
        
        public static void SetSprite(this UXImage uxImage, string collectionPath, string spritePath)
        {
            GameEntry.SpriteCollection.SetSprite(WaitSetUXImage.Create(uxImage, collectionPath,spritePath));
        }
        
        /// <summary>
        /// 设置精灵，collectionPath与spritePath相同的路径和文件名时候使用此方法
        /// </summary>
        /// <param name="image"></param>
        /// <param name="spritePath">精灵名称</param>
        public static void SetSprite(this Image image, string spritePath)
        {
            GameEntry.SpriteCollection.SetSprite(WaitSetImage.Create(image, Path.ChangeExtension(spritePath, ".asset"), spritePath));
        }
        
        public static void SetSprite(this UXImage uxImage, string spritePath)
        {
            GameEntry.SpriteCollection.SetSprite(WaitSetUXImage.Create(uxImage, Path.ChangeExtension(spritePath, ".asset"), spritePath));
        }
    }
}