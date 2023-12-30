using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Slate
{

    ///<summary>Creates a snapshot of an object to be restored later on.</summary>
    public class ObjectSnapshot
    {

        ///----------------------------------------------------------------------------------------------
        private static List<System.Type> ignoreTypes = new List<System.Type>();
        ///<summary>Allows appending types to be ignored by ObjectSnapshot</summary>
        public static void AppendIgnoreType(System.Type type) {
            if ( !ignoreTypes.Contains(type) ) { ignoreTypes.Add(type); }
        }
        ///----------------------------------------------------------------------------------------------

        private Dictionary<Object, string> serialized;

        ///<summary>Create new ObjectSnapShot storing target object</summary>
        public ObjectSnapshot(Object target, bool fullObjectHierarchy = false) {
            Store(target, fullObjectHierarchy);
        }

        ///<summary>Store target state</summary>
        public void Store(Object target, bool fullObjectHierarchy = false) {

            if ( target == null ) {
                return;
            }

            serialized = new Dictionary<Object, string>();

            if ( target is MonoBehaviour || target is ScriptableObject ) {
                if ( !ignoreTypes.Contains(target.GetType()) ) {
                    serialized[target] = JsonUtility.ToJson(target);
                }
            }

            if ( target is GameObject ) {
                var go = (GameObject)target;
                var components = fullObjectHierarchy ? go.GetComponentsInChildren<Component>(true) : go.GetComponents<Component>();
                for ( var i = 0; i < components.Length; i++ ) {
                    var component = components[i];
                    if ( component is MonoBehaviour ) {
                        if ( !ignoreTypes.Contains(target.GetType()) ) {
                            serialized[component] = JsonUtility.ToJson(component);
                        }
                    }
                }
            }
        }

        ///<summary>Restore previously stored state</summary>
        public void Restore() {
            foreach ( var pair in serialized ) {
                if ( pair.Key != null ) {
                    if ( pair.Key is MonoBehaviour || pair.Key is ScriptableObject ) {
                        if ( !ignoreTypes.Contains(pair.Key.GetType()) ) {
                            JsonUtility.FromJsonOverwrite(pair.Value, pair.Key);
                        }
                    }
                }
            }
        }

    }
}