using System.IO;
using Cysharp.Threading.Tasks;

namespace ET
{
    public class ConfigReader : IConfigReader
    {
        public async UniTask<byte[]> ReadBytesAsync(string file)
        {
            return await File.ReadAllBytesAsync(GetLubanAssetPath(file, false));
        }

        public async UniTask<string> ReadTextAsync(string file)
        {
            return await File.ReadAllTextAsync(GetLubanAssetPath(file, true));
        }
        
        private string GetLubanAssetPath(string fileName, bool isJson)
        {
            if (isJson)
            {
                return $"../Config/Luban/{fileName}.json";
            }

            return $"../Config/Luban/{fileName}.bytes";
        }
    }
}