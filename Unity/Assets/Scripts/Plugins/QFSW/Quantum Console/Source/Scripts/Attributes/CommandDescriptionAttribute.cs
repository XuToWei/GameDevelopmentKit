using System;

namespace QFSW.QC
{
    /// <summary>Provides a command with a description. If the [Command] attribute already provides a description, that will supersede this one. Useful for when you have several [Command]s on a single method.</summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class CommandDescriptionAttribute : Attribute
    {
        public readonly string Description;
        public readonly bool Valid;

        public CommandDescriptionAttribute(string description)
        {
            Description = description;
            Valid = !string.IsNullOrWhiteSpace(description);
        }
    }
}
