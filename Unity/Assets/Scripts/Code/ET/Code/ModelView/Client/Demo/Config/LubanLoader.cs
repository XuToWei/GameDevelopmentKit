using System;
using System.Reflection;
using System.Threading.Tasks;
using Bright.Serialization;
using Game;
using SimpleJSON;
using UnityEngine;
using UnityGameFramework.Extension;

namespace ET.Client
{
    [Invoke]
    public class LubanLoadAllAsyncHandler : AInvokeHandler<ConfigComponent.LoadAll, ETTask>
    {
        public override async ETTask Handle(ConfigComponent.LoadAll arg)
        {
            Type tablesType = typeof (Tables);

            MethodInfo loadMethodInfo = tablesType.GetMethod("LoadAsync");

            Type loaderReturnType = loadMethodInfo.GetParameters()[0].ParameterType.GetGenericArguments()[1];
            
            // 根据cfg.Tables的构造函数的Loader的返回值类型决定使用json还是ByteBuf Loader
            if (loaderReturnType == typeof (Task<ByteBuf>))
            {
                async Task<ByteBuf> LoadByteBuf(string file)
                {
                    TextAsset textAsset = await GameEntry.Resource.LoadAssetAsync<TextAsset>(Define.GetLubanAssetPath(file, false));
                    return new ByteBuf(textAsset.bytes);
                }

                Func<string, Task<ByteBuf>> func = LoadByteBuf;
                await (Task)loadMethodInfo.Invoke(Tables.Instance, new object[] { func });
            }
            else
            {
                async Task<JSONNode> LoadJson(string file)
                {
                    TextAsset textAsset = await GameEntry.Resource.LoadAssetAsync<TextAsset>(Define.GetLubanAssetPath(file, true));
                    return JSON.Parse(textAsset.text);
                }

                Func<string, Task<JSONNode>> func = LoadJson;
                await (ETTask)loadMethodInfo.Invoke(Tables.Instance, new object[] { func });
            }
        }
    }
    
    [Invoke]
    public class LubanLoadOneAsyncHandler: AInvokeHandler<ConfigComponent.LoadOne, ETTask>
    {
        public override async ETTask Handle(ConfigComponent.LoadOne arg)
        {
            await Tables.Instance.GetDataTable(arg.ConfigName).LoadAsync();
        }
    }
}