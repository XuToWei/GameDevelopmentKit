using System.Collections.Generic;
using System.Linq;
using JetBrains.Collections.Viewable;
using JetBrains.Core;
using JetBrains.Lifetimes;
using JetBrains.RiderFlow.Core.Launchers;
using JetBrains.RiderFlow.Core.Requirements;
using JetBrains.RiderFlow.Core.UI.SceneIntegration.Tools;
using JetBrains.RiderFlow.Core.UI.SceneIntegration.Tools.AltActions;
using JetBrains.RiderFlow.Core.UI.SceneIntegration.Tools.BookmarkExplorer;
using JetBrains.RiderFlow.Core.UI.SceneIntegration.Tools.CameraExplorer;
using JetBrains.RiderFlow.Core.UI.SceneIntegration.Tools.SceneHighlighting;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

namespace JetBrains.RiderFlow.Since2021_2.SceneIntegration
{
    [EditorToolbarElement("RiderFlow.ToolbarRoot", typeof(SceneView))]
    public class RiderFlowSceneToolbarRoot : VisualElement
    {
        private bool myShowingFindUsages = false;
        private LifetimeDefinition myDefinition = new LifetimeDefinition();
        private List<VisualElement> myRiderFlowToolbarElements;

        private Signal<Unit> myParentSet = new Signal<Unit>();
        private SceneOccurrenceExplorerTool mySceneOccurrenceExplorerTool;
        private IList<VisualElement> myControls;

        public RiderFlowSceneToolbarRoot()
        {
            SetRootStyleProperties();
            var startupLabel = new Label("Initializing...");
            Add(startupLabel);
            ContainerReadyRequirement.Instance.IsReady.WhenTrue(myDefinition.Lifetime, (b) =>
            {
                Remove(startupLabel);
                myRiderFlowToolbarElements = new List<VisualElement>()
                {
                    ComponentContainer.GetComponent<SceneSearchPopupTool>().MakeVisualElement(myDefinition.Lifetime),
                    ComponentContainer.GetComponent<CameraPresetCreationTool>().MakeVisualElement(),
                    ComponentContainer.GetComponent<CameraPresetExplorerTool>().MakeVisualElement(),
                    ComponentContainer.GetComponent<BookmarkExplorerTool>().MakeVisualElement(),
                };
                foreach (var toolbarElement in myRiderFlowToolbarElements)
                {
                    Add(toolbarElement);
                }
                RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
                myParentSet.Advise(myDefinition.Lifetime, SetUpChildElements);
                mySceneOccurrenceExplorerTool = ComponentContainer.GetComponent<SceneOccurrenceExplorerTool>();
                mySceneOccurrenceExplorerTool.Session.View(myDefinition.Lifetime, (lf, session) =>
                {
                    if (session == null)
                        return;
                    
                    if (!myShowingFindUsages)
                    {
                        lf.Bracket(() => AddFindUsages(session), RemoveFindUsages);
                    }
                });
                
                RegisterCallback<DetachFromPanelEvent>(OnDestroy);
            });
        }

        private void OnEscape(SceneView _)
        {
            if (Event.current.keyCode == KeyCode.Escape)
            {
                mySceneOccurrenceExplorerTool.CloseSession();
                SceneView.duringSceneGui -= OnEscape;
            }
        }

        private void OnDestroy(DetachFromPanelEvent evt)
        {
            myDefinition.Terminate();
            SceneView.duringSceneGui -= OnEscape;
            UnregisterCallback<DetachFromPanelEvent>(OnDestroy);
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            if (parent != null)
            {
                UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
                myParentSet.Fire();
            }
        }

        private void SetUpChildElements()
        {
            
            int marginVertical = 0;
            int marginHorizontal = 0;
            if (HasHorizontalAlignment)
            {
                style.flexDirection = FlexDirection.Column;
                marginVertical = 1;
                marginHorizontal = 0;
            }
            else
            {
                style.flexDirection = FlexDirection.Row;
                marginVertical = 0;
                marginHorizontal = 1;
            }
            
            var altActionProvider =
                new NewToolbarCompositeAltActionProvider(myRiderFlowToolbarElements.OfType<IPartialAltActionProvider>());
            AltActions.SetDefaultActionProvider(altActionProvider);
            
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
            style.flexWrap = Wrap.Wrap;
            style.alignItems = Align.Stretch;
            style.alignContent = Align.Stretch;
            style.justifyContent = Justify.SpaceAround;
            style.marginTop = 0;
            style.marginBottom = 0;
            style.marginLeft = 0;
            style.marginRight = 0;
        }

        private void AddFindUsages(SceneOccurrenceExplorerTool.SceneOccurrenceExplorerSession session)
        {
            if (session.Lifetime.IsNotAlive)
                return;
            
            myControls = mySceneOccurrenceExplorerTool.MakeVisualElements(myDefinition.Lifetime, HasHorizontalAlignment).ToList();
            myRiderFlowToolbarElements.AddRange(myControls);
            foreach (var visualElement in myControls)
            {
                Add(visualElement);
            }
            SetUpChildElements();
            SceneView.duringSceneGui += OnEscape;
            myShowingFindUsages = true;
        }

        private void RemoveFindUsages()
        {
            foreach (var visualElement in myControls)
            {
                Remove(visualElement);
            }
            SetUpChildElements();
            myShowingFindUsages = false;
        }

        public bool HasHorizontalAlignment {
            get
            {
                return parent != null && parent.contentContainer.layout.width <
                    parent.contentContainer.layout.height;
            }
        }
    }

    public interface IRiderFlowToolbarElement
    {
        IEnumerable<VisualElement> GetActiveElements();
        void SetVerticalMargin(int top, int bottom);
        void SetHorizontalMargin(int left, int right);
    }
}