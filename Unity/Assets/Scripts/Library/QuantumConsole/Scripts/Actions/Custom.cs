using System;

namespace QFSW.QC.Actions
{
    /// <summary>
    /// Custom action implemented via delegates.
    /// For more complex actions it is usually recommended to create a new action implementing <c>ICommandAction</c>.
    /// </summary>
    public class Custom : ICommandAction
    {
        private readonly Func<bool> _isFinished;
        private readonly Func<bool> _startsIdle;
        private readonly Action<ActionContext> _start;
        private readonly Action<ActionContext> _finalize;

        public Custom(
            Func<bool> isFinished,
            Func<bool> startsIdle,
            Action<ActionContext> start,
            Action<ActionContext> finalize
        )
        {
            _isFinished = isFinished;
            _startsIdle = startsIdle;
            _start = start;
            _finalize = finalize;
        }

        public bool IsFinished => _isFinished();
        public bool StartsIdle => _startsIdle();

        public void Start(ActionContext context) { _start(context); }
        public void Finalize(ActionContext context) { _finalize(context); }
    }

}