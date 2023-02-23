using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using Cysharp.Threading.Tasks;

namespace ET
{
    public class CodeLoader : Singleton<CodeLoader>
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
                    this.model = assembly;
                    break;
                }
            }

            await this.LoadHotfixAsync();

            IStaticMethod start = new StaticMethod(this.model, "ET.Entry", "Start");
            start.Run();
        }

        public async UniTask LoadHotfixAsync()
        {
            assemblyLoadContext?.Unload();
            GC.Collect();
            assemblyLoadContext = new AssemblyLoadContext("Hotfix", true);
            byte[] dllBytes = await File.ReadAllBytesAsync("./Hotfix.dll");
            byte[] pdbBytes = await File.ReadAllBytesAsync("./Hotfix.pdb");
            Assembly hotfixAssembly = assemblyLoadContext.LoadFromStream(new MemoryStream(dllBytes), new MemoryStream(pdbBytes));

            Dictionary<string, Type> types = AssemblyHelper.GetAssemblyTypes(Assembly.GetEntryAssembly(), typeof (Init).Assembly, typeof (Game).Assembly, this.model, hotfixAssembly);

            EventSystem.Instance.Add(types);
        }
    }
}