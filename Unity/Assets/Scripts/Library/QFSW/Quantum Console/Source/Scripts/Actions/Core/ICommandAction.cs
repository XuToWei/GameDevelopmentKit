namespace QFSW.QC
{
    /// <summary>
    /// Creates an action that can be yielded in commands.
    /// </summary>
    public interface ICommandAction
    {
        /// <summary>
        /// Starts the action.
        /// </summary>
        /// <param name="context">The context that the action is being executed on.</param>
        void Start(ActionContext context);

        /// <summary>
        /// Finalizes the action. Should not be called unless <c>IsFinished</c> is true.
        /// </summary>
        /// <param name="context">The context that the action is being executed on.</param>
        void Finalize(ActionContext context);

        /// <summary>
        /// If the action has finished. Should not be called before <c>Start</c>.
        /// </summary>
        bool IsFinished { get; }

        /// <summary>
        /// If the action should start off idle, causing the execution to suspend until executed again.
        /// It is recommended to make this <c>false</c> if the action should be instant.
        /// </summary>
        bool StartsIdle { get; }
    }
}