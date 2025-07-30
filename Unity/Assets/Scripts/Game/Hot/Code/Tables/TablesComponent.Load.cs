using System;
using System.Reflection;
using Cysharp.Threading.Tasks;
using GameFramework;
using Luban;
using SimpleJSON;
using UnityEngine;
using UnityGameFramework.Extension;

namespace Game.Hot
{
    public partial class TablesComponent : HotComponent
    {
        public async UniTask LoadAllAsync()
        {
            Type tablesType = this.GetType();

            MethodInfo loadMethodInfo = tablesType.GetMethod("LoadAsync");

            Type loaderReturnType = loadMethodInfo.GetParameters()[0].ParameterType.GetGenericArguments()[1];
            
            // 根据cfg.Tables的构造函数的Loader的返回值类型决定使用json还是ByteBuf Loader
            if (loaderReturnType == typeof (UniTask<ByteBuf>))
            {
                async UniTask<ByteBuf> LoadByteBuf(string file)
                {
                    string lubanAssetFile = AssetUtility.GetGameHotAsset(Utility.Text.Format("Luban/{0}.bytes", file));
                    TextAsset textAsset = await GameEntry.Resource.LoadAssetAsync<TextAsset>(lubanAssetFile);
                    ByteBuf byteBuf = new ByteBuf(textAsset.bytes);
                    GameEntry.Resource.UnloadAsset(textAsset);
                    return byteBuf;
                }

                Func<string, UniTask<ByteBuf>> func = LoadByteBuf;
                await (UniTask)loadMethodInfo.Invoke(this, new object[] { func });
            }
            else
            {
                async UniTask<JSONNode> LoadJson(string file)
                {
                    string lubanAssetFile = AssetUtility.GetGameHotAsset(Utility.Text.Format("Luban/{0}.json", file));
                    TextAsset textAsset = await GameEntry.Resource.LoadAssetAsync<TextAsset>(lubanAssetFile);
                    JSONNode jsonNode = JSON.Parse(textAsset.text);
                    GameEntry.Resource.UnloadAsset(textAsset);
                    return jsonNode;
                }

                Func<string, UniTask<JSONNode>> func = LoadJson;
                await (UniTask)loadMethodInfo.Invoke(this, new object[] { func });
            }
        }
    }
}