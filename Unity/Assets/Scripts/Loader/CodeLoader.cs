using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UGF;
using UnityEngine;

namespace ET
{
    public class CodeLoader: Singleton<CodeLoader>
    {
        private Assembly model;

        public async ETTask StartAsync()
        {
            if (Define.EnableCodes)
            {
                CodeMode codeMode = GameEntry.Builtin.GlobalConfig.CodeMode;
                if (codeMode == CodeMode.Client || codeMode == CodeMode.Server ||
                    !Application.isEditor && (codeMode == CodeMode.ClientServerWhenEditor || codeMode == CodeMode.ServerClientWhenEditor))
                {
                    throw new Exception("ENABLE_CODES mode must use ClientServer code mode!");
                }

                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                Dictionary<string, Type> types = AssemblyHelper.GetAssemblyTypes(assemblies);
                EventSystem.Instance.Add(types);
                foreach (Assembly ass in assemblies)
                {
                    string name = ass.GetName().Name;
                    if (name == "Unity.Model.Codes")
                    {
                        this.model = ass;
                    }
                }

                IStaticMethod start = new StaticMethod(this.model, "ET.Entry", "Start");
                start.Run();
            }
            else
            {
                byte[] assBytes = await this.LoadCodeBytesAsync("Model.dll");
                byte[] pdbBytes = await this.LoadCodeBytesAsync("Model.pdb");

                this.model = Assembly.Load(assBytes, pdbBytes);
                await this.LoadHotfixAsync();

                IStaticMethod start = new StaticMethod(this.model, "ET.Entry", "Start");
                start.Run();
            }
        }

        // 热重载调用该方法
        public async ETTask LoadHotfixAsync()
        {
            byte[] assBytes;
            byte[] pdbBytes;
            if (!Define.IsEditor)
            {
                assBytes = await this.LoadCodeBytesAsync("Hotfix.dll");
                pdbBytes = await this.LoadCodeBytesAsync("Hotfix.pdb");
            }
            else
            {
                // 傻屌Unity在这里搞了个傻逼优化，认为同一个路径的dll，返回的程序集就一样。所以这里每次编译都要随机名字
                string[] logicFiles = Directory.GetFiles(Define.BuildOutputDir, "Hotfix_*.dll");
                if (logicFiles.Length != 1)
                {
                    throw new Exception("Logic dll count != 1");
                }

                string logicName = Path.GetFileNameWithoutExtension(logicFiles[0]);
                assBytes = await File.ReadAllBytesAsync(Path.Combine(Define.BuildOutputDir, $"{logicName}.dll"));
                pdbBytes = await File.ReadAllBytesAsync(Path.Combine(Define.BuildOutputDir, $"{logicName}.pdb"));
            }

            Assembly hotfixAssembly = Assembly.Load(assBytes, pdbBytes);

            Dictionary<string, Type> types = AssemblyHelper.GetAssemblyTypes(typeof (Game).Assembly, typeof (CodeLoader).Assembly, this.model, hotfixAssembly);

            EventSystem.Instance.Add(types);
        }
        
        private async ETTask<byte[]> LoadCodeBytesAsync(string fileName)
        {
            TextAsset textAsset = await GameEntry.Resource.LoadAssetAsync<TextAsset>(AssetUtility.GetCodeAsset(fileName));
            byte[] bytes = textAsset.bytes;
            GameEntry.Resource.UnloadAsset(textAsset);
            return bytes;
        }

#if !UNITY_EDITOR
        private async Task LoadMetadataForAOTAssembly()
        {
            // 可以加载任意aot assembly的对应的dll。但要求dll必须与unity build过程中生成的裁剪后的dll一致，而不能直接使用原始dll。
            // 我们在BuildProcessor_xxx里添加了处理代码，这些裁剪后的dll在打包时自动被复制到 {项目目录}/HybridCLRData/AssembliesPostIl2CppStrip/{Target} 目录。

            /// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
            /// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
            /// 
            List<string> aotDllList = new List<string>
            {
                "mscorlib",
                "System",
                "System.Core", // 如果使用了Linq，需要这个
                
                "UnityGameFramework.Runtime",
                "Unity.TextMeshPro",
                "GameFramework",
                "Unity.ThirdParty.LubanLib",
                "Unity.ThirdParty",
                "Assembly-CSharp",
                "Unity.Loader",
                "UnityEngine.UI",
                

                //
                // 注意！修改这个列表请同步修改 BuildConfig_Custom文件中的 AOTMetaAssemblies 列表。
                // 两者需要完全一致
                //
                // "Newtonsoft.Json.dll",
                // "protobuf-net.dll",
                // "Google.Protobuf.dll",
                // "MongoDB.Bson.dll",
                // "DOTween.Modules.dll",
                // "UniTask.dll",
            };
            foreach (var aotDllName in aotDllList)
            {
                byte[] dllBytes = (await UGFEntry.Resource.LoadAssetAsync<TextAsset>(AssetUtility.GetHotfixDllAsset(aotDllName))).bytes;
                unsafe
                {
                    fixed (byte* ptr = dllBytes)
                    {
                        // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
                        LoadImageErrorCode err = (LoadImageErrorCode)RuntimeApi.LoadMetadataForAOTAssembly((IntPtr)ptr, dllBytes.Length);
                        Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. ret:{err}");
                    }
                }
            }
        }
#endif
    }
}