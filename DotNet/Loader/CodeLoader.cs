using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using Cysharp.Threading.Tasks;

namespace ET
{
    public class CodeLoader : ICodeLoader
    {
        private AssemblyLoadContext assemblyLoadContext;

        private Assembly model;

        public async UniTask StartAsync()
        {
            await UniTask.CompletedTask;
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly ass in assemblies)
            {
                if (ass.GetName().Name == "Model")
                {
                    this.model = ass;
                    break;
                }
            }

            Assembly hotfixAssembly = this.LoadHotfix();

            World.Instance.AddSingleton<CodeTypes, Assembly[]>(new[] { typeof (World).Assembly, typeof(Init).Assembly, this.model, hotfixAssembly });

            IStaticMethod start = new StaticMethod(this.model, "ET.Entry", "Start");
            start.Run();
        }

        private Assembly LoadHotfix()
        {
            assemblyLoadContext?.Unload();
            GC.Collect();
            assemblyLoadContext = new AssemblyLoadContext("Hotfix", true);
            byte[] dllBytes = File.ReadAllBytes("./Hotfix.dll");
            byte[] pdbBytes = File.ReadAllBytes("./Hotfix.pdb");
            Assembly hotfixAssembly = assemblyLoadContext.LoadFromStream(new MemoryStream(dllBytes), new MemoryStream(pdbBytes));
            return hotfixAssembly;
        }

        public async UniTask ReloadAsync()
        {
            await UniTask.CompletedTask;
            Assembly hotfixAssembly = this.LoadHotfix();

            CodeTypes codeTypes = World.Instance.AddSingleton<CodeTypes, Assembly[]>(new[] { typeof (World).Assembly, typeof(Init).Assembly, this.model, hotfixAssembly });

            codeTypes.CreateCode();
            Log.Debug($"reload dll finish!");
        }
    }
}