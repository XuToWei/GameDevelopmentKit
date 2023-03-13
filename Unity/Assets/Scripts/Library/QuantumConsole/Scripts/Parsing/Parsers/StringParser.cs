namespace QFSW.QC.Parsers
{
    public class StringParser : BasicCachedQcParser<string>
    {
        public override int Priority => int.MaxValue;

        public override string Parse(string value)
        {
            return value
                .ReduceScope('"', '"')
                .UnescapeText('"');
        }
    }
}
