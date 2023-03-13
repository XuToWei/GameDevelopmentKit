using System.Collections.Generic;

namespace QFSW.QC.Actions
{
    /// <summary>
    /// Combines a sequence of actions into a single action.
    /// </summary>
    public class Composite : ICommandAction
    {
        private ActionContext _context;
        private readonly IEnumerator<ICommandAction> _actions;

        public bool IsFinished => _actions.Execute(_context) == ActionState.Complete;
        public bool StartsIdle => false;

        /// <param name="actions">The sequence of actions to create the composite from.</param>
        public Composite(IEnumerator<ICommandAction> actions)
        {
            _actions = actions;
        }

        /// <param name="actions">The sequence of actions to create the composite from.</param>
        public Composite(IEnumerable<ICommandAction> actions) : this(actions.GetEnumerator())
        {

        }

        public void Start(ActionContext context)
        {
            _context = context;
        }

        public void Finalize(ActionContext context) { }
    }
}