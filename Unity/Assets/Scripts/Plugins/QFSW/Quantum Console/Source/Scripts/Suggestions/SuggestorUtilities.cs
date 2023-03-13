using QFSW.QC.Utilities;

namespace QFSW.QC
{
    /// <summary>
    /// Common utilities used by the suggestion system.
    /// </summary>
    public static class SuggestorUtilities
    {
        /// <summary>
        /// Determines if a prompt is compatible with a string suggestion.
        /// </summary>
        /// <param name="prompt">The prompt to test.</param>
        /// <param name="suggestion">The string suggestion to test again.</param>
        /// <param name="options">The options used by the suggestor.</param>
        /// <returns>If the prompt is compatible.</returns>
        public static bool IsCompatible(string prompt, string suggestion, SuggestorOptions options)
        {
            if (prompt.Length > suggestion.Length)
            {
                return false;
            }

            if (options.Fuzzy)
            {
                return options.CaseSensitive
                    ? suggestion.Contains(prompt)
                    : suggestion.ContainsCaseInsensitive(prompt);
            }

            return suggestion.StartsWith(prompt, !options.CaseSensitive, null);
        }
    }
}