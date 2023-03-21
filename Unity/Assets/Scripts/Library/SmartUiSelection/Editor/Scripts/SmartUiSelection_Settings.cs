// Copyright (C) 2018 KAMGAM e.U. - All rights reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

#if UNITY_EDITOR
#define KAMGAM_SMART_UI_SELECTION
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

namespace kamgam.editor.smartuiselection
{
    [System.Serializable]
    public class SmartUiSelection_Settings : ScriptableObject
    {
        private static SmartUiSelection_Settings _instance;

        /// <summary>
        /// Gets or creates a single instance of the settings.
        /// </summary>
        public static SmartUiSelection_Settings instance
        {
            get
            {
                if (SmartUiSelection_Settings._instance == null)
                {
                    SmartUiSelection_Settings._instance = LoadOrCreateSettingsInstance(false);
#if UNITY_5_5_OR_NEWER
                    AssetDatabase.importPackageCompleted += OnPackageImported;
#endif
                }

                return SmartUiSelection_Settings._instance;
            }
        }

        public static SmartUiSelection_Settings LoadOrCreateSettingsInstance(bool suppressWarning)
        {
            SmartUiSelection_Settings settings = getSettingsFromFile();

            // no settings from file, create an instance
            if (settings == null)
            {
                settings = createSettingsInstance();
            }

            if (settings == null && !suppressWarning)
            {
                Debug.LogWarning("Smart Ui Selection: no settings file found. Call Tools > Smart Ui Selection > Settings to create one. Falling back to default settings.");
            }

            return settings;
        }

        public static SmartUiSelection_Settings CreateSettingsFileIfNotExisting(bool notifyUser = true)
        {
            // no settings file found, try to create one from the settings instance
            SmartUiSelection_Settings settings = getSettingsFromFile();
            if (settings == null)
            {
                // fetch or create instance
                settings = LoadOrCreateSettingsInstance(true);

                // select asset file location
                string path = "Assets";
                if (AssetDatabase.IsValidFolder("Assets/SmartUiSelection/Editor"))
                {
                    path = "Assets/SmartUiSelection/Editor";
                }
                else if (AssetDatabase.IsValidFolder("Assets/Plugins/SmartUiSelection/Editor"))
                {
                    path = "Assets/Plugins/SmartUiSelection/Editor";
                }
                else
                {
                    if (!AssetDatabase.IsValidFolder("Assets/Editor"))
                    {
                        AssetDatabase.CreateFolder("Assets", "Editor");
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                    path = "Assets/Editor";
                }
                path = path + "/SmartUiSelection Settings.asset";

                // create asset file
                AssetDatabase.CreateAsset(settings, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                if (settings != null)
                {
                    _instance = settings;
                }

                // notify user
                if (notifyUser)
                {
                    EditorUtility.DisplayDialog("SmartUiSelection settings created.", "The 'SmartUiSelection Settings' file has been created in:\n'" + path + "'\n\nYou can also find it through the menu:\nTools > Smart Ui Selection > Settings", "Ok");
                }

                // select settings file
                Selection.activeObject = settings;
                EditorGUIUtility.PingObject(settings);
            }

            return settings;
        }

        static SmartUiSelection_Settings getSettingsFromFile()
        {
            SmartUiSelection_Settings settings = null;

            string[] foundPathGuids = AssetDatabase.FindAssets("t:SmartUiSelection_Settings");
            if (foundPathGuids.Length > 0)
            {
                //Debug.Log("Num of Assets: " + foundPathGuids.Length);
                settings = AssetDatabase.LoadAssetAtPath<SmartUiSelection_Settings>(AssetDatabase.GUIDToAssetPath(foundPathGuids[0]));
            }

            return settings;
        }

        static SmartUiSelection_Settings createSettingsInstance()
        {
            SmartUiSelection_Settings settings = ScriptableObject.CreateInstance<SmartUiSelection_Settings>();
            settings._excludeByName = new List<string>();
            settings._excludeByTag = new List<string>();
            settings._excludeByType = new List<MonoScript>();

            return settings;
        }

        public static void OnPackageImported(string packageName)
        {
            if (packageName.IndexOf("SmartUiSelection", System.StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                packageName.IndexOf("Smart Ui Selection", System.StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                packageName.IndexOf("Ui Select", System.StringComparison.CurrentCultureIgnoreCase) >= 0)
            {
                CreateSettingsFileIfNotExisting(false);

                EditorUtility.DisplayDialog("SmartUiSelection", "Smart Ui Selection imported.\nYou can open the settings through the menu:\n\nTools > Smart Ui Selection > Settings\n\nPlease read the manual.", "Ok");

                if (_instance != null) // TODO: _instance seems to be null at import time
                {
                    // select settings file
                    Selection.activeObject = _instance;
                    EditorGUIUtility.PingObject(_instance);
                }
            }
        }

        public void ForceSave()
        {
            EditorUtility.SetDirty(this); // TODO: maybe use AssetDatabase.ForceReserializeAssets() in newer versions.
        }

        // Plugin On/Off
        [MenuItem("Tools/Smart Ui Selection/Turn Plugin On", priority = 1)]
        public static void TurnPluginOn()
        {
            instance._enablePlugin = true;
            instance.ForceSave();
        }

        [MenuItem("Tools/Smart Ui Selection/Turn Plugin On", true)]
        public static bool ValidateTurnPluginOn()
        {
            return !instance._enablePlugin;
        }

        [MenuItem("Tools/Smart Ui Selection/Turn Plugin Off", priority = 1)]
        public static void TurnPluginOff()
        {
            instance._enablePlugin = false;
            instance.ForceSave();
        }

        [MenuItem("Tools/Smart Ui Selection/Turn Plugin Off", true)]
        public static bool ValidateTurnPluginOff()
        {
            return instance._enablePlugin;
        }

        // Click Through Canvas On/Off
        [MenuItem("Tools/Smart Ui Selection/Turn 'Click Throuch Canvas' On", priority = 100)]
        public static void TurnClickThroughCanvasOn()
        {
            instance._select3dObjectsBehindCanvas = true;
            instance.ForceSave();
        }

        [MenuItem("Tools/Smart Ui Selection/Turn 'Click Throuch Canvas' On", true)]
        public static bool ValidateTurnClickThroughCanvasOn()
        {
            return !instance._select3dObjectsBehindCanvas && instance._enablePlugin;
        }

        [MenuItem("Tools/Smart Ui Selection/Turn 'Click Throuch Canvas' Off", priority = 100)]
        public static void TurnClickThroughCanvasOff()
        {
            instance._select3dObjectsBehindCanvas = false;
            instance.ForceSave();
        }

        [MenuItem("Tools/Smart Ui Selection/Turn 'Click Throuch Canvas' Off", true)]
        public static bool ValidateTurnClickThroughCanvasOff()
        {
            return instance._select3dObjectsBehindCanvas && instance._enablePlugin;
        }

        // Click Through Canvas On/Off
        [MenuItem("Tools/Smart Ui Selection/Turn 'Smart UI Selection' On", priority = 200)]
        public static void TurnSmartUiSelectionOn()
        {
            instance._enableSmartUiSelection = true;
            instance.ForceSave();
        }

        [MenuItem("Tools/Smart Ui Selection/Turn 'Smart UI Selection' On", true)]
        public static bool ValidateTurnSmartUiSelectionOn()
        {
            return !instance._enableSmartUiSelection && instance._enablePlugin;
        }

        [MenuItem("Tools/Smart Ui Selection/Turn 'Smart UI Selection' Off", priority = 200)]
        public static void TurnSmartUiSelectionOff()
        {
            instance._enableSmartUiSelection = false;
            instance.ForceSave();
        }

        [MenuItem("Tools/Smart Ui Selection/Turn 'Smart UI Selection' Off", true)]
        public static bool ValidateTurnSmartUiSelectionOff()
        {
            return instance._enableSmartUiSelection && instance._enablePlugin;
        }

        // Auto Hide On/Off
        [MenuItem("Tools/Smart Ui Selection/Turn 'Canvas auto-hide' On", priority = 300)]
        public static void TurnAutoHideOn()
        {
            instance._enableAutoHide = true;
            instance.ForceSave();
        }

        [MenuItem("Tools/Smart Ui Selection/Turn 'Canvas auto-hide' On", true)]
        public static bool ValidateTurnAutoHideOn()
        {
            return !instance._enableAutoHide && instance._enablePlugin;
        }

        [MenuItem("Tools/Smart Ui Selection/Turn 'Canvas auto-hide' Off", priority = 300)]
        public static void TurnAutoHideOff()
        {
            instance._enableAutoHide = false;
            instance.ForceSave();
        }

        [MenuItem("Tools/Smart Ui Selection/Turn 'Canvas auto-hide' Off", true)]
        public static bool ValidateTurnAutoHideOff()
        {
            return instance._enableAutoHide && instance._enablePlugin;
        }

        // Plugin On/Off
        [MenuItem("Tools/Smart Ui Selection/Turn 'Push SPACE to use Smart Ui' ON", priority = 400)]
        public static void TurnPushKeyToUseSmartUiOn()
        {
            instance._pushKeyToUseUiSelection = true;
            instance.ForceSave();
        }

        [MenuItem("Tools/Smart Ui Selection/Turn 'Push SPACE to use Smart Ui' ON", true)]
        public static bool ValidateTurnPushKeyToUseSmartUiOn()
        {
            return !instance._pushKeyToUseUiSelection;
        }

        [MenuItem("Tools/Smart Ui Selection/Turn 'Push SPACE to use Smart Ui' OFF", priority = 400)]
        public static void TurnPushKeyToUseSmartUiOff()
        {
            instance._pushKeyToUseUiSelection = false;
            instance.ForceSave();
        }

        [MenuItem("Tools/Smart Ui Selection/Turn 'Push SPACE to use Smart Ui' OFF", true)]
        public static bool ValidateTurnPushKeyToUseSmartUiOff()
        {
            return instance._pushKeyToUseUiSelection && instance._enablePlugin;
        }

        // settings
        [MenuItem("Tools/Smart Ui Selection/Settings", priority = 500)]
        public static void SelectSettingsFile()
        {
            var settings = CreateSettingsFileIfNotExisting();
            if (settings != null)
            {
                Selection.activeObject = settings;
                EditorGUIUtility.PingObject(settings);
            }
            else
            {
                EditorUtility.DisplayDialog("SmartUiSelection settings could not be found.", "Settings file not found.\nPlease create it in Assets/Editor/Resources with Right-Click > Create > Smart Ui Selection > Settings.", "Ok");
            }
        }

        // manual
        [MenuItem("Tools/Smart Ui Selection/Manual", priority = 400)]
        public static void SelectManualFile()
        {
            string[] foundPathGuids = AssetDatabase.FindAssets("SmartUiSelection-manual");
            if (foundPathGuids.Length > 0)
            {
                var manual = AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(foundPathGuids[0]));
                Selection.activeObject = manual;
                EditorGUIUtility.PingObject(manual);
            }
            else
            {
                Debug.Log("SmartUiSelection: manual not found.");
            }
        }

        // manual
        [MenuItem("Tools/Smart Ui Selection/Feedback and Support (Unity Forum)", priority = 600)]
        public static void SelectFeedbackForum()
        {
            Application.OpenURL("https://forum.unity.com/threads/smart-ui-selection-unity-editor-extension.707522/");
        }

        [MenuItem("Tools/Smart Ui Selection/Feedback and Support (KAMGAM website)", priority = 600)]
        public static void SelectFeedbackWeb()
        {
            Application.OpenURL("https://kamgam.com/unity/support/smart_ui_selection");
        }

        [MenuItem("Tools/Smart Ui Selection/Feedback and Support (office@kamgam.com)", priority = 600)]
        public static void SelectFeedbackMail()
        {
            Application.OpenURL("mailto:office@kamgam.com");
        }

        // manual
        [MenuItem("Tools/Smart Ui Selection/▶ PLEASE WRITE A REVIEW, thanks.", priority = 700)]
        public static void WriteAReview()
        {
            Application.OpenURL("https://assetstore.unity.com/packages/tools/gui/smart-ui-selection-unity-editor-124328");
        }

        public static string Version
        {
            get { return instance._version; }
        }
        private string _version = "5.0.5"; // semver, of course

        // GENERNAL SETTINGS

        // enabled
        public static bool enablePlugin
        {
            get { return instance != null && instance._enablePlugin; }
        }
        [Header("Smart UI Selection - v5.0.5")]
        [Tooltip("Enables or disables the whole plugin.")]
        [SerializeField]
        private bool _enablePlugin = true;

        // multiClickTimeThreshold
        public static float multiClickTimeThreshold
        {
            get { return instance._multiClickTimeThreshold; }
        }
        [Tooltip("If you click twice within this time then the selection will cycle through all found ui elements. Time is in seconds.")]
        [SerializeField]
        [Range(0, 2)]
        private float _multiClickTimeThreshold = 1.0f;

        // selectOnlyEditableObjects
        public static bool selectOnlyEditableObjects
        {
            get { return instance != null && instance._selectOnlyEditableObjects; }
        }
        [Tooltip("If checked then objects whose hideFlags are set to HideFlags.NotEditable will be ignored.")]
        [SerializeField]
        private bool _selectOnlyEditableObjects = true;

        // exludeByNameList
        public static List<string> excludeByName
        {
            get { return instance._excludeByName; }
        }
        [Tooltip("Add names of game objects which should not be selectable.")]
        [UnityEngine.Serialization.FormerlySerializedAs("_excudeByName")]
        [SerializeField]
        private List<string> _excludeByName;

        // exludeByTagList
        public static List<string> excludeByTag
        {
            get { return instance._excludeByTag; }
        }
        [Tooltip("Add tags of game objects which should not be selectable.")]
        [UnityEngine.Serialization.FormerlySerializedAs("_excludeByTag")]
        [SerializeField]
        private List<string> _excludeByTag;

        // exludeByTypeList
        public static List<MonoScript> excludeByType
        {
            get { return instance._excludeByType; }
        }
        [Tooltip("Add Types (MonoScripts/components) which should not be selectable.Keep the list short because a lot of entries may impact performance.")]
        [SerializeField]
        private List<MonoScript> _excludeByType;

        // CLICK THROUGH CANVAS

        // select3dObjectsBehindCanvas
        public static bool select3dObjectsBehindCanvas
        {
            get { return instance._select3dObjectsBehindCanvas; }
        }
        [Header("Click Through Canvas")]
        [Tooltip("If no ui element has been selected then try to click through the canvas and select 3D objects behind it. Uses the 3d objects colliders or a bounding box if there is no collider.")]
        [SerializeField]
        private bool _select3dObjectsBehindCanvas = true;

        // select3dObjectsByMesh
        public static bool select3dObjectsByMesh
        {
            get { return instance._select3dObjectsByMesh; }
        }
        [Tooltip("Enabling this will make the click-through feature more accurate for meshes without colliders. It will do a raycast on all triangles of each mesh (even those without a collider). It may slow down the click handling (if clicked through a canvas in big scenes), turn it off if you have any issues.")]
        [SerializeField]
        private bool _select3dObjectsByMesh = true;

        // select3dColliders
        public static bool select3dColliders
        {
            get { return instance._select3dColliders; }
        }
        [Tooltip("Select 3D objects based on their colliders too. Useful for invisible objects which solely consist of colliders (like a trigger). Usually 3D objects are only selected based on their mesh.")]
        [SerializeField]
        private bool _select3dColliders = true;

        // highPrecisionSpriteSelection
        public static bool highPrecisionSpriteSelection
        {
            get { return instance._highPrecisionSpriteSelection; }
        }
        [Tooltip("Turn off if selection is slow in scenes with a lot of SpriteRenderers. If turned on then clicks are checked against the actual sprite mesh.")]
        [SerializeField]
        private bool _highPrecisionSpriteSelection = true;

        // maxDistanceFor3DSelection
        public static float maxDistanceFor3DSelection
        {
            get { return instance._maxDistanceFor3DSelection; }
        }
        [Tooltip("A raycast is used to detect 3D objects. This sets the maximum distance for the raycast in world units.")]
        [SerializeField]
        private float _maxDistanceFor3DSelection = 9000;

        // UI SETTINGS

        // smart ui selection
        public static bool enableSmartUiSelection
        {
            get { return instance._enableSmartUiSelection; }
        }
        [Header("Smart UI Selection")]
        [Tooltip("Enable ui selection improvements (always select what has been clicked).")]
        [SerializeField]
        private bool _enableSmartUiSelection = true;

        // pushKeyToUseUiSelection
        public static bool pushKeyToUseUiSelection
        {
            get { return instance != null && instance._pushKeyToUseUiSelection; }
        }
        [Space(10)]
        [Tooltip("If checked then Smart Ui Selection is only enabled if you press the 'Enable Smart Ui Key Code' key.")]
        [SerializeField]
        private bool _pushKeyToUseUiSelection = false;

        // This is the key which you need to press if "pushKeyToUseUiSelection" is checked.
        public static KeyCode EnableSmartUiKeyCode
        {
            get { return instance._enableSmartUiKeyCode; }
        }
        [Tooltip("Push and HOLD this key to disable Smart UI Selection (works only if pushKeyToUseUiSelection is turned on).")]
        [SerializeField]
        public KeyCode _enableSmartUiKeyCode = KeyCode.Space;

        // pushKeyToDisableExcludeLists
        public static bool pushKeyToDisableExcludeLists
        {
            get { return instance != null && instance._pushKeyToDisableExcludeLists; }
        }
        [Space(10)]
        [Tooltip("If checked and if the key is pressed then Smart Ui Selection will ignore your exclude lists (act as if they are empty).")]
        [SerializeField]
        private bool _pushKeyToDisableExcludeLists = false;

        // This is the key which you need to press if "pushKeyToDisableExcludeLists" is checked.
        public static KeyCode DisableExcludeListsKeyCode
        {
            get { return instance._disableExcludeListsKeyCode; }
        }
        [Tooltip("Push and HOLD this key to disable the exclude lists (works only if \"Push Key To Disable Exclude Lists\" is turned on).")]
        [SerializeField]
        public KeyCode _disableExcludeListsKeyCode = KeyCode.Escape;


        // limitSelectionToGraphics
        public static bool limitSelectionToGraphics
        {
            get { return instance._limitSelectionToGraphics; }
        }
        [Space(10)]
        [Tooltip("Limit ui selection to elements with graphics (objects which have a 'Graphic' component).")]
        [SerializeField]
        private bool _limitSelectionToGraphics = true;

        // alphaThreshold
        public static float alphaMinThreshold
        {
            get { return instance._graphicsAlphaMinThreshold; }
        }
        [Tooltip("Select elements only if they have an alpha value above the threshold. 'Limit Selection To Graphics' needs to be turned on for this to have any effect.")]
        [SerializeField]
        [Range(0, 1)]
        private float _graphicsAlphaMinThreshold = 0;

        // ignoreSelectionBaseAttributes
        public static bool ignoreSelectionBaseAttributes
        {
            get { return instance._ignoreSelectionBaseAttributes; }
        }
        [Tooltip("Check to completely ignore the [SelectionBase] Attributes. Hint: Buttons have this Attribute by default.")]
        [SerializeField]
        private bool _ignoreSelectionBaseAttributes = true;


        // ignoreScrolRects
        public static bool ignoreScrollRects
        {
            get { return instance._ignoreScrollRects; }
        }
        [Tooltip("Check to completely ignore ScrolRect transforms. Usually you just want the content, not the scroll rect.")]
        [SerializeField]
        private bool _ignoreScrollRects = true;

        // ignoreMaskImages
        public static bool ignoreMaskImages
        {
            get { return instance._ignoreMaskImages; }
        }
        [Tooltip("Check to completely ignore mask images (if the are hidden). Usually you just want the content of the masked area, not the image that defines the mask.")]
        [SerializeField]
        private bool _ignoreMaskImages = true;


        // SCREEN SPACE OVERLAY SETTINGS

        // hideScreenSpaceOverlaysOnCloseUp
        public static bool enableAutoHide
        {
            get { return instance._enableAutoHide; }
        }
        [Header("Canvas auto-hide")]
        [Tooltip("Should ScreenSpaceOverlay canvases be hidden in the scene view if the editor camera gets very close? Usefull to prohibit unwanted canvas selections while you edit the 3d scene. They will only be hidden if your mouse cursor is in the scene view.")]
        [SerializeField]
        private bool _enableAutoHide = false;

        // autoHideAlways
        public static bool autoHideAlways
        {
            get { return instance._autoHideAlways; }
        }
        [Tooltip("USE WITH CAUTION! If enabled then auto hide will always affect the canvases independently of where the mouse cursor is (this may confuse users). Enabling this works only in Unity 2019.2+. ")]
        [SerializeField]
        private bool _autoHideAlways = false;

        // dontAudoHideIfUiSelected
        public static bool dontAudoHideIfUiSelected
        {
            get { return instance._dontAudoHideIfUiSelected; }
        }
        [Tooltip("If enabled then canvases will not be auto-hidden if a GameObject with a RectTransform is selected. Useful if switching often between 3D and UI. Off by default.")]
        [SerializeField]
        private bool _dontAudoHideIfUiSelected = false;

        // autoHideDistanceThreshold
        public static float autoHideDistanceThreshold
        {
            get { return instance._autoHideDistanceThreshold; }
        }
        [Tooltip("ScreenSpaceOverlay canvases will be hidden if the editor camera distance to the XY plane is less than X.")]
        [SerializeField]
        [Range(0, 1000)]
        private float _autoHideDistanceThreshold = 400f;

        // autoHideDuringPlayback
        public static bool autoHideDuringPlayback
        {
            get { return instance._autoHideDuringPlayback; }
        }
        [Tooltip("Should ScreenSpaceOverlay canvases be hidden in play mode too? - BETA")]
        [SerializeField]
        private bool _autoHideDuringPlayback = false;

        // showAutoHideWarningGizmo
        public static bool showAutoHideWarningGizmo
        {
            get { return instance._showAutoHideWarningGizmo; }
        }
        [Tooltip("Show a warning text next to canvases to indicate that auto-hide is turned on?")]
        [SerializeField]
        private bool _showAutoHideWarningGizmo = true;

        // autoHideWarningTextColor
        public static Color autoHideWarningTextColor
        {
            get { return instance._autoHideWarningTextColor; }
        }
        [Tooltip("Color of the Text shown beneath a canvas which will be hidden.")]
        [SerializeField]
        private Color _autoHideWarningTextColor = new Color(1.0f, 0.7f, 0.0f);

        public void OnValidate()
        {
#if !UNITY_2019_2_OR_NEWER
            if (_autoHideAlways == true)
            {
                _autoHideAlways = false;
                Debug.LogWarning("Smart Ui: Auto hide canvas 'Auto Hide Always' is only supported in Unity 2019.2.0 or newer. Resetting it to disabled now.");
            }
#endif
        }
    }
}
#endif
