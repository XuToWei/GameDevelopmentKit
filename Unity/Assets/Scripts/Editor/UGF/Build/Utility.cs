using System.Text.RegularExpressions;

namespace UGF.Editor
{
    public static class UriUtility
    {
        private static readonly Regex UriRegex = new Regex(@"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\*\+,;=.]+$");
        
        public static bool CheckUri(string uri)
        {
            if (string.IsNullOrEmpty(uri))
            {
                return false;
            }
            return UriRegex.IsMatch(uri);
        }
    }
}