using System;

namespace ET.Server
{
    [FriendOf(typeof(ActorLocationSender))]
    public static partial class ActorLocationSenderSystem
    {
        [EntitySystem]
        private class ActorLocationSenderAwakeSystem : AwakeSystem<ActorLocationSender>
        {
            protected override void Awake(ActorLocationSender self)
            {
                self.LastSendOrRecvTime = TimeHelper.ServerNow();
                self.ActorId = 0;
                self.Error = 0;
            }
        }

        [EntitySystem]
        private class ActorLocationSenderDestroySystem : DestroySystem<ActorLocationSender>
        {
            protected override void Destroy(ActorLocationSender self)
            {
                Log.Debug($"actor location remove: {self.Id}");
                self.LastSendOrRecvTime = 0;
                self.ActorId = 0;
                self.Error = 0;
            }
        }
    }
}