using System;

namespace QFSW.QC
{
    /// <summary>
    /// Creates a Parser for a custom grammar construct that is loaded and used by the QuantumParser.
    /// Grammar constructs are tested and used before resorting to IQcParsers for object value parsing.
    /// </summary>
    public interface IQcGrammarConstruct
    {
        /// <summary>
        /// The precedence of this grammar construct.
        /// </summary>
        int Precedence { get; }

        /// <summary>
        /// If the incoming data matches this grammar construct.
        /// </summary>
        /// <param name="value">The incoming string data.</param>
        /// <param name="type">The type to test.</param>
        /// <returns>If it matches the grammar defined by this construct.</returns>
        bool Match(string value, Type type);

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
