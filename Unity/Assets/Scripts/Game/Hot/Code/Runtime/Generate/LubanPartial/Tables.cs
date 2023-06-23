using System;
using System.Reflection;
using System.Threading.Tasks;
using Bright.Serialization;
using Cysharp.Threading.Tasks;
using GameFramework;
using SimpleJSON;
using UnityEngine;
using UnityGameFramework.Extension;

namespace Game.Hot
{
    public partial class Tables
    {
        public static Tables Instance { get; } = new Tables();
        
        public async UniTask LoadAllAsync()
        {
            Type tablesType = this.GetType();

            MethodInfo loadMethodInfo = tablesType.GetMethod("LoadAsync");

            Type loaderReturnType = loadMethodInfo.GetParameters()[0].ParameterType.GetGenericArguments()[1];
            
            // 根据cfg.Tables的构造函数的Loader的返回值类型决定使用json还是ByteBuf Loader
            if (loaderReturnType == typeof (Task<ByteBuf>))
            {
                async Task<ByteBuf> LoadByteBuf(string file)
                {
                    string lubanAssetFile = AssetUtility.GetGameHotAsset(Utility.Text.Format("Luban/{0}.bytes", file));
                    TextAsset textAsset = await GameEntry.Resource.LoadAssetAsync<TextAsset>(lubanAssetFile);
                    return new ByteBuf(textAsset.bytes);
                }

                Func<string, Task<ByteBuf>> func = LoadByteBuf;
                await (Task)loadMethodInfo.Invoke(this, new object[] { func });
            }
            else
            {
                async Task<JSONNode> LoadJson(string file)
                {
                    string lubanAssetFile = AssetUtility.GetGameHotAsset(Utility.Text.Format("Luban/{0}.json", file));
                    TextAsset textAsset = await GameEntry.Resource.LoadAssetAsync<TextAsset>(lubanAssetFile);
                    return JSON.Parse(textAsset.text);
                }

                Func<string, Task<JSONNode>> func = LoadJson;
                await (Task)loadMethodInfo.Invoke(this, new object[] { func });
            }
        }
    }
}