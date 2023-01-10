using System;

namespace ET
{
    [ConsoleHandler(ConsoleMode.ReloadConfig)]
    public class ReloadConfigConsoleHandler: IConsoleHandler
    {
        public async ETTask Run(ModeContex contex, string content)
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
                    Type type = EventSystem.Instance.GetType($"ET.{configName}");
                    if (type == null)
                    {
                        Log.Console($"reload config but not find {configName}");
                        return;
                    }
                    await ConfigComponent.Instance.LoadOneAsync(configName);
                    Log.Console($"reload config {configName} finish!");
                    break;
            }
            
            await ETTask.CompletedTask;
        }
    }
}