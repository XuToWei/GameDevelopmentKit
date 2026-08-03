using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(UIFormSurvivorHudComponent))]
    [ETReactiveSystem]
    public static partial class UIFormSurvivorHudComponentSystem
    {
        [UGFUIFormSystem]
        private static void UGFUIFormOnOpen(this UIFormSurvivorHudComponent self)
        {
            self.Client = self.Root().GetComponent<SurvivorClientComponent>();
            self.ObserveChanges();
        }

        [UGFUIFormSystem]
        private static void UGFUIFormOnUpdate(
            this UIFormSurvivorHudComponent self,
            float elapseSeconds,
            float realElapseSeconds)
        {
            self.Client.SendInput(
                (int)(Input.GetAxisRaw("Horizontal") * SurvivorDefaults.InputScale),
                (int)(Input.GetAxisRaw("Vertical") * SurvivorDefaults.InputScale));
            self.ObserveChanges();
        }

        [UGFUIFormSystem]
        private static void UGFUIFormOnClose(this UIFormSurvivorHudComponent self, bool isShutdown)
        {
            self.ClearReactive();
            self.Client = null;
        }

        private static SurvivorWorldData WorldData(this UIFormSurvivorHudComponent self)
        {
            return self.Client?.World?.Data;
        }

        private static SurvivorPlayerState LocalPlayerState(this UIFormSurvivorHudComponent self)
        {
            SurvivorClientComponent client = self.Client;
            if (client?.Runtime == null)
            {
                return null;
            }

            client.Runtime.PlayerStates.TryGetValue(client.PlayerId, out SurvivorPlayerState state);
            return state;
        }

        [ETReactiveSource]
        private static string RoomCode(this UIFormSurvivorHudComponent self)
        {
            return self.WorldData()?.RoomCode ?? string.Empty;
        }

        [ETReactiveSource]
        private static long ServerTick(this UIFormSurvivorHudComponent self)
        {
            return self.WorldData()?.ServerTick ?? 0;
        }

        [ETReactiveSource]
        private static SurvivorRoomPhase Phase(this UIFormSurvivorHudComponent self)
        {
            return self.WorldData()?.Phase ?? SurvivorRoomPhase.Lobby;
        }

        [ETReactiveSource]
        private static int Hp(this UIFormSurvivorHudComponent self)
        {
            return self.LocalPlayerState()?.Hp ?? 0;
        }

        [ETReactiveSource]
        private static int MaxHp(this UIFormSurvivorHudComponent self)
        {
            return self.LocalPlayerState()?.MaxHp ?? 0;
        }

        [ETReactiveSource]
        private static int Level(this UIFormSurvivorHudComponent self)
        {
            return self.LocalPlayerState()?.Level ?? 0;
        }

        [ETReactiveBind(nameof(RoomCode))]
        private static void OnRoomCodeChanged(this UIFormSurvivorHudComponent self, string roomCode)
        {
            self.View.RoomText.text = roomCode;
        }

        [ETReactiveBind(nameof(ServerTick))]
        private static void OnServerTickChanged(this UIFormSurvivorHudComponent self, long serverTick)
        {
            self.View.TickText.text = serverTick.ToString();
        }

        [ETReactiveBind(nameof(Phase))]
        private static void OnPhaseChanged(
            this UIFormSurvivorHudComponent self,
            SurvivorRoomPhase phase)
        {
            self.View.PhaseText.text = phase.ToString();
        }

        [ETReactiveBind(nameof(Hp), nameof(MaxHp))]
        private static void OnHealthChanged(
            this UIFormSurvivorHudComponent self,
            int hp,
            int maxHp)
        {
            self.View.HpText.text = $"{hp}/{maxHp}";
        }

        [ETReactiveBind(nameof(Level))]
        private static void OnLevelChanged(this UIFormSurvivorHudComponent self, int level)
        {
            self.View.LevelText.text = level.ToString();
        }
    }
}
