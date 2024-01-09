using System;
using System.Reflection;
using System.Threading.Tasks;
using Bright.Serialization;
using Cysharp.Threading.Tasks;
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

            Type loaderReturnType = loadMethodInfo.GetParameters()[0].ParameterType.GetGenericArguments()[1];
            
            // 根据cfg.Tables的构造函数的Loader的返回值类型决定使用json还是ByteBuf Loader
            if (loaderReturnType == typeof (Task<ByteBuf>))
            {
                async Task<ByteBuf> LoadByteBuf(string file)
                {
                    return new ByteBuf(await ConfigComponent.Instance.ReadBytesAsync(file));
                }

                Func<string, Task<ByteBuf>> func = LoadByteBuf;
                await (Task)loadMethodInfo.Invoke(Tables.Instance, new object[] { func });
            }
            else
            {
                async Task<JSONNode> LoadJson(string file)
                {
                    return JSON.Parse(await ConfigComponent.Instance.ReadTextAsync(file));
                }

                Func<string, Task<JSONNode>> func = LoadJson;
                await (Task)loadMethodInfo.Invoke(Tables.Instance, new object[] { func });
            }
        }
    }
    
    [Invoke]
    public class LubanLoadOneAsyncHandler: AInvokeHandler<ConfigComponent.ReloadOne, UniTask>
    {
        public override async UniTask Handle(ConfigComponent.ReloadOne arg)
        {
            await Tables.Instance.GetDataTable(arg.ConfigName).LoadAsync();
            Tables.Instance.Refresh();
        }
    }
    
    [Invoke]
    public class LubanReloadAllAsyncHandler: AInvokeHandler<ConfigComponent.ReloadAll, UniTask>
    {
        public override async UniTask Handle(ConfigComponent.ReloadAll arg)
        {
            foreach (var dataTable in Tables.Instance.DataTables)
            {
                await dataTable.LoadAsync();
            }
            Tables.Instance.Refresh();
        }
    }
}