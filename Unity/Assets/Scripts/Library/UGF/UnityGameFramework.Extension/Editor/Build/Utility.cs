using System.Text.RegularExpressions;

namespace UnityGameFramework.Extension.Editor
{
    public static class UriUtility
    {
        static readonly Regex regex = new Regex(@"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\*\+,;=.]+$");
            
        public static bool CheckUri(string uri)
        {
            if (string.IsNullOrEmpty(uri))
            {
                return false;
            }
            return regex.IsMatch(uri);
        }
    }
}