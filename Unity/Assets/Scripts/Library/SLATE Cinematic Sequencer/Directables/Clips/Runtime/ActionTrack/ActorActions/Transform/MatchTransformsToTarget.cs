using UnityEngine;
using System.Collections;

namespace Slate.ActionClips
{

    [Category("Transform")]
    [Description("Smoothly match the selected transforms of the actor and to the target for a period of time and then back again to their original values. If you don't want to smooth back to the original values, set BlendOut to 0.")]
    public class MatchTransformsToTarget : ActorActionClip
    {

        [SerializeField]
        [HideInInspector]
        private float _length = 2f;
        [SerializeField]
        [HideInInspector]
        private float _blendIn = 0.8f;
        [SerializeField]
        [HideInInspector]
        private float _blendOut = 0.8f;

        [Required]
        public Transform targetObject;
        public EaseType interpolation = EaseType.QuadraticInOut;

        public bool matchPosition = true;
        public Vector3 positionOffset;
        public bool matchRotation = true;
        public Vector3 rotationOffset;
        public bool matchScale = false;
        public Vector3 scaleOffset;

        private Vector3 lastPos;
        private Quaternion lastRot;
        private Vector3 lastScale;

        public override string info {
            get { return "Match Transforms\n" + ( targetObject ? targetObject.name : "NONE" ); }
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

        public override bool isValid {
            get { return targetObject != null; }
        }

        protected override void OnEnter() {
            lastPos = actor.transform.position;
            lastRot = actor.transform.rotation;
            lastScale = actor.transform.localScale;
        }

        protected override void OnUpdate(float time) {

            if ( matchPosition ) {
                var targetPos = targetObject.transform.TransformPoint(positionOffset);
                if ( length > 0 ) {
                    actor.transform.position = Easing.Ease(interpolation, lastPos, targetPos, GetClipWeight(time));
                } else {
                    actor.transform.position = targetPos;
                }
            }

            if ( matchRotation ) {
                var targetRot = targetObject.rotation * Quaternion.Euler(rotationOffset);
                if ( length > 0 ) {
                    actor.transform.rotation = Easing.Ease(interpolation, lastRot, targetRot, GetClipWeight(time));
                } else {
                    actor.transform.rotation = targetRot;
                }
            }

            if ( matchScale ) {
                if ( length > 0 ) {
                    actor.transform.localScale = Easing.Ease(interpolation, lastScale, targetObject.localScale + scaleOffset, GetClipWeight(time));
                } else {
                    actor.transform.localScale = targetObject.localScale + scaleOffset;
                }
            }
        }

        protected override void OnReverse() {
            actor.transform.position = lastPos;
            actor.transform.rotation = lastRot;
            actor.transform.localScale = lastScale;
        }
    }
}