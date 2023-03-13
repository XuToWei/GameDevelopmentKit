namespace QFSW.QC.Parsers
{
    public class BoolParser : BasicCachedQcParser<bool>
    {
        public override bool Parse(string value)
        {
            value = value.ToLower().Trim();
            switch (value)
            {
                case "true": return true;
                case "on": return true;
                case "1": return true;
                case "yes": return true;
                case "false": return false;
                case "off": return false;
                case "0": return false;
                case "no": return false;
                default: throw new ParserInputException($"Cannot parse '{value}' to a bool.");
            }
        }
    }
}
