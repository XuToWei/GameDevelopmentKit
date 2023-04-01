using Cysharp.Threading.Tasks;
using Game;
using GameFramework;
using UnityEngine;
using UnityGameFramework.Extension;

namespace ET
{
    public class ConfigReader : IConfigReader
    {
        public async UniTask<byte[]> ReadBytesAsync(string file)
        {
            TextAsset textAsset = await GameEntry.Resource.LoadAssetAsync<TextAsset>(GetLubanAssetPath(file, false));
            byte[] bytes = textAsset.bytes;
            GameEntry.Resource.UnloadAsset(textAsset);
            return bytes;
        }

        public async UniTask<string> ReadTextAsync(string file)
        {
            TextAsset textAsset = await GameEntry.Resource.LoadAssetAsync<TextAsset>(GetLubanAssetPath(file, true));
            string text = textAsset.text;
            GameEntry.Resource.UnloadAsset(textAsset);
            return text;
        }
        
        private string GetLubanAssetPath(string fileName, bool isJson)
        {
            if (Define.CodeMode == CodeMode.Client)
            {
                if (isJson)
                {
                    return AssetUtility.GetETAsset(Utility.Text.Format("Client/Luban/{0}.json", fileName));
                }
                else
                {
                    return AssetUtility.GetETAsset(Utility.Text.Format("Client/Luban/{0}.bytes", fileName));
                }
            }
            else
            {
                if (isJson)
                {
                    return AssetUtility.GetETAsset(Utility.Text.Format("ClientServer/Luban/{0}.json", fileName));
                }
                else
                {
                    return AssetUtility.GetETAsset(Utility.Text.Format("ClientServer/Luban/{0}.bytes", fileName));
                }
            }
        }
    }
}
