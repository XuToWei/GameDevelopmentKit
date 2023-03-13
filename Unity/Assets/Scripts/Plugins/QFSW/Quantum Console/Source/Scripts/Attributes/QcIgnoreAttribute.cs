using System;

namespace QFSW.QC
{
    /// <summary>
    /// Instructs QC to ignore this entity when scanning the code base for commands.
    /// This can be used to optimise QCs loading times in large codebases when there are large entities that do not have any commands present.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class QcIgnoreAttribute : Attribute { }
}
