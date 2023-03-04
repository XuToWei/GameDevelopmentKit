using System;
using System.Reflection;
using System.Threading.Tasks;
using Bright.Serialization;
using Cysharp.Threading.Tasks;
using SimpleJSON;
using UnityEngine;
using UnityGameFramework.Extension;

namespace Game
{
    public static class TablesLoader
    {
        public static async UniTask LoadAsync()
        {
            Type tablesType = typeof(Tables);

            MethodInfo loadMethodInfo = tablesType.GetMethod("LoadAsync");

            Type loaderReturnType = loadMethodInfo.GetParameters()[0].ParameterType.GetGenericArguments()[1];
            
            // 根据cfg.Tables的构造函数的Loader的返回值类型决定使用json还是ByteBuf Loader
            if (loaderReturnType == typeof (Task<ByteBuf>))
            {
                async Task<ByteBuf> LoadByteBuf(string file)
                {
                    TextAsset textAsset = await GameEntry.Resource.LoadAssetAsync<TextAsset>(AssetUtility.GetLubanAsset(file, false));
                    return new ByteBuf(textAsset.bytes);
                }

                Func<string, Task<ByteBuf>> func = LoadByteBuf;
                await (Task)loadMethodInfo.Invoke(GameEntry.Tables, new object[] { func });
            }
            else
            {
                async Task<JSONNode> LoadJson(string file)
                {
                    TextAsset textAsset = await GameEntry.Resource.LoadAssetAsync<TextAsset>(AssetUtility.GetLubanAsset(file, true));
                    return JSON.Parse(textAsset.text);
                }

                Func<string, Task<JSONNode>> func = LoadJson;
                await (Task)loadMethodInfo.Invoke(GameEntry.Tables, new object[] { func });
            }
        }
    }
}
