using System;
using CommandLine;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ET
{
    [DisallowMultipleComponent]
    public class Init : MonoBehaviour
    {
        public static Init Instance { get; private set; }

        private class Runner : MonoBehaviour
        {
            private void Update()
            {
                TimeInfo.Instance.Update();
                FiberManager.Instance.Update();
            }

            private void LateUpdate()
            {
                FiberManager.Instance.LateUpdate();
            }

            private void OnDestroy()
            {
                EventSystem.Instance.Invoke(new OnShutdown());
                World.Instance.Dispose();
            }

            private void OnApplicationPause(bool pauseStatus)
            {
                EventSystem.Instance.Invoke(new OnApplicationPause(pauseStatus));
            }

            private void OnApplicationFocus(bool hasFocus)
            {
                EventSystem.Instance.Invoke(new OnApplicationFocus(hasFocus));
            }
        }

        private Runner m_RunnerComponent;

        private void Awake()
        {
            Instance = this;
#if UNITY_ET_VIEW && UNITY_EDITOR
            Entity.SetRootView(this.transform);
#endif
        }

        private void Start()
        {
            StartAsync().Forget();
        }

        private void OnDestroy()
        {
            if (this.m_RunnerComponent != null)
            {
                DestroyImmediate(this.m_RunnerComponent);
            }
        }

        private async UniTaskVoid StartAsync()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Log.Error(e.ExceptionObject.ToString());
            };

            // 命令行参数
            string[] args = "".Split(" ");
            Parser.Default.ParseArguments<Options>(args)
                    .WithNotParsed(error => throw new Exception($"命令行格式错误! {error}"))
                    .WithParsed((o) => World.Instance.AddSingleton(o));
            Options.Instance.StartConfig = "Localhost";

            World.Instance.AddSingleton<Logger, ILog>(new UnityLogger());
            World.Instance.AddSingleton<TimeInfo, ITimeNow>(new UnityTimeNow());
            World.Instance.AddSingleton<FiberManager>();
            World.Instance.AddSingleton<ConfigComponent, IConfigReader>(new ConfigReader());
            World.Instance.AddSingleton<CodeLoaderComponent, ICodeLoader>(new CodeLoader());

            await CodeLoaderComponent.Instance.StartAsync();
            this.m_RunnerComponent = this.gameObject.AddComponent<Runner>();
        }
    }
}