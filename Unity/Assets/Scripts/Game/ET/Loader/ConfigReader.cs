using Cysharp.Threading.Tasks;
using Game;
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
                    return $"Assets/Res/ET/Client/Luban/{fileName}.json";
                }

                return $"Assets/Res/ET/Client/Luban/{fileName}.bytes";
            }
            
            if (isJson)
            {
                return $"Assets/Res/ET/ClientServer/Luban/{fileName}.json";
            }

            return $"Assets/Res/ET/ClientServer/Luban/{fileName}.bytes";
        }
    }
}
