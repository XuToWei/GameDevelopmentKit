using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace ET
{
    [EntitySystemOf(typeof(AIComponent))]
    [FriendOf(typeof(AIComponent))]
    [FriendOf(typeof(AIDispatcherComponent))]
    public static partial class AIComponentSystem
    {
        [Invoke(TimerInvokeType.AITimer)]
        public class AITimer: ATimer<AIComponent>
        {
            protected override void Run(AIComponent self)
            {
                try
                {
                    self.Check();
                }
                catch (Exception e)
                {
                    Log.Error($"move timer error: {self.Id}\n{e}");
                }
            }
        }
    
        [EntitySystem]
        private static void Awake(this AIComponent self, int aiConfigId)
        {
            self.AIConfigId = aiConfigId;
            self.Timer = self.Root().GetComponent<TimerComponent>().NewRepeatedTimer(1000, TimerInvokeType.AITimer, self);
        }

        [EntitySystem]
        private static void Destroy(this AIComponent self)
        {
            self.Root().GetComponent<TimerComponent>()?.Remove(ref self.Timer);
            self.CancellationTokenSource?.Cancel();
            self.CancellationTokenSource = null;
            self.Current = 0;
        }

        private static void Check(this AIComponent self)
        {
            Fiber fiber = self.Fiber();
            if (self.Parent == null)
            {
                fiber.Root.GetComponent<TimerComponent>().Remove(ref self.Timer);
                return;
            }

            var oneAI = Tables.Instance.DTAIConfig.AIConfigs[self.AIConfigId];

            foreach (var aiConfig in oneAI.Values)
            {

                AAIHandler aaiHandler = AIDispatcherComponent.Instance.Get(aiConfig.Name);

                if (aaiHandler == null)
                {
                    Log.Error($"not found aihandler: {aiConfig.Name}");
                    continue;
                }

                int ret = aaiHandler.Check(self, aiConfig);
                if (ret != 0)
                {
                    continue;
                }

                if (self.Current == aiConfig.Id)
                {
                    break;
                }

                self.Cancel(); // 取消之前的行为
                CancellationTokenSource cts = new();
                self.CancellationTokenSource = cts;
                self.Current = aiConfig.Id;

                aaiHandler.Execute(self, aiConfig, cts.Token).Forget();
                return;
            }
            
        }

        private static void Cancel(this AIComponent self)
        {
            self.CancellationTokenSource?.Cancel();
            self.Current = 0;
            self.CancellationTokenSource = null;
        }
    }
} 