#if UNITY_IOS && !UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;
using ThinkingAnalytics;
using UnityGameFramework.Runtime;
using GameFramework.DataTable;

namespace Game
{
    public class PlatformIOS : IPlatform
    {
        [DllImport("__Internal")]
        private static extern void RewardVideoAd();
        [DllImport("__Internal")]
        private static extern bool IsBannerAdShow();
        [DllImport("__Internal")]
        private static extern bool BannerAd();
        [DllImport("__Internal")]
        private static extern string GetDeviceID();
        [DllImport("__Internal")]
        private static extern void InitSDK();
        [DllImport("__Internal")]
        private static extern void AppStar();

        private StringBuilder m_TrackEventStringBuilder = new StringBuilder(100);
        private Dictionary<string, object> m_EventProperties = new Dictionary<string, object>();

        public void Init()
        {
            InitSDK();
        }

        public void ShowRewardAd(string tag)
        {
            RewardVideoAd();
            //GameObject.Find("Platform").SendMessage("OnRewardAdSuccessCallBack", tag);
        }

        public bool CanShowRewardAd()
        {
            return true;
        }

        public void ShowInteractionAd()
        {
            
        }

        public bool BannerAdIsShow()
        {
            return IsBannerAdShow();
        }

        public void ShowBannerAd()
        {
            BannerAd();
        }

        public void OnPressEscape()
        {
            
        }

        public void TrackEvent(string eventName, Dictionary<string, object> properties)
        {
            m_EventProperties.Clear();
            foreach (KeyValuePair<string,object> pair in properties)
            {
                m_EventProperties.Add(pair.Key, pair.Value);
            }
            ThinkingAnalyticsAPI.Track(eventName, m_EventProperties);

            m_TrackEventStringBuilder.Clear();
            m_TrackEventStringBuilder.AppendFormat("NewEventName:{0}, ", eventName);
            foreach (KeyValuePair<string, object> keyValuePair in properties)
            {
                m_TrackEventStringBuilder.AppendFormat("{0}:", keyValuePair.Key);
                m_TrackEventStringBuilder.AppendFormat("{0}, ", keyValuePair.Value);
            }
            Log.Info(m_TrackEventStringBuilder.ToString());
        }

        public string GetPkgId()
        {
            return "1";
        }

        public string GetDeviceId()
        {
            return GetDeviceID();
        }

        public void AppRate()
        {
            AppStar();
        }

        public bool CanAppRate()
        {
            string pkgId = GameEntry.Platform.GetPkgId();
            if (int.TryParse(pkgId, out var pid))
            {
                IDataTable<DRAuthenticationSwitch> drAuthenticationSwitches =
                    GameEntry.DataTable.GetDataTable<DRAuthenticationSwitch>();
                DRAuthenticationSwitch drAuthenticationSwitch =
                    drAuthenticationSwitches.GetDataRow(pid);
                return !string.IsNullOrEmpty(drAuthenticationSwitch.Remark);
            }
            return false;
        }
    }
}
#endif