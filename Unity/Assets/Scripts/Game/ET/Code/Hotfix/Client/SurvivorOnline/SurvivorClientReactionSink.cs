namespace ET.Client
{
    [EnableClass]
    public sealed class SurvivorClientReactionSink: ISurvivorClientReactionSink
    {
        public void OnMembershipChanged(SurvivorClientComponent client)
        {
            client.ReconcileStateEntries();
        }
    }
}
