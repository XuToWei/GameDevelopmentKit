using System;

namespace ET.Server
{
    [EntitySystemOf(typeof(AgentHeartbeatComponent))]
    [FriendOf(typeof(AgentHeartbeatComponent))]
    public static partial class AgentHeartbeatComponentSystem
    {
        // 心跳间隔 5 秒
        private const int HeartbeatInterval = 5000;

        [Invoke(TimerInvokeType.AgentHeartbeat)]
        public class AgentHeartbeatTimer : ATimer<AgentHeartbeatComponent>
        {
            protected override void Run(AgentHeartbeatComponent self)
            {
                try
                {
                    self.SendHeartbeat();
                }
                catch (Exception e)
                {
                    Log.Error($"Agent heartbeat timer error: {self.Id}\n{e}");
                }
            }
        }

        [EntitySystem]
        private static void Awake(this AgentHeartbeatComponent self)
        {
            self.Timer = self.Root().GetComponent<TimerComponent>().NewRepeatedTimer(HeartbeatInterval, TimerInvokeType.AgentHeartbeat, self);
            // 立即发送一次心跳
            self.SendHeartbeat();
        }

        [EntitySystem]
        private static void Destroy(this AgentHeartbeatComponent self)
        {
            self.Root().GetComponent<TimerComponent>()?.Remove(ref self.Timer);
        }

        private static void SendHeartbeat(this AgentHeartbeatComponent self)
        {
            var adminConfig = Tables.Instance.DTStartSceneConfig.AdminConfig;
            if (adminConfig == null)
            {
                return;
            }

            var report = Agent2Admin_StatusReport.Create();
            report.AgentProcessId = Options.Instance.Process;
            report.ManagedProcessCount = self.Root().GetComponent<AgentProcessComponent>()?.Processes.Count ?? 0;
            report.MemoryUsage = GC.GetTotalMemory(false);

            var adminActorId = new ActorId(adminConfig.Process, adminConfig.Id);
            self.Root().GetComponent<MessageSender>()?.Send(adminActorId, report);
        }
    }
}
