using System;
using Cysharp.Threading.Tasks;

namespace ET
{
    [ConsoleHandler(ConsoleMode.ReloadConfig)]
    public class ReloadConfigConsoleHandler: IConsoleHandler
    {
        public async UniTask Run(Fiber fiber, ModeContex contex, string content)
        {
            switch (content)
            {
                case ConsoleMode.ReloadConfig:
                    contex.Parent.RemoveComponent<ModeContex>();
                    Log.Console("C must have config name, like: C UnitConfig");
                    break;
                default:
                    string[] ss = content.Split(" ");
                    string configName = ss[1];
                    if (configName == "-a")
                    {
                        await ConfigComponent.Instance.ReloadAllAsync();
                        Log.Console($"reload all config finish!");
                    }
                    else
                    {
                        Type type = CodeTypes.Instance.GetType($"ET.{configName}");
                        if (type == null)
                        {
                            Log.Console($"reload config but not find {configName}");
                            return;
                        }

                        await ConfigComponent.Instance.ReloadOneAsync(configName);
                        Log.Console($"reload config {configName} finish!");
                    }
                    break;
            }
        }
    }
}