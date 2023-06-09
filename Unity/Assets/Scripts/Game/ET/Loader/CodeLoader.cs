using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security;
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
        private Assembly modelView;

        public async UniTask StartAsync()
        {
            model = null;
            modelView = null;
            
            if (Define.EnableHotfix && GameEntry.CodeRunner.EditorCodeBytesMode)
            {
                byte[] assBytes_Model = await LoadCodeBytesAsync("Game.ET.Code.Model.dll.bytes");
                byte[] pdbBytes_Model = await LoadCodeBytesAsync("Game.ET.Code.Model.pdb.bytes");
                byte[] assBytes_ModelView = await LoadCodeBytesAsync("Game.ET.Code.ModelView.dll.bytes");
                byte[] pdbBytes_ModelView = await LoadCodeBytesAsync("Game.ET.Code.ModelView.pdb.bytes");
                model = Assembly.Load(assBytes_Model, pdbBytes_Model);
                modelView = Assembly.Load(assBytes_ModelView, pdbBytes_ModelView);
                
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
                    else if (name == "Game.ET.Code.ModelView")
                    {
                        modelView = ass;
                    }

                    if (model != null && modelView != null)
                    {
                        break;
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
            
            byte[] assBytes_Hotfix = await LoadCodeBytesAsync("Game.ET.Code.Hotfix.dll.bytes");
            byte[] pdbBytes_Hotfix = await LoadCodeBytesAsync("Game.ET.Code.Hotfix.pdb.bytes");
            byte[] assBytes_HotfixView = await LoadCodeBytesAsync("Game.ET.Code.HotfixView.dll.bytes");
            byte[] pdbBytes_HotfixView = await LoadCodeBytesAsync("Game.ET.Code.HotfixView.pdb.bytes");

            Assembly hotfix = Assembly.Load(assBytes_Hotfix, pdbBytes_Hotfix);
            Assembly hotfixView = Assembly.Load(assBytes_HotfixView, pdbBytes_HotfixView);

            Dictionary<string, Type> types = AssemblyHelper.GetAssemblyTypes(typeof (Game).Assembly, typeof (Init).Assembly, model, modelView, hotfix, hotfixView);

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