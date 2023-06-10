using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Game;
using GameFramework;
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
                byte[] assBytes = await LoadCodeBytesAsync("Model.dll.bytes");
                byte[] pdbBytes = await LoadCodeBytesAsync("Model.pdb.bytes");
                model = Assembly.Load(assBytes, pdbBytes);
                
                await LoadHotfixAsync();
            }
            else
            {
                Assembly[] assemblies = Utility.Assembly.GetAssemblies();
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
            if (!Define.EnableHotfix)
            {
                throw new GameFrameworkException("Client ET LoadHotfix only run when EnableHotfix!");
            }
#if UNITY_EDITOR
            // 傻屌Unity在这里搞了个傻逼优化，认为同一个路径的dll，返回的程序集就一样。所以这里每次编译都要随机名字
            string[] logicFiles = Directory.GetFiles(Define.ReloadHotfixDir, "Hotfix_*.dll");
            if (logicFiles.Length != 1)
            {
                throw new Exception("Logic dll count != 1");
            }
            string logicName = Path.GetFileNameWithoutExtension(logicFiles[0]);
            byte[] assBytes = File.ReadAllBytes(Path.Combine(Define.ReloadHotfixDir, $"{logicName}.dll"));
            byte[] pdbBytes = File.ReadAllBytes(Path.Combine(Define.ReloadHotfixDir, $"{logicName}.pdb"));
           
#else
            byte[] assBytes = await LoadCodeBytesAsync("Hotfix.dll.bytes");
            byte[] pdbBytes = await LoadCodeBytesAsync("Hotfix.pdb.bytes");
#endif

            Assembly hotfixAssembly = Assembly.Load(assBytes, pdbBytes);

            Dictionary<string, Type> types = AssemblyHelper.GetAssemblyTypes(typeof (Game).Assembly, typeof (Init).Assembly, model, hotfixAssembly);

            EventSystem.Instance.Add(types);
        }
        
        private async UniTask<byte[]> LoadCodeBytesAsync(string fileName)
        {
            fileName = AssetUtility.GetETAsset(Utility.Text.Format("Code/{0}", fileName));
            TextAsset textAsset = await GameEntry.Resource.LoadAssetAsync<TextAsset>(fileName);
            byte[] bytes = textAsset.bytes;
            GameEntry.Resource.UnloadAsset(textAsset);
            return bytes;
        }
    }
}