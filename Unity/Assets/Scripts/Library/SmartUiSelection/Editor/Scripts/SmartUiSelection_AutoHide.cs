// Copyright (C) 2018 KAMGAM e.U. - All rights reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace kamgam.editor.smartuiselection
{
    public class ManagedRootCanvas
    {
        public Canvas Canvas;
        public bool IsVisible;

        public ManagedRootCanvas(Canvas canvas, bool isVisible)
        {
            Canvas = canvas;
            IsVisible = isVisible;
        }
    }

    // Whenever a cavas is hidden a list of the already hidden objects is made to
    // restore them once the canvas is shown again.
    // All the serializing stuff is for remembering the hidden objects across
    // recompiles and domain reloads.

    // Because the JSON Utility can not serialize generic Lists without a wrapper class.
    [System.Serializable]
    public class SerializableHiddenObjects
    {
        [SerializeField]
        public List<HiddenObject> Objects = new List<HiddenObject>();
    }

    [Serializable]
    public class HiddenObject
    {
        [System.NonSerialized]
        protected RectTransform _transform;
        public RectTransform Transform
        {
            get => _transform;
            set 
            {
                _transform = value;
                _path = null;
                _sceneName = null;
                GetPath();
            }
        }

        [SerializeField]
        protected string _sceneName;

        [SerializeField]
        protected string _path;

        public bool AreAllDescendantsHidden;

        public string GetPath()
        {
            if (_path == null && Transform != null)
            {
                _sceneName = Transform.gameObject.scene.name;
                _path = GetPath(Transform);
            }
            return _path;
        }

        public string GetSceneName()
        {
            return _sceneName;
        }

        public static string GetPath(Transform transform)
        {
            string path = "";
            Transform t = transform;
            while(t != null)
            {
                path = "/" + t.name + "/" + t.GetSiblingIndex() + path;
                t = t.parent;
            }

            return path;
        }

        public HiddenObject(RectTransform transform, bool areAllDescendantsHidden)
        {
            Transform = transform;
            AreAllDescendantsHidden = areAllDescendantsHidden;
        }

        public void InitAfterDeserialization()
        {
            if (_transform == null && _sceneName != null && _path != null)
            {
                _transform = Find(_sceneName, _path).GetComponent<RectTransform>();
            }
        }

        static List<GameObject> _tmpGameObjectsA = new List<GameObject>();
        static List<GameObject> _tmpGameObjectsB = new List<GameObject>();
        public static GameObject Find(string sceneName, string path)
        {
            _tmpGameObjectsA.Clear();
            _tmpGameObjectsB.Clear();

            var pathParts = path.Substring(1, path.Length - 1).Split('/');
            int maxDepth = pathParts.Length / 2;

            int sceneCount = EditorSceneManager.sceneCount;
            for (int s = 0; s < sceneCount; s++)
            {
                var scene = EditorSceneManager.GetSceneAt(s);

                // Skip if scene names don't match
                if (scene.name != sceneName)
                    continue;
    
                scene.GetRootGameObjects(_tmpGameObjectsA);
                for (int depth = 0; depth < maxDepth; depth++)
                {
                    _tmpGameObjectsB.Clear();
                    foreach (var obj in _tmpGameObjectsA)
                    {
                        string name = pathParts[(depth * 2)];
                        if (obj.name != name)
                            continue;

                        string siblingIndexAsString = pathParts[(depth * 2) + 1];
                        int siblingIndex = int.Parse(siblingIndexAsString);
                        if (obj.transform.GetSiblingIndex() != siblingIndex)
                            continue;

                        if (depth < maxDepth - 1)
                        {
                            for (int i = 0; i < obj.transform.childCount; i++)
                            {
                                var t = obj.transform.GetChild(i);
                                _tmpGameObjectsB.Add(t.gameObject);
                            }
                        }
                        else
                        {
                            return obj;
                        }
                    }

                    _tmpGameObjectsA.Clear();
                    _tmpGameObjectsA.AddRange(_tmpGameObjectsB);
                }
            }

            _tmpGameObjectsA.Clear();
            _tmpGameObjectsB.Clear();

            return null;
        }
    }

    [InitializeOnLoad]
    public static class SmartUiSelection_AutoHide
    {
        public static List<ManagedRootCanvas> ManagedRootCanvases;
        public static SerializableHiddenObjects HiddenObjects;

        // Check timer
        static double _lastAutoHideCheckTime;
        static float _autoHideCheckIntervaInSec = 0.100f; // update every 100 ms

        // Detected distance to XY plane
        static float? _camToXYPlaneDistance;

        // Remember last editor camera in case it is null at some point.
        static Camera _lastKnownCamera;

        static bool _isVisible
        {
            get => SessionState.GetBool("SmartUI::autoHideIsVisible", true);
            set => SessionState.SetBool("SmartUI::autoHideIsVisible", value);
        }
        static bool _mouseIsInSceneView = false;
        static bool _editorIsInPlayMode = false;
        static bool _enabled = false;
        static bool _sceneOpened = false;

        static SmartUiSelection_AutoHide()
        {
            if (SmartUiSelection_Settings.instance == null)
            {
                Debug.LogWarning("SmartUiSelection plugin did not find any settings and will do nothing.\nPlease create them in a 'Resources' folder via Assets -> Create -> SmartUiSelection Settings.");
            }

            ManagedRootCanvases = new List<ManagedRootCanvas>();
            HiddenObjects = new SerializableHiddenObjects();
            loadHiddenObjects();

            _camToXYPlaneDistance = null;

            EditorApplication.update += AutoHideScreenSpaceOverlayCanvases;
            EditorSceneManager.sceneOpened += onSceneOpened;
            EditorSceneManager.sceneClosing += onSceneClosing;
            AssemblyReloadEvents.beforeAssemblyReload += beforeAssemblyReload;
        }

        private static void onSceneClosing(Scene scene, bool removingScene)
        {
            updateCanvases(visible: true, null);
        }

        private static void onSceneOpened(Scene scene, OpenSceneMode mode)
        {
            _sceneOpened = true;
        }

        private static void beforeAssemblyReload()
        {
            saveHiddenObjects();
            updateCanvases(visible: true, null);
        }

        [MenuItem("Tools/Smart Ui Selection/Clear auto-hide user change memory", priority = 301)]
        static void resetHiddenObjects()
        {
            clearHiddenObjects();
            HiddenObjects.Objects.Clear();
        }

        static void clearHiddenObjects()
        {
            HiddenObjects.Objects.Clear();
            SessionState.EraseString("SmartUI::autoHide.hiddenObjects");
        }

        static void saveHiddenObjects()
        {
            // Load old
            var stringData = SessionState.GetString("SmartUI::autoHide.hiddenObjects", "{}");
            var serializedData = JsonUtility.FromJson<SerializableHiddenObjects>(stringData);

            foreach (var hiddenObject in HiddenObjects.Objects)
            {
                // Update
                bool updated = false;
                for (int i = 0; i < serializedData.Objects.Count; i++)
                {
                    if (serializedData.Objects[i].GetPath() == hiddenObject.GetPath())
                    {
                        serializedData.Objects[i] = hiddenObject;
                        updated = true;
                    }
                }
                // or Add
                if (!updated)
                {
                    serializedData.Objects.Add(hiddenObject);
                }
            }

            // Save
            string data = JsonUtility.ToJson(serializedData);
            SessionState.SetString("SmartUI::autoHide.hiddenObjects", data);
        }

        static void loadHiddenObjects()
        {
            var stringData = SessionState.GetString("SmartUI::autoHide.hiddenObjects", "{}");

            // Try to FIND the hidden objects in the current scenes.
            HiddenObjects = JsonUtility.FromJson<SerializableHiddenObjects>(stringData);
            foreach (var hiddenObject in HiddenObjects.Objects)
            {
                hiddenObject.InitAfterDeserialization();
            }
        }

        static List<ManagedRootCanvas> _canvasesToShowAfterRemoval = new List<ManagedRootCanvas>();

        private static void AutoHideScreenSpaceOverlayCanvases()
        {
            try
            {
                // Is enabled?
                if (SmartUiSelection_Settings.enablePlugin && SmartUiSelection_Settings.enableAutoHide)
                {
                    _enabled = true;

                    // Should be checked this frame?
                    float autoHideInterval = SmartUiSelection_Settings.autoHideAlways ? _autoHideCheckIntervaInSec * 2 : _autoHideCheckIntervaInSec;
                    if (EditorApplication.timeSinceStartup - _lastAutoHideCheckTime > autoHideInterval)
                    {
                        _lastAutoHideCheckTime = EditorApplication.timeSinceStartup;

                        updateManagedRootCanvasesList(_canvasesToShowAfterRemoval);

                        if (ManagedRootCanvases.Count > 0)
                        {
                            // Skip if Editor is refreshing.
                            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
                            {
                                return;
                            }

                            bool updateNeeded = false;
                            bool shouldBeVisible = true;

                            // Calc distance and update "shouldBeVisible"
                            if (getValidSceneViewCamera() != null)
                            {
                                // calculate the distance to the XY plane
                                var zeroPlane = new Plane(Vector3.forward, Vector3.zero); // screenspace canvases are in zero plane
                                var oldDistance = _camToXYPlaneDistance.HasValue ? _camToXYPlaneDistance.Value : float.MaxValue;
                                var newDistance = Mathf.Abs(zeroPlane.GetDistanceToPoint(_lastKnownCamera.transform.position));

                                // Did change to being below the threshold.
                                if (   oldDistance >  SmartUiSelection_Settings.autoHideDistanceThreshold
                                    && newDistance <= SmartUiSelection_Settings.autoHideDistanceThreshold)
                                {
                                    updateNeeded = true;
                                }
                                // Did change to being above the threshold.
                                else if (oldDistance < SmartUiSelection_Settings.autoHideDistanceThreshold
                                      && newDistance >= SmartUiSelection_Settings.autoHideDistanceThreshold)
                                {
                                    updateNeeded = true;
                                }

                                // Remember show/hide based on the distance result.
                                shouldBeVisible = newDistance > SmartUiSelection_Settings.autoHideDistanceThreshold;

                                // Update distance
                                _camToXYPlaneDistance = newDistance;
                            }

                            // Don't auto hide if a GO with a RectTransform is selected since we assume the user is currently editing this ui.
                            if ( SmartUiSelection_Settings.dontAudoHideIfUiSelected
                                && Selection.activeGameObject != null
                                && Selection.activeGameObject.transform is RectTransform)
                            {
                                shouldBeVisible = true;
                            }

                            // Is a build in progress (buggy, EditorApplication.update is not called while building) ..
                            if (BuildPipeline.isBuildingPlayer)
                            {
                                updateNeeded = true;
                                shouldBeVisible = true;
                            }
                            // .. not a 100% solution but at least it works if the user uses the mouse to click the "build" button.
                            if (EditorWindow.mouseOverWindow != null && EditorWindow.mouseOverWindow.ToString().Contains("BuildPlayerWindow"))
                            {
                                updateNeeded = true;
                                shouldBeVisible = true;
                            }

                            // Is mouse out of scene view and autoHideAlways is not enabled
                            bool mouseIsInSceneView = (EditorWindow.mouseOverWindow != null && SceneView.sceneViews.Contains(EditorWindow.mouseOverWindow));
                            if (mouseIsInSceneView == false && SmartUiSelection_Settings.autoHideAlways == false)
                            {
                                shouldBeVisible = true;
                            }
                            if(_mouseIsInSceneView != mouseIsInSceneView)
                            {
                                updateNeeded = true;
                            }
                            _mouseIsInSceneView = mouseIsInSceneView;

                            // Is Editor playing and autoHideDuringPlayback is set to false.
                            if (EditorApplication.isPlayingOrWillChangePlaymode && SmartUiSelection_Settings.autoHideDuringPlayback == false)
                            {
                                shouldBeVisible = true;
                            }
                            if (_editorIsInPlayMode != EditorApplication.isPlayingOrWillChangePlaymode)
                            {
                                updateNeeded = true;
                            }
                            _editorIsInPlayMode = EditorApplication.isPlayingOrWillChangePlaymode;

                            if (_sceneOpened)
                            {
                                updateNeeded = true;
                                _sceneOpened = false;
                                loadHiddenObjects();
                            }

                            // Update only if needed
                            if (updateNeeded && _isVisible != shouldBeVisible)
                            {
                                updateCanvases(shouldBeVisible, _canvasesToShowAfterRemoval);
                                _canvasesToShowAfterRemoval.Clear();
                            }
                        }
                    }
                }
                else if(_enabled)
                {
                    // on disabled auto-hide
                    _enabled = false;
                    updateCanvases(visible: true, _canvasesToShowAfterRemoval);
                    _canvasesToShowAfterRemoval.Clear();
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("SmartUiSelection auto-hide caused an unexpected Error:\n" + e.Message + "\nStacktrace:\n" + e.StackTrace);
            }
        }

        private static void updateManagedRootCanvasesList(List<ManagedRootCanvas> outRemovedCanvasesToShow)
        {
            // add new screen space canvases to the list
            var overlayCanvases = GameObject.FindObjectsOfType<Canvas>().Where(canvas => canvas.renderMode == RenderMode.ScreenSpaceOverlay);
            if (overlayCanvases.Count() > 0)
            {
                foreach (var canvas in overlayCanvases)
                {
                    if (!canvas.isRootCanvas)
                        continue;

                    if (ManagedRootCanvases.FirstOrDefault(c => c.Canvas == canvas) == null)
                    {
                        // add
                        var managedCanvas = new ManagedRootCanvas(canvas, isVisible: true);
                        ManagedRootCanvases.Add(managedCanvas);
                    }
                }
            }

            // Clean up in case ..
            for (int i = ManagedRootCanvases.Count - 1; i >= 0; --i)
            {
                //  .. a canvas got destroyed (i.e. scene change) or ..
                if (ManagedRootCanvases[i] == null || ManagedRootCanvases[i].Canvas == null)
                {
                    ManagedRootCanvases.RemoveAt(i);
                }
                //  .. if it is no longer a ScreenSpaceOverlay canvas.
                else if (ManagedRootCanvases[i].Canvas != null && ManagedRootCanvases[i].Canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                {
                    // schedule for update
                    if (outRemovedCanvasesToShow != null)
                        outRemovedCanvasesToShow.Add(ManagedRootCanvases[i]);
                    // remove
                    ManagedRootCanvases.RemoveAt(i);
                }
            }
        }

        private static Camera getValidSceneViewCamera()
        {
            Camera cam = null;
            // Fetch the scene view camera (only return once it has been initialized, aka position != 0/0/0)
            if (SceneView.lastActiveSceneView != null
                && SceneView.lastActiveSceneView.camera.transform.position != Vector3.zero)
            {
                cam = SceneView.lastActiveSceneView.camera;
            }
            // Remember: If you ping an object in the editor Camera.current
            //           becomes null, also if you change to play mode.
            if (cam != null)
            {
                _lastKnownCamera = cam;
            }

            return _lastKnownCamera;
        }

        private static List<RectTransform> _tmpTransforms = new List<RectTransform>(30);

        private static void updateCanvases(bool visible, List<ManagedRootCanvas> canvasesToShowAfterRemoval)
        {
            _isVisible = visible;

            if (ManagedRootCanvases != null)
            {
                // Remember if any children in the canvas are hidden
                foreach (var managedCanvas in ManagedRootCanvases)
                {
                    // Skip
                    if (managedCanvas.Canvas == null)
                        continue;

                    if (managedCanvas.Canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                        continue;

                    if (managedCanvas.IsVisible != visible)
                    {
                        if (!visible)
                        {
                            managedCanvas.Canvas.transform.GetComponentsInChildren<RectTransform>(includeInactive: true, _tmpTransforms);
                            foreach (var transform in _tmpTransforms)
                            {
                                // Skip if already in list (or if a parent is already in the list)
                                bool alreadyInList = false;
                                foreach (var hiddenObj in HiddenObjects.Objects)
                                {
                                    if (hiddenObj.Transform == null)
                                        continue;
                                    if (transform.IsChildOf(hiddenObj.Transform) && hiddenObj.AreAllDescendantsHidden)
                                    {
                                        alreadyInList = true;
                                    }
                                }
                                if (alreadyInList)
                                    continue;

                                if (SceneVisibilityManager.instance.IsHidden(transform.gameObject))
                                {
                                    if (SceneVisibilityManager.instance.AreAllDescendantsHidden(transform.gameObject))
                                    {
                                        // Find parent with "all descendants hidden". Save that parent instead of the object itself.
                                        var t = (Transform)transform;
                                        while (
                                            t.parent != null
                                            && SceneVisibilityManager.instance.IsHidden(t.parent.gameObject)
                                            && SceneVisibilityManager.instance.AreAllDescendantsHidden(t.parent.gameObject)
                                            )
                                        {
                                            t = t.parent;
                                        }

                                        // save parent if not yet in the list
                                        if (!hiddenObjectsContain(t))
                                        {
                                            var hiddenObject = new HiddenObject(t as RectTransform, true);
                                            HiddenObjects.Objects.Add(hiddenObject);
                                        }
                                    }
                                    else
                                    {
                                        if (!hiddenObjectsContain(transform))
                                        {
                                            var hiddenObject = new HiddenObject(transform, false);
                                            HiddenObjects.Objects.Add(hiddenObject);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // Update canvas visibility
                foreach (var managedCanvas in ManagedRootCanvases)
                {
                    // Skip
                    if (managedCanvas.Canvas == null)
                        continue;

                    if (managedCanvas.Canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                        continue;

                    if (managedCanvas.IsVisible != visible)
                    {
                        managedCanvas.IsVisible = visible;
                        setObjectVisible(managedCanvas.Canvas.gameObject, visible);
                    }
                }

                foreach (var hiddenObject in HiddenObjects.Objects)
                {
                    var path = hiddenObject.GetPath();
                    if (string.IsNullOrEmpty(path))
                    {
                        continue;
                    }

                    var go = HiddenObject.Find(hiddenObject.GetSceneName(), path);
                    if (go != null)
                    {
                        setObjectVisible(go, false, hiddenObject.AreAllDescendantsHidden);
                    }
                }

                if (visible)
                {
                    clearHiddenObjects();
                }
                else
                {
                    saveHiddenObjects();
                }
            }
        }

        static bool hiddenObjectsContain(Transform transform)
        {
            string path = HiddenObject.GetPath(transform);
            foreach (var hiddenObj in HiddenObjects.Objects)
            {
                if (hiddenObj.GetPath() == path)
                {
                    return true;
                }
            }

            return false;
        }

        private static void setObjectVisible(GameObject go, bool visible, bool includeDescendants = true)
        {
            // Try with reflection to not trigger any undos.
            bool success = SetGameObjectHiddenNoUndoViaReflection(go, !visible, includeChildren: includeDescendants);

            // FALLBACK (usually this will NOT be executed).
            // If SetGameObjectHiddenNoUndoViaReflection() fails then fall back on the regular show/hide.
            if (!success)
            {
                if (visible)
                {
                    SceneVisibilityManager.instance.Show(go, includeDescendants);
                }
                else
                {
                    SceneVisibilityManager.instance.Hide(go, includeDescendants);
                }
            }
        }

        public static float GetCamToXYPlaneDistance()
        {
            return _camToXYPlaneDistance.HasValue ? _camToXYPlaneDistance.Value : float.MaxValue;
        }


        // Thanks SceneVis, *sigh*
        #region reflection

        // reflection cache for mesh raycast (speeds up code execution)
        private static Type[] _refEditorTypes;
        private static Type _refSceneVisibilityStateType;
        private static System.Reflection.MethodInfo _refSceneVisibilityState_GetInstanceMethod;
        private static System.Reflection.MethodInfo _refSceneVisibilityState_SetGameObjectHiddenMethod;
        private static System.Reflection.PropertyInfo _refSceneVisibilityState_VisibilityActiveProperty;

        private static void buildReflectionCache()
        {
            try
            {
                if (_refEditorTypes != null)
                    return;
                    
                _refEditorTypes = typeof(Editor).Assembly.GetTypes();
                if (_refEditorTypes != null && _refEditorTypes.Length > 0)
                {
                    _refSceneVisibilityStateType = _refEditorTypes.FirstOrDefault(t => t.Name == "SceneVisibilityState");
                    if (_refSceneVisibilityStateType != null)
                    {
                        _refSceneVisibilityState_GetInstanceMethod = _refSceneVisibilityStateType.GetMethod(
                            "GetInstance",
                            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

                        _refSceneVisibilityState_SetGameObjectHiddenMethod = _refSceneVisibilityStateType.GetMethod(
                            "SetGameObjectHidden",
                            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

                        _refSceneVisibilityState_VisibilityActiveProperty = _refSceneVisibilityStateType.GetProperty("visibilityActive");
                    }
                }
            }
            catch (Exception)
            {
                // fail silently
            }
        }

        /// <summary>
        /// Based on the info found here:
        /// https://github.com/Unity-Technologies/UnityCsReference/blob/61f92bd79ae862c4465d35270f9d1d57befd1761/Editor/Mono/SceneView/SceneVisibilityState.bindings.cs#L20
        /// and here:
        /// https://github.com/Unity-Technologies/UnityCsReference/blob/61f92bd79ae862c4465d35270f9d1d57befd1761/Editor/Mono/SceneVisibilityManager.cs
        /// </summary>
        /// <returns></returns>
        public static UnityEngine.Object GetSceneVisibilityStateViaReflection()
        {
            try
            {
                buildReflectionCache();
                return (UnityEngine.Object) _refSceneVisibilityState_GetInstanceMethod.Invoke(null, new object[] { });
            }
            catch (Exception)
            {
                // fail silently
                return null;
            }
        }

        /// <summary>
        /// Based on the info found here:
        /// https://github.com/Unity-Technologies/UnityCsReference/blob/61f92bd79ae862c4465d35270f9d1d57befd1761/Editor/Mono/SceneView/SceneVisibilityState.bindings.cs#L20
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="isHidden"></param>
        /// <param name="includeChildren"></param>
        /// <returns>True if the reflection code has executed without exceptions.</returns>
        public static bool SetGameObjectHiddenNoUndoViaReflection(GameObject gameObject, bool isHidden, bool includeChildren)
        {
            try
            { 
                buildReflectionCache();
                var state = GetSceneVisibilityStateViaReflection();
                if (state != null)
                {
                    _refSceneVisibilityState_SetGameObjectHiddenMethod.Invoke(state, new object[] { gameObject, isHidden, includeChildren });
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Return true is visiblity is active, otherwise false<br />
        /// Notice: It will return false if reflection failed.
        /// </summary>
        /// <returns></returns>
        public static bool IsVisibilityActiveViaReflection()
        {
            try
            {
                buildReflectionCache();
                var state = GetSceneVisibilityStateViaReflection();
                if (state != null)
                {
                    return (bool)_refSceneVisibilityState_VisibilityActiveProperty.GetValue(state, null);
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion
    }
}
#endif
