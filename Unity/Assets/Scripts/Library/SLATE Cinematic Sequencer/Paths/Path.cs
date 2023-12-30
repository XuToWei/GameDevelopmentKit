using UnityEngine;

namespace Slate
{

    ///<summary>An abstract base type for all Path types</summary>
    abstract public class Path : MonoBehaviour
    {

        abstract public float length { get; }
        abstract public Vector3 GetPositionAt(float t);
        abstract public void Compute();

        ///<summary>Get position on curve from, to, by t</summary>
        public static Vector3 GetPositionAlongCurve(Vector3 from, Vector3 to, Vector3 fromTangent, Vector3 toTangent, float t) {
            float u = 1.0f - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;
            Vector3 result = uuu * from;
            result += 3 * uu * t * ( from + fromTangent );
            result += 3 * u * tt * ( to + toTangent );
            result += ttt * to;
            return result;
        }

        public static Vector3 GetPosition(float t, params Vector3[] path) {
            if ( t <= 0 ) return path[0];
            if ( t >= 1 ) return path[path.Length - 1];
            var a = Vector3.zero;
            var b = Vector3.zero;
            var total = 0f;
            var segmentDistance = 0f;
            var pathLength = GetLength(path);
            for ( var i = 0; i < path.Length - 1; i++ ) {
                segmentDistance = Vector3.Distance(path[i], path[i + 1]) / pathLength;
                if ( total + segmentDistance > t ) {
                    a = path[i];
                    b = path[i + 1];
                    break;
                } else {
                    total += segmentDistance;
                }
            }
            t -= total;
            return Vector3.Lerp(a, b, t / segmentDistance);
        }

        public static float GetLength(Vector3[] path) {
            if ( path == null ) { return 0; }
            var dist = 0f;
            for ( var i = 0; i < path.Length; i++ ) {
                dist += Vector3.Distance(path[i], path[i == path.Length - 1 ? i : i + 1]);
            }
            return dist;
        }
    }
}