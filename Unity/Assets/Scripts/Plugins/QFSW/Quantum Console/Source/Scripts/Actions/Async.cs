using System;
using System.Threading.Tasks;

namespace QFSW.QC.Actions
{
    /// <summary>
    /// Converts an async Task into an action.
    /// </summary>
    public class Async : ICommandAction
    {
        private readonly Task _task;

        public bool IsFinished => _task.IsCompleted ||
                                  _task.IsCanceled ||
                                  _task.IsFaulted;
        public bool StartsIdle => false;

        /// <param name="task">The async Task to convert.</param>
        public Async(Task task)
        {
            _task = task;
        }

        public void Start(ActionContext context) { }

        public void Finalize(ActionContext context)
        {
            if (_task.IsFaulted)
            {
                throw _task.Exception.InnerException;
            }
            if (_task.IsCanceled)
            {
                throw new TaskCanceledException();
            }
        }

    }

    /// <summary>
    /// Converts an async Task into an action.
    /// </summary>
    /// <typeparam name="T">The return type of the Task to convert.</typeparam>
    public class Async<T> : ICommandAction
    {
        private readonly Task<T> _task;
        private readonly Action<T> _onResult;

        public bool IsFinished => _task.IsCompleted ||
                                  _task.IsCanceled ||
                                  _task.IsFaulted;
        public bool StartsIdle => false;

        /// <param name="task">The async Task to convert.</param>
        /// <param name="onResult">The action to invoke when the Task completes.</param>
        public Async(Task<T> task, Action<T> onResult)
        {
            _task = task;
            _onResult = onResult;
        }

        public void Start(ActionContext context) { }

        public void Finalize(ActionContext context)
        {
            if (_task.IsFaulted)
            {
                throw _task.Exception.InnerException;
            }
            if (_task.IsCanceled)
            {
                throw new TaskCanceledException();
            }

            _onResult(_task.Result);
        }
    }
}