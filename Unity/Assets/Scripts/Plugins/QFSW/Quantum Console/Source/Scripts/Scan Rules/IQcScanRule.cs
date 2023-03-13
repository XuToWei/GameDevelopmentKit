using System.Reflection;

namespace QFSW.QC
{
    /// <summary>
    /// A rule for determining which entities should and shouldn't be scanned by Quantum Console for commands.
    /// </summary>
    public interface IQcScanRule
    {
        /// <summary>
        /// Queries if the entity should be scanned.
        /// </summary>
        /// <param name="entity">The entity to query.</param>
        /// <returns>The result of the rule query.</returns>
        ScanRuleResult ShouldScan<T>(T entity) where T : ICustomAttributeProvider;
    }
}