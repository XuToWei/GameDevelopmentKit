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
        private Assembly modelView;

        public async UniTask StartAsync()
        {
            this.model = null;
            this.modelView = null;

            if (Define.EnableHotfix && GameEntry.CodeRunner.EnableCodeBytesMode)
            {
                byte[] assBytes_Model = await LoadCodeBytesAsync("Game.ET.Code.Model.dll.bytes");
                byte[] pdbBytes_Model = await LoadCodeBytesAsync("Game.ET.Code.Model.pdb.bytes");
                byte[] assBytes_ModelView = await LoadCodeBytesAsync("Game.ET.Code.ModelView.dll.bytes");
                byte[] pdbBytes_ModelView = await LoadCodeBytesAsync("Game.ET.Code.ModelView.pdb.bytes");
                this.model = Assembly.Load(assBytes_Model, pdbBytes_Model);
                this.modelView = Assembly.Load(assBytes_ModelView, pdbBytes_ModelView);
                
                var hotfixAssemblies = await LoadHotfixAsync();
                
                World.Instance.AddSingleton<CodeTypes, Assembly[]>(new []
                    {typeof (World).Assembly, typeof(Init).Assembly, this.model, this.modelView, hotfixAssemblies.Item1, hotfixAssemblies.Item2});
            }
            else
            {
                Assembly[] assemblies = Utility.Assembly.GetAssemblies();
                foreach (Assembly ass in assemblies)
                {
                    string name = ass.GetName().Name;
                    if (name == "Game.ET.Code.Model")
                    {
                        this.model = ass;
                    }
                    else if (name == "Game.ET.Code.ModelView")
                    {
                        this.modelView = ass;
                    }

                    if (this.model != null && this.modelView != null)
                    {
                        break;
                    }
                }
                World.Instance.AddSingleton<CodeTypes, Assembly[]>(assemblies);
            }
            
            IStaticMethod start = new StaticMethod(this.model, "ET.Entry", "Start");
            start.Run();
        }

        public async UniTask ReloadAsync()
        {
            var hotfixAssemblies = await LoadHotfixAsync();
            World.Instance.AddSingleton<CodeTypes, Assembly[]>(new[]
                { typeof(World).Assembly, typeof(Init).Assembly, this.model, this.modelView, hotfixAssemblies.Item1, hotfixAssemblies.Item2 });
            CodeTypes.Instance.CreateCode();
            EventSystem.Instance.Invoke(new OnCodeReload());
        }

        private async UniTask<(Assembly, Assembly)> LoadHotfixAsync()
        {
            if (!Define.EnableHotfix)
            {
                throw new GameFrameworkException("Client ET LoadHotfix only run when EnableHotfix!");
            }
#if UNITY_EDITOR
            await UniTask.CompletedTask;
            string[] hotfixFiles = System.IO.Directory.GetFiles(Define.ReloadHotfixDir, "Game.ET.Code.Hotfix_*.dll");
            if (hotfixFiles.Length != 1)
            {
                throw new GameFrameworkException("Hotfix dll count != 1");
            }
            string[] hotfixViewFiles = System.IO.Directory.GetFiles(Define.ReloadHotfixDir, "Game.ET.Code.HotfixView_*.dll");
            if (hotfixViewFiles.Length != 1)
            {
                throw new GameFrameworkException("HotfixView dll count != 1");
            }
            string hotfixName = System.IO.Path.GetFileNameWithoutExtension(hotfixFiles[0]);
            string hotfixViewName = System.IO.Path.GetFileNameWithoutExtension(hotfixViewFiles[0]);
            byte[] assBytes_Hotfix = System.IO.File.ReadAllBytes($"{Define.ReloadHotfixDir}/{hotfixName}.dll");
            byte[] pdbBytes_Hotfix = System.IO.File.ReadAllBytes($"{Define.ReloadHotfixDir}/{hotfixName}.pdb");
            byte[] assBytes_HotfixView = System.IO.File.ReadAllBytes($"{Define.ReloadHotfixDir}/{hotfixViewName}.dll");
            byte[] pdbBytes_HotfixView = System.IO.File.ReadAllBytes($"{Define.ReloadHotfixDir}/{hotfixViewName}.pdb");
#else
            byte[] assBytes_Hotfix = await LoadCodeBytesAsync("Game.ET.Code.Hotfix.dll.bytes");
            byte[] pdbBytes_Hotfix = await LoadCodeBytesAsync("Game.ET.Code.Hotfix.pdb.bytes");
            byte[] assBytes_HotfixView = await LoadCodeBytesAsync("Game.ET.Code.HotfixView.dll.bytes");
            byte[] pdbBytes_HotfixView = await LoadCodeBytesAsync("Game.ET.Code.HotfixView.pdb.bytes");
#endif
            Assembly hotfix = Assembly.Load(assBytes_Hotfix, pdbBytes_Hotfix);
            Assembly hotfixView = Assembly.Load(assBytes_HotfixView, pdbBytes_HotfixView);
            return (hotfix, hotfixView);
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