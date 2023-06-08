using System;

namespace ET.Server
{
    [FriendOf(typeof(MailBoxComponent))]
    public static partial class MailBoxComponentSystem
    {
        [EntitySystem]
        private class MailBoxComponentAwakeSystem : AwakeSystem<MailBoxComponent, MailboxType>
        {
            protected override void Awake(MailBoxComponent self, MailboxType mailboxType)
            {
                self.MailboxType = mailboxType;
                self.ParentInstanceId = self.Parent.InstanceId;
                ActorMessageDispatcherComponent.Instance.Add(self.Parent);
            }
        }

        [EntitySystem]
        private class MailBoxComponentDestroySystem : DestroySystem<MailBoxComponent>
        {
            protected override void Destroy(MailBoxComponent self)
            {
                ActorMessageDispatcherComponent.Instance?.Remove(self.ParentInstanceId);
            }
        }
    }
}