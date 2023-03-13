using System;

namespace QFSW.QC
{
    /// <summary>
    /// Exception to be thrown by an IQcParser to indicate the input was invalid.
    /// </summary>
    public class ParserInputException : ParserException
    {
        public ParserInputException(string message) : base(message) { }
        public ParserInputException(string message, Exception innerException) : base(message, innerException) { }
    }
}
