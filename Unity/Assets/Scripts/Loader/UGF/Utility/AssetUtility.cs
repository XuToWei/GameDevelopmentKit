//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using ET;
using GameFramework;

namespace UGF
{
    public static class AssetUtility
    {
        public static string GetConfigAsset(string assetName, bool fromBytes)
        {
            return Utility.Text.Format("Assets/Res/Config/{0}.{1}", assetName, fromBytes ? "bytes" : "txt");
        }

        public static string GetDataTableAsset(string assetName, bool fromBytes)
        {
            return Utility.Text.Format("Assets/Res/DataTable/{0}.{1}", assetName, fromBytes ? "bytes" : "txt");
        }

        public static string GetDictionaryAsset(string assetName, bool fromBytes)
        {
            return Utility.Text.Format("Assets/Res/Localization/{0}/Dictionary/{1}.{2}", GameEntry.Localization.Language, assetName, fromBytes ? "bytes" : "xml");
        }

        public static string GetFontAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Font/{0}.ttf", assetName);
        }

        public static string GetSceneAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Scene/{0}.unity", assetName);
        }

        public static string GetMusicAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Music/{0}.mp3", assetName);
        }

        public static string GetSoundAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Sound/{0}.wav", assetName);
        }

        public static string GetEntityAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Entity/{0}.prefab", assetName);
        }

        public static string GetUIFormAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/UI/UIForm/{0}.prefab", assetName);
        }

        public static string GetUISoundAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/UI/UISound/{0}.wav", assetName);
        }
        
        public static string GetCodeAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Code/{0}.bytes", assetName);
        }

        public static string GetLubanAsset(string assetName, bool fromJson, CodeMode codeMode)
        {
            if (Define.IsEditor)
            {
                return Utility.Text.Format("Assets/Res/Config/ClientServer/{0}.{1}", assetName, fromJson? "json" : "bytes");
            }
            else
            {
                switch (codeMode)
                {
                    case CodeMode.Client:
                    case CodeMode.ClientServerWhenEditor:
                        return Utility.Text.Format("Assets/Res/Config/Client/{0}.{1}", assetName, fromJson? "json" : "bytes");
                    case CodeMode.Server:
                    case CodeMode.ServerClientWhenEditor:
                        return Utility.Text.Format("Assets/Res/Config/Server/{0}.{1}", assetName, fromJson? "json" : "bytes");
                    case CodeMode.ClientServer:
                        return Utility.Text.Format("Assets/Res/Config/ClientServer/{0}.{1}", assetName, fromJson? "json" : "bytes");
                    default:
                        throw new Exception("GetLubanAsset Fail!");
                }
            }
        }

        public static string GetLocalizationAsset()
        {
            return "Assets/Res/Localization/LocalizationSource.prefab";
        }
    }
}
