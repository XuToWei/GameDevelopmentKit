#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Slate
{

    [CustomEditor(typeof(RuntimeRecorderClip))]
    public class RuntimeRecorderClipInspector : ActionClipInspector<RuntimeRecorderClip>
    {

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if ( action.dataClip == null ) {
                if ( GUILayout.Button("Create Data Clip") ) {
                    var path = EditorUtility.SaveFilePanel("Create Data Clip", Application.dataPath, action.actor.name, "anim");
                    if ( !string.IsNullOrEmpty(path) ) {
                        var clip = new AnimationClip();
                        var fileName = path.Split('/').Last().Split('.').First();
                        clip.name = fileName;
                        var dataPath = StringUtility.AbsToRelativePath(path);
                        AssetDatabase.CreateAsset(clip, dataPath);
                        AssetDatabase.ImportAsset(dataPath);
                        action.dataClip = clip;
                    }
                }
            }

            GUI.enabled = action.dataClip != null;
            if ( GUILayout.Button(action.armed ? "Armed" : "Arm for Recording", GUILayout.Height(60)) ) {
                action.armed = !action.armed;
            }
            GUI.enabled = true;
            EditorGUILayout.HelpBox("1) Create an animation data clip for the recording.\n2) Arm this clip for recording.\n3) Enter play mode and play the cutscene.\n4) Animations made to this actor (inluding its whole hierarchy) will be recorded to the animation data clip, while the cutscene is within this clip range and possible to playback as well.\n\nClips will automatically un-Arm after exiting play mode.", MessageType.Info);
        }
    }
}

#endif