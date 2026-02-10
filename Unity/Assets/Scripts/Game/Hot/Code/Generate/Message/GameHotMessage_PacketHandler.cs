// This is an automatically generated class by Share.Tool. Please do not modify it.

using ProtoBuf;
using System;
using System.Collections.Generic;
using GameFramework;
using GameFramework.Network;
using Game;

namespace Game.Hot.GameHotMessage
{
    public sealed partial class SCHeartBeatTest_Handler : PacketHandlerBase
    {
        public override int Id => GameHotMessageId.SCHeartBeatTest;
        public override void Handle(object sender, Packet packet)
        {
            SCHeartBeatTest rsp = (SCHeartBeatTest)packet;
            OnHandle(sender, rsp);
            GameEntry.Event.FireNow(sender, OnHandelPacketEventArgs.Create(rsp));
        }
        partial void OnHandle(object sender, SCHeartBeatTest packet);
    }

    public sealed partial class SCTest_Handler : PacketHandlerBase
    {
        public override int Id => GameHotMessageId.SCTest;
        public override void Handle(object sender, Packet packet)
        {
            SCTest rsp = (SCTest)packet;
            OnHandle(sender, rsp);
            GameEntry.Event.FireNow(sender, OnHandelPacketEventArgs.Create(rsp));
        }
        partial void OnHandle(object sender, SCTest packet);
    }

    public sealed partial class SCHeartBeatTest22_Handler : PacketHandlerBase
    {
        public override int Id => GameHotMessageId.SCHeartBeatTest22;
        public override void Handle(object sender, Packet packet)
        {
            SCHeartBeatTest22 rsp = (SCHeartBeatTest22)packet;
            OnHandle(sender, rsp);
            GameEntry.Event.FireNow(sender, OnHandelPacketEventArgs.Create(rsp));
        }
        partial void OnHandle(object sender, SCHeartBeatTest22 packet);
    }

}
