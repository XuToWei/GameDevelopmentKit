using UnityEditor;
using UnityEditor.Compilation;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine;
using System.Reflection;

namespace Game.Editor
{
    /// <summary>
    /// ____DESC:   手动reload domain 工具 
    /// </summary>
    public static class ScriptCompileReloadTool
    {
        /* 说明
         * 关于域重载 https://docs.unity.cn/cn/2021.3/Manual/DomainReloading.html
         * EditorApplication.LockReloadAssemblies()和 EditorApplication.UnlockReloadAssemblies() 最好成对
         * 如果不小心LockReloadAssemblies3次 但是只UnlockReloadAssemblies了一次 那么还是不会重载 必须也要但是只UnlockReloadAssemblies3次
         */

        const string menuEnableManualReload = "Game/Script Reload/开启手动Reload Domain";
        const string menuDisenableManualReload = "Game/Script Reload/关闭手动Reload Domain";
        const string menuRealodDomain = "Game/Script Reload/Unlock Reload %t";

        const string kManualReloadDomain = "ManualReloadDomain";
        const string kFirstEnterUnity = "FirstEnterUnity"; //是否首次进入unity 
        const string kReloadDomainTimer = "ReloadDomainTimer"; //计时


        /**************************************************/
        //编译时间
        static Stopwatch compileSW = new Stopwatch();

        //是否手动reload
        static bool IsManualReload => PlayerPrefs.GetInt(kManualReloadDomain, -1) == 1;

        //缓存数据 域重载之后数据会变成false 如果不是false 那么就要重载
        static bool tempData = false;

        //https://github.com/INeatFreak/unity-background-recompiler 来自这个库 反射获取是否锁住
        static MethodInfo CanReloadAssembliesMethod;

        static bool IsLocked
        {
            get
            {
                if (CanReloadAssembliesMethod == null)
                {
                    // source: https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/EditorApplication.bindings.cs#L154
                    CanReloadAssembliesMethod = typeof(EditorApplication).GetMethod("CanReloadAssemblies",
                        BindingFlags.NonPublic | BindingFlags.Static);
                    if (CanReloadAssembliesMethod == null)
                        Debug.LogError("Can't find CanReloadAssemblies method. It might have been renamed or removed.");
                }

                return !(bool)CanReloadAssembliesMethod.Invoke(null, null);
            }
        }
        /**************************************************/


        [InitializeOnLoadMethod]
        static void InitCompile()
        {
            //**************不需要这个可以注释********************************
            CompilationPipeline.compilationStarted -= OnCompilationStarted;
            CompilationPipeline.compilationStarted += OnCompilationStarted;
            CompilationPipeline.compilationFinished -= OnCompilationFinished;
            CompilationPipeline.compilationFinished += OnCompilationFinished;
            //**************************************************************

            //域重载事件监听
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;

            EditorApplication.playModeStateChanged -= EditorApplication_PlayModeStateChanged;
            EditorApplication.playModeStateChanged += EditorApplication_PlayModeStateChanged;


            //Bug 首次启动的时候 并不会马上设置
            //if (PlayerPrefs.HasKey(kManualReloadDomain))
            //{
            //    Menu.SetChecked(menuEnableManualReload, IsManualReload ? true : false);
            //    Menu.SetChecked(menuDisenableManualReload, IsManualReload ? false : true);
            //}
            FirstCheckAsync();
        }

        //首次打开检测
        async static void FirstCheckAsync()
        {
            await System.Threading.Tasks.Task.Delay(100);
            //判断是否首次打开
            //https://docs.unity.cn/cn/2021.3/ScriptReference/SessionState.html
            if (SessionState.GetBool(kFirstEnterUnity, true))
            {
                SessionState.SetBool(kFirstEnterUnity, false);
                Menu.SetChecked(menuEnableManualReload, IsManualReload ? true : false);
                Menu.SetChecked(menuDisenableManualReload, IsManualReload ? false : true);

                if (IsManualReload)
                {
                    UnlockReloadDomain();
                    LockRealodDomain();
                }

                Debug.Log($"<color=lime>当前ReloadDomain状态,是否手动: {IsManualReload}</color>");
            }
        }


        //运行模式改变
        private static void EditorApplication_PlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.EnteredEditMode:
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    if (tempData)
                    {
                        UnlockReloadDomain();
                        EditorUtility.RequestScriptReload();
                    }

                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    tempData = true;
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
            }
        }

        //当开始编译脚本
        private static void OnCompilationStarted(object obj)
        {
            if (IsManualReload)
            {
                compileSW.Start();
                Debug.Log("<color=yellow>Begin Compile</color>");
            }
        }

        //结束编译
        private static void OnCompilationFinished(object obj)
        {
            if (IsManualReload)
            {
                compileSW.Stop();
                Debug.Log($"<color=yellow>End Compile 耗时:{compileSW.ElapsedMilliseconds} ms</color>");
                compileSW.Reset();
            }
        }

        //开始reload domain
        private static void OnBeforeAssemblyReload()
        {
            if (IsManualReload)
            {
                Debug.Log("<color=yellow>Begin Reload Domain ......</color>");
                //记录时间
                SessionState.SetInt(kReloadDomainTimer, (int)(EditorApplication.timeSinceStartup * 1000));
            }
        }

        //结束reload domain
        private static void OnAfterAssemblyReload()
        {
            if (IsManualReload)
            {
                var timeMS = (int)(EditorApplication.timeSinceStartup * 1000) -
                             SessionState.GetInt(kReloadDomainTimer, 0);
                Debug.Log($"<color=yellow>End Reload Domain 耗时:{timeMS} ms</color>");
                LockRealodDomain();
            }
        }


        static void LockRealodDomain()
        {
            //如果没有锁住 锁住
            if (!IsLocked)
            {
                EditorApplication.LockReloadAssemblies();
            }
        }

        static void UnlockReloadDomain()
        {
            //如果锁住了 打开
            if (IsLocked)
            {
                EditorApplication.UnlockReloadAssemblies();
            }
        }

        [MenuItem(menuEnableManualReload)]
        static void EnableManualReloadDomain()
        {
            Debug.Log("<color=cyan>开启手动 Reload Domain</color>");

            Menu.SetChecked(menuEnableManualReload, true);
            Menu.SetChecked(menuDisenableManualReload, false);

            PlayerPrefs.SetInt(kManualReloadDomain, 1);
            //编辑器设置 projectsetting->editor->enterPlayModeSetting
            EditorSettings.enterPlayModeOptionsEnabled = true;
            EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload;

            LockRealodDomain();
        }

        [MenuItem(menuDisenableManualReload)]
        static void DisenableManualReloadDomain()
        {
            Debug.Log("<color=cyan>关闭手动 Reload Domain</color>");

            Menu.SetChecked(menuEnableManualReload, false);
            Menu.SetChecked(menuDisenableManualReload, true);

            PlayerPrefs.SetInt(kManualReloadDomain, 0);
            UnlockReloadDomain();
            EditorSettings.enterPlayModeOptionsEnabled = false;
        }

        //手动刷新
        [MenuItem(menuRealodDomain)]
        static void ManualReload()
        {
            if (IsManualReload)
            {
                UnlockReloadDomain();
                EditorUtility.RequestScriptReload();
            }
        }
    }
}