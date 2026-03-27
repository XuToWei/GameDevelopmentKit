using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [FriendOf(typeof(AgentProcessComponent))]
    [Invoke((long)SceneType.Agent)]
    public class FiberInit_Agent : AInvokeHandler<FiberInit, UniTask>
    {
        public override async UniTask Handle(FiberInit fiberInit)
        {
            Scene root = fiberInit.Fiber.Root;
            root.AddComponent<MailBoxComponent, MailBoxType>(MailBoxType.UnOrderedMessage);
            root.AddComponent<TimerComponent>();
            root.AddComponent<ProcessInnerSender>();
            root.AddComponent<MessageSender>();

            // 启动本机所有非Agent和非Admin的进程
            var agentProcess = root.AddComponent<AgentProcessComponent>();
            StartServerProcesses(agentProcess);

            // 添加心跳组件，定期向 Admin 发送状态上报
            root.AddComponent<AgentHeartbeatComponent>();

            await UniTask.CompletedTask;
        }

        private static void StartServerProcesses(AgentProcessComponent agentProcess)
        {
            string[] localIP = NetworkHelper.GetAddressIPs();
            var sceneConfig = Tables.Instance.DTStartSceneConfig;

            foreach (var startProcessConfig in Tables.Instance.DTStartProcessConfig.DataList)
            {
                if (startProcessConfig.StartConfig != Options.Instance.StartConfig)
                {
                    continue;
                }

                if (startProcessConfig.Id == Options.Instance.Process)
                {
                    continue;
                }

                if (!WatcherHelper.IsThisMachine(startProcessConfig.InnerIP, localIP))
                {
                    continue;
                }

                // 跳过包含Agent或Admin场景的进程
                if (IsAgentOrAdminProcess(sceneConfig, startProcessConfig.Id))
                {
                    continue;
                }

                var process = WatcherHelper.StartProcess(startProcessConfig.Id);
                agentProcess.Processes.Add(startProcessConfig.Id, process);
            }
        }

        private static bool IsAgentOrAdminProcess(DTStartSceneConfig sceneConfig, int processId)
        {
            List<DRStartSceneConfig> scenes = sceneConfig.GetByProcess(processId);
            if (scenes == null)
            {
                return false;
            }

            foreach (var scene in scenes)
            {
                if (scene.Type == SceneType.Agent || scene.Type == SceneType.Admin)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
