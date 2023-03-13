using System;
using System.Linq;
using UnityEngine;

namespace QFSW.QC.Actions
{
    /// <summary>
    /// Waits for any key to be pressed and returns the key via the given delegate.
    /// </summary>
    public class GetKey : ICommandAction
    {
        private KeyCode _key;
        private readonly Action<KeyCode> _onKey;
        private static readonly KeyCode[] KeyCodes = Enum.GetValues(typeof(KeyCode))
            .Cast<KeyCode>()
            .Where(k => (int)k < (int)KeyCode.Mouse0)
            .ToArray();

        public bool IsFinished
        {
            get
            {
                _key = GetCurrentKeyDown();
                return _key != KeyCode.None;
            }
        }

        public bool StartsIdle => true;

        /// <param name="onKey">The action to perform when a key is pressed.</param>
        public GetKey(Action<KeyCode> onKey)
        {
            _onKey = onKey;
        }

        private KeyCode GetCurrentKeyDown()
        {
            return KeyCodes.FirstOrDefault(InputHelper.GetKeyDown);
        }

        public void Start(ActionContext context) { }

        public void Finalize(ActionContext context)
        {
            _onKey(_key);
        }
    }
}