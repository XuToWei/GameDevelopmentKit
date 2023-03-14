namespace QFSW.QC
{
    /// <summary>
    /// Options used by the suggestor when producing suggestions.
    /// </summary>
    public struct SuggestorOptions
    {
        /// <summary>
        /// If case of the prompt should be considered when producing suggestions.
        /// </summary>
        public bool CaseSensitive;

        /// <summary>
        /// If fuzzy searching should be used when producing suggestions.
        /// </summary>
        public bool Fuzzy;

        /// <summary>
        /// If overloads of the same suggestion should be collapsed into
        /// a single suggestion with optional elements where possible.
        /// </summary>
        public bool CollapseOverloads;
    }
}