using System;

namespace MultiPlay
{
    public static class ByteExtensions
    {
        public enum SizeUnits
        {
            Byte, KB, MB, GB, TB, PB, EB, ZB, YB
        }

        public static string ToSize(this Int64 value, SizeUnits unit)
        {
            var convertedValue = (value / (double)Math.Pow(1024, (Int64)unit)).ToString("0.00");
            return convertedValue + $" {unit.ToString()}s";
        }
    }
}