namespace QFSW.QC.Suggestors.Tags
{
    public struct CommandNameTag : IQcSuggestorTag
    {

    }

    /// <summary>
    /// Specifies that command name values should be suggested for the parameter.
    /// </summary>
    public sealed class CommandNameAttribute : SuggestorTagAttribute
    {
        private readonly IQcSuggestorTag[] _tags = { new CommandNameTag() };

        public override IQcSuggestorTag[] GetSuggestorTags()
        {
            return _tags;
        }
    }
}