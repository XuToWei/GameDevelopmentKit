using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace ET
{
    public class CodeLoader : ICodeLoader
    {
        private AssemblyLoadContext assemblyLoadContext;

        private Assembly model;

        public async UniTask StartAsync()
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

        public async UniTask LoadHotfixAsync()
        {
            SynchronizationContext current = SynchronizationContext.Current;
            byte[] dllBytes = await File.ReadAllBytesAsync("./Hotfix.dll").ConfigureAwait(false);
            byte[] pdbBytes = await File.ReadAllBytesAsync("./Hotfix.pdb").ConfigureAwait(false);
            UniTask.ReturnToSynchronizationContext(current);//防止ReadAllBytesAsync闪退

            assemblyLoadContext?.Unload();
            GC.Collect();
            assemblyLoadContext = new AssemblyLoadContext("Hotfix", true);
            Assembly hotfixAssembly = assemblyLoadContext.LoadFromStream(new MemoryStream(dllBytes), new MemoryStream(pdbBytes));

            Dictionary<string, Type> types = AssemblyHelper.GetAssemblyTypes(Assembly.GetEntryAssembly(), typeof (Init).Assembly, typeof (Game).Assembly, model, hotfixAssembly);

            EventSystem.Instance.Add(types);
        }
    }
}