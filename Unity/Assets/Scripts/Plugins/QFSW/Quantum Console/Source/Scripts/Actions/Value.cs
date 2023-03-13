namespace QFSW.QC.Actions
{
    /// <summary>
    /// Serializes and logs a value to the console.
    /// </summary>
    public class Value : ICommandAction
    {
        private readonly object _value;
        private readonly bool _newline;

        public bool IsFinished => true;
        public bool StartsIdle => false;

        /// <param name="value">The value to log to the console.</param>
        /// <param name="newline">If the value should be logged on a new line.</param>
        public Value(object value, bool newline = true)
        {
            _value = value;
            _newline = newline;
        }

        public void Start(ActionContext context) { }

        public void Finalize(ActionContext context)
        {
            QuantumConsole console = context.Console;
            string serialized = _value as string ?? console.Serialize(_value);
            console.LogToConsole(serialized, _newline);
        }
    }
}