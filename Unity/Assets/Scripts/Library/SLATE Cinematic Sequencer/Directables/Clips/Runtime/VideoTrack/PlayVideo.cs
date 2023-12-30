#if UNITY_2017_2_OR_NEWER

using UnityEngine;
using UnityEngine.Video;

namespace Slate
{

    [Attachable(typeof(VideoTrack))]
    [Name("Video Clip")]
    [Description("Play an imported Video Clip on the render camera or on a gameobject renderer.")]
    public class PlayVideo : ActionClip, ISubClipContainable
    {

        [SerializeField, HideInInspector]
        private float _length = 2;
        [SerializeField, HideInInspector]
        private float _blendIn = 0.2f;
        [SerializeField, HideInInspector]
        private float _blendOut = 0.2f;

        public VideoSource videoSource = VideoSource.VideoClip;
        [ShowIf("videoSource", 0)]
        public VideoClip videoClip;
        [ShowIf("videoSource", 1)]
        public string videoURL;
        public float clipOffset;

        private VideoSampler.SampleSettings settings;

        public override bool isValid {
            get { return videoSource == VideoSource.VideoClip && videoClip != null || videoSource == VideoSource.Url && !string.IsNullOrEmpty(videoURL); }
        }

        public override string info {
            get { return videoSource == VideoSource.VideoClip ? ( videoClip != null ? videoClip.name + (videoClip.originalPath.Substring(videoClip.originalPath.Length - 4)) : "No Video" ) : videoURL; }
        }

        public override float length {
            get { return _length; }
            set { _length = value; }
        }

        public override float blendIn {
            get { return _blendIn; }
            set { _blendIn = value; }
        }

        public override float blendOut {
            get { return _blendOut; }
            set { _blendOut = value; }
        }

        private VideoTrack track {
            get { return (VideoTrack)parent; }
        }

        public float subClipOffset {
            get { return clipOffset; }
            set { clipOffset = value; }
        }
        public float subClipLength => videoClip != null ? (float)videoClip.length : 0f;
        public float subClipSpeed => 1f;


        protected override void OnEnter() { Enable(); }
        protected override void OnReverseEnter() { Enable(); }
        protected override void OnReverse() { Disable(); }
        protected override void OnExit() { Disable(); }

        void Enable() {
            settings = VideoSampler.SampleSettings.Default();
            settings.source = videoSource;
            settings.renderTarget = track.renderTarget;
            settings.targetMaterialRenderer = track.targetMaterialRenderer;
            settings.aspectRatio = track.aspectRatio;

            track.source.source = videoSource;
            if ( videoSource == VideoSource.VideoClip ) {
                track.source.clip = videoClip;
            }
            if ( videoSource == VideoSource.Url ) {
                track.source.url = videoURL;
            }

            track.source.Play();
        }

        protected override void OnUpdate(float time, float previousTime) {
            var weight = GetClipWeight(time);
            var clipTime = ( time - clipOffset ) * root.playbackSpeed;
            var clipPreviousTime = ( previousTime - clipOffset ) * root.playbackSpeed;
            settings.playbackSpeed = root.playbackSpeed;
            settings.alpha = weight * track.targetCameraAlpha;
            settings.audioVolume = weight * track.targetCameraAlpha;
            if ( videoSource == VideoSource.VideoClip ) {
                VideoSampler.Sample(track.source, videoClip, null, clipTime, clipPreviousTime, settings);
            }
            if ( videoSource == VideoSource.Url ) {
                VideoSampler.Sample(track.source, null, videoURL, clipTime, clipPreviousTime, settings);
            }

        }

        void Disable() {
            track.source.Stop();
        }

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        protected override void OnClipGUI(Rect rect) {
            if ( videoSource == VideoSource.VideoClip && videoClip != null ) {
                EditorTools.DrawLoopedLines(rect, (float)videoClip.length / subClipSpeed, this.length, clipOffset);
            }
        }

#endif
        ///----------------------------------------------------------------------------------------------


    }
}

#endif