using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Game;
using UnityGameFramework.Extension;

namespace ET
{
    public class CodeLoader: Singleton<CodeLoader>
    {
        private Assembly model;

        public async ETTask StartAsync()
        {
            if (Define.EnableCode)
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                Dictionary<string, Type> types = AssemblyHelper.GetAssemblyTypes(assemblies);
                EventSystem.Instance.Add(types);
                foreach (Assembly ass in assemblies)
                {
                    string name = ass.GetName().Name;
                    if (name == "Code.ET.Code.Model")
                    {
                        this.model = ass;
                    }
                }
            }
            else
            {
                byte[] assBytes = await this.LoadCodeBytesAsync("Model.dll");
                byte[] pdbBytes = await this.LoadCodeBytesAsync("Model.pdb");

                this.model = Assembly.Load(assBytes, pdbBytes);
                await this.LoadHotfixAsync();
            }
            
            IStaticMethod start = new StaticMethod(this.model, "ET.Entry", "Start");
            start.Run();
        }

        // 热重载调用该方法
        public async ETTask LoadHotfixAsync()
        {
            byte[] assBytes = await this.LoadCodeBytesAsync("Hotfix.dll");
            byte[] pdbBytes = await this.LoadCodeBytesAsync("Hotfix.pdb");
            
            Assembly hotfixAssembly = Assembly.Load(assBytes, pdbBytes);

            Dictionary<string, Type> types = AssemblyHelper.GetAssemblyTypes(typeof (Game).Assembly, typeof (Init).Assembly, this.model, hotfixAssembly);

            EventSystem.Instance.Add(types);
        }

        private async ETTask<byte[]> LoadCodeBytesAsync(string fileName)
        {
            TextAsset textAsset = await GameEntry.Resource.LoadAssetAsync<TextAsset>(AssetUtility.GetCodeAsset(fileName));
            byte[] bytes = textAsset.bytes;
            GameEntry.Resource.UnloadAsset(textAsset);
            return bytes;
        }
    }
}