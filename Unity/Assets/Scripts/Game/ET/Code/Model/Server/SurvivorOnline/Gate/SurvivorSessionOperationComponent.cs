namespace ET.Server
{
    [ComponentOf(typeof(Session))]
    public sealed class SurvivorSessionOperationComponent: Entity, IAwake
    {
        private EntityRef<SurvivorRoomDirectoryComponent> directory;

        public SurvivorRoomDirectoryComponent Directory
        {
            get
            {
                return this.directory;
            }
            set
            {
                this.directory = value;
            }
        }

        public string RoomCode { get; set; }

        public int FiberId { get; set; }

        public ActorId RoomActorId { get; set; }

        public G2SurvivorRoom_Init InitRequest { get; set; }

        public SurvivorRoom2G_Init InitResponse { get; set; }

        public G2SurvivorRoom_Join JoinRequest { get; set; }

        public SurvivorRoom2G_Join JoinResponse { get; set; }

        public G2SurvivorRoom_Start StartRequest { get; set; }

        public SurvivorRoom2G_Start StartResponse { get; set; }

        public G2SurvivorRoom_Input InputMessage { get; set; }

        public G2SurvivorRoom_RequestFullSnapshot FullSnapshotRequest { get; set; }
    }
}
