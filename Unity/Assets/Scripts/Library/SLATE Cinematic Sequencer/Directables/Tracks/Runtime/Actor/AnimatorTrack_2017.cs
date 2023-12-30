#if UNITY_2017_1_OR_NEWER

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Slate.ActionClips;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Slate
{

    [Description("The Animator Track works with an 'Animator' Component attached on the actor, but does not require or use the Controller assigned. Instead animation clips can be played directly.\n\nMultiple Animator Tracks can also be added each representing a different animation layer. Root Motion will only be used in Animator Track of Layer 0 (if enabled), which is always the first (bottom) Track.")]
    [Icon(typeof(Animator))]
    [Attachable(typeof(ActorGroup))]
    partial class AnimatorTrack
    {

        const int ROOTMOTION_FRAMERATE = 30;

        public AvatarMask mask;
        public AnimationBlendMode blendMode;
        [Range(0, 1)]
        public float weight = 1f;
        [ShowIf("isMasterTrack", 1)]
        public bool useRootMotion = true;
        [ShowIf("isMasterTrack", 1)]
        public bool applyFootIK = false;

        [SerializeField, HideInInspector]
        public bool isRootMotionPreBaked;
        [SerializeField, HideInInspector]
        private List<Vector3> rmPositions;
        [SerializeField, HideInInspector]
        private List<Quaternion> rmRotations;

        private int activeClips;
        private float compountClipsWeight;

        private Dictionary<PlayAnimatorClip, int> ports;
        private PlayableGraph graph;
        private AnimationPlayableOutput animationOutput;
        private AnimationLayerMixerPlayable masterLayerMixer;
        private AnimationMixerPlayable clipsMixer;
        private AnimatorControllerPlayable animatorPlayable;
        private List<AnimatorTrack> siblingTracks;

        private bool wasRootMotion;
        private AnimatorCullingMode wasCullingMode;
        private bool useBakedRootMotion;

        ///----------------------------------------------------------------------------------------------

        private Animator _animator;
        public Animator animator {
            get
            {
                if ( _animator == null || _animator.gameObject != actor.gameObject ) {
                    _animator = actor.GetComponentInChildren<Animator>();
                }
                return _animator;
            }
        }

        public override string info {
            get
            {
                var info = string.Format("Layer: {0} | Mask: {1} | {2}", layerOrder.ToString(), mask != null ? mask.name : "None", blendMode);
                if ( isMasterTrack ) {
                    info += ( useRootMotion ? " | Use RM" : " | No RM" );
                }
                return info;
            }
        }

        public override bool isLocked => base.isLocked || ( isRootMotionPreBaked && isMasterTrack );
        public bool isMasterTrack => layerOrder == 0;
        public bool isLastTrack => layerOrder == siblingTracks.Count - 1;
        private AnimatorTrack masterTrack => siblingTracks[0];

        //...
        protected override bool OnInitialize() {
            if ( animator == null ) {
                Debug.LogError("Animator Track requires that the actor has the Animator Component attached.", actor);
                return false;
            }
            siblingTracks = parent.children.OfType<AnimatorTrack>().Where(t => t.isActive).Reverse().ToList();
            return true;
        }

        //...
        protected override void OnEnter() {
            activeClips = 0;
            if ( isMasterTrack ) {
                wasCullingMode = animator.cullingMode;
                animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                wasRootMotion = animator.applyRootMotion;
                animator.applyRootMotion = false;

                var wasActive = animator.gameObject.activeSelf;
                animator.gameObject.SetActive(true);
                CreateAndPlayTree();
                if ( useRootMotion ) {
                    BakeRootMotion();
                }
                animator.gameObject.SetActive(wasActive);
            }
            activeClips = 0;
        }

        //...
        protected override void OnUpdate(float time, float previousTime) {
            if ( animator == null || !animator.gameObject.activeInHierarchy || !masterTrack.graph.IsValid() ) { return; }
            if ( isLastTrack ) { masterTrack.PostUpdateMasterTrack(time, previousTime); }
        }

        void PostUpdateMasterTrack(float time, float previousTime) {
            // if ( time >= endTime ) { return; }

            for ( var i = 0; i < siblingTracks.Count; i++ ) {
                var siblingCompountClipsWeight = siblingTracks[i].compountClipsWeight * siblingTracks[i].weight;
                masterLayerMixer.SetInputWeight(i + 1, siblingCompountClipsWeight);
            }

            graph.Evaluate(time - previousTime);

            if ( useRootMotion && useBakedRootMotion ) {
                ApplyBakedRootMotion(time);
            }
        }

        //...
        protected override void OnReverseEnter() {
            activeClips = 0;
            if ( isMasterTrack ) {
                wasCullingMode = animator.cullingMode;
                animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                wasRootMotion = animator.applyRootMotion;
                animator.applyRootMotion = false;

                CreateAndPlayTree();
                //DO NOT Re-Bake root motion
            }
        }

        //...
        protected override void OnExit() {
            if ( isLastTrack ) { masterTrack.PostExitMasterTrack(); }
        }

        void PostExitMasterTrack() {
            animator.applyRootMotion = wasRootMotion;
            animator.cullingMode = wasCullingMode;
            if ( graph.IsValid() ) {
                graph.Destroy();
            }

            if ( useRootMotion ) {
                ApplyBakedRootMotion(endTime - startTime);
            }
        }

        //...
        protected override void OnReverse() {
            if ( isMasterTrack ) {
                animator.applyRootMotion = wasRootMotion;
                animator.cullingMode = wasCullingMode;
                if ( graph.IsValid() ) {
                    graph.Destroy();
                }

                if ( useRootMotion ) {
                    ApplyBakedRootMotion(0);
                }
            }
        }

        ///----------------------------------------------------------------------------------------------

        //...
        public void EnableClip(PlayAnimatorClip playAnimClip, float clipWeight) {
            activeClips++;
            var index = ports[playAnimClip];
            clipsMixer.SetInputWeight(index, activeClips == 1 ? 1 : clipWeight);
            if ( activeClips == 1 ) compountClipsWeight = clipWeight;
            if ( activeClips >= 2 ) compountClipsWeight = 1;
        }

        //...
        public void UpdateClip(PlayAnimatorClip playAnimClip, float clipTime, float clipPrevious, float clipWeight) {
            var index = ports[playAnimClip];
            var clipPlayable = clipsMixer.GetInput(index);
            clipPlayable.SetTime(clipTime);
            clipsMixer.SetInputWeight(index, activeClips == 1 ? 1 : clipWeight);
            if ( activeClips == 0 ) compountClipsWeight = 0;
            if ( activeClips == 1 ) compountClipsWeight = clipWeight;
            if ( activeClips >= 2 ) compountClipsWeight = 1;
        }

        //...
        public void DisableClip(PlayAnimatorClip playAnimClip, float clipWeight) {
            activeClips--;
            var index = ports[playAnimClip];
            clipsMixer.SetInputWeight(index, 0f);
            if ( activeClips == 0 ) compountClipsWeight = 0;
        }

        ///----------------------------------------------------------------------------------------------

        //Create and play the playable graph
        void CreateAndPlayTree() {
            graph = PlayableGraph.Create();
            animationOutput = AnimationPlayableOutput.Create(graph, "Animation", animator);
            masterLayerMixer = AnimationLayerMixerPlayable.Create(graph, siblingTracks.Count + 1);

            animatorPlayable = AnimatorControllerPlayable.Create(graph, animator.runtimeAnimatorController);
            graph.Connect(animatorPlayable, 0, masterLayerMixer, 0);
            masterLayerMixer.SetInputWeight(0, 1f);

            for ( var i = 0; i < siblingTracks.Count; i++ ) {
                var animatorTrack = siblingTracks[i];
                var clipsMixer = animatorTrack.CreateClipsMixer(graph);
                var targetLayerMixInput = i + 1; //0 is animator
                graph.Connect(clipsMixer, 0, masterLayerMixer, targetLayerMixInput);
                masterLayerMixer.SetInputWeight(targetLayerMixInput, 1f);
                if ( animatorTrack.mask != null ) {
                    masterLayerMixer.SetLayerMaskFromAvatarMask((uint)targetLayerMixInput, animatorTrack.mask);
                }
                masterLayerMixer.SetLayerAdditive((uint)targetLayerMixInput, animatorTrack.blendMode == AnimationBlendMode.Additive);
            }

            animationOutput.SetSourcePlayable(masterLayerMixer);

            // GraphVisualizerClient.Show(graph, this.name);
        }

        //Create and return the track mixer playable
        Playable CreateClipsMixer(PlayableGraph graph) {
            var clipActions = clips.OfType<PlayAnimatorClip>().ToList();
            ports = new Dictionary<PlayAnimatorClip, int>();
            clipsMixer = AnimationMixerPlayable.Create(graph, clipActions.Count);
            for ( var i = 0; i < clipActions.Count; i++ ) {
                var playAnimClip = clipActions[i];
                var clipPlayable = AnimationClipPlayable.Create(graph, playAnimClip.animationClip);
                clipPlayable.SetApplyFootIK(applyFootIK && isMasterTrack);
                graph.Connect(clipPlayable, 0, clipsMixer, i);
                clipsMixer.SetInputWeight(i, 0f);
                ports[playAnimClip] = i;
            }

            //need to pause clips mixer.
            clipsMixer.Pause();
            return clipsMixer;
        }

        ///----------------------------------------------------------------------------------------------

        public void PreBakeRootMotion() {
            root.Sample(float.Epsilon);
            root.Sample(0);
            isRootMotionPreBaked = true;
        }

        public void ClearPreBakeRootMotion() {
            root.Sample(0);
            rmPositions = new List<Vector3>();
            rmRotations = new List<Quaternion>();
            isRootMotionPreBaked = false;
        }

        //The root motion must be baked if required.
        void BakeRootMotion() {

            if ( isRootMotionPreBaked ) {
                animator.applyRootMotion = false;
                useBakedRootMotion = true;
                return;
            }

            var rb = animator.GetComponent<Rigidbody>();
            if ( rb != null ) {
                rb.MovePosition(animator.transform.localPosition);
                rb.MoveRotation(animator.transform.localRotation);
            }

            useBakedRootMotion = false;
            animator.applyRootMotion = true;
            rmPositions = new List<Vector3>();
            rmRotations = new List<Quaternion>();
            var updateInterval = ( 1f / ROOTMOTION_FRAMERATE );
            for ( var time = startTime - updateInterval; time <= endTime + updateInterval; time += updateInterval ) {

                EvaluateTrackClips(time, time - updateInterval);

                for ( var i = 0; i < siblingTracks.Count; i++ ) {
                    var siblingCompountClipsWeight = siblingTracks[i].compountClipsWeight * siblingTracks[i].weight;
                    masterLayerMixer.SetInputWeight(i + 1, siblingCompountClipsWeight);
                }

                if ( activeClips > 0 ) {
                    graph.Evaluate(updateInterval);
                }

                //apparently animator automatically sets rigidbody pos/rot if attached on same go when evaluated.
                //thus we read pos/rot from rigidbody in such cases.
                var pos = rb != null ? rb.position : animator.transform.localPosition;
                var rot = rb != null ? rb.rotation : animator.transform.localRotation;
                rmPositions.Add(pos);
                rmRotations.Add(rot);
            }
            animator.applyRootMotion = false;
            useBakedRootMotion = true;
        }

        //Evaluate the track clips
        void EvaluateTrackClips(float time, float previousTime) {
            foreach ( var clip in ( this as IDirectable ).children ) {
                if ( time >= clip.startTime && previousTime < clip.startTime ) {
                    clip.Enter();
                }

                if ( time >= clip.startTime && time <= clip.endTime ) {
                    clip.Update(time - clip.startTime, previousTime - clip.startTime);
                }

                if ( time >= clip.endTime && previousTime < clip.endTime ) {
                    clip.Exit();
                }
            }
        }

        //Apply baked root motion by lerping between stored frames.
        void ApplyBakedRootMotion(float time) {
            var frame = Mathf.FloorToInt(time * ROOTMOTION_FRAMERATE);
            var nextFrame = frame + 1;
            nextFrame = nextFrame < rmPositions.Count ? nextFrame : rmPositions.Count - 1;

            var tNow = frame * ( 1f / ROOTMOTION_FRAMERATE );
            var tNext = nextFrame * ( 1f / ROOTMOTION_FRAMERATE );

            var posNow = rmPositions[frame];
            var posNext = rmPositions[nextFrame];

            var snap = Vector3.Distance(posNow, posNext) > 1;
            animator.transform.localPosition = snap ? posNext : Vector3.Lerp(posNow, posNext, Mathf.InverseLerp(tNow, tNext, time));

            var rotNow = rmRotations[frame];
            var rotNext = rmRotations[nextFrame];
            animator.transform.localRotation = snap ? rotNext : Quaternion.Lerp(rotNow, rotNext, Mathf.InverseLerp(tNow, tNext, time));
        }
    }
}

#endif
