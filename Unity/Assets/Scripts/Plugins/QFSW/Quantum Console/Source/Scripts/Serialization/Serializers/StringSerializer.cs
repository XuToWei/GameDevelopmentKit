namespace QFSW.QC.Serializers
{
    public class StringSerializer : BasicQcSerializer<string>
    {
        public override int Priority => int.MaxValue;

        public override string SerializeFormatted(string value, QuantumTheme theme)
        {
            return value;
        }
    }
}
