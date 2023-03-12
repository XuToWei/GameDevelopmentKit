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
            return textAsset.bytes;
        }

        public async UniTask<string> ReadTextAsync(string file)
        {
            TextAsset textAsset = await GameEntry.Resource.LoadAssetAsync<TextAsset>(GetLubanAssetPath(file, true));
            return textAsset.text;
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
