using GameFramework;
using UnityEngine;

namespace UnityGameFramework.Extension
{
    public interface ISetTexture2dObject : IReference
    {
        /// <summary>
        /// 图片文件地址
        /// </summary>
        string Texture2dFilePath { get; }

        /// <summary>
        /// 设置图片
        /// </summary>
        void SetTexture(Texture2D texture);

        /// <summary>
        /// 是否可以回收
        /// </summary>
        bool IsCanRelease();
    }
}