using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Game
{
    public class PlatformEditor : IPlatform
    {
        private StringBuilder m_TrackEventStringBuilder;
        public void Init()
        {
            m_TrackEventStringBuilder = new StringBuilder(100);
        }

        public void ShowRewardAd(string tag)
        {
            
        }

        IEnumerator DelayRewardCallBack(string tag)
        {
            yield return new WaitForSeconds(0.5f);
            GameObject.Find("Platform").SendMessage("OnRewardAdSuccessCallBack", tag);
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
            return false;
        }

        public void ShowBannerAd()
        {
            
        }

        public void OnPressEscape()
        {
            
        }

        public string GetPkgId()
        {
            return "0";
        }

        public string GetDeviceId()
        {
            return SystemInfo.deviceName;
        }

        public void AppRate()
        {
           
        }

        public bool CanAppRate()
        {
            return false;
        }

        public void TrackEvent(string eventName, Dictionary<string, object> properties)
        {
            m_TrackEventStringBuilder.Clear();
            m_TrackEventStringBuilder.AppendFormat("EventName:{0}, ", eventName);
            foreach (KeyValuePair<string,object> keyValuePair in properties)
            {
                m_TrackEventStringBuilder.AppendFormat("{0}:", keyValuePair.Key);
                m_TrackEventStringBuilder.AppendFormat("{0}, ", keyValuePair.Value);
            }
            Log.Info(m_TrackEventStringBuilder.ToString());
        }
    }
}
