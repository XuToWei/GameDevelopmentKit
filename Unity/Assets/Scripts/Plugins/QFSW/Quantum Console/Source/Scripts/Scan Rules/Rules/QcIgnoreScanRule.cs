using System.Reflection;
using System.Runtime.CompilerServices;
using QFSW.QC.Utilities;

namespace QFSW.QC.ScanRules
{
    public class QcIgnoreScanRule : IQcScanRule
    {
        public ScanRuleResult ShouldScan<T>(T entity) where T : ICustomAttributeProvider
        {
            if (entity.HasAttribute<QcIgnoreAttribute>(false))
            {
                return ScanRuleResult.Reject;
            }

            // Allow compiler generated members as this includes backing fields which may be used by the user
            if (!(entity is MemberInfo) && entity.HasAttribute<CompilerGeneratedAttribute>(true))
            {
                return ScanRuleResult.Reject;
            }

            return ScanRuleResult.Accept;
        }
    }
}