using System;

namespace QFSW.QC.Actions
{
    /// <summary>
    /// Waits until the given condition is met.
    /// </summary>
    public class WaitUntil : WaitWhile
    {
        /// <param name="condition">The condition to wait on.</param>
        public WaitUntil(Func<bool> condition) : base(() => !condition())
        {

        }
    }
}