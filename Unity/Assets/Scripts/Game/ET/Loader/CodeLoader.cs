using System;
using System.Collections.Generic;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Game;
using UnityGameFramework.Extension;

namespace ET
{
    public class CodeLoader : ICodeLoader
    {
        private Assembly model;

        public async UniTask StartAsync()
        {
            model = null;
            if (Define.EnableHotfix)
            {
                if (!Define.IsEditor && Define.EnableHotfix)
                {
                    HybridCLRHelper.Load();
                }
                
                byte[] assBytes = await LoadCodeBytesAsync("Model.dll");
                byte[] pdbBytes = await LoadCodeBytesAsync("Model.pdb");

                model = Assembly.Load(assBytes, pdbBytes);
                await LoadHotfixAsync();
            }
            else
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                Dictionary<string, Type> types = AssemblyHelper.GetAssemblyTypes(assemblies);
                EventSystem.Instance.Add(types);
                foreach (Assembly ass in assemblies)
                {
                    string name = ass.GetName().Name;
                    if (name == "Game.ET.Code.Model")
                    {
                        model = ass;
                    }
                }
            }
            
            IStaticMethod start = new StaticMethod(model, "ET.Entry", "Start");
            start.Run();
        }

        public async UniTask LoadHotfixAsync()
        {
            byte[] assBytes = await LoadCodeBytesAsync("Hotfix.dll");
            byte[] pdbBytes = await LoadCodeBytesAsync("Hotfix.pdb");

            Assembly hotfixAssembly = Assembly.Load(assBytes, pdbBytes);

            Dictionary<string, Type> types = AssemblyHelper.GetAssemblyTypes(typeof (Game).Assembly, typeof (Init).Assembly, model, hotfixAssembly);

            EventSystem.Instance.Add(types);
        }
        
        private async UniTask<byte[]> LoadCodeBytesAsync(string fileName)
        {
            TextAsset textAsset = await GameEntry.Resource.LoadAssetAsync<TextAsset>(AssetUtility.GetCodeAsset(fileName));
            byte[] bytes = textAsset.bytes;
            GameEntry.Resource.UnloadAsset(textAsset);
            return bytes;
        }
    }
}