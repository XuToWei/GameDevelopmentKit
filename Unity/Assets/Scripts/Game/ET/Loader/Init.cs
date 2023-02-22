using System;
using CommandLine;
using Game;
using UnityEngine;

namespace ET
{
    public class Init : MonoBehaviour
    {
        private class Runner : MonoBehaviour
        {
            private void Update()
            {
                Game.Update();
            }

            private void LateUpdate()
            {
                Game.LateUpdate();
                Game.FrameFinishUpdate();
            }

            private void OnApplicationQuit()
            {
                Game.Close();
            }
        }

        private void Start()
        {
            this.gameObject.name = "ET";//提供给编辑器ETView使用
            GameEntry.ETRunner.SetRunAction(StartAsync);
        }

        private async void StartAsync()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => { Log.Error(e.ExceptionObject.ToString()); };

            Game.AddSingleton<MainThreadSynchronizationContext>();

            // 命令行参数
            string[] args = "".Split(" ");
            Parser.Default.ParseArguments<Options>(args)
                    .WithNotParsed(error => throw new Exception($"命令行格式错误! {error}"))
                    .WithParsed(Game.AddSingleton);

            Game.AddSingleton<TimeInfo>();
            Game.AddSingleton<Logger>().ILog = new UnityLogger();
            Game.AddSingleton<ObjectPool>();
            Game.AddSingleton<IdGenerater>();
            Game.AddSingleton<EventSystem>();
            Game.AddSingleton<TimerComponent>();
            Game.AddSingleton<CoroutineLockComponent>();

            //ETTask.ExceptionHandler += Log.Error;
            
            await Game.AddSingleton<CodeLoader>().StartAsync();

            this.gameObject.AddComponent<Runner>();
        }
    }
}