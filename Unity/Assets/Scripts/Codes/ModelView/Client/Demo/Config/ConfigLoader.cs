using System;
using System.IO;
using System.Reflection;
using Bright.Serialization;
using cfg;
using SimpleJSON;
using UGF;
using UnityEngine;

namespace ET.Client
{
    [Invoke]
    public class LoadLubanAsync: AInvokeHandler<ConfigComponent.LoadLubanAsync, ETTask<ISingleton>>
    {
        public override ETTask<ISingleton> Handle(ConfigComponent.LoadLubanAsync args)
        {
            ETTask<ISingleton> task = ETTask<ISingleton>.Create();
            LoadAsync(task).Coroutine();
            return task;
        }

        private async ETTask LoadAsync(ETTask<ISingleton> task)
        {
            CodeMode codeMode = GameEntry.Builtin.GlobalConfig.CodeMode;
            
            Type tablesType = typeof (DataTables);

            MethodInfo loadMethodInfo = tablesType.GetMethod("LoadAsync");

            Type loaderReturnType = loadMethodInfo.GetParameters()[0].ParameterType.GetGenericArguments()[1];

            DataTables dataTables = new();
            // 根据cfg.Tables的构造函数的Loader的返回值类型决定使用json还是ByteBuf Loader
            if (loaderReturnType == typeof (ETTask<ByteBuf>))
            {
                async ETTask<ByteBuf> LoadByteBuf(string file)
                {
                    TextAsset textAsset = await GameEntry.Resource.LoadAssetAsync<TextAsset>(AssetUtility.GetLubanAsset(file, false, codeMode));
                    return new ByteBuf(textAsset.bytes);
                }

                Func<string, ETTask<ByteBuf>> func = LoadByteBuf;
                await (ETTask)loadMethodInfo.Invoke(dataTables, new object[] { func });
            }
            else
            {
                async ETTask<JSONNode> LoadJson(string file)
                {
                    TextAsset textAsset = await GameEntry.Resource.LoadAssetAsync<TextAsset>(AssetUtility.GetLubanAsset(file, true, codeMode));
                    return JSON.Parse(textAsset.text);
                }

                Func<string, ETTask<JSONNode>> func = LoadJson;
                await (ETTask)loadMethodInfo.Invoke(dataTables, new object[] { func });
            }
            
            task.SetResult(dataTables);
        }
    }
    
    [Invoke]
    public class LoadLuban: AInvokeHandler<ConfigComponent.LoadLuban, ISingleton>
    {
        public override ISingleton Handle(ConfigComponent.LoadLuban args)
        {
            return this.Load();
        }

        private ISingleton Load()
        {
            Type tablesType = typeof (DataTables);

            MethodInfo loadMethodInfo = tablesType.GetMethod("Load");

            Type loaderReturnType = loadMethodInfo.GetParameters()[0].ParameterType.GetGenericArguments()[1];

            DataTables dataTables = new();
            // 根据cfg.Tables的构造函数的Loader的返回值类型决定使用json还是ByteBuf Loader
            if (loaderReturnType == typeof (ByteBuf))
            {
                ByteBuf LoadByteBuf(string file)
                {
                    byte[] bytes = File.ReadAllBytes(file);
                    return new ByteBuf(bytes);
                }

                Func<string, ByteBuf> func = LoadByteBuf;
                loadMethodInfo.Invoke(dataTables, new object[] { func });
            }
            else
            {
                JSONNode LoadJson(string file)
                {
                    string text = File.ReadAllText(file);
                    return JSON.Parse(text);
                }

                Func<string, JSONNode> func = LoadJson;
                loadMethodInfo.Invoke(dataTables, new object[] { func });
            }

            return dataTables;
        }
    }
}
