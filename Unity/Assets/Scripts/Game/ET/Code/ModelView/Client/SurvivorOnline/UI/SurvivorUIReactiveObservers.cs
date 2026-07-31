using ReactiveBinding;

namespace ET.Client
{
    public interface ISurvivorLobbyReactionSink
    {
        void OnRoomChanged(
            UIFormSurvivorLobbyComponent ui,
            string roomCode,
            SurvivorRoomPhase phase,
            bool isHost);
    }

    public interface ISurvivorHudReactionSink
    {
        void OnRoomCodeChanged(UIFormSurvivorHudComponent ui, string roomCode);

        void OnServerTickChanged(UIFormSurvivorHudComponent ui, long serverTick);

        void OnPhaseChanged(UIFormSurvivorHudComponent ui, SurvivorRoomPhase phase);

        void OnHealthChanged(UIFormSurvivorHudComponent ui, int hp, int maxHp);

        void OnLevelChanged(UIFormSurvivorHudComponent ui, int level);
    }

    [EnableClass]
    [ReactiveObserveIgnore]
    public sealed partial class SurvivorLobbyReactiveObserver: IReactiveObserver
    {
        private EntityRef<UIFormSurvivorLobbyComponent> ui;
        private EntityRef<SurvivorClientComponent> client;
        private ISurvivorLobbyReactionSink sink;

        public SurvivorLobbyReactiveObserver(
            UIFormSurvivorLobbyComponent ui,
            SurvivorClientComponent client,
            ISurvivorLobbyReactionSink sink)
        {
            this.ui = ui;
            this.client = client;
            this.sink = sink;
        }

        private SurvivorClientComponent Client
        {
            get
            {
                return this.client;
            }
        }

        private SurvivorWorldData WorldData
        {
            get
            {
                return this.Client.Room?.GetComponent<SurvivorWorldComponent>()?.Data;
            }
        }

        [ReactiveSource]
        private string RoomCode
        {
            get
            {
                return this.WorldData?.RoomCode ?? string.Empty;
            }
        }

        [ReactiveSource]
        private SurvivorRoomPhase Phase
        {
            get
            {
                return this.WorldData?.Phase ?? SurvivorRoomPhase.Lobby;
            }
        }

        [ReactiveSource]
        private bool IsHost
        {
            get
            {
                return this.Client.IsHost;
            }
        }

        [ReactiveBind(nameof(RoomCode), nameof(Phase), nameof(IsHost))]
        private void OnRoomChanged(string roomCode, SurvivorRoomPhase phase, bool isHost)
        {
            this.sink.OnRoomChanged(this.ui, roomCode, phase, isHost);
        }
    }

    [EnableClass]
    [ReactiveObserveIgnore]
    public sealed partial class SurvivorHudReactiveObserver: IReactiveObserver
    {
        private EntityRef<UIFormSurvivorHudComponent> ui;
        private EntityRef<SurvivorClientComponent> client;
        private ISurvivorHudReactionSink sink;

        public SurvivorHudReactiveObserver(
            UIFormSurvivorHudComponent ui,
            SurvivorClientComponent client,
            ISurvivorHudReactionSink sink)
        {
            this.ui = ui;
            this.client = client;
            this.sink = sink;
        }

        private SurvivorClientComponent Client
        {
            get
            {
                return this.client;
            }
        }

        private SurvivorWorldData WorldData
        {
            get
            {
                return this.Client.Room?.GetComponent<SurvivorWorldComponent>()?.Data;
            }
        }

        [ReactiveSource]
        private string RoomCode
        {
            get
            {
                return this.WorldData?.RoomCode ?? string.Empty;
            }
        }

        [ReactiveSource]
        private long ServerTick
        {
            get
            {
                return this.WorldData?.ServerTick ?? 0;
            }
        }

        [ReactiveSource]
        private SurvivorRoomPhase Phase
        {
            get
            {
                return this.WorldData?.Phase ?? SurvivorRoomPhase.Lobby;
            }
        }

        [ReactiveSource]
        private int Hp
        {
            get
            {
                return this.WorldData != null && this.WorldData.Players.ContainsKey(this.Client.PlayerId)
                        ? this.WorldData.Players[this.Client.PlayerId].Hp
                        : 0;
            }
        }

        [ReactiveSource]
        private int MaxHp
        {
            get
            {
                return this.WorldData != null && this.WorldData.Players.ContainsKey(this.Client.PlayerId)
                        ? this.WorldData.Players[this.Client.PlayerId].MaxHp
                        : 0;
            }
        }

        [ReactiveSource]
        private int Level
        {
            get
            {
                return this.WorldData != null && this.WorldData.Players.ContainsKey(this.Client.PlayerId)
                        ? this.WorldData.Players[this.Client.PlayerId].Level
                        : 0;
            }
        }

        [ReactiveBind(nameof(RoomCode))]
        private void OnRoomCodeChanged(string roomCode)
        {
            this.sink.OnRoomCodeChanged(this.ui, roomCode);
        }

        [ReactiveBind(nameof(ServerTick))]
        private void OnServerTickChanged(long serverTick)
        {
            this.sink.OnServerTickChanged(this.ui, serverTick);
        }

        [ReactiveBind(nameof(Phase))]
        private void OnPhaseChanged(SurvivorRoomPhase phase)
        {
            this.sink.OnPhaseChanged(this.ui, phase);
        }

        [ReactiveBind(nameof(Hp), nameof(MaxHp))]
        private void OnHealthChanged(int hp, int maxHp)
        {
            this.sink.OnHealthChanged(this.ui, hp, maxHp);
        }

        [ReactiveBind(nameof(Level))]
        private void OnLevelChanged(int level)
        {
            this.sink.OnLevelChanged(this.ui, level);
        }
    }
}
