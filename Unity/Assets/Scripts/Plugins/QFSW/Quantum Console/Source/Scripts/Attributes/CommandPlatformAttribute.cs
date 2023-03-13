using System;

namespace QFSW.QC
{
    /// <summary>Determines which platforms the command is available on. Supersedes platform availability determined in the [Command].</summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class CommandPlatformAttribute : Attribute
    {
        public readonly Platform SupportedPlatforms;

        public CommandPlatformAttribute(Platform supportedPlatforms)
        {
            SupportedPlatforms = supportedPlatforms;
        }
    }
}