// Copyright (C) 2018 KAMGAM e.U. - All rights reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;

namespace kamgam.editor.smartuiselection
{
    public class SelectionResult
    {
        public float        DistanceToMouse;
        public Canvas       Canvas;
        public GameObject   GameObject;
        public int          HierarchyOrder;
        public GameObject   SelectionBase;

        public SelectionResult(float distanceToMouse, int hierarchyOrder, Canvas canvas, GameObject gameObject)
        {
            this.DistanceToMouse    = distanceToMouse;
            this.HierarchyOrder     = hierarchyOrder;
            this.Canvas             = canvas;
            this.GameObject         = gameObject;
        }

        public GameObject GetGameObjectForSelection()
        {
            if (SelectionBase != null)
            {
                return SelectionBase;
            }
            else
            {
                return GameObject;
            }
        }
    }

    [InitializeOnLoad]
    [Serializable]
    public static class SmartUiSelection
    {
        static Vector2? _mouseDownPos = null;       // used to detect mouse UP events.

        static Vector2  _lastMouseDownPos;
        static Vector2  _lastknownMousePos;
        static Vector2? _autoSelectOnMousePos = null;
        static double   _lastMouseUpTime;          // used to determine if we have to cycle through results.
        static int      _resultIndex;              // used to cycle through results on double, tripple, ... clicks.
        static bool     _isEnableKeyPressed = false;   // used to check if key is pressed if "push to use" is activated.
        static bool     _isDisableExcludesKeyPressed = false;   // used to check if key is pressed if "disable exclude lists" is activated.

        static SmartUiSelection()
        {
            if (SmartUiSelection_Settings.instance == null)
            {
                Debug.LogWarning("SmartUiSelection plugin did not find any settings and will do nothing.\nPlease create them in a 'Resources' folder via Assets -> Create -> SmartUiSelection Settings.");
            }

            _lastMouseDownPos = Vector2.zero;
#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui += UpdateView;
#else
            SceneView.onSceneGUIDelegate += UpdateView;
#endif
            Selection.selectionChanged += OnSelectionChanged;

            buildReflectionCache();
        }

        private static bool isInPrefabStage()
        {
            try
            {
#if UNITY_2021_2_OR_NEWER
                var stage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
                return stage != null;
#elif UNITY_2018_3_OR_NEWER
                var stage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
                return stage != null;
#else
                return false;
#endif
            }
            catch (Exception e)
            {
                Debug.LogError("Smart Ui Selection: Prefab Stage  object not found. Maybe Unity has changed the namespace of 'PrefabStageUtility' (thrown error: " + e.Message + "). Please let us know of this error (Tools > Smart Ui Selection > Feedback and Support). Assuming we are not in PrefabStage to continue.");
                return false;
            }
        }

        private static GameObject getPrefabStageRoot()
        {
            try
            {
#if UNITY_2021_2_OR_NEWER
                var root = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot;
                return root;
#elif UNITY_2018_3_OR_NEWER
                var root = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot;
                return root;
#else
                return null;
#endif
            }
            catch (Exception e)
            {
                Debug.LogError("Smart Ui Selection: Prefab Stage root object not found. Maybe Unity has changed the namespace of 'PrefabStageUtility' (thrown error: " + e.Message + "). Please let us know of this error (Tools > Smart Ui Selection > Feedback and Support).");
                return null;
            }
        }

        private static bool isMouseInSceneView()
        {
            return EditorWindow.mouseOverWindow != null && SceneView.sceneViews.Contains(EditorWindow.mouseOverWindow);
        }

        private static void OnSelectionChanged()
        {
            // single selections which have not originated from our checks in OnMouseUpInSceneView
            if ( SmartUiSelection_Settings.enablePlugin && Selection.gameObjects.Length == 1 && _mouseDownPos != null )
            {
                // remember selection to reset on error
                var selectedObjects = Selection.gameObjects;

                try
                {
                    // ignore selections outside of scene view
                    if ( isMouseInSceneView() )
                    {
                        // ignore selection if it's no rect transform
                        if ( Selection.gameObjects[0].GetComponent<RectTransform>() != null )
                        {
                            // ignore selections from dragging (large distances)
                            float mouseMoveDistanceInInch = Vector2.Distance(_mouseDownPos.Value, _lastknownMousePos) / Screen.dpi;
                            if (mouseMoveDistanceInInch <= 0.1f) // 0.1inch / about 2.5mm
                            {
                                // User clicked and moved the mouse a little. Not enought to make a selection rect but
                                // enough to avoid the custom MouseUp detection in UpdateView(). In that case the unity native
                                // selection would kick in. To avoid this we are selecting again by hand.
                                // We can not use OnMouseUpInSceneView() directly because if we do HandleUtility.GUIPointToWorldRay() 
                                // will give weird results. Thus we schedule UpdateView() to do it for us.
                                _autoSelectOnMousePos = _mouseDownPos.Value;
                                _mouseDownPos = null;
                                _lastknownMousePos = Vector2.zero;
                                // Debug.Log("where are you coming from?");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    // reset selection in case of an error.
                    Selection.objects = selectedObjects;
                    clearSelectionMemory();
                    Debug.LogWarning("SmartUiSelection caused an unexpected Error:\n" + e.Message + "\nStack:\n" + e.StackTrace + "\n\n Please let us know of this error (Tools > Smart Ui Selection > Feedback and Support).");
                }
            }
        }

        private static void UpdateView(SceneView sceneView)
        {
            if ( SmartUiSelection_Settings.enablePlugin )
            {
                // remember selection to reset on error
                var selectedObjects = Selection.gameObjects;

                try
                {
                    // find pressed keys
                    Event e = Event.current;
                    if (e != null)
                    {
                        if (e.type == EventType.KeyDown)
                        {
                            if (Event.current.keyCode == SmartUiSelection_Settings.EnableSmartUiKeyCode)
                            {
                                _isEnableKeyPressed = true;
                            }
                            if (Event.current.keyCode == SmartUiSelection_Settings.DisableExcludeListsKeyCode)
                            {
                                _isDisableExcludesKeyPressed = true;
                            }
                        }
                        else if (e.type == EventType.KeyUp)
                        {
                            if (Event.current.keyCode == SmartUiSelection_Settings.EnableSmartUiKeyCode)
                            {
                                _isEnableKeyPressed = false;
                            }
                            if (Event.current.keyCode == SmartUiSelection_Settings.DisableExcludeListsKeyCode)
                            {
                                _isDisableExcludesKeyPressed = false;
                            }
                        }
                    }

                    // trigger by schedule due to selection change
                    if (_autoSelectOnMousePos.HasValue)
                    {
                        OnMouseUpInSceneView(_autoSelectOnMousePos.Value);
                        _autoSelectOnMousePos = null;
                    }
                    // trigger by click
                    else
                    {
                        Event evt = Event.current;
                        if (evt != null)
                        {
                            _lastknownMousePos = evt.mousePosition;
                            if (evt.type == EventType.MouseDown)
                            {
                                _mouseDownPos = evt.mousePosition;
                            }
                            if (_mouseDownPos.HasValue)
                            {
                                if (evt.type == EventType.Used || evt.type == EventType.MouseUp)
                                {
                                    if (evt.button == 0) // react only to left mouse button clicks
                                    {
                                        if (Vector2.Distance(evt.mousePosition, _mouseDownPos.Value) == 0)
                                        {
                                            _mouseDownPos = null;
                                            OnMouseUpInSceneView(evt.mousePosition, evt);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    // reset selection in case of an error.
                    Selection.objects = selectedObjects;
                    clearSelectionMemory();
                    Debug.LogWarning("SmartUiSelection caused an unexpected Error:\n" + e.Message + "\nStack:\n" + e.StackTrace + "\n\n Please let us know of this error (Tools > Smart Ui Selection > Feedback and Support).");
                }
            }
        }

        private static Nullable<bool> isVisible( GameObject obj )
        {
            // Skip if object is hidden by the SceneVis feature introduced in 2019.1
#if UNITY_2019_2_OR_NEWER
            return !SceneVisibilityManager.instance.IsHidden(obj);
#elif UNITY_2019_1
            // Sadly the SceneVisibilityManager API is not available in 2019.1 (SceneVis was introduced in 2019.1)
            // We have to use reflections to get this info in 2019.1
            var editorTypes = typeof(Editor).Assembly.GetTypes();
            if (editorTypes != null && editorTypes.Length > 0 )
            {
                var sceneVMType = editorTypes.FirstOrDefault(t => t.Name == "SceneVisibilityManager");
                if (sceneVMType != null)
                {
                    var sceneVMMethod = sceneVMType.GetMethod("IsGameObjectHidden", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                    if (sceneVMMethod != null)
                    {
                        var isHidden = sceneVMMethod.Invoke(null, new object[] { rectTransforms[i].gameObject });
                        if (isHidden != null)
                        {
                            return !(bool)isHidden;
                        }
                    }
                }
            }
#else
            return null;
#endif
        }

        private static void OnMouseUpInSceneView(Vector2 mousePosition, Event mouseEvent = null )
        {
            // ignore mouse events outside of scene view
            if ( !isMouseInSceneView() )
            {
                _resultIndex = 0;
                clearSelectionMemory();
                return;
            }

            bool selectionChangedBySmartUi = false;

            // Cycle through results just like unity does (if time since last mouse up is low and
            // position has not changed and it's not a multi-select).
            double timeSinceLastMouseUp = EditorApplication.timeSinceStartup - _lastMouseUpTime;
            float positionChangeSinceLastMouseUp = Vector2.Distance(_lastMouseDownPos, mousePosition);
            if (timeSinceLastMouseUp < SmartUiSelection_Settings.multiClickTimeThreshold
                && positionChangeSinceLastMouseUp == 0
                && (mouseEvent == null || (mouseEvent != null && (!mouseEvent.shift && !mouseEvent.control))) )
            {
                // increase cycle index
                _resultIndex++;
            }
            else
            {
                // reset cycle (too much time passed or click position changed)
                _resultIndex = 0;
            }
            _lastMouseDownPos = mousePosition;
            _lastMouseUpTime = EditorApplication.timeSinceStartup;

            // clear selection memory if shift is not pressed
            if( mouseEvent != null && (!mouseEvent.shift && !mouseEvent.control) )
            {
                clearSelectionMemory();
            }

            // ignore events if activation key is not pressed (only if "push to use" is activated).
            if (SmartUiSelection_Settings.pushKeyToUseUiSelection == true && _isEnableKeyPressed == false)
            {
                return;
            }

            //Ray ray = Camera.current.ScreenPointToRay(GUIUtility.GUIToScreenPoint(mousePosition)); // does not work properly
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
            var results = new List<SelectionResult>();
            int hierarchyOrder = 0;
            List<Canvas> canvases;
            if (isInPrefabStage())
            {
                var root = getPrefabStageRoot();
                // Find the real root of the prefab stage.
                var rootParent = root.transform.parent; // A root can have a parent? Apparently yes!
                if( rootParent != null )
                {
                    root = rootParent.gameObject;
                }
                canvases = root.GetComponentsInChildren<Canvas>().ToList();
            }
            else
            {
                canvases = GameObject.FindObjectsOfType<Canvas>().ToList();
            }

            // Sort canvases by their GetSiblingIndex() (may be slow for many canvases)
            canvases.Sort(CompareHierarchyDepths);
            // Go through all transforms in canvas. Currently this relies on the depth-first behaviour
            // of Transform.GetComponentsInChildren() to get the right order (hierarchyOrder).
            for (int c = canvases.Count - 1; c >= 0; c--)
            {
                // ignore disabled canvases and canvases on the ignore lists (by name or tag)
                if ( canvases[c].enabled
                     && !IsExcludedByName(canvases[c].name)
                     && !IsExcludedByTag(canvases[c].gameObject.tag)
                     && !IsExcludedByType(canvases[c].transform) )
                {
                    // get all rect transforms
                    var rectTransforms = canvases[c].transform.GetComponentsInChildren<RectTransform>(false);
                    for (int i = rectTransforms.Length - 1; i >= 0; i--)
                    {
                        // Skip temporarily if the transform is in a nested canvas.
                        // It will be checked once we get to the nested canvas in the canvases list.
                        if (rectTransforms[i].GetComponentInParent<Canvas>() != canvases[c])
                        {
                            continue;
                        }

                        // Skip if it does not have a graphic
                        if (SmartUiSelection_Settings.limitSelectionToGraphics)
                        {
                            var graphic = rectTransforms[i].GetComponent<Graphic>();
                            if (graphic == null || !graphic.enabled)
                            {
                                continue;
                            }

                            // Skip if the graphic has alpha <= threshold (usually 0)
                            if (graphic.color.a <= SmartUiSelection_Settings.alphaMinThreshold)
                            {
                                continue;
                            }

                            // Skip if the renderer has alpha <= threshold (usually 0)
                            var renderer = rectTransforms[i].GetComponent<CanvasRenderer>();
                            if (renderer == null || renderer.GetAlpha() <= SmartUiSelection_Settings.alphaMinThreshold)
                            {
                                continue;
                            }

                            // skip if it's part of an invisible (alpha <= threshold) canvas group
                            if (rectTransforms[i].GetComponentsInParent<CanvasGroup>().Any(cg => cg.alpha <= SmartUiSelection_Settings.alphaMinThreshold))
                            {
                                continue;
                            }
                        }

                        // skip if it machtes exclusion names
                        if (IsExcludedByName(rectTransforms[i].name))
                        {
                            continue;
                        }

                        // skip if it matches exclusion tags
                        if (IsExcludedByTag(rectTransforms[i].gameObject.tag))
                        {
                            continue;
                        }

                        // skip if it matches exclusion types
                        if (IsExcludedByType(rectTransforms[i]))
                        {
                            continue;
                        }

                        // Ignore scroll rects
                        if (IsExcludedBecauseOfScrollRect(rectTransforms[i]))
                            continue;

                        // Ignore hidden masked images
                        if (IsExcludedBecauseOfMask(rectTransforms[i]))
                            continue;

                        // Skip if it is not hit by the raycast
                        var transformHitWorldPos = WorldPointOnRectTransform(ray, rectTransforms[i]);
                        //Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red);
                        if (transformHitWorldPos == null)
                        {
                            continue;
                        }

                        // Skip if object is hidden by the SceneVis feature introduced in 2019.1
#if UNITY_2019_1_OR_NEWER
                        var isObjVisible = isVisible(rectTransforms[i].gameObject);
                        if (isObjVisible.HasValue && isObjVisible == false)
                        {
                            continue;
                        }
#endif

                        // skip if it's on a locked layer
                        // Notice that the "visibility" of layers is ignored on UI elements since Unity does not hide ui
                        // elements on invisible layers, only 3d objects are affected.
                        int layer = rectTransforms[i].gameObject.layer;
                        if ( isLayerInLayers(layer, Tools.lockedLayers) ) // || !isLayerInLayers(layer, Tools.visibleLayers) )
                        {
                            continue;
                        }

                        // skip objects which are no editable
                        if (SmartUiSelection_Settings.selectOnlyEditableObjects == true)
                        {
                            if ((rectTransforms[i].hideFlags & HideFlags.NotEditable) != 0)
                            {
                                continue;
                            }
                        }

                        // skip if masked by parent (RectMask2D)
                        var rectMask = rectTransforms[i].GetComponentInParent<RectMask2D>();
                        if (rectMask != null && rectMask.enabled)
                        {
                            // Nice 2 Have: add detection of padding (is ignore at the moment).
                            var rectMaskHitWorldPos = WorldPointOnRectTransform(ray, rectMask.rectTransform);
                            //Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red);
                            if (rectMaskHitWorldPos == null)
                            {
                                continue;
                            }
                        }

                        // skip if masked by parent (Mask)
                        var uiMask = rectTransforms[i].GetComponentInParent<Mask>();
                        if (uiMask != null && uiMask.enabled)
                        {
                            // Nice 2 Have: add detection of padding (is ignore at the moment).
                            var uiMaskHitWorldPos = WorldPointOnRectTransform(ray, uiMask.rectTransform);
                            //Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red);
                            if (uiMaskHitWorldPos == null)
                            {
                                continue;
                            }
                        }

                        // If it's a normal textfield then get the text bounds and check for intersection
                        var textComp = rectTransforms[i].gameObject.GetComponent<Text>();
                        if (textComp != null)
                        {
                            // get the 4 corners of actual text characters
                            var positions = textComp.cachedTextGenerator.verts.Select(v => v.position);
                            Vector3 minTextPos = new Vector3(999000, 999000, 999000);
                            Vector3 maxTextPos = new Vector3(-999000, -999000, -999000);
                            foreach (var pos in positions)
                            {
                                minTextPos = Vector3.Min(minTextPos, pos);
                                maxTextPos = Vector3.Max(maxTextPos, pos);
                            }
                            
                            // check if there is a canvas scaler and correct for dynamicPixelsPerUnit or scaleFactor
                            var canvasScaler = rectTransforms[i].GetComponentInParent<CanvasScaler>();
                            if (canvasScaler != null)
                            {
                                var canvasScalerCanvas = canvasScaler.gameObject.GetComponent<Canvas>();
                                if (canvasScaler != null && canvasScalerCanvas != null)
                                {
                                    if (canvasScalerCanvas.renderMode == RenderMode.WorldSpace && canvasScaler.dynamicPixelsPerUnit != 1.0f)
                                    {
                                        minTextPos /= canvasScaler.dynamicPixelsPerUnit;
                                        maxTextPos /= canvasScaler.dynamicPixelsPerUnit;
                                    }
                                    else if (canvasScalerCanvas.renderMode != RenderMode.WorldSpace )
                                    {
                                        if ( canvasScaler.uiScaleMode == CanvasScaler.ScaleMode.ConstantPixelSize && canvasScaler.scaleFactor != 1.0f)
                                        {
                                            minTextPos /= canvasScaler.scaleFactor;
                                            maxTextPos /= canvasScaler.scaleFactor;
                                        }
                                        else if(canvasScaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize && canvasScalerCanvas.scaleFactor != 1.0f)
                                        {
                                            minTextPos /= canvasScalerCanvas.scaleFactor;
                                            maxTextPos /= canvasScalerCanvas.scaleFactor;
                                        }
                                        else if (canvasScaler.uiScaleMode == CanvasScaler.ScaleMode.ConstantPhysicalSize && canvasScalerCanvas.scaleFactor != 1.0f)
                                        {
                                            minTextPos /= canvasScalerCanvas.scaleFactor;
                                            maxTextPos /= canvasScalerCanvas.scaleFactor;
                                        }
                                    }
                                }
                            }

                            // convert corners to world space
                            Vector3[] worldCorners = getWorldCornersFromGuiTextMinMaxPositions(rectTransforms[i], minTextPos, maxTextPos);
                            /*
                            Debug.DrawLine(Vector3.zero, worldCorners[0], Color.green,   1    );
                            Debug.DrawLine(Vector3.zero, worldCorners[1], Color.red,     1.5f );
                            Debug.DrawLine(Vector3.zero, worldCorners[2], Color.cyan,    2    );
                            Debug.DrawLine(Vector3.zero, worldCorners[3], Color.magenta, 2.5f );
                            //*/

                            // Skip if it is not hit by the raycast
                            var textRectHitWorldPos = WorldPointOnRectTransform(ray, worldCorners);
                            if (textRectHitWorldPos == null)
                            {
                                continue;
                            }
                        }

                        // "TextMesh Pro SubMesh" special handling (ignore "TMP SubMeshUI..." objects)
                        if (rectTransforms[i].name.StartsWith("TMP SubMesh") && rectTransforms[i].parent != null)
                        {
                            continue;
                        }

                        // If it's a TMPro textfield then get the text bounds and check for intersection.
                        var components = rectTransforms[i].gameObject.GetComponents<Component>();
                        bool skipDueToTextMeshPro = false;
                        foreach( var comp in components )
                        {
                            if(comp.GetType().ToString().EndsWith("TextMeshProUGUI"))
                            {
                                Bounds tmpTextBounds;
                                if( GetPropertyValue<Bounds>(comp, "textBounds", out tmpTextBounds) )
                                {
                                    // convert corners to world space
                                    Vector3[] worldCorners = getWorldCornersFromGuiTextMinMaxPositions(rectTransforms[i], tmpTextBounds.min, tmpTextBounds.max);
                                    
                                    // Skip if it is not hit by the raycast
                                    var textRectHitWorldPos = WorldPointOnRectTransform(ray, worldCorners);
                                    skipDueToTextMeshPro = (textRectHitWorldPos == null);
                                }
                            }
                        }
                        if (skipDueToTextMeshPro)
                        {
                            continue;
                        }

                        if (results.FirstOrDefault((r) => r.GameObject == rectTransforms[i].gameObject) == null)
                        {
                            GameObject objectToAdd = rectTransforms[i].gameObject;
                            results.Add(new SelectionResult(Vector3.Distance(ray.origin, transformHitWorldPos.Value), hierarchyOrder++, rectTransforms[i].GetComponentInParent<Canvas>(), objectToAdd));
                        }

                        //Debug.Log(rectTransforms[i].name);
                    }
                }
            }

            /*
            Debug.Log("\nUn-sorted:");
            foreach (var re in results)
            {
                Debug.Log(re.GameObject.name + ", distance: " + re.DistanceToMouse + ", sortingOrder: " + re.Canvas.sortingOrder + ", canvas: " + re.Canvas.name + ", hierarchyOrder: " + re.HierarchyOrder); //+ ", gDepth: " + re.GameObject.GetComponent<Graphic>().depth);
            }
            //*/

            results.Sort(CompareSelectionResults);

            if ( !SmartUiSelection_Settings.ignoreSelectionBaseAttributes )
            {
                // mark objects which have a selection base parent
                bool foundSelectionBaseAttributes = false;
                foreach (var re in results)
                {
                    var selectionBase = re.GameObject.transform.GetComponentsInParent<MonoBehaviour>()
                        .FirstOrDefault(m => m.GetType().GetCustomAttributes(typeof(SelectionBaseAttribute), true).Length > 0);
                    if (selectionBase != null)
                    {
                        re.SelectionBase = selectionBase.gameObject;
                        foundSelectionBaseAttributes = true;
                    }
                }

                if (foundSelectionBaseAttributes)
                {
                    results.Sort(CompareSelectionResultBySelectionBase);
                }
            }

            /*
            Debug.Log("\nSorted:");
            foreach (var re in results)
            {
                Debug.Log(re.GameObject.name + ", distance: " + re.DistanceToMouse + ", sortingOrder: " + re.Canvas.sortingOrder + ", hierarchyOrder: " + re.HierarchyOrder + ", selectionBase: " + (re.SelectionBase == null) );//+ ", gDepth: " + re.GameObject.GetComponent<Graphic>().depth);
            }
            //*/

            if (results.Count > 0 && SmartUiSelection_Settings.enableSmartUiSelection )
            {
                selectionChangedBySmartUi = true;
                setSelection( new GameObject[] { results[_resultIndex % results.Count].GetGameObjectForSelection() }, mouseEvent != null && (mouseEvent.shift || mouseEvent.control));
            }
            else
            {
                // check if unity default selectin has picked something up
                if (Selection.gameObjects != null)
                {
                    // detect if mouse click happened within any visible canvas.
                    bool mouseIsInACanvas = false;
                    foreach (var canvas in canvases)
                    {
                        if (canvas.gameObject.activeInHierarchy
                            && !isLayerInLayers(canvas.gameObject.layer, Tools.lockedLayers)
                            && isLayerInLayers(canvas.gameObject.layer, Tools.visibleLayers)
#if UNITY_2019_1_OR_NEWER
                            && isVisible(canvas.gameObject).Value == true
#endif
                            )
                        {
                            // Skip if it is not hit by the raycast
                            var canvasHitWorldPos = WorldPointOnRectTransform(ray, canvas.transform as RectTransform);
                            if (canvasHitWorldPos != null)
                            {
                                mouseIsInACanvas = true;
                                break;
                            }
                        }
                    }

                    // Check if the mouse click has happened within a canvas
                    // or if the default selection got a ui object.
                    // If yes, proceed by raycasting for 3d objects.
                    if ( mouseIsInACanvas || Selection.gameObjects.Any(s => s.GetComponent<RectTransform>() != null) )
                    {
                        // search for 3d objects behind the canvas
                        if (SmartUiSelection_Settings.select3dObjectsBehindCanvas)
                        {
                            List<SelectionResult> found3DObjects = new List<SelectionResult>();

                            // raycast colliders
                            if (SmartUiSelection_Settings.select3dColliders == true )
                            {
                                var colliderHits = Physics.RaycastAll(ray, SmartUiSelection_Settings.maxDistanceFor3DSelection);
                                if (colliderHits.Length > 0)
                                {
                                    for (int i = 0; i < colliderHits.Length; i++)
                                    {
                                        // Add only if the object has no renderer (renderer objects will be picked up by the other methods of raycasting below).
                                        if (colliderHits[i].transform.gameObject.GetComponent<Renderer>() == null)
                                        {
                                            found3DObjects.Add(new SelectionResult(colliderHits[i].distance, 0, null, colliderHits[i].transform.gameObject));
                                        }
                                    }
                                }
                            }

                            // raycast for 3D renderers without collider (mesh filter with reflections or bounding box as fallback)
                            bool useBoundingBoxFallback = true;
                            if (SmartUiSelection_Settings.select3dObjectsByMesh == true)
                            {
                                // Sadly this may cause an unrecoverable crash (observed primarily in big scenes), thus it's disabled now (v3.4.0+) by default.
                                // We now do this by hand (see "tmpFound3DObjects" and RayCastMesh() below).
                                //
                                // 0x00007FF62977CB5E (Unity) (function-name not available)
                                // 0x0000013B6340B8CB (UnityEditor) UnityEditor.HandleUtility.IntersectRayMesh_Injected()
                                // 0x0000013B6340B03B (UnityEditor) UnityEditor.HandleUtility.IntersectRayMesh()
                                // 0x0000013B6340B70D (UnityEditor) <Module>.runtime_invoke_bool_Ray_object_Matrix4x4_intptr&()
                                // 0x00007FFDE03ED6B0 (mono-2.0-bdwgc) mono_get_runtime_build_info
                                // 0x00007FFDE0372912 (mono-2.0-bdwgc) mono_perfcounters_init
                                // 0x00007FFDE037BB42 (mono-2.0-bdwgc) mono_runtime_invoke_array
                                // 0x00007FFDE037C2D9 (mono-2.0-bdwgc) mono_runtime_set_main_args
                                // 0x00007FFDE037BAD6 (mono-2.0-bdwgc) mono_runtime_invoke_array
                                // 0x00007FFDE0320384 (mono-2.0-bdwgc) mono_lookup_internal_call
                                // 0x0000013B10D28B16 (mscorlib) System.Reflection.MonoMethod.InternalInvoke()
                                // 0x0000013B10D274CB (mscorlib) System.Reflection.MonoMethod.Invoke()
                                // 0x0000013B10D2718F (mscorlib) System.Reflection.MethodBase.Invoke()
                                // 0x0000013B6340ACB3 (Assembly-CSharp-Editor-firstpass) kamgam.editor.smartuiselection.SmartUiSelection.IntersectRayMesh()
                                /*
                                useBoundingBoxFallback = false;
                                var meshFilters = Transform.FindObjectsOfType<MeshFilter>();
                                foreach (var meshFilter in meshFilters)
                                {
                                    if (found3DObjects.FirstOrDefault(r => r.GameObject == meshFilter.gameObject) == null) // avoid duplicates
                                    {
                                        // try to find mesh by direct hit (uses reflections and internal classes)
                                        RaycastHit hit;
                                        int result = IntersectRayMesh(ray, meshFilter, out hit);
                                        if (result == 1)
                                        {
                                            if (hit.distance < SmartUiSelection_Settings.maxDistanceFor3DSelection)
                                            {
                                                found3DObjects.Add(new SelectionResult(hit.distance, 0, null, meshFilter.gameObject));
                                            }
                                        }
                                        // fall back to bounding box if reflections fail
                                        else if (result == -1)
                                        {
                                            useBoundingBoxFallback = true;
                                            break;
                                        }
                                    }
                                }*/
                            }

                            // raycast for 3D MeshRenderers without collider (bounding box fallback)
                            if (useBoundingBoxFallback)
                            {
                                var meshRenderers = Transform.FindObjectsOfType<MeshRenderer>();
                                var tmpFound3DObjects = new List<SelectionResult>();
                                foreach (var renderer in meshRenderers)
                                {
                                    if (found3DObjects.FirstOrDefault(r => r.GameObject == renderer.gameObject) == null) // avoid duplicates
                                    {
                                        float distance;
                                        if (renderer.bounds.IntersectRay(ray, out distance))
                                        {
                                            if (distance < SmartUiSelection_Settings.maxDistanceFor3DSelection)
                                            {
                                                tmpFound3DObjects.Add(new SelectionResult(distance, 0, null, renderer.gameObject));
                                            }
                                        }
                                    }
                                }
                                // Improve the distance measurement by raycasting every triangle in the mesh (this might be slow but at
                                // least we only do it for objects already hit by the bounding box test).
                                if (SmartUiSelection_Settings.select3dObjectsByMesh == true)
                                {
                                    bool foundOneByTriangleTest = false;
                                    foreach (var tmpMesh in tmpFound3DObjects)
                                    {
                                        float distance = RayCastMesh(ray, tmpMesh.GameObject);
                                        if (float.IsNaN(distance) == false)
                                        {
                                            foundOneByTriangleTest = true;
                                            tmpMesh.DistanceToMouse = distance;
                                            found3DObjects.Add(tmpMesh);
                                        }
                                    }
                                    // If the triangle test failed we will add the bounding box test results to the final list.
                                    if(foundOneByTriangleTest == false)
                                    {
                                        found3DObjects.AddRange(tmpFound3DObjects);
                                    }
                                }
                                else
                                {
                                    found3DObjects.AddRange(tmpFound3DObjects);
                                }
                            }

                            // raycast for 2D SpriteRenderers in 3D space.
                            var spriteRenderers = Transform.FindObjectsOfType<SpriteRenderer>();
                            foreach (var renderer in spriteRenderers)
                            {
                                if (found3DObjects.FirstOrDefault(r => r.GameObject == renderer.gameObject) == null) // avoid duplicates
                                {
                                    // use bounding box first
                                    float distance;
                                    if (renderer.bounds.IntersectRay(ray, out distance))
                                    {
                                        if (distance < SmartUiSelection_Settings.maxDistanceFor3DSelection)
                                        {
                                            // check the actial sprite renderer mesh ?
                                            if (SmartUiSelection_Settings.highPrecisionSpriteSelection == false)
                                            {
                                                found3DObjects.Add(new SelectionResult(distance, 0, null, renderer.gameObject));
                                            }
                                            else
                                            {
                                                // check the actual sprite renderer mesh
                                                if (renderer.sprite != null)
                                                {
                                                    distance = float.NaN;
                                                    // check all triangles for hit with raycast
                                                    ushort[] triangles = renderer.sprite.triangles;
                                                    Vector2[] vertices = renderer.sprite.vertices;
                                                    int a, b, c;
                                                    for (int i = 0; i < triangles.Length; i = i + 3)
                                                    {
                                                        a = triangles[i];
                                                        b = triangles[i + 1];
                                                        c = triangles[i + 2];
                                                        /*
                                                        Debug.DrawLine(renderer.transform.TransformPoint(vertices[a]), renderer.transform.TransformPoint(vertices[b]), Color.red, 3.0f);
                                                        Debug.DrawLine(renderer.transform.TransformPoint(vertices[b]), renderer.transform.TransformPoint(vertices[c]), Color.red, 3.0f);
                                                        Debug.DrawLine(renderer.transform.TransformPoint(vertices[c]), renderer.transform.TransformPoint(vertices[a]), Color.red, 3.0f); //*/
                                                        distance = IntersectRayTriangle(
                                                            ray,
                                                            renderer.transform.TransformPoint(vertices[a]),
                                                            renderer.transform.TransformPoint(vertices[b]),
                                                            renderer.transform.TransformPoint(vertices[c]));
                                                        if (!float.IsNaN(distance))
                                                        {
                                                            break;
                                                        }
                                                    }
                                                    if (!float.IsNaN(distance)) // previously renderer.bounds.IntersectRay(ray, out distance)
                                                    {
                                                        if (distance < SmartUiSelection_Settings.maxDistanceFor3DSelection)
                                                        {
                                                            found3DObjects.Add(new SelectionResult(distance, 0, null, renderer.gameObject));
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            // remove 3D objects which are on a locked or invisible layer
                            found3DObjects = found3DObjects.Where(
                                o => (
                                          !isLayerInLayers(o.GameObject.layer, Tools.lockedLayers )
                                        && isLayerInLayers(o.GameObject.layer, Tools.visibleLayers)
                                     )
                                ).ToList();

                            // remove 3D objects which macht exclusion names
                            found3DObjects = found3DObjects.Where(
                                o => ( !IsExcludedByName(o.GameObject.name) )
                                ).ToList();

                            // remove 3D objects which match "excluded by tag" tags.
                            found3DObjects = found3DObjects.Where(
                                o => (!IsExcludedByTag(o.GameObject.tag))
                                ).ToList();

                            // remove 3D objects which match "excluded by type" tags.
                            found3DObjects = found3DObjects.Where(
                                o => (!IsExcludedByType(o.GameObject.transform))
                                ).ToList();

                            // remove 3D objects which have set the Material to Sprite/Default with Tint alpha = 0.
                            found3DObjects = found3DObjects.Where(
                                o => {
                                    var meshRenderer = o.GameObject.GetComponent<MeshRenderer>();
                                    if( meshRenderer != null )
                                    {
                                        if(    meshRenderer.sharedMaterial != null
                                            && meshRenderer.sharedMaterial.shader != null
                                            && meshRenderer.sharedMaterial.shader.name == "Sprites/Default"
                                             )
                                        {
                                            if (    meshRenderer.sharedMaterial.HasProperty("_Color")
                                                 && meshRenderer.sharedMaterial.GetColor("_Color").a <= SmartUiSelection_Settings.alphaMinThreshold )
                                            {
                                                return false;
                                            }
                                        }
                                    }
                                    return true;
                                }).ToList();

                            // skip objects which are no editable
                            if (SmartUiSelection_Settings.selectOnlyEditableObjects == true)
                            {
                                found3DObjects = found3DObjects.Where(
                                    o => (
                                           (o.GameObject.hideFlags & HideFlags.NotEditable) == 0
                                         )
                                    ).ToList();
                            }

                            // 3D selection
                            if (found3DObjects.Count > 0)
                            {
                                // deselect wrongly selected ui element (clears unitys native selection)
                                Selection.activeGameObject = null;

                                // sort by distance
                                found3DObjects.Sort((a, b) => {
                                    if (a.DistanceToMouse < b.DistanceToMouse) return -1;
                                    if (a.DistanceToMouse > b.DistanceToMouse) return 1;
                                    return 0;
                                });

                                // set as selection
                                selectionChangedBySmartUi = true;
                                setSelection( new GameObject[] { found3DObjects[_resultIndex % found3DObjects.Count].GameObject }, mouseEvent != null && (mouseEvent.shift || mouseEvent.control) );

                                /*
                                Debug.Log("\nSorted 3D:");
                                foreach (var re in found3DObjects)
                                {
                                    Debug.Log(re.GameObject.name + ", distance: " + re.DistanceToMouse );
                                }
                                //*/
                            }
                            else
                            {
                                // Filter NATIVE selection (happens if SmartUi has found nothing to select)

                                // Do some modifications on the currently selected objects.
                                bool selectionChanged = false;

                                List<GameObject> nativeSelection = new List<GameObject>(Selection.gameObjects);

                                // Replace
                                for (int i=0; i<nativeSelection.Count; ++i)
                                {
                                    // Replace selections of TMP SubMesh with TMPUGui textfield since no one wants to select the SubMesh.
                                    if ( nativeSelection[i].name.StartsWith("TMP SubMesh") && nativeSelection[i].transform.parent != null )
                                    {
                                        nativeSelection[i] = nativeSelection[i].transform.parent.gameObject;
                                        selectionChanged = true;
                                    }
                                }

                                // Remove
                                for (int i = nativeSelection.Count-1; i >= 0; --i)
                                {
                                    // Remove "excluded by name" objects from selection.
                                    if (IsExcludedByName( nativeSelection[i].name ) )
                                    {
                                        nativeSelection.RemoveAt(i);
                                        selectionChanged = true;
                                    }
                                    // Remove "excluded by tag" objects from selection.
                                    else if (IsExcludedByTag(nativeSelection[i].tag))
                                    {
                                        nativeSelection.RemoveAt(i);
                                        selectionChanged = true;
                                    }
                                    // Remove "excluded by type" objects from selection.
                                    else if (IsExcludedByType(nativeSelection[i].transform))
                                    {
                                        nativeSelection.RemoveAt(i);
                                        selectionChanged = true;
                                    }
                                    else
                                    {
                                        // Remove "excluded by tag/name" children of a canvas
                                        var nativeObjCanvas = nativeSelection[i].GetComponentInParent<Canvas>();
                                        if( nativeObjCanvas != null
                                            && ( IsExcludedByName(nativeObjCanvas.name)
                                              || IsExcludedByTag(nativeObjCanvas.tag)
                                              || IsExcludedByType(nativeObjCanvas.transform) )
                                          )
                                        {
                                            nativeSelection.RemoveAt(i);
                                            selectionChanged = true;
                                        }
                                    }

                                }

                                if ( selectionChanged )
                                {
                                    selectionChangedBySmartUi = true;
                                    Selection.objects = nativeSelection.ToArray();
                                }
                            }
                        }
                    }
                }
            }

            if ( mouseEvent != null && selectionChangedBySmartUi == true )
            {
                // Consume event to avoid selection by unity.
                mouseEvent.Use();
                // Reset hot control to avoid random selection afterwards (Unity 2017+)
                GUIUtility.hotControl = 0;
            }
        }

        /// <summary>
        /// Returns the distance to the mesh from the raycast origin. Returns float.NaN if no hit was found.
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="objectWithMeshFilter"></param>
        /// <returns>Distance as float or float.NaN</returns>
        private static float RayCastMesh(Ray ray, GameObject objectWithMeshFilter)
        {
            if (objectWithMeshFilter != null)
            {
                float distance = float.NaN;
                // check all triangles for hit with raycast
                var meshFilter = objectWithMeshFilter.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    int[] triangles = meshFilter.sharedMesh.triangles;
                    Vector3[] vertices = meshFilter.sharedMesh.vertices;
                    int a, b, c;
                    for (int i = 0; i < triangles.Length; i = i + 3)
                    {
                        a = triangles[i];
                        b = triangles[i + 1];
                        c = triangles[i + 2];
                        /*
                        Debug.DrawLine(meshFilter.transform.TransformPoint(vertices[a]), meshFilter.transform.TransformPoint(vertices[b]), Color.red, 3.0f);
                        Debug.DrawLine(meshFilter.transform.TransformPoint(vertices[b]), meshFilter.transform.TransformPoint(vertices[c]), Color.red, 3.0f);
                        Debug.DrawLine(meshFilter.transform.TransformPoint(vertices[c]), meshFilter.transform.TransformPoint(vertices[a]), Color.red, 3.0f); //*/
                        distance = IntersectRayTriangle(
                            ray,
                            meshFilter.transform.TransformPoint(vertices[a]),
                            meshFilter.transform.TransformPoint(vertices[b]),
                            meshFilter.transform.TransformPoint(vertices[c]));
                        if (!float.IsNaN(distance))
                        {
                            break;
                        }
                    }
                }
                if (float.IsNaN(distance) == false)
                {
                    if (distance < SmartUiSelection_Settings.maxDistanceFor3DSelection)
                    {
                        return distance;
                    }
                }
            }

            return float.NaN;
        }

        // reflection cache for mesh raycast (speeds up code execution)
        private static Type[] _rcEditorTypes;
        private static Type _rcHandleUtilityType;
        private static System.Reflection.MethodInfo _rcIntersectRayMeshMethod;

        private static void buildReflectionCache()
        {
            try
            {
                _rcEditorTypes = typeof(Editor).Assembly.GetTypes();
                if (_rcEditorTypes != null && _rcEditorTypes.Length > 0)
                {
                    _rcHandleUtilityType = _rcEditorTypes.FirstOrDefault(t => t.Name == "HandleUtility");
                    if (_rcHandleUtilityType != null)
                    {
                        _rcIntersectRayMeshMethod = _rcHandleUtilityType.GetMethod(
                            "IntersectRayMesh",
                            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                    }
                }
            }
            catch (Exception)
            {
                // fail silently
            }
        }

        public static bool IsExcludedByName(string name)
        {
            if(SmartUiSelection_Settings.pushKeyToDisableExcludeLists && _isDisableExcludesKeyPressed)
            {
                return false;
            }
            
            return SmartUiSelection_Settings.excludeByName.Contains(name);
        }

        public static bool IsExcludedByTag(string tag)
        {
            if (SmartUiSelection_Settings.pushKeyToDisableExcludeLists && _isDisableExcludesKeyPressed)
            {
                return false;
            }

            return SmartUiSelection_Settings.excludeByTag.Contains(tag);
        }

        public static bool IsExcludedByType(Transform transform)
        {
            if (SmartUiSelection_Settings.pushKeyToDisableExcludeLists && _isDisableExcludesKeyPressed)
            {
                return false;
            }

            if (SmartUiSelection_Settings.excludeByType.Count > 0)
            {
                foreach (var excludedType in SmartUiSelection_Settings.excludeByType)
                {
                    if (excludedType != null && excludedType.GetClass() != null)
                    {
                        if (transform.GetComponent(excludedType.GetClass()) != null)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static bool IsExcludedBecauseOfScrollRect(RectTransform transform)
        {
            if (!SmartUiSelection_Settings.ignoreScrollRects)
                return false;

            return transform.GetComponent<ScrollRect>() != null;
        }

        public static bool IsExcludedBecauseOfMask(RectTransform transform)
        {
            if (!SmartUiSelection_Settings.ignoreMaskImages)
                return false;

            var mask = transform.GetComponent<Mask>();
            return mask != null && mask.enabled && !mask.showMaskGraphic;
        }

        /// <summary>
        /// Checks if a ray intersects with a mesh and saves the result in hit.
        /// Return an integer meaning: -1 = reflection didn't work, 1 = mesh was hit by the ray, 0 = mesh was not hit by the ray.
        /// Thanks to: https://forum.unity.com/threads/editor-raycast-against-scene-meshes-without-collider-editor-select-object-using-gui-coordinate.485502/#post-3162431
        /// and https://gist.github.com/MattRix/9205bc62d558fef98045
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="meshFilter"></param>
        /// <param name="hit"></param>
        /// <returns>An integer meaning: -1 = reflection didn't work, 1 = mesh was hit by the ray, 0 = mesh was not hit by the ray</returns>
        public static int IntersectRayMesh(Ray ray, MeshFilter meshFilter, out RaycastHit hit)
        {
            // this depends on the reflectin cache
            if (_rcIntersectRayMeshMethod != null)
            {
                var parameters = new object[] { ray, meshFilter.sharedMesh, meshFilter.transform.localToWorldMatrix, null };
                var result = _rcIntersectRayMeshMethod.Invoke(null, parameters);
                hit = (RaycastHit)parameters[3];
                if( result != null )
                {
                    if( (bool) result == true )
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }

            hit = default(RaycastHit);
            return -1;
        }

        private static bool isLayerInLayers( int layer, int layers )
        {
            return ((1 << layer) & layers) != 0;
        }

        static List<GameObject> _selectionMemory;

        private static void clearSelectionMemory()
        {
            if (_selectionMemory != null)
            {
                _selectionMemory.Clear();
            }
        }

        private static void setSelection( GameObject[] objects, bool addToSelection )
        {
            if (_selectionMemory == null)
            {
                _selectionMemory = new List<GameObject>();
            }

            if ( !addToSelection )
            {
                clearSelectionMemory();
            }

            for( int i = 0; i < objects.Length; ++i )
            {
                if( _selectionMemory.Contains(objects[i]) )
                {
                    // deselect
                    _selectionMemory.Remove(objects[i]);
                }
                else
                {
                    // select
                    _selectionMemory.Add(objects[i]);
                }
            }
            
            Selection.objects = _selectionMemory.ToArray();
        }

        /// <summary>
        /// Calculates all corners in world space and returns them in clockwise order.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private static Vector3[] getWorldCornersFromGuiTextMinMaxPositions( RectTransform transform, Vector3 min, Vector3 max )
        {
            // min is bottomLeftPos
            // max is topRightPos
            // top left
            var topLeftTextPos = min;
            topLeftTextPos.y = max.y;
            // top right
            var bottomRightTextPos = min;
            bottomRightTextPos.x = max.x;

            // convert the corners to world space
            var worldTextCorners = new List<Vector3>() {
                                 transform.TransformPoint(min)
                                ,transform.TransformPoint(topLeftTextPos)
                                ,transform.TransformPoint(max)
                                ,transform.TransformPoint(bottomRightTextPos)
                            };

            // Sort clockwise
            Vector3 center = (worldTextCorners[0] + worldTextCorners[2]) * 0.5f;
            Vector3 normal = Vector3.Cross(worldTextCorners[1] - worldTextCorners[0], worldTextCorners[3] - worldTextCorners[0]);
            worldTextCorners.Sort( ( a, b ) => compareVectorsClockwise( center, normal, a, b ) );

            return worldTextCorners.ToArray();
        }

        private static int compareVectorsClockwise(Vector3 center, Vector3 normal, Vector3 a, Vector3 b)
        {
            float result = Vector3.Dot(normal, Vector3.Cross(a - center, b - center));
            if (result > 0) return -1;
            if (result < 0) return  1;
            return 0;
        }

        /// <summary>
        /// Returns the vector from the list of positions which is closest to the reference point.
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="referencePoint"></param>
        /// <returns></returns>
        private static Vector3 getClosestPositionToReferenceInXY(Vector3[] positions, Vector3 referencePoint)
        {
            Vector3 closestPosition = default(Vector3);
            float minDistance = Mathf.Infinity;
            float currentDistance;
            foreach (var pos in positions)
            {
                currentDistance = Vector3.Distance(pos, referencePoint);
                if (currentDistance < minDistance )
                {
                    closestPosition = pos;
                    minDistance = currentDistance;
                }
            }

            return closestPosition;
        }

        /// <summary>
        /// Tries to retrieve the value from obj.propName and stores it in out result. Returns true on success.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="propName"></param>
        /// <param name="result">The value of the property of type T.</param>
        /// <returns>True if value could be fetched, false otherwise.</returns>
        public static bool GetPropertyValue<T>(object obj, string propName, out T result)
        {
            if (obj == null)
            {
                result = default(T);
                return false;
            }

            if( obj.GetType().GetProperty(propName) == null )
            {
                result = default(T);
                return false;
            }

            try
            {
                result = (T)obj.GetType().GetProperty(propName).GetValue(obj, null);
                return true;
            }
            catch( Exception )
            {
                result = default(T);
                return false;
            }
        }

        public static int CompareHierarchyDepths(Canvas a, Canvas b)
        {
            return CompareHierarchyDepths(a.transform, b.transform);
        }

        public static int CompareHierarchyDepths( Transform a, Transform b )
        {
            int result = 0;

            // go through the hierarchy tree bottom up
            var chainA = new List<int>();
            chainA.Add(a.GetSiblingIndex());
            while (a.parent != null)
            {
                a = a.parent;
                chainA.Add(a.GetSiblingIndex());
            }

            var chainB = new List<int>();
            chainB.Add(b.GetSiblingIndex());
            while (b.parent != null)
            {
                b = b.parent;
                chainB.Add(b.GetSiblingIndex());
            }

            // compare top down
            int minLength = Mathf.Min(chainA.Count, chainB.Count);
            int aLength = chainA.Count;
            int bLength = chainB.Count;
            for ( int i=1; i <= minLength; ++i )
            {
                if( chainA[aLength-i] > chainB[bLength-i] )
                {
                    return -1;
                }
                else if( chainA[aLength-i] < chainB[bLength-i] )
                {
                    return 1;
                }
                else
                {
                    if( chainA.Count > chainB.Count )
                    {
                        return -1;
                    }
                    else if (chainA.Count < chainB.Count)
                    {
                        return 1;
                    }
                }
            }

            return result;
        }

        public static int CompareSelectionResultBySelectionBase(SelectionResult x, SelectionResult y)
        {
            if( x.SelectionBase != null && y.SelectionBase == null )
            {
                return -1;
            }
            else if (x.SelectionBase == null && y.SelectionBase != null)
            {
                return 1;
            }
            return 0;
        }

        public static int CompareSelectionResults(SelectionResult x, SelectionResult y)
        {
            // is canvas sorting necessary?
            if (x.Canvas != y.Canvas)
            {
                // screen space overlay canvas is always on top (ignores sorting layers completely)
                if (x.Canvas.renderMode == RenderMode.ScreenSpaceOverlay && y.Canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                {
                    return -1;
                }
                else if (x.Canvas.renderMode != RenderMode.ScreenSpaceOverlay && y.Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    return 1;
                }

                // sort by sorting layers (if no canvas is ScreenSpaceOverlay)
                if (x.Canvas.renderMode != RenderMode.ScreenSpaceOverlay && y.Canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                {
                    int layerOrderX = SortingLayer.GetLayerValueFromID(x.Canvas.sortingLayerID);
                    int layerOrderY = SortingLayer.GetLayerValueFromID(y.Canvas.sortingLayerID);
                    if (layerOrderX > layerOrderY)
                    {
                        return -1;
                    }
                    if (layerOrderX < layerOrderY)
                    {
                        return 1;
                    }
                }

                // sort by sort order within layer
                if (x.Canvas.sortingOrder > y.Canvas.sortingOrder)
                {
                    return -1;
                }
                else if (x.Canvas.sortingOrder < y.Canvas.sortingOrder)
                {
                    return 1;
                }

                // sort by depth (distance to mouse)
                float distance = Mathf.Abs(x.DistanceToMouse - y.DistanceToMouse);
                if (distance > 0.0001 && x.DistanceToMouse < y.DistanceToMouse)
                {
                    return -1;
                }
                else if (distance > 0.0001 && x.DistanceToMouse > y.DistanceToMouse)
                {
                    return 1;
                }
                else
                {
                    // Debug.Log("SmartUiSelection: ui selection is ambiguous between '" + x.GameObject.name + " ("+x.Canvas.name+")' and '" + y.GameObject.name + " (" + y.Canvas.name + ")'.");

                    // The situation: same sorting layer, same sorting order, same distance to mouse = ?!?
                    // I call this undefined since it usually would be a case of z-fighting.
                    if (x.Canvas.renderMode != y.Canvas.renderMode)
                    {
                        // Unitys native selection actually shows the last activated in front (instead of z-fighting) but
                        // always selects ScreenSpaceCamera canvases before WorldSpace canvases (which is not consistent with
                        // what it shows to the user in the editor). We'll do the same here to be consistent with Unitys inconsistency (pun intended).
                        if (x.Canvas.renderMode == RenderMode.ScreenSpaceCamera) return -1;
                        if (y.Canvas.renderMode == RenderMode.ScreenSpaceCamera) return 1;
                    }
                    else
                    {
                        var xGraphic = x.GameObject.GetComponent<Graphic>();
                        var yGraphic = y.GameObject.GetComponent<Graphic>();
                        if (xGraphic != null && yGraphic != null)
                        {
                            return yGraphic.depth - xGraphic.depth;
                        }
                        else
                        {
                            // order based on position in hierarchy if both canvases have the same RenderMode
                            if (x.HierarchyOrder < y.HierarchyOrder)
                            {
                                return -1;
                            }
                            else if (x.HierarchyOrder > y.HierarchyOrder)
                            {
                                return 1;
                            }
                        }
                    }
                    return 0;
                }
            }
            else
            {
                // order based on position in hierarchy
                if (x.HierarchyOrder < y.HierarchyOrder)
                {
                    return -1;
                }
                else if (x.HierarchyOrder > y.HierarchyOrder)
                {
                    return 1;
                }
                return 0;
            }
        }

        public static bool DoesRayIntersectRectTransform(Ray ray, RectTransform transform)
        {
            return WorldPointOnRectTransform(ray, transform).HasValue;
        }

        /// <summary>
        /// Alias for WorldPointOnRectTransform(Ray ray, Vector3[] worldCorners)
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static Vector3? WorldPointOnRectTransform(Ray ray, RectTransform transform)
        {
            Vector3[] rectCorners = new Vector3[4];
            transform.GetWorldCorners(rectCorners);

            return WorldPointOnRectTransform(ray, rectCorners);
        }

        /// <summary>
        /// Calculates the intersection point of the ray with the given rect transform in world space.
        /// Returns null if the ray does not hit the rect transform.
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="worldCorners">Each corner provides its world space value. The array of 4 vertices is clockwise. It starts bottom left and rotates to top left, then top right, and finally bottom right. Note that bottom left, for example, is an (x, y, z) vector with x being left and y being bottom.</param>
        /// <returns></returns>
        public static Vector3? WorldPointOnRectTransform(Ray ray, Vector3[] worldCorners)
        {
            // check tri 1
            float distance = IntersectRayTriangle(ray, worldCorners[0], worldCorners[1], worldCorners[2]);
            if (float.IsNaN(distance))
            {
                distance = IntersectRayTriangle(ray, worldCorners[0], worldCorners[2], worldCorners[3]);
            }

            if (!float.IsNaN(distance))
            {
                // hit found, calc point
                return ray.origin + ray.direction.normalized * distance;
            }
            else
            {
                // ray does not hit the transform
                return null;
            }
        }

        const float kEpsilon = 0.000001f;

        /// <summary>
        /// Thanks to: https://answers.unity.com/questions/861719/a-fast-triangle-triangle-intersection-algorithm-fo.html
        /// Ray-versus-triangle intersection test suitable for ray-tracing etc.
        /// Port of Möller–Trumbore algorithm c++ version from:
        /// https://en.wikipedia.org/wiki/Möller–Trumbore_intersection_algorithm
        /// </summary>
        /// <returns><c>The distance along the ray to the intersection</c> if one exists, <c>NaN</c> if one does not.</returns>
        /// <param name="ray">the ray</param>
        /// <param name="v0">A vertex 0 of the triangle.</param>
        /// <param name="v1">A vertex 1 of the triangle.</param>
        /// <param name="v2">A vertex 2 of the triangle.</param>
        public static float IntersectRayTriangle(Ray ray, Vector3 v0, Vector3 v1, Vector3 v2)
        {
            // edges from v1 & v2 to v0.     
            Vector3 e1 = v1 - v0;
            Vector3 e2 = v2 - v0;

            Vector3 h = Vector3.Cross(ray.direction, e2);
            float a = Vector3.Dot(e1, h);
            if ((a > -kEpsilon) && (a < kEpsilon))
            {
                return float.NaN;
            }

            float f = 1.0f / a;

            Vector3 s = ray.origin - v0;
            float u = f * Vector3.Dot(s, h);
            if ((u < 0.0f) || (u > 1.0f))
            {
                return float.NaN;
            }

            Vector3 q = Vector3.Cross(s, e1);
            float v = f * Vector3.Dot(ray.direction, q);
            if ((v < 0.0f) || (u + v > 1.0f))
            {
                return float.NaN;
            }

            float t = f * Vector3.Dot(e2, q);
            if (t > kEpsilon)
            {
                return t;
            }
            else
            {
                return float.NaN;
            }
        }
    }
}
#endif
