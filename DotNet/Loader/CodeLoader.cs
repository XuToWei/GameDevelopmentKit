using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using Cysharp.Threading.Tasks;

namespace ET
{
    [Invoke]
    public class CodeStartAsyncHandler : AInvokeHandler<CodeLoaderComponent.CodeStartAsync, UniTask>
    {
        public override async UniTask Handle(CodeLoaderComponent.CodeStartAsync a)
        {
            await CodeLoader.StartAsync();
        }
    }

    // 热重载调用该方法
    [Invoke]
    public class CodeLoadHotfixAsyncHandler : AInvokeHandler<CodeLoaderComponent.CodeLoadHotfixAsync, UniTask>
    {
        public override async UniTask Handle(CodeLoaderComponent.CodeLoadHotfixAsync a)
        {
            await CodeLoader.LoadHotfixAsync();
        }
    }
    
    public static class CodeLoader
    {
        private static AssemblyLoadContext assemblyLoadContext;

        private static Assembly model;

        public static async UniTask StartAsync()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                if (assembly.GetName().Name == "Model")
                {
                    model = assembly;
                    break;
                }
            }

            await LoadHotfixAsync();

            IStaticMethod start = new StaticMethod(model, "ET.Entry", "Start");
            start.Run();
        }

        public static async UniTask LoadHotfixAsync()
        {
            assemblyLoadContext?.Unload();
            GC.Collect();
            assemblyLoadContext = new AssemblyLoadContext("Hotfix", true);
            byte[] dllBytes = await File.ReadAllBytesAsync("./Hotfix.dll");
            byte[] pdbBytes = await File.ReadAllBytesAsync("./Hotfix.pdb");
            Assembly hotfixAssembly = assemblyLoadContext.LoadFromStream(new MemoryStream(dllBytes), new MemoryStream(pdbBytes));

            Dictionary<string, Type> types = AssemblyHelper.GetAssemblyTypes(Assembly.GetEntryAssembly(), typeof (Init).Assembly, typeof (Game).Assembly, model, hotfixAssembly);

            EventSystem.Instance.Add(types);
        }
    }
}