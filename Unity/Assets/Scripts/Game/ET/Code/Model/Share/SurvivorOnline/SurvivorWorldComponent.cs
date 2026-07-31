using ReactiveBinding;

namespace ET
{
    [ComponentOf(typeof(SurvivorRoom))]
    public partial class SurvivorWorldComponent: Entity, IAwake<SurvivorWorldRole, string>, IDestroy, IVersionSync
    {
        [VersionField]
        private SurvivorWorldData __Data;

        public SurvivorWorldRole Role { get; set; }

        public SurvivorWorldRuntime Runtime { get; set; }
    }
}
