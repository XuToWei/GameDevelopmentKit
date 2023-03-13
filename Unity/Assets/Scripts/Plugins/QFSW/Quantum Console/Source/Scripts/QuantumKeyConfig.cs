using UnityEngine;
using UnityEngine.Serialization;

namespace QFSW.QC
{
    [CreateAssetMenu(fileName = "Untitled Key Config", menuName = "Quantum Console/Key Config")]
    public class QuantumKeyConfig : ScriptableObject
    {
        public KeyCode SubmitCommandKey = KeyCode.Return;
        public ModifierKeyCombo ShowConsoleKey = KeyCode.None;
        public ModifierKeyCombo HideConsoleKey = KeyCode.None;
        public ModifierKeyCombo ToggleConsoleVisibilityKey = KeyCode.Escape;

        public ModifierKeyCombo ZoomInKey = new ModifierKeyCombo { Key = KeyCode.Equals, Ctrl = true };
        public ModifierKeyCombo ZoomOutKey = new ModifierKeyCombo { Key = KeyCode.Minus, Ctrl = true };
        public ModifierKeyCombo DragConsoleKey = new ModifierKeyCombo { Key = KeyCode.Mouse0, Shift = true };

        [FormerlySerializedAs("SuggestNextCommandKey")]
        public ModifierKeyCombo SelectNextSuggestionKey = KeyCode.Tab;
        [FormerlySerializedAs("SuggestPreviousCommandKey")]
        public ModifierKeyCombo SelectPreviousSuggestionKey = new ModifierKeyCombo { Key = KeyCode.Tab, Shift = true };

        public KeyCode NextCommandKey = KeyCode.UpArrow;
        public KeyCode PreviousCommandKey = KeyCode.DownArrow;

        public ModifierKeyCombo CancelActionsKey = new ModifierKeyCombo { Key = KeyCode.C, Ctrl = true };
    }
}
