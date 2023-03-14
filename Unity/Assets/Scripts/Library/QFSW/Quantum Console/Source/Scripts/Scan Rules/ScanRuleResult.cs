namespace QFSW.QC
{
    /// <summary>
    /// The result of querying an entity with a scan rule.
    /// </summary>
    public enum ScanRuleResult
    {
        /// <summary>
        /// If the entity should be accepted by the scan rule.
        /// This is the lowest priority response and should be used when in doubt.
        /// </summary>
        Accept = 0,

        /// <summary>
        /// If the entity should be rejected by the scan rule.
        /// This takes priority over any other <c>Accept</c> responses.
        /// </summary>
        Reject = 1,

        /// <summary>
        /// If the entity should be forcefully accepted by the scan rule.
        /// This will be immediate and overrides any other <c>Reject</c> responses.
        /// No further scan rules will be considered
        /// </summary>
        ForceAccept = 2
    }
}