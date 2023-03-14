namespace QFSW.QC.Actions
{
    /// <summary>
    /// Removes the most recent log from the console.
    /// </summary>
    public class RemoveLog : ICommandAction
    {
        public bool IsFinished => true;
        public bool StartsIdle => false;

        public void Start(ActionContext context) { }

        public void Finalize(ActionContext context)
        {
            context.Console.RemoveLogTrace();
        }
    }
}