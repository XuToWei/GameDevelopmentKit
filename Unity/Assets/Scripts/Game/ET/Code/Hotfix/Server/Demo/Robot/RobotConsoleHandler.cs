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
<<<<<<< HEAD
                        fiber.Debug($"run case start: {caseType}");
                        await EventSystem.Instance.Invoke<RobotInvokeArgs, UniTask>(caseType, new RobotInvokeArgs() { Fiber = fiber, Content = content });
                        fiber.Debug($"run case finish: {caseType}");
=======
                        Log.Debug($"run case start: {caseType}");
                        await EventSystem.Instance.Invoke<RobotInvokeArgs, ETTask>(caseType, new RobotInvokeArgs() { Fiber = fiber, Content = content });
                        Log.Debug($"run case finish: {caseType}");
>>>>>>> 7d37d33dfbf69d664e224d4387156fcf2fda4f70
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
<<<<<<< HEAD
                            fiber.Debug($"run case start: {caseType}");
                            await EventSystem.Instance.Invoke<RobotInvokeArgs, UniTask>(caseType, new RobotInvokeArgs() { Fiber = fiber, Content = content});
                            fiber.Debug($"---------run case finish: {caseType}");
=======
                            Log.Debug($"run case start: {caseType}");
                            await EventSystem.Instance.Invoke<RobotInvokeArgs, ETTask>(caseType, new RobotInvokeArgs() { Fiber = fiber, Content = content});
                            Log.Debug($"---------run case finish: {caseType}");
>>>>>>> 7d37d33dfbf69d664e224d4387156fcf2fda4f70
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