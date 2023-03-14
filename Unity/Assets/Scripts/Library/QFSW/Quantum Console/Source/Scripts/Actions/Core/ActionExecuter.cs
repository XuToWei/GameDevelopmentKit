using System.Collections.Generic;

namespace QFSW.QC
{
    public static class ActionExecuter
    {
        /// <summary>
        /// Executes an action command until it becomes idle.
        /// </summary>
        /// <param name="action">The action command to execute.</param>
        /// <param name="context">The context that the command is being executed on.</param>
        /// <returns>The current state of the action command.</returns>
        public static ActionState Execute(this IEnumerator<ICommandAction> action, ActionContext context)
        {
            ActionState state = ActionState.Running;
            bool idle = false;

            void MoveNext()
            {
                if (action.MoveNext())
                {
                    action.Current?.Start(context);
                    idle = action.Current?.StartsIdle ?? false;
                }
                else
                {
                    idle = true;
                    state = ActionState.Complete;
                    action.Dispose();
                }
            }

            while (!idle)
            {
                if (action.Current == null)
                {
                    MoveNext();
                }
                else if (action.Current.IsFinished)
                {
                    action.Current.Finalize(context);
                    MoveNext();
                }
                else
                {
                    idle = true;
                }
            }

            return state;
        }
    }
}