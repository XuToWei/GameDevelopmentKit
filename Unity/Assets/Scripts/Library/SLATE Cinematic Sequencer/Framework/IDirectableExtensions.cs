using System;
using System.Reflection;
using System.Linq;
using UnityEngine;

namespace Slate
{

    public static class IDirectableExtensions
    {

        ///<summary>The length of the directable</summary>
        public static float GetLength(this IDirectable directable) {
            return directable.endTime - directable.startTime;
        }

        ///<summary>The local time of the directable based on root current time</summary>
        public static float RootTimeToLocalTime(this IDirectable directable) {
            return directable.ToLocalTime(directable.root.currentTime);
        }

        ///<summary>The local time of the directable based on root current time</summary>
        public static float RootTimeToLocalTimeUnclamped(this IDirectable directable) {
            return directable.ToLocalTimeUnclamped(directable.root.currentTime);
        }

        ///<summary>Is root current time within directable range?</summary>
        public static bool IsRootTimeWithinClip(this IDirectable directable) {
            return IsTimeWithinClip(directable, directable.root.currentTime);
        }

        ///<summary>The local time of the directable based on provided time</summary>
        public static float ToLocalTime(this IDirectable directable, float time) {
            return Mathf.Clamp(time - directable.startTime, 0, directable.GetLength());
        }

        ///<summary>The local time of the directable based on provided time</summary>
        public static float ToLocalTimeUnclamped(this IDirectable directable, float time) {
            return time - directable.startTime;
        }

        ///<summary>Is time within directable range?</summary>
        public static bool IsTimeWithinClip(this IDirectable directable, float time) {
            return time >= directable.startTime && time <= directable.endTime && time > 0;
        }

        ///<summary>Should two provided directables be able to cross-blend?</summary>
        public static bool CanCrossBlend(this IDirectable directable, IDirectable other) {
            if ( directable == null || other == null ) { return false; }
            if ( ( directable.canCrossBlend || other.canCrossBlend ) && directable.GetType() == other.GetType() ) {
                return true;
            }
            return false;
        }

        ///<summary>Does the directable has real blend in ability?</summary>
        public static bool CanBlendIn(this IDirectable directable) {
            var blendInProp = directable.GetType().GetProperty("blendIn", BindingFlags.Instance | BindingFlags.Public);
            return blendInProp != null && blendInProp.CanWrite && directable.blendIn != -1 && blendInProp.DeclaringType != typeof(ActionClip);
        }

        ///<summary>Does the directable has real blend out ability?</summary>
        public static bool CanBlendOut(this IDirectable directable) {
            var blendOutProp = directable.GetType().GetProperty("blendOut", BindingFlags.Instance | BindingFlags.Public);
            return blendOutProp != null && blendOutProp.CanWrite && directable.blendOut != -1 && blendOutProp.DeclaringType != typeof(ActionClip);
        }

        ///<summary>Can the directable scale (adjust length)?</summary>
        public static bool CanScale(this IDirectable directable) {
            var lengthProp = directable.GetType().GetProperty("length", BindingFlags.Instance | BindingFlags.Public);
            return lengthProp != null && lengthProp.CanWrite && lengthProp.DeclaringType != typeof(ActionClip);
        }

        ///----------------------------------------------------------------------------------------------

        ///<summary>Utility to check if delta (time - previous time) are close enough to trigger something that should only trigger when they are</summary>
        public static bool WithinBufferTriggerRange(this IDirectable directable, float time, float previousTime, bool bypass = false) {
            if ( directable.root.isReSampleFrame ) { return false; }
            return ( time - previousTime ) <= ( 0.1f * directable.root.playbackSpeed ) || bypass;
        }

        ///----------------------------------------------------------------------------------------------

        ///<summary>Returns the first child directable of provided name</summary>
        public static IDirectable FindChild(this IDirectable directable, string name) {
            if ( directable.children == null ) { return null; }
            return directable.children.FirstOrDefault(d => d.name.ToLower() == name.ToLower());
        }

        ///<summary>Returns the previous sibling in the parent (eg previous clip)</summary>
        public static T GetPreviousSibling<T>(this IDirectable directable) where T : IDirectable { return (T)GetPreviousSibling(directable); }
        public static IDirectable GetPreviousSibling(this IDirectable directable) {
            if ( directable.parent != null ) {
                return directable.parent.children.LastOrDefault(d => d != directable && d.startTime < directable.startTime);
            }
            return null;
        }

        ///<summary>Returns the next sibling in the parent (eg next clip)</summary>
        public static T GetNextSibling<T>(this IDirectable directable) where T : IDirectable { return (T)GetNextSibling(directable); }
        public static IDirectable GetNextSibling(this IDirectable directable) {
            if ( directable.parent != null ) {
                return directable.parent.children.FirstOrDefault(d => d != directable && d.startTime > directable.startTime);
            }
            return null;
        }

        ///<summary>Going upwards, returns the first parent of type T</summary>
        public static T GetFirstParentOfType<T>(this IDirectable directable) where T : IDirectable {
            var current = directable.parent;
            while ( current != null ) {
                if ( current is T ) {
                    return (T)current;
                }
                current = current.parent;
            }
            return default(T);
        }

        ///----------------------------------------------------------------------------------------------

        ///<summary>The current weight based on blend properties and based on root current time.</summary>
        public static float GetWeight(this IDirectable directable) { return GetWeight(directable, RootTimeToLocalTime(directable)); }
        ///<summary>The weight at specified local time based on its blend properties.</summary>
        public static float GetWeight(this IDirectable directable, float time) { return GetWeight(directable, time, directable.blendIn, directable.blendOut); }
        ///<summary>The weight at specified local time based on provided override blend in/out properties</summary>
        public static float GetWeight(this IDirectable directable, float time, float blendInOut) { return GetWeight(directable, time, blendInOut, blendInOut); }
        public static float GetWeight(this IDirectable directable, float time, float blendIn, float blendOut) {
            var length = GetLength(directable);
            if ( time <= 0 ) {
                return blendIn <= 0 ? 1 : 0;
            }

            if ( time >= length ) {
                return blendOut <= 0 ? 1 : 0;
            }

            if ( time < blendIn ) {
                return time / blendIn;
            }

            if ( time > length - blendOut ) {
                return ( length - time ) / blendOut;
            }

            return 1;
        }

        ///----------------------------------------------------------------------------------------------

        ///<summary>Returns the transform object used for specified Space transformations. Null if World Space.</summary>
        public static Transform GetSpaceTransform(this IDirectable directable, TransformSpace space, GameObject actorOverride = null) {

            if ( space == TransformSpace.CutsceneSpace ) {
                return directable.root?.context?.transform;
            }

            var actor = actorOverride != null ? actorOverride : directable.actor;

            if ( actor != null ) {
                if ( space == TransformSpace.ActorSpace ) {
                    return actor != null ? actor.transform : null;
                }

                if ( space == TransformSpace.ParentSpace ) {
                    return actor != null ? actor.transform.parent : null;
                }
            }

            return null; //world space
        }

        ///<summary>Transforms a point in specified space</summary>
        public static Vector3 TransformPosition(this IDirectable directable, Vector3 point, TransformSpace space) {
            var t = directable.GetSpaceTransform(space);
            return t != null ? t.TransformPoint(point) : point;
        }

        ///<summary>Inverse Transforms a point in specified space</summary>
        public static Vector3 InverseTransformPosition(this IDirectable directable, Vector3 point, TransformSpace space) {
            var t = directable.GetSpaceTransform(space);
            return t != null ? t.InverseTransformPoint(point) : point;
        }

        public static Quaternion TransformRotation(this IDirectable directable, Vector3 euler, TransformSpace space) {
            var t = directable.GetSpaceTransform(space);
            if ( t != null ) { return t.rotation * Quaternion.Euler(euler); }
            return Quaternion.Euler(euler);
        }

        public static Vector3 InverseTransformRotation(this IDirectable directable, Quaternion rot, TransformSpace space) {
            var t = directable.GetSpaceTransform(space);
            if ( t != null ) { return ( Quaternion.Inverse(t.rotation) * rot ).eulerAngles; }
            return rot.eulerAngles;
        }

        ///<summary>Returns the final actor position in specified Space (InverseTransform Space)</summary>
        public static Vector3 ActorPositionInSpace(this IDirectable directable, TransformSpace space) {
            return directable.actor != null ? directable.InverseTransformPosition(directable.actor.transform.position, space) : directable.root.context.transform.position;
        }

        ///----------------------------------------------------------------------------------------------

        ///<summary>Returns the previous local time (clip length) of a loop.</summary>
        public static float GetPreviousLoopLocalTime(this ISubClipContainable clip) {
            var clipLength = clip.GetLength();
            var loopLength = clip.subClipLength / clip.subClipSpeed;
            if ( clipLength > loopLength ) {
                var mod = ( clipLength - clip.subClipOffset ) % loopLength;
                var aproxZero = Mathf.Abs(mod) < 0.01f;
                return clipLength - ( aproxZero ? loopLength : mod );
            }
            return clipLength;
        }

        ///<summary>Returns the next local time (clip length) of a loop.</summary>
        public static float GetNextLoopLocalTime(this ISubClipContainable clip) {
            var clipLength = clip.GetLength();
            var loopLength = clip.subClipLength / clip.subClipSpeed;
            var mod = ( clipLength - clip.subClipOffset ) % loopLength;
            var aproxZero = Mathf.Abs(mod) < 0.01f || Mathf.Abs(loopLength - mod) < 0.01f;
            return clipLength + ( aproxZero ? loopLength : ( loopLength - mod ) );
        }

        ///----------------------------------------------------------------------------------------------

#if SLATE_USE_EXPRESSIONS

		public static StagPoint.Eval.Environment CreateExpressionEnvironment(this IDirector director){
			var env = Slate.Expressions.GlobalEnvironment.Get().Push();
			Slate.Expressions.ExpressionsUtility.Wrap(director, env);
			return env;			
		}

		public static StagPoint.Eval.Environment GetExpressionEnvironment(this IDirectable directable){
			var env = directable.root.CreateExpressionEnvironment().Push();
			Slate.Expressions.ExpressionsUtility.Wrap(directable, env);
			return env;
		}

#endif

        ///----------------------------------------------------------------------------------------------

        ///<summary>Returns AnimationCurves of ALL (enabled and disabled) animated parameters stored in animationData of keyable.</summary>
        public static AnimationCurve[] GetCurvesAll(this IKeyable keyable) {
            if ( keyable.animationData != null && keyable.animationData.isValid ) {
                return keyable.animationData.GetCurvesAll();
            }
            return new AnimationCurve[0];
        }

#if UNITY_EDITOR

        ///<summary>Try add key at time in keyable animation data</summary>
        public static void TryAddIdentityKey(this IKeyable keyable, float time) {
            if ( keyable != null && keyable.animationData != null && keyable.animationData.isValid ) {
                keyable.animationData.TryKeyIdentity(time);
            }
        }

        ///<summary>Try record keys</summary>
        public static bool TryAutoKey(this IKeyable keyable, float time) {

            if ( Application.isPlaying || keyable.root.isReSampleFrame ) {
                return false;
            }

            if ( !Prefs.autoKey || GUIUtility.hotControl != 0 ) {
                return false;
            }

            var case1 = ReferenceEquals(CutsceneUtility.selectedObject, keyable);
            var activeTransform = UnityEditor.Selection.activeTransform;
            var actor = keyable.actor;
            var case2 = actor != null && activeTransform != null && activeTransform.IsChildOf(actor.transform);
            if ( case1 || case2 ) { return keyable.animationData.TryAutoKey(time); }
            return false;
        }

        ///<summary>Try add parameter in animation data of keyable</summary>
        public static bool TryAddParameter(this IKeyable keyable, Type type, string memberPath, string transformPath) {
            return keyable.animationData.TryAddParameter(keyable, type, memberPath, transformPath);
        }
#endif

    }
}