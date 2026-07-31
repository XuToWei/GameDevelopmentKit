namespace ET.Client
{
    [EnableClass]
    public sealed class SurvivorLobbyReactionSink: ISurvivorLobbyReactionSink
    {
        public void OnRoomChanged(
            UIFormSurvivorLobbyComponent ui,
            string roomCode,
            SurvivorRoomPhase phase,
            bool isHost)
        {
            ui.View.StartButton.gameObject.SetActive(
                roomCode.Length > 0 &&
                isHost &&
                phase == SurvivorRoomPhase.Lobby);
            if (roomCode.Length == 0)
            {
                return;
            }

            ui.View.StatusText.text = phase == SurvivorRoomPhase.Lobby
                    ? $"已加入房间 {roomCode}"
                    : phase == SurvivorRoomPhase.Running
                            ? "游戏开始"
                            : "游戏结束";
        }
    }

    [EnableClass]
    public sealed class SurvivorHudReactionSink: ISurvivorHudReactionSink
    {
        public void OnRoomCodeChanged(UIFormSurvivorHudComponent ui, string roomCode)
        {
            ui.View.RoomText.text = roomCode;
        }

        public void OnServerTickChanged(UIFormSurvivorHudComponent ui, long serverTick)
        {
            ui.View.TickText.text = serverTick.ToString();
        }

        public void OnPhaseChanged(UIFormSurvivorHudComponent ui, SurvivorRoomPhase phase)
        {
            ui.View.PhaseText.text = phase.ToString();
        }

        public void OnHealthChanged(UIFormSurvivorHudComponent ui, int hp, int maxHp)
        {
            ui.View.HpText.text = $"{hp}/{maxHp}";
        }

        public void OnLevelChanged(UIFormSurvivorHudComponent ui, int level)
        {
            ui.View.LevelText.text = level.ToString();
        }
    }
}
