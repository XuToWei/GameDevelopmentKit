using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Slate
{

    [System.Serializable]
    ///<summary>A wrapped collection of Animated Parameters. Basically what AnimationClip is to Unity, but for Slate.</summary>
    public class AnimationDataCollection : IAnimatableData
    {

        public delegate bool AddParameterDelegate(System.Type type, string memberPath, string transformPath);

        [SerializeField]
        private List<AnimatedParameter> _animatedParameters;
        public List<AnimatedParameter> animatedParameters {
            get { return _animatedParameters; }
        }

        ///<summary>are there any parameters?</summary>
        public bool isValid {
            get { return animatedParameters != null && animatedParameters.Count > 0; }
        }

        ///<summary>indexer by index in list</summary>
        public AnimatedParameter this[int i] {
            get { return animatedParameters != null && i < animatedParameters.Count ? animatedParameters[i] : null; }
        }

        ///<summary>indexer by name of parameter</summary>
        public AnimatedParameter this[string name] {
            get { return GetParameterOfName(name); }
        }

        public AnimationDataCollection() { }
        public AnimationDataCollection(IKeyable keyable, System.Type type, string[] memberPaths, string transformPath) {
            foreach ( var memberPath in memberPaths ) {
                TryAddParameter(keyable, type, memberPath, transformPath);
            }
        }

        ///<summary>Creates a new animated parameter out of a member info that optionaly exists on a component in child transform of root transform.</summary>
        public bool TryAddParameter(IKeyable keyable, System.Type type, string memberPath, string transformPath) {

            if ( animatedParameters == null ) {
                _animatedParameters = new List<AnimatedParameter>();
            }

            var newParam = new AnimatedParameter(keyable, type, memberPath, transformPath);
            if ( !newParam.isValid ) {
                return false;
            }

            var found = _animatedParameters.Find(p => p.CompareTo(newParam));
            if ( found != null ) {
                //handle possible changes from property to field and vice-verse
                if ( found.parameterType != newParam.parameterType ) {
                    found.ChangeMemberType(newParam.parameterType);
                }
                return false;
            }

            _animatedParameters.Add(newParam);
            if ( newParam.isExternal ) { ReOrderParameters(); }
            return true;
        }

        ///<summary>Remove a parameter from the collection</summary>
        public void RemoveParameter(AnimatedParameter animParam) {
            _animatedParameters.Remove(animParam);
        }

        ///<summary>Re-orders the parameters based on paths</summary>
        public void ReOrderParameters() {
            _animatedParameters = animatedParameters.OrderBy(p => p.ToString()).OrderBy(p => p.transformHierarchyPath).ToList();
        }

        ///<summary>Fetch a parameter with specified name</summary>
        public AnimatedParameter GetParameterOfName(string name) {
            if ( animatedParameters == null ) {
                return null;
            }
            return _animatedParameters.Find(d => d.parameterName.ToLower() == name.ToLower());
        }

        ///<summary>Get all parameter animation curves</summary>
        public AnimationCurve[] GetCurves() { return Internal_GetCurves(true); }
        public AnimationCurve[] GetCurvesAll() { return Internal_GetCurves(false); }
        AnimationCurve[] Internal_GetCurves(bool enabledParamsOnly) {

            if ( animatedParameters == null ) {
                return new AnimationCurve[0];
            }

            var result = new List<AnimationCurve>();
            for ( var i = 0; i < animatedParameters.Count; i++ ) {
                if ( !enabledParamsOnly || animatedParameters[i].enabled ) {
                    var curves = animatedParameters[i].GetCurves();
                    if ( curves != null ) {
                        result.AddRange(curves);
                    }
                }
            }
            return result.ToArray();
        }


        ///<summary>0. Validate the parameters within the context of a keyable reference</summary>
        public void Validate(IKeyable keyable) {
            if ( animatedParameters != null ) {
                for ( var i = 0; i < animatedParameters.Count; i++ ) {
                    animatedParameters[i].Validate(keyable);
                }
            }
        }

        ///<summary>1. If a virtualTransformParent is set, transforms will be virtually parented to that tranform</summary>
        public void SetVirtualTransformParent(Transform virtualTransformParent) {
            if ( animatedParameters != null ) {
                for ( var i = 0; i < animatedParameters.Count; i++ ) {
                    animatedParameters[i].SetVirtualTransformParent(virtualTransformParent);
                }
            }
        }

        ///<summary>2. Set snapshot of current value</summary>
        public void SetSnapshot() {
            if ( animatedParameters != null ) {
                for ( var i = 0; i < animatedParameters.Count; i++ ) {
                    animatedParameters[i].SetSnapshot();
                }
            }
        }

        ///<summary>3. Will key all parameters that have their value changed</summary>
        public bool TryAutoKey(float time) {
            if ( animatedParameters != null ) {
                var anyKeyAdded = false;
                for ( var i = 0; i < animatedParameters.Count; i++ ) {
                    if ( animatedParameters[i].TryAutoKey(time) ) {
                        anyKeyAdded = true;
                    }
                }

                return anyKeyAdded;
            }

            return false;
        }

        ///<summary>4. Evaluate parameters</summary>
        public void Evaluate(float time, float previousTime, float weight = 1) {
            if ( animatedParameters != null ) {
                for ( var i = 0; i < animatedParameters.Count; i++ ) {
                    animatedParameters[i].Evaluate(time, previousTime, weight);
                }
            }
        }

        ///<summary>5. Restore stored snapshot</summary>
        public void RestoreSnapshot() {
            if ( animatedParameters != null ) {
                for ( var i = 0; i < animatedParameters.Count; i++ ) {
                    animatedParameters[i].RestoreSnapshot();
                }
            }
        }


        ///<summary>Try add key at time, with identity value either from existing curves at that time, or in case of no curves from current property value.</summary>
        public bool TryKeyIdentity(float time) {
            if ( animatedParameters != null ) {
                var anyKeyAdded = false;
                for ( var i = 0; i < animatedParameters.Count; i++ ) {
                    if ( animatedParameters[i].TryKeyIdentity(time) ) {
                        anyKeyAdded = true;
                    }
                }

                return anyKeyAdded;
            }

            return false;
        }

        ///<summary>Remove keys at time</summary>
        public void RemoveKey(float time) {
            if ( animatedParameters != null ) {
                for ( var i = 0; i < animatedParameters.Count; i++ ) {
                    animatedParameters[i].RemoveKey(time);
                }
            }
        }

        ///<summary>Is any parameter in this collection changed?</summary>
        public bool HasChanged() {
            if ( animatedParameters != null ) {
                for ( var i = 0; i < animatedParameters.Count; i++ ) {
                    if ( animatedParameters[i].HasChanged() ) {
                        return true;
                    }
                }
            }

            return false;
        }

        ///<summary>Is there any key at time?</summary>
        public bool HasKey(float time) {
            if ( animatedParameters != null ) {
                for ( var i = 0; i < animatedParameters.Count; i++ ) {
                    if ( animatedParameters[i].HasKey(time) ) {
                        return true;
                    }
                }
            }

            return false;
        }

        ///<summary>Are there any keys at all?</summary>
        public bool HasAnyKey() {
            if ( animatedParameters != null ) {
                for ( var i = 0; i < animatedParameters.Count; i++ ) {
                    if ( animatedParameters[i].HasAnyKey() ) {
                        return true;
                    }
                }
            }

            return false;
        }

        ///<summary>Set key in all parameters at current value</summary>
        public void SetKeyCurrent(float time) {
            if ( animatedParameters != null ) {
                for ( var i = 0; i < animatedParameters.Count; i++ ) {
                    animatedParameters[i].SetKeyCurrent(time);
                }
            }
        }


        ///<summary>The next key time after time</summary>
        public float GetKeyNext(float time) {
            if ( animatedParameters != null ) {
                return animatedParameters.Select(p => p.GetKeyNext(time)).OrderBy(t => t).FirstOrDefault(t => t > time);
            }
            return 0;
        }

        ///<summary>The previous key time before time</summary>
        public float GetKeyPrevious(float time) {
            if ( animatedParameters != null ) {
                return animatedParameters.Select(p => p.GetKeyPrevious(time)).OrderBy(t => t).LastOrDefault(t => t < time);
            }
            return 0;
        }

        ///<summary>A value label at time</summary>
        public string GetKeyLabel(float time) {
            if ( animatedParameters != null ) {
                if ( animatedParameters.Count == 1 ) {
                    return animatedParameters[0].GetKeyLabel(time);
                }
                return string.Format("[#{0}]", animatedParameters.Where(p => p.HasKey(time)).ToArray().Length);
            }
            return string.Empty;
        }

        ///<summary>Set all parameters Pre Wrap Mode</summary>
        public void SetPreWrapMode(WrapMode mode) {
            if ( animatedParameters != null ) {
                for ( var i = 0; i < animatedParameters.Count; i++ ) {
                    animatedParameters[i].SetPreWrapMode(mode);
                }
            }
        }

        ///<summary>Set all parameters Post Wrap Mode</summary>
        public void SetPostWrapMode(WrapMode mode) {
            if ( animatedParameters != null ) {
                for ( var i = 0; i < animatedParameters.Count; i++ ) {
                    animatedParameters[i].SetPostWrapMode(mode);
                }
            }
        }

        ///<summary>Reset all animated parameters</summary>
        public void Reset() {
            if ( animatedParameters != null ) {
                for ( var i = 0; i < animatedParameters.Count; i++ ) {
                    animatedParameters[i].Reset();
                }
            }
        }

        ///<summary>...</summary>
        public override string ToString() {
            if ( animatedParameters == null || animatedParameters.Count == 0 ) {
                return "No Parameters";
            }

            return animatedParameters.Count == 1 ? animatedParameters[0].ToString() : "Multiple Parameters";
        }
    }
}