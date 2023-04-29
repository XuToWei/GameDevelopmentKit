using System.IO;
using Cysharp.Threading.Tasks;

namespace ET
{
    public class ConfigReader : IConfigReader
    {
        public async UniTask<byte[]> ReadBytesAsync(string file)
        {
            await UniTask.CompletedTask;
            return File.ReadAllBytes(GetLubanAssetPath(file, false));
        }

        public async UniTask<string> ReadTextAsync(string file)
        {
            await UniTask.CompletedTask;
            return File.ReadAllText(GetLubanAssetPath(file, true));
        }
        
        private string GetLubanAssetPath(string fileName, bool isJson)
        {
            if (isJson)
            {
                return $"../Config/ShareTool/{fileName}.json";
            }
            else
            {
                return $"../Config/ShareTool/{fileName}.bytes";
            }
        }
    }
}