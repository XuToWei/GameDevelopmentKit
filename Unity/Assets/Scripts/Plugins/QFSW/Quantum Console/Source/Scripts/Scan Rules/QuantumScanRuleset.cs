using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace QFSW.QC
{
    /// <summary>
    /// A set of rules for determining which entities should and shouldn't be scanned by Quantum Console for commands.
    /// </summary>
    public class QuantumScanRuleset
    {
        private readonly IQcScanRule[] _scanRules;

        /// <summary>
        /// Creates a Quantum Scan Ruleset with a custom set of scan rules.
        /// </summary>
        /// <param name="scanRules">The IQcScanRules to use in this Quantum Scan Ruleset.</param>
        public QuantumScanRuleset(IEnumerable<IQcScanRule> scanRules)
        {
            _scanRules = scanRules.ToArray();
        }

        /// <summary>
        /// Creates a Quantum Scan Ruleset with the default injected scan rules
        /// </summary>
        public QuantumScanRuleset() : this(new InjectionLoader<IQcScanRule>().GetInjectedInstances())
        {

        }

        /// <summary>
        /// Queries if the entity should be scanned.
        /// </summary>
        /// <param name="entity">The entity to query.</param>
        /// <returns>If the entity should be scanned.</returns>
        public bool ShouldScan<T>(T entity) where T : ICustomAttributeProvider
        {
            bool shouldScan = true;
            foreach (IQcScanRule scanRule in _scanRules)
            {
                switch (scanRule.ShouldScan(entity))
                {
                    case ScanRuleResult.Accept:
                    {
                        break;
                    }
                    case ScanRuleResult.Reject:
                    {
                        shouldScan = false;
                        break;
                    }
                    case ScanRuleResult.ForceAccept:
                    {
                        return true;
                    }
                }
            }

            return shouldScan;
        }
    }
}