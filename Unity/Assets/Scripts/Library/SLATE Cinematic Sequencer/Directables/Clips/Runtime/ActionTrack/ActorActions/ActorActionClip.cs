using UnityEngine;
using System.Collections;

namespace Slate.ActionClips
{

    [Attachable(typeof(ActorActionTrack))]
    abstract public class ActorActionClip : ActionClip { }

    [Attachable(typeof(ActorActionTrack))]
    ///<summary>The .actor property of the generic ActorActionClip version, returns the T argument component directly</summary>
    abstract public class ActorActionClip<T> : ActionClip where T : Component
    {

        [System.NonSerialized]
        private T _actorComponent;
        new public T actor {
            get
            {
                if ( _actorComponent != null ) {
                    return _actorComponent;
                }
                return _actorComponent = base.actor != null ? base.actor.GetComponentInChildren<T>() : null;
                /*
                if ( _actorComponent != null && _actorComponent.gameObject == base.actor ) {
                    return _actorComponent;
                }
                return _actorComponent = base.actor != null ? base.actor.GetComponent<T>() : null;
                */
            }
        }

        public override bool isValid {
            get { return actor != null; }
        }
    }
}