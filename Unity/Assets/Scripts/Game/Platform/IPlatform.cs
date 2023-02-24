using System.Collections.Generic;

namespace Game
{
    public interface IPlatform
    {
        /// <summary>
        /// SDK初始化
        /// </summary>
        void Init();
        /// <summary>
        /// 展示激励广告
        /// </summary>
        void ShowRewardAd(string tag);
        /// <summary>
        /// 展示激励广告
        /// </summary>
        bool CanShowRewardAd();
        /// <summary>
        /// 展示插屏广告
        /// </summary>
        void ShowInteractionAd();
        /// <summary>
        /// Banner广告是否展示
        /// </summary>
        bool BannerAdIsShow();
        /// <summary>
        /// 展示Banner广告
        /// </summary>
        void ShowBannerAd();
        /// <summary>
        /// 按返回按钮
        /// </summary>
        void OnPressEscape();
        /// <summary>
        /// 数据打点
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="properties"></param>
        void TrackEvent(string eventName, Dictionary<string, object> properties);
        /// <summary>
        /// 分包id
        /// </summary>
        /// <returns></returns>
        string GetPkgId();
        /// <summary>
        /// 设备id
        /// </summary>
        /// <returns></returns>
        string GetDeviceId();
        /// <summary>
        /// App评分
        /// </summary>
        void AppRate();

        bool CanAppRate();
    }
}
