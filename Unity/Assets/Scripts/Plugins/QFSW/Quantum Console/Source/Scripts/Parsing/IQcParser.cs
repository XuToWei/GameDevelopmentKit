using System;

namespace QFSW.QC
{
    /// <summary>
    /// Creates a Parser that is loaded and used by the QuantumParser.
    /// </summary>
    public interface IQcParser
    {
        /// <summary>
        /// The priority of this parser to resolve multiple parsers covering the same type.
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// If this parser can parse to the incoming type.
        /// </summary>
        /// <param name="type">The type to test.</param>
        /// <returns>If it can be parsed.</returns>
        bool CanParse(Type type);

        /// <summary>
        /// Parses the incoming string to the specified type.
        /// </summary>
        /// <param name="value">The incoming string data.</param>
        /// <param name="type">The type to parse the incoming string to.</param>
        /// <param name="recursiveParser">Delegate back to the main parser to allow for recursive parsing of sub elements.</param>
        /// <returns>The parsed object.</returns>
        object Parse(string value, Type type, Func<string, Type, object> recursiveParser);
    }
}
