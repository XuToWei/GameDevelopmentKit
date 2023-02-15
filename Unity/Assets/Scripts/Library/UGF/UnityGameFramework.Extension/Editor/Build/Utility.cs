using System;
using System.Text.RegularExpressions;

namespace UnityGameFramework.Extension.Editor
{
    public static class Utility
    {
        public static class Uri
        {
            public static bool CheckUri(string uri)
            {
                if (string.IsNullOrEmpty(uri))
                {
                    return false;
                }
                Regex regex = new Regex(@"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\*\+,;=.]+$");
                return regex.IsMatch(uri);
            }
        }
    }
}