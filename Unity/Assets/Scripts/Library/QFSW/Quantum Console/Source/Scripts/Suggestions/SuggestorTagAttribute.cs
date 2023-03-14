using System;

namespace QFSW.QC
{
    /// <summary>
    /// Base attribute for all IQcSuggestorTag sources.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public abstract class SuggestorTagAttribute : Attribute
    {
        public abstract IQcSuggestorTag[] GetSuggestorTags();
    }
}