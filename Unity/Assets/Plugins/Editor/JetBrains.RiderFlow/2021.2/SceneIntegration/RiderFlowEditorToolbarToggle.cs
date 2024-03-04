using JetBrains.RiderFlow.Core.UI.SceneIntegration.Tools;
using JetBrains.RiderFlow.Core.UI.SceneIntegration.Tools.AltActions;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

namespace JetBrains.RiderFlow.Since2021_2.SceneIntegration
{
    public abstract class RiderFlowEditorToolbarToggle : EditorToolbarToggle, IPositionProvider, IPartialAltActionProvider
    {
        protected RiderFlowEditorToolbarToggle()
        {
            this.RegisterValueChangedCallback(ValueChanged);
        }

        protected abstract void ValueChanged(ChangeEvent<bool> evt);

        public Vector2 Position => this.CalculateDropdownPosition();
        public int NumberOfActions => 1;

        public void AltAction(int i)
        {
            value = true;
        }

        public bool IsAltActionValid(int i)
        {
            return i < NumberOfActions;
        }
    }
}