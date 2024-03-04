using System.Collections.Generic;
using System.Linq;
using JetBrains.Collections.Viewable;
using JetBrains.Lifetimes;
using JetBrains.RiderFlow.Core.Launchers;
using JetBrains.RiderFlow.Core.Requirements;
using JetBrains.RiderFlow.Core.UI.SceneIntegration.Tools;
using JetBrains.RiderFlow.Core.UI.SceneIntegration.Tools.AltActions;
using JetBrains.RiderFlow.Core.UI.SceneIntegration.Tools.BookmarkExplorer;
using JetBrains.RiderFlow.Core.UI.SceneIntegration.Tools.CameraExplorer;
using JetBrains.RiderFlow.Core.UI.SceneIntegration.Tools.SceneHighlighting;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace JetBrains.RiderFlow.Since2022_2.SceneIntegration
{
    public class RiderFlowSceneToolbarRoot : OverlayToolbar
    {
        private bool myIsHorizontal;
        private bool myShowingFindUsages;
        private LifetimeDefinition myDefinition = new LifetimeDefinition();
        private List<VisualElement> myRiderFlowToolbarElements;

        private SceneOccurrenceExplorerTool mySceneOccurrenceExplorerTool;
        private IList<VisualElement> myFindUsagesControls; 

        public RiderFlowSceneToolbarRoot()
        {
            SetRootStyleProperties();
            Add(new Label("Initializing..."));
            ContainerReadyRequirement.Instance.IsReady.WhenTrue(myDefinition.Lifetime, (_) =>
            { 
                this.Clear();
                myRiderFlowToolbarElements = new List<VisualElement>()
                {
                    ComponentContainer.GetComponent<SceneSearchPopupTool>().MakeVisualElement(myDefinition.Lifetime),
                    ComponentContainer.GetComponent<CameraPresetCreationTool>().MakeVisualElement(),
                    ComponentContainer.GetComponent<CameraPresetExplorerTool>().MakeVisualElement(),
                    ComponentContainer.GetComponent<BookmarkExplorerTool>().MakeVisualElement(),
                };
                mySceneOccurrenceExplorerTool = ComponentContainer.GetComponent<SceneOccurrenceExplorerTool>();
                mySceneOccurrenceExplorerTool?.Session.View(myDefinition.Lifetime, (lf, session) =>
                {
                    if (session == null)
                        return;
                    
                    if (!myShowingFindUsages)
                    {
                        lf.Bracket(() => AddFindUsages(session), RemoveFindUsages);
                    }
                });
                
                foreach (var control in myRiderFlowToolbarElements)
                {
                    Add(control);
                }
                
                var altActionProvider =
                    new NewToolbarCompositeAltActionProvider(myRiderFlowToolbarElements.OfType<IPartialAltActionProvider>());
                AltActions.SetDefaultActionProvider(altActionProvider);
                SetUpChildElements();
            });
        }
        
        public OverlayToolbar MakeHorizontalToolbar()
        {
            myIsHorizontal = true;
            SetUpChildElements();
            return this;
        }

        public OverlayToolbar MakeVerticalToolbar()
        {
            myIsHorizontal = false;
            SetUpChildElements();
            return this;
        }

        private void OnEscape(SceneView _)
        {
            if (Event.current.keyCode == KeyCode.Escape)
            {
                mySceneOccurrenceExplorerTool.CloseSession();
                SceneView.duringSceneGui -= OnEscape;
            }
        }

        public void Destroy()
        {
            myDefinition.Terminate();
            SceneView.duringSceneGui -= OnEscape;
        }

        private void SetUpChildElements()
        {
            int marginVertical;
            int marginHorizontal;
            if (myIsHorizontal)
            {
                style.flexDirection = FlexDirection.Row;
                marginVertical = 0;
                marginHorizontal = 1; 
            }
            else
            { 
                style.flexDirection = FlexDirection.Column;
                marginVertical = 1;
                marginHorizontal = 0;
            }
            
            SetHorizontalMargin(0, marginHorizontal);
            SetVerticalMargin(marginVertical, marginVertical);
            foreach (var child in Children().OfType<IPresentationRefreshable>())
            {
                child.RefreshPresentation();
            }
        }

        private void SetVerticalMargin(int top, int bottom)
        {
            foreach (var child in Children())
            {
                child.style.marginTop = top;
                child.style.marginBottom = bottom;
            }
        }

        private void SetHorizontalMargin(int left, int right)
        {
            foreach (var child in Children())
            {
                child.style.marginLeft = left;
                child.style.marginRight = right;
            }
        }

        private void SetRootStyleProperties()
        {
            style.flexDirection = FlexDirection.Column;
            style.marginTop = 0;
            style.marginBottom = 0;
            style.marginLeft = 0;
            style.marginRight = 0;
        }

        private void AddFindUsages(SceneOccurrenceExplorerTool.SceneOccurrenceExplorerSession session)
        {
            if (session.Lifetime.IsNotAlive)
                return;
            
            myFindUsagesControls = mySceneOccurrenceExplorerTool.MakeVisualElements(myDefinition.Lifetime, myIsHorizontal).ToList();
            myRiderFlowToolbarElements.AddRange(myFindUsagesControls);
            foreach (var visualElement in myFindUsagesControls)
            {
                Add(visualElement);
            }
            SetUpChildElements();
            
            var altActionProvider =
                new NewToolbarCompositeAltActionProvider(myRiderFlowToolbarElements.OfType<IPartialAltActionProvider>());
            AltActions.SetDefaultActionProvider(altActionProvider);
            
            SceneView.duringSceneGui += OnEscape;
            myShowingFindUsages = true;
        }
        
        private void RemoveFindUsages()
        {
            foreach (var visualElement in myFindUsagesControls)
            {
                Remove(visualElement);
                myRiderFlowToolbarElements.Remove(visualElement);
            }
            myFindUsagesControls.Clear();
            myShowingFindUsages = false;
        }
    }
}