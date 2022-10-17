using System;
using CommandLine;
using ET;
using GameFramework.Fsm;
using GameFramework.Procedure;

namespace UGF
{
    public class ProcedurePreloadET : ProcedureBase
    {
        protected override async void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);

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

            ETTask.ExceptionHandler += Log.Error;

            await Game.AddSingleton<CodeLoader>().StartAsync();

            UnityGameFramework.Runtime.Log.Debug("ET load successfully!");

            ChangeState<ProcedureET>(procedureOwner);
        }
    }
}