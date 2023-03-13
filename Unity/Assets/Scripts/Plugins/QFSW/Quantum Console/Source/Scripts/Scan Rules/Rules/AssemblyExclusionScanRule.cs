using System.Reflection;

namespace QFSW.QC.ScanRules
{
    public class AssemblyExclusionScanRule : IQcScanRule
    {
        public ScanRuleResult ShouldScan<T>(T entity) where T : ICustomAttributeProvider
        {
            if (entity is Assembly assembly)
            {
                string[] bannedPrefixes = new string[]
                {
                    "System", "Unity", "Microsoft", "Mono.", "mscorlib", "NSubstitute", "JetBrains", "nunit.",
                    "GeNa."
#if QC_DISABLE_BUILTIN_ALL
                    , "QFSW.QC"
#elif QC_DISABLE_BUILTIN_EXTRA
                    , "QFSW.QC.Extra"
#endif
                };

                string[] bannedAssemblies = new string[]
                {
                    "mcs", "AssetStoreTools",
                    "Facepunch.Steamworks"
                };

                string assemblyFullName = assembly.FullName;
                foreach (string prefix in bannedPrefixes)
                {
                    if (assemblyFullName.StartsWith(prefix))
                    {
                        return ScanRuleResult.Reject;
                    }
                }

                string assemblyShortName = assembly.GetName().Name;
                foreach (string name in bannedAssemblies)
                {
                    if (assemblyShortName == name)
                    {
                        return ScanRuleResult.Reject;
                    }
                }
            }

            return ScanRuleResult.Accept;
        }
    }
}