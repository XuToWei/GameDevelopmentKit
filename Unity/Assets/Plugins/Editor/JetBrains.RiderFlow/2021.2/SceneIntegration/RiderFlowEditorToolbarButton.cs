using System;
using JetBrains.RiderFlow.Core.UI.SceneIntegration.Tools;
using JetBrains.RiderFlow.Core.UI.SceneIntegration.Tools.AltActions;
using UnityEditor.Toolbars;
using UnityEngine;

namespace JetBrains.RiderFlow.Since2021_2.SceneIntegration
{
    public class RiderFlowEditorToolbarButton : EditorToolbarButton, IPartialAltActionProvider, IPositionProvider
    {
        public int NumberOfActions => 1;

        private Action myOnClickHandler;
        
        public void SetOnClick(Action action)
        {
            if (myOnClickHandler != null)
                clicked -= myOnClickHandler;
            
            myOnClickHandler = action;
            clicked += myOnClickHandler;
        }

        public void AltAction(int i) => myOnClickHandler();

        public bool IsAltActionValid(int i)
        {
            return i < NumberOfActions;
        }

        public Vector2 Position => this.CalculateDropdownPosition();
    }
}