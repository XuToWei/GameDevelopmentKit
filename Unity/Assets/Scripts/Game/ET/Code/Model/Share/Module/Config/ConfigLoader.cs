using System;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Luban;
using SimpleJSON;

namespace ET
{
    [Invoke]
    public class LubanLoadAllAsyncHandler : AInvokeHandler<ConfigComponent.LoadAll, UniTask>
    {
        public override async UniTask Handle(ConfigComponent.LoadAll arg)
        {
            World.Instance.AddSingleton<Tables>();
            
            Type tablesType = typeof (Tables);

            MethodInfo loadMethodInfo = tablesType.GetMethod("LoadAsync");
            if (loadMethodInfo == null)
                return;

            Type loaderReturnType = loadMethodInfo.GetParameters()[0].ParameterType.GetGenericArguments()[1];
            
            // 根据cfg.Tables的构造函数的Loader的返回值类型决定使用json还是ByteBuf Loader
            if (loaderReturnType == typeof (UniTask<ByteBuf>))
            {
                async UniTask<ByteBuf> LoadByteBuf(string file)
                {
                    return new ByteBuf(await ConfigComponent.Instance.ReadBytesAsync(file));
                }

                Func<string, UniTask<ByteBuf>> func = LoadByteBuf;
                await (UniTask)loadMethodInfo.Invoke(Tables.Instance, new object[] { func });
            }
            else
            {
                async UniTask<JSONNode> LoadJson(string file)
                {
                    return JSON.Parse(await ConfigComponent.Instance.ReadTextAsync(file));
                }

                Func<string, UniTask<JSONNode>> func = LoadJson;
                await (UniTask)loadMethodInfo.Invoke(Tables.Instance, new object[] { func });
            }
        }
    }
    
    [Invoke]
    public class LubanLoadOneAsyncHandler: AInvokeHandler<ConfigComponent.ReloadOne, UniTask>
    {
        public override async UniTask Handle(ConfigComponent.ReloadOne arg)
        {
            await UniTask.CompletedTask;
            // await Tables.Instance.GetDataTable(arg.ConfigName).LoadAsync();
            // Tables.Instance.Refresh();
        }
    }
    
    [Invoke]
    public class LubanReloadAllAsyncHandler: AInvokeHandler<ConfigComponent.ReloadAll, UniTask>
    {
        public override async UniTask Handle(ConfigComponent.ReloadAll arg)
        {
            await UniTask.CompletedTask;
            // foreach (var dataTable in Tables.Instance.DataTables)
            // {
            //     await dataTable.LoadAsync();
            // }
            // Tables.Instance.Refresh();
        }
    }
}