using System.Text.RegularExpressions;

namespace UnityGameFramework.Extension.Editor
{
    public static class UriUtility
    {
        private static readonly Regex s_Regex = new Regex(@"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\*\+,;=.]+$");
            
        public static bool CheckUri(string uri)
        {
            if (string.IsNullOrEmpty(uri))
            {
                return false;
            }
            return s_Regex.IsMatch(uri);
        }
    }
}