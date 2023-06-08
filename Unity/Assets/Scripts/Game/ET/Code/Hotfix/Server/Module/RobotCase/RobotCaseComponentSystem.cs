using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [FriendOf(typeof(RobotCaseComponent))]
    public static partial class RobotCaseComponentSystem
    {
        [EntitySystem]
        private class RobotCaseComponentAwakeSystem : AwakeSystem<RobotCaseComponent>
        {
            protected override void Awake(RobotCaseComponent self)
            {
                RobotCaseComponent.Instance = self;
            }
        }

        [EntitySystem]
        private class RobotCaseComponentDestroySystem : DestroySystem<RobotCaseComponent>
        {
            protected override void Destroy(RobotCaseComponent self)
            {
                RobotCaseComponent.Instance = null;
            }
        }

        public static int GetN(this RobotCaseComponent self)
        {
            return ++self.N;
        }
        
        public static async UniTask<RobotCase> New(this RobotCaseComponent self)
        {
            await UniTask.CompletedTask;
            RobotCase robotCase = self.AddChild<RobotCase>();
            return robotCase;
        }
    }
}