using System;
using System.IO;
using System.Reflection;
using Bright.Serialization;
using cfg;
using SimpleJSON;

namespace ET.Server
{
    public static class ConfigLoader
    {
        public static string GetLubanAsset(string assetName, bool fromJson)
        {
            return $"../Config/Excel/Server/{assetName}.{(fromJson? "json" : "bytes")}";
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
                        byte[] bytes = File.ReadAllBytes(GetLubanAsset(file, false));
                        return new ByteBuf(bytes);
                    }

                    Func<string, ByteBuf> func = LoadByteBuf;
                    loadMethodInfo.Invoke(dataTables, new object[] { func });
                }
                else
                {
                    JSONNode LoadJson(string file)
                    {
                        string text = File.ReadAllText(GetLubanAsset(file, true));
                        return JSON.Parse(text);
                    }

                    Func<string, JSONNode> func = LoadJson;
                    loadMethodInfo.Invoke(dataTables, new object[] { func });
                }

                return dataTables;
            }
        }

        [Invoke]
        public class LoadLubanAsync: AInvokeHandler<ConfigComponent.LoadLubanAsync, ETTask<ISingleton>>
        {
            public override ETTask<ISingleton> Handle(ConfigComponent.LoadLubanAsync args)
            {
                ISingleton singleton = this.Load();
                var task = ETTask<ISingleton>.Create();
                task.SetResult(singleton);
                return task;
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
                        byte[] bytes = File.ReadAllBytes(GetLubanAsset(file, false));
                        return new ByteBuf(bytes);
                    }

                    Func<string, ByteBuf> func = LoadByteBuf;
                    loadMethodInfo.Invoke(dataTables, new object[] { func });
                }
                else
                {
                    JSONNode LoadJson(string file)
                    {
                        string text = File.ReadAllText(GetLubanAsset(file, true));
                        return JSON.Parse(text);
                    }

                    Func<string, JSONNode> func = LoadJson;
                    loadMethodInfo.Invoke(dataTables, new object[] { func });
                }

                return dataTables;
            }
        }
    }
}