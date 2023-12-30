#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Slate
{

    [CustomEditor(typeof(DirectorCamera))]
    public class DirectorCameraInspector : Editor
    {

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            EditorGUILayout.HelpBox("This is the master Director Camera Root. The child 'Render Camera' is from within where all cutscenes are rendered from. You can add any Image Effects in that Camera and even animate them if so required by using a Properties Track in the Director Group.", MessageType.Info);

            GUILayout.Space(10);
            DirectorCamera.autoHandleActiveState = EditorGUILayout.Toggle("Auto Handle Active State", DirectorCamera.autoHandleActiveState);
            EditorGUILayout.HelpBox("If true, the Render Camera active state is automatically handled (Highly Recommended).\nIf false, the Render Camera active state will remain intact.", MessageType.None);

            GUILayout.Space(10);
            DirectorCamera.matchMainWhenActive = EditorGUILayout.Toggle("Match Main When Active", DirectorCamera.matchMainWhenActive);
            EditorGUILayout.HelpBox("If true, will copy the Main Camera settings to Render Camera when it becomes active.\nIf false, the Render Camera settings will remain intact.", MessageType.None);

            GUILayout.Space(10);
            DirectorCamera.setMainWhenActive = EditorGUILayout.Toggle("Set Main When Active", DirectorCamera.setMainWhenActive);
            EditorGUILayout.HelpBox("If true, will set the Render Camera as MainCamera tag (Camera.main) for the duration of cutscenes.\nIf false, tags will remain intact.", MessageType.None);

            GUILayout.Space(10);
            DirectorCamera.ignoreFOVChanges = EditorGUILayout.Toggle("Ignore Field Of View Changes", DirectorCamera.ignoreFOVChanges);
            EditorGUILayout.HelpBox("If true, FOV changes will be ignored. Usefull for VR so that the device FOV is used.\nIf false, FOV will be possible to change and be animated normally.", MessageType.None);

            GUILayout.Space(10);
            DirectorCamera.dontDestroyOnLoad = EditorGUILayout.Toggle("Dont Destroy On Load", DirectorCamera.dontDestroyOnLoad);
            EditorGUILayout.HelpBox("If true, will make this Director Camera instance persist when loading a new level.\nIf false, the DirectorCamera of the new level will be used (or auto-created if none).", MessageType.None);

#if SLATE_USE_HDRP || SLATE_USE_URP
            GUILayout.Space(10);
            DirectorCamera.globalPostVolume = (UnityEngine.Rendering.Volume)EditorGUILayout.ObjectField("Global Post Processing Volume", DirectorCamera.globalPostVolume, typeof(UnityEngine.Rendering.Volume), true);
            EditorGUILayout.HelpBox("Assign a global 'Post Processing Volume' with Depth of Field that is used by your game camera. This will only be used for blending game camera to Slate shot camera Depth of Field. Neither the volume nor its profile parameters are modified.", MessageType.None);

#elif SLATE_USE_POSTSTACK
            GUILayout.Space(10);
            DirectorCamera.globalPostVolume = (UnityEngine.Rendering.PostProcessing.PostProcessVolume)EditorGUILayout.ObjectField("Global Post Processing Volume", DirectorCamera.globalPostVolume, typeof(UnityEngine.Rendering.PostProcessing.PostProcessVolume), true);
            EditorGUILayout.HelpBox("Assign a global 'Post Processing Volume' with Depth of Field that is used by your game camera. This will only be used for blending game camera to Slate shot camera Depth of Field. Neither the volume nor its profile parameters are modified.", MessageType.None);
#endif

            if ( GUI.changed ) {
                EditorUtility.SetDirty(target);
            }

        }
    }
}

#endif