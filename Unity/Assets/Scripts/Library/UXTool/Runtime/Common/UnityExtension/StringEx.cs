using System.Collections.Generic;
using System;
namespace ThunderFireUnityEx
{
    public static class StringExtension
    {
        public static bool StartsWith(this string target, string value)
        {
            return target.StartsWith(value, StringComparison.Ordinal);
        }

        public static bool EndsWith(this string target, string value)
        {
            return target.EndsWith(value, StringComparison.Ordinal);
        }

        public static int IndexOf(this string target, String value)
        {
            return target.IndexOf(value, StringComparison.Ordinal);
        }

        public static int LastIndexOf(this string target, String value)
        {
            return target.LastIndexOf(value, StringComparison.Ordinal);
        }

        public static int Compare(this string target, String strB)
        {
            return String.Compare(target, strB, StringComparison.Ordinal);
        }
    }
}