using UnityEngine;
using System.Collections;

namespace Slate.ActionClips
{

    [Category("Transform")]
    [Description("Grounds the actor gameobject to the nearest collider object within max check distance.")]
    public class SimpleGrounder : ActorActionClip
    {

        [SerializeField, HideInInspector]
        private float _length = 1;

        [Range(1, 100)]
        public float maxCheckDistance = 3;
        [Min(0)]
        public float offset = 0.01f;

        private Vector3 lastPos;

        public override float length {
            get { return _length; }
            set { _length = value; }
        }

        protected override void OnEnter() {
            lastPos = actor.transform.position;
        }

        protected override void OnUpdate(float time) {
            var pos = actor.transform.position + new Vector3(0, maxCheckDistance, 0);
            var hits = Physics.RaycastAll(new Ray(pos, Vector3.down), maxCheckDistance * 2);
            var bestPointY = 0f;
            var bestDistance = float.PositiveInfinity;
            for ( var i = 0; i < hits.Length; i++ ) {
                var hit = hits[i];
                if ( hit.distance <= maxCheckDistance * 2 && hit.transform != actor.transform ) {
                    if ( hit.distance < bestDistance ) {
                        bestDistance = hit.distance;
                        bestPointY = hit.point.y;
                    }
                }
            }

            if ( bestDistance != float.PositiveInfinity ) {
                pos.y = bestPointY + offset;
                actor.transform.position = pos;
            }
        }

        protected override void OnReverse() {
            actor.transform.position = lastPos;
        }
    }
}