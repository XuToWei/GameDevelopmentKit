using System.Text.RegularExpressions;
using GameFramework;
using UnityEngine;

namespace Game
{
    public class HyperlinkLogHelper : GameFrameworkLog.ILogHelper
    {
        private readonly Regex m_HyperlinkRegex = new Regex(@"\)\s\[0x[0-9,a-f]*\]\sin\s(.*:[0-9]*)\s");
        
        /// <summary>
        /// 记录日志。
        /// </summary>
        /// <param name="level">日志等级。</param>
        /// <param name="message">日志内容。</param>
        public void Log(GameFrameworkLogLevel level, object message)
        {
            switch (level)
            {
                case GameFrameworkLogLevel.Debug:
                    Debug.Log(HyperlinkFormat(Utility.Text.Format("<color=#888888>{0}</color>", message)));
                    break;

                case GameFrameworkLogLevel.Info:
                    Debug.Log(HyperlinkFormat(message.ToString()));
                    break;

                case GameFrameworkLogLevel.Warning:
                    Debug.LogWarning(HyperlinkFormat(message.ToString()));
                    break;

                case GameFrameworkLogLevel.Error:
                    Debug.LogError(HyperlinkFormat(message.ToString()));
                    break;

                default:
                    throw new GameFrameworkException(HyperlinkFormat(message.ToString()));
            }
        }

        private string HyperlinkFormat(string message)
        {
            return m_HyperlinkRegex.Replace(message, ") (at $1) ");
        }
    }
}