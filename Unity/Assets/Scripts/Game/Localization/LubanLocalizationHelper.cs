//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework.Localization;
using System;
using System.Collections.Generic;
using Luban;
using SimpleJSON;
using UnityGameFramework.Runtime;

namespace Game
{
    /// <summary>
    /// XML 格式的本地化辅助器。
    /// </summary>
    public class LubanLocalizationHelper : DefaultLocalizationHelper
    {
        /// <summary>
        /// 解析字典。
        /// </summary>
        /// <param name="dictionaryString">要解析的字典字符串。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>是否解析字典成功。</returns>
        public override bool ParseData(ILocalizationManager localizationManager, string dictionaryString, object userData)
        {
            try
            {
                JSONObject jsonObject = (JSONObject)JSONNode.Parse(dictionaryString);
                foreach (KeyValuePair<string, JSONNode> pair in jsonObject)
                {
                    string dictionaryKey = pair.Key;
                    string dictionaryValue = pair.Value;
                    if (!localizationManager.AddRawString(dictionaryKey, dictionaryValue))
                    {
                        Log.Warning("Can not add raw string with key '{0}' which may be invalid or duplicate.", dictionaryKey);
                        return false;
                    }
                }
                return true;
            }
            catch (Exception exception)
            {
                Log.Warning("Can not parse dictionary data with exception '{0}'.", exception.ToString());
                return false;
            }
        }
        
        /// <summary>
        /// 解析字典。
        /// </summary>
        /// <param name="localizationManager">本地化管理器。</param>
        /// <param name="dictionaryBytes">要解析的字典二进制流。</param>
        /// <param name="startIndex">字典二进制流的起始位置。</param>
        /// <param name="length">字典二进制流的长度。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>是否解析字典成功。</returns>
        public override bool ParseData(ILocalizationManager localizationManager, byte[] dictionaryBytes, int startIndex, int length, object userData)
        {
            try
            {
                ByteBuf byteBuf = new ByteBuf(dictionaryBytes);
                while (byteBuf.NotEmpty)
                {
                    string dictionaryKey = byteBuf.ReadString();
                    string dictionaryValue = byteBuf.ReadString();
                    if (!localizationManager.AddRawString(dictionaryKey, dictionaryValue))
                    {
                        Log.Warning("Can not add raw string with dictionary key '{0}' which may be invalid or duplicate.", dictionaryKey);
                        return false;
                    }
                }
                return true;
            }
            catch (Exception exception)
            {
                Log.Warning("Can not parse dictionary bytes with exception '{0}'.", exception);
                return false;
            }
        }
    }
}
