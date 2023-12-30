using UnityEngine;
using System.Reflection;
using System.Linq;

namespace Slate.ActionClips
{

    [Description("Animate any number of properties on any component of the actor, or within it's hierarchy.\nYou can use the clip's Blend In/Out to optionally smooth blend from current property values over to the keyframed ones and back.")]
    [Attachable(typeof(ActorActionTrack), typeof(DirectorActionTrack))]
    public class AnimateProperties : ActionClip
    {

        [SerializeField]
        [HideInInspector]
        private float _length = 5f;

        [SerializeField]
        [HideInInspector]
        private float _blendIn;
        [SerializeField]
        [HideInInspector]
        private float _blendOut;

        [SerializeField]
        protected string _name;

        public EaseType interpolation = EaseType.QuadraticInOut;


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


        public override bool isValid { //valid when there is at least 1 parameter added.
            get { return base.animationData != null && base.animationData.isValid; }
        }

        public override string info {
            get { return isValid ? ( string.IsNullOrEmpty(_name) ? base.animationData.ToString() : _name ) : "No Properties Added"; }
        }

        //By default the target is the actionclip instance. In this case, the target is the actor.
        //This also makes the clip eligable for manual parameters registration which is done here.
        public override object animatedParametersTarget {
            get { return actor; }
        }

        //The interpolation to use for the animated paramters blend in and out.
        public override EaseType animatedParametersInterpolation {
            get { return interpolation; }
        }

        //We want the clip weight to automatically be used in parameters.
        public override bool useWeightInParameters {
            get { return true; }
        }


        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        protected override void OnSceneGUI() {

            if ( !isValid ) {
                return;
            }

            for ( var i = 0; i < animationData.animatedParameters.Count; i++ ) {
                var animParam = animationData.animatedParameters[i];
                if ( animParam.parameterName == "localPosition" ) {
                    var transform = animParam.ResolvedMemberObject() as Transform;
                    if ( transform != null ) {
                        var context = transform.parent != null ? transform.parent : GetSpaceTransform(TransformSpace.CutsceneSpace);
                        CurveEditor3D.Draw3DCurve(animParam, this, context, length / 2, length);
                    }
                }
            }
        }

#endif

    }
}