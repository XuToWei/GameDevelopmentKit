using UnityEngine;

namespace Slate
{
    [Attachable(typeof(RuntimeRecorderTrack))]
    [Description("Use the runtime recorder clip to record transform animation (position, rotation, scale) of the actor and its children objects in runtime.\n\nIt is highly recomended to group multiple objects if they are related (like for example a character or chunks of a physics shatter) under an empty root gameobject and use that empty root gameobject as the actor of this group.")]
    public class RuntimeRecorderClip : ActionClip, ISubClipContainable
    {

        [SerializeField, HideInInspector]
        private float _length = 5;

        public override float length {
            get { return _length; }
            set { _length = value; }
        }

        public override string info => "Data Clip: " + ( dataClip != null ? dataClip.name : "null" );
        public override bool isValid => dataClip != null;

        public AnimationClip dataClip;
        public float clipOffset;
        public bool armed;
        public RuntimeRecorder recorder { get; set; }
        public RuntimeRecorderTrack track => (RuntimeRecorderTrack)parent;

        public float subClipOffset { get => clipOffset; set => clipOffset = value; }
        public float subClipSpeed => 1f;
        public float subClipLength => dataClip != null ? dataClip.length : 0;

        ///----------------------------------------------------------------------------------------------

        protected override void OnEnter() { Open(); }
        protected override void OnReverseEnter() { Open(); }
        protected override void OnReverse() { Close(); }
        protected override void OnExit() { Close(); }

        ///----------------------------------------------------------------------------------------------

        void Open() {
            if ( armed && Application.isPlaying ) {
                recorder = new RuntimeRecorder(actor, track.recorderOptions);
            }
        }

        protected override void OnUpdate(float time, float previousTime) {
            if ( armed && Application.isPlaying ) {
                recorder.RecordFrame(time);
                return;
            }

            if ( dataClip != null && !dataClip.empty ) {
                dataClip.SampleAnimation(actor, Mathf.Repeat(time - clipOffset, subClipLength));
            }
        }

        void Close() {
            if ( armed && Application.isPlaying ) {
                recorder.ApplyToClip(dataClip);
                armed = false;
                recorder = null;
            }
        }

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        protected override void OnAfterValidate() {
            UnityEditor.EditorApplication.playModeStateChanged -= PlayModeChange;
            UnityEditor.EditorApplication.playModeStateChanged += PlayModeChange;
        }

        void PlayModeChange(UnityEditor.PlayModeStateChange state) {
            if ( state == UnityEditor.PlayModeStateChange.EnteredEditMode ) {
                armed = false;
            }
            if ( state == UnityEditor.PlayModeStateChange.EnteredPlayMode && armed ) {
                dataClip.ClearCurves();
            }
        }

        protected override void OnClipGUI(Rect rect) {
            if ( armed ) {
                GUI.color = new Color(1, 0, 0, 0.7f);
                GUI.Label(rect, "<b>ARMED</b>", Styles.centerLabel);
            }

            if ( dataClip != null ) {
                EditorTools.DrawLoopedLines(rect, dataClip.length / subClipSpeed, this.length, subClipOffset);
            }
        }
#endif
        ///----------------------------------------------------------------------------------------------

    }
}