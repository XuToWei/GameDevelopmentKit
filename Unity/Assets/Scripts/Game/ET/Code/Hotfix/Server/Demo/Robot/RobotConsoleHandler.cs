using System;
using System.Reflection;
using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [ConsoleHandler(ConsoleMode.Robot)]
    public class RobotConsoleHandler: IConsoleHandler
    {
        public async UniTask Run(Fiber fiber, ModeContex contex, string content)
        {
            string[] ss = content.Split(" ");
            switch (ss[0])
            {
                case ConsoleMode.Robot:
                    break;

                case "Run":
                {
                    int caseType = int.Parse(ss[1]);

                    try
                    {
                        Log.Debug($"run case start: {caseType}");
                        await EventSystem.Instance.Invoke<RobotInvokeArgs, UniTask>(caseType, new RobotInvokeArgs() { Fiber = fiber, Content = content });
                        Log.Debug($"run case finish: {caseType}");
                    }
                    catch (Exception e)
                    {
                        Log.Debug($"run case error: {caseType}\n{e}");
                    }
                    break;
                }
                case "RunAll":
                {
                    FieldInfo[] fieldInfos = typeof (RobotCaseType).GetFields();
                    foreach (FieldInfo fieldInfo in fieldInfos)
                    {
                        int caseType = (int)fieldInfo.GetValue(null);
                        if (caseType > RobotCaseType.MaxCaseType)
                        {
                            Log.Debug($"case > {RobotCaseType.MaxCaseType}: {caseType}");
                            break;
                        }
                        try
                        {
                            Log.Debug($"run case start: {caseType}");
                            await EventSystem.Instance.Invoke<RobotInvokeArgs, UniTask>(caseType, new RobotInvokeArgs() { Fiber = fiber, Content = content});
                            Log.Debug($"---------run case finish: {caseType}");
                        }
                        catch (Exception e)
                        {
                            Log.Debug($"run case error: {caseType}\n{e}");
                            break;
                        }
                    }
                    break;
                }
            }
            await UniTask.CompletedTask;
        }
    }
}