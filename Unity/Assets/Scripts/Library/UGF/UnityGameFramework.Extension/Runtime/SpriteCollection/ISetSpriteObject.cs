using GameFramework;
using UnityEngine;

namespace UnityGameFramework.Extension
{
    public interface ISetSpriteObject: IReference
    {
        /// <summary>
        /// 精灵名称
        /// </summary>
        string SpritePath { get;}
        /// <summary>
        /// 精灵所在收集器地址
        /// </summary>
        string CollectionPath { get;}
        /// <summary>
        /// 设置精灵
        /// </summary>
        void SetSprite(Sprite sprite);
        /// <summary>
        /// 是否可以回收
        /// </summary>
        bool IsCanRelease();
    }
}