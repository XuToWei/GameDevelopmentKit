using UnityEngine;

namespace QFSW.QC.Actions
{
    /// <summary>
    /// Waits until the given key is pressed.
    /// </summary>
    public class WaitKey : WaitUntil
    {
        /// <param name="key">The key to wait for.</param>
        public WaitKey(KeyCode key) : base(() => InputHelper.GetKeyDown(key))
        {

        }
    }
}