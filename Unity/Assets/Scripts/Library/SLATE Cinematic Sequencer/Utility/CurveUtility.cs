#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Slate
{

    public enum TangentMode
    {
        Editable,
        Smooth,
        Linear,
        Constant,
        ClampedAuto,
    }

    //Wraps Unity's AnimationUtility class a bit better like exposing -for reasons unknown- internal methods.
    public static class CurveUtility
    {

        const float KEY_PROXIMITY_TOLERANCE = 0.001f;

#if UNITY_EDITOR

        private static Action<AnimationCurve> UpdateTangentsFromModeDelegate;
        private static Func<Keyframe, AnimationUtility.TangentMode> GetKeyLeftTangentModeDelegate;
        private static Func<Keyframe, AnimationUtility.TangentMode> GetKeyRightTangentModeDelegate;
        private static Func<Keyframe, bool> GetKeyBrokenDelegate;

        //init
        static CurveUtility() {
            var flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            MethodInfo method = null;

            method = typeof(AnimationUtility).GetMethod("UpdateTangentsFromMode", flags);
            UpdateTangentsFromModeDelegate = method.RTCreateDelegate<Action<AnimationCurve>>(null);

            method = typeof(AnimationUtility).GetMethod("GetKeyLeftTangentMode", flags, null, new Type[] { typeof(Keyframe) }, null);
            GetKeyLeftTangentModeDelegate = method.RTCreateDelegate<Func<Keyframe, AnimationUtility.TangentMode>>(null);
            method = typeof(AnimationUtility).GetMethod("GetKeyRightTangentMode", flags, null, new Type[] { typeof(Keyframe) }, null);
            GetKeyRightTangentModeDelegate = method.RTCreateDelegate<Func<Keyframe, AnimationUtility.TangentMode>>(null);
            method = typeof(AnimationUtility).GetMethod("GetKeyBroken", flags, null, new Type[] { typeof(Keyframe) }, null);
            GetKeyBrokenDelegate = method.RTCreateDelegate<Func<Keyframe, bool>>(null);
        }

        //...
        public static void UpdateTangentsFromMode(this AnimationCurve curve) {
            UpdateTangentsFromModeDelegate(curve);
            if ( curve.length > 1 ) {
                var firstKey = curve[0];
                if ( GetKeyTangentMode(firstKey) == TangentMode.Smooth ) {
                    firstKey.inTangent = 0;
                    firstKey.outTangent = 0;
                    curve.MoveKey(0, firstKey);
                }
                var lastKey = curve[curve.length - 1];
                if ( GetKeyTangentMode(lastKey) == TangentMode.Smooth ) {
                    lastKey.inTangent = 0;
                    lastKey.outTangent = 0;
                    curve.MoveKey(curve.length - 1, lastKey);
                }
            }
        }

        //...
        public static void SetKeyTangentMode(this AnimationCurve curve, int index, TangentMode tangentMode) {
            SetKeyLeftTangentMode(curve, index, tangentMode);
            SetKeyRightTangentMode(curve, index, tangentMode);
            SetKeyBroken(curve, index, false);
        }

        //...
        public static void SetKeyLeftTangentMode(AnimationCurve curve, int index, TangentMode tangentMode) {
            AnimationUtility.SetKeyLeftTangentMode(curve, index, (AnimationUtility.TangentMode)tangentMode);
        }

        //...
        public static void SetKeyRightTangentMode(AnimationCurve curve, int index, TangentMode tangentMode) {
            AnimationUtility.SetKeyRightTangentMode(curve, index, (AnimationUtility.TangentMode)tangentMode);
        }

        //...
        public static TangentMode GetKeyTangentMode(Keyframe keyframe) {
            var leftTangent = GetKeyLeftTangentMode(keyframe);
            var rightTangent = GetKeyRightTangentMode(keyframe);
            if ( leftTangent == rightTangent ) {
                return leftTangent;
            }
            return TangentMode.Editable;
        }

        //...
        public static TangentMode GetKeyLeftTangentMode(Keyframe keyframe) {
            return (TangentMode)GetKeyLeftTangentModeDelegate(keyframe);
        }

        //...
        public static TangentMode GetKeyRightTangentMode(Keyframe keyframe) {
            return (TangentMode)GetKeyRightTangentModeDelegate(keyframe);
        }

        //...
        public static Keyframe SetKeyBroken(this AnimationCurve curve, int index, bool broken) {
            AnimationUtility.SetKeyBroken(curve, index, broken);
            return curve[index];
        }

        //...
        public static bool GetKeyBroken(Keyframe keyframe) {
            return GetKeyBrokenDelegate(keyframe);
        }

        ///----------------------------------------------------------------------------------------------

        ///<summary>Add key at time/value and set tangent mode from neighbors</summary>
        public static bool AddKey(AnimationCurve curve, float time, float value, TangentMode mode) {
            time = Mathf.Max(time, 0);
            var keys = curve.keys;
            for ( var i = 0; i < keys.Length; i++ ) {
                if ( Mathf.Abs(keys[i].time - time) < KEY_PROXIMITY_TOLERANCE ) {
                    var key = keys[i];
                    key.time = time;
                    key.value = value;
                    curve.MoveKey(i, key);
                    curve.UpdateTangentsFromMode();
                    return false;
                }
            }

            var index = curve.AddKey(time, value);
            if ( mode == TangentMode.Constant ) {
                curve.SetKeyTangentMode(index, TangentMode.Constant);
                curve.UpdateTangentsFromMode();
                return true;
            }

            //if it's the first key added and preference is set to specific mode
            if ( curve.length == 1 && mode != TangentMode.Editable ) {
                curve.SetKeyTangentMode(index, mode);
                curve.UpdateTangentsFromMode();
                return true;
            }

            //else set mode from neighbors
            var nextIndex = index + 1;
            if ( nextIndex < curve.length ) {
                var nextTangent = GetKeyLeftTangentMode(curve[nextIndex]);
                SetKeyTangentMode(curve, index, nextTangent);
            }

            var previousIndex = index - 1;
            if ( previousIndex >= 0 ) {
                var previousTangent = GetKeyRightTangentMode(curve[previousIndex]);
                SetKeyTangentMode(curve, index, previousTangent);
            }

            curve.UpdateTangentsFromMode();
            return true;
        }


        ///<summary>Remove keys at time in all curves</summary>
        public static void RemoveKeys(float time, params AnimationCurve[] curves) {
            foreach ( var curve in curves ) {
                var keys = curve.keys;
                for ( var i = 0; i < keys.Length; i++ ) {
                    var key = keys[i];
                    if ( Mathf.Abs(key.time - time) < KEY_PROXIMITY_TOLERANCE ) {
                        curve.RemoveKey(i);
                        break;
                    }
                }

                curve.UpdateTangentsFromMode();
            }
        }

#endif


        ///----------------------------------------------------------------------------------------------
        ///----------------------------------------------------------------------------------------------


        ///<summary>Returns the key time after time, or first key if time is last key time.</summary>
        public static float GetKeyNext(float time, params AnimationCurve[] curves) {
            var keys = new List<Keyframe>();
            foreach ( var curve in curves ) {
                keys.AddRange(curve.keys);
            }
            return keys.OrderBy(k => k.time).FirstOrDefault(k => k.time > time + KEY_PROXIMITY_TOLERANCE).time;
        }

        ///<summary>Returns the key time before time, or last key if time is first key time.</summary>
        public static float GetKeyPrevious(float time, params AnimationCurve[] curves) {
            var keys = new List<Keyframe>();
            foreach ( var curve in curves ) {
                keys.AddRange(curve.keys);
            }
            keys = keys.OrderBy(k => k.time).ToList();
            return keys.LastOrDefault(k => k.time < time - KEY_PROXIMITY_TOLERANCE).time;
        }

        ///<summary>Does any key exists at time within curves?</summary>
        public static bool HasKey(float time, params AnimationCurve[] curves) {
            for ( var i = 0; i < curves.Length; i++ ) {
                if ( curves[i].keys.Any(k => Mathf.Abs(k.time - time) < KEY_PROXIMITY_TOLERANCE) ) {
                    return true;
                }
            }
            return false;
        }

        ///<summary>Are there any keys at all?</summary>
        public static bool HasAnyKey(params AnimationCurve[] curves) {
            for ( var i = 0; i < curves.Length; i++ ) {
                if ( curves[i].length > 0 ) {
                    return true;
                }
            }
            return false;
        }

        ///<summary>Offset all curve key values by delta value</summary>
        public static void OffsetCurveValue(this AnimationCurve curve, float deltaValue) {
            for ( var i = 0; i < curve.length; i++ ) {
                var key = curve[i];
                key.value += deltaValue;
                curve.MoveKey(i, key);
            }
        }

        ///<summary>Offset all curve key times by delta time</summary>
        public static void OffsetCurveTime(this AnimationCurve curve, float deltaTime) {
            var finalKeys = new Keyframe[curve.length];
            for ( var i = 0; i < curve.length; i++ ) {
                var key = curve[i];
                key.time += deltaTime;
                finalKeys[i] = key;
            }
            curve.keys = finalKeys;
        }

        ///<summary>Remove all keys of negative time (< 0)</summary>
        public static void RemoveNegativeKeys(this AnimationCurve curve) {
            curve.RemoveKeysOffRange(0, float.PositiveInfinity);
        }

        ///<summary>Remove all keys outside of min max range</summary>
        public static void RemoveKeysOffRange(this AnimationCurve curve, float min, float max) {
            var finalKeys = new List<Keyframe>();
            for ( var i = 0; i < curve.length; i++ ) {
                var key = curve[i];
                if ( key.time >= min && key.time <= max ) {
                    finalKeys.Add(key);
                }
            }
            curve.keys = finalKeys.ToArray();
        }

        ///----------------------------------------------------------------------------------------------
    }
}