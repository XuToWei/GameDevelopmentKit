#if UNITY_ANDROID && !UNITY_EDITOR
using System.Collections.Generic;
using System.Text;
using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Game
{
    public class PlatformAndroid : IPlatform
    {
        private AndroidJavaObject m_AndroidJavaObject;
        private StringBuilder m_TrackEventStringBuilder;
        private Dictionary<string, object> m_EventProperties;
        
        public void Init()
        {
            m_TrackEventStringBuilder = new StringBuilder(100);
            try
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                m_AndroidJavaObject = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                //初始TapDB打点
                // TapDB.onStartWithProperties("0gl6bh7doygamhq3", GetPkgId(), Version.GameVersion, null);
            }
            catch (System.Exception e)
            {
                Log.Error(Utility.Text.Format("Call java error : {0}", e.Message));
            }
            m_EventProperties = new Dictionary<string, object>();
        }

        public string GetPkgId()
        {
            return m_AndroidJavaObject.Call<string>("GetPkgId");
        }

        public string GetDeviceId()
        {
            return m_AndroidJavaObject.Call<string>("GetDeviceId");
        }

        public void AppRate()
        {
            
        }

        public bool CanAppRate()
        {
            
            return false;
        }

        public void ShowRewardAd(string tag)
        {
            m_AndroidJavaObject.Call("ShowRewardAd", tag);
        }

        public bool CanShowRewardAd()
        {
            return m_AndroidJavaObject.Call<bool>("CanShowRewardAd");
        }

        public void ShowInteractionAd()
        {
            m_AndroidJavaObject.Call("ShowInteractionAd");
        }

        public bool BannerAdIsShow()
        {
            return m_AndroidJavaObject.Call<bool>("BannerAdIsShow");
        }

        public void ShowBannerAd()
        {
            m_AndroidJavaObject.Call("ShowBannerAd");
        }

        public void OnPressEscape()
        {
            m_AndroidJavaObject.Call("onBackPressed");
        }

        public void TrackEvent(string eventName, Dictionary<string, object> properties)
        {
            // TapDB.trackEvent(ReviseSymbol(eventName), m_EventProperties);
        }
    }
}
#endif
