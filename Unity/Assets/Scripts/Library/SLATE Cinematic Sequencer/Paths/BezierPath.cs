/* Inspired by Bezier Curve Editor by Arkham Interactive */

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Slate
{

    ///<summary>A Path defined out of Bezier Curves</summary>
    [AddComponentMenu("SLATE/Path")]
    public class BezierPath : Path
    {

        [RangeAttribute(1, 100)]
        public int resolution = 30;
        [SerializeField]
        private List<BezierPoint> _points = new List<BezierPoint>();
        private Vector3[] _sampledPathPoints;
        private float _length;

        public List<BezierPoint> points => _points;
        public BezierPoint this[int index] => points[index];
        public int pointCount => points.Count;
        public override float length => _length;

        ///----------------------------------------------------------------------------------------------

        void Awake() { Compute(); }
        void OnValidate() { Compute(); }
        public override void Compute() {
            ComputeSampledPathPoints();
            ComputeLength();
        }

        public void SetDirty() {
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "Path Change");
#endif

            Compute();

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        void ComputeLength() {
            _length = GetLength(_sampledPathPoints);
        }

        void ComputeSampledPathPoints() {
            if ( points.Count == 0 ) {
                _sampledPathPoints = new Vector3[0];
                return;
            }

            var result = new List<Vector3>();
            for ( int i = 0; i < points.Count - 1; i++ ) {
                var current = points[i];
                var next = points[i + 1];
                result.AddRange(GetSampledPathPositions(current, next, resolution));
            }

            _sampledPathPoints = result.ToArray();
        }

        ///<summary>Create a new BezierPath object</summary>
        public static BezierPath Create(Transform targetParent = null) {
            var rootName = "[ PATHS ]";
            GameObject root = null;
            if ( targetParent == null ) {
                root = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().FirstOrDefault(x => x.name == rootName);
                if ( root == null ) {
                    root = new GameObject(rootName);
                }
            } else {
                var child = targetParent.Find(rootName);
                if ( child != null ) {
                    root = child.gameObject;
                } else {
                    root = new GameObject(rootName);
                }
            }
            root.transform.SetParent(targetParent, false);

            var path = new GameObject("Path").AddComponent<BezierPath>();
            path.transform.SetParent(root.transform, false);
            path.transform.localPosition = Vector3.zero;
            path.transform.localRotation = Quaternion.identity;
            return path;
        }

        ///<summary>Add a new bezier point at index.</summary>
        public BezierPoint AddPointAt(Vector3 position, int index = -1) {
            var newPoint = new BezierPoint(this, position);
            if ( index == -1 ) {
                points.Add(newPoint);
            } else {
                points.Insert(index, newPoint);
            }
            SetDirty();
            return newPoint;
        }

        ///<summary>Remove a bezier point.</summary>
        public void RemovePoint(BezierPoint point) {
            points.Remove(point);
            SetDirty();
        }

        ///<summary>Get a bezier point index.</summary>
        public int GetPointIndex(BezierPoint point) {
            for ( int i = 0; i < points.Count; i++ ) {
                if ( points[i] == point ) { return i; }
            }
            return -1;
        }

        ///<summary>Get interpolated Vector3 positions between two control points with specified resolution</summary>
        public static Vector3[] GetSampledPathPositions(BezierPoint p1, BezierPoint p2, int resolution) {
            var result = new List<Vector3>();
            var limit = resolution + 1;
            float _res = resolution;

            for ( int i = 0; i < limit; i++ ) {
                var currentPoint = GetPositionAt(p1, p2, i / _res);
                result.Add(currentPoint);
            }

            return result.ToArray();
        }

        ///<summary>Get the position along the path at normalized t.</summary>
        public override Vector3 GetPositionAt(float t) {
            if ( t <= 0 ) return points[0].position;
            if ( t >= 1 ) return points[points.Count - 1].position;
            return GetPosition(t, _sampledPathPoints);
        }

        ///<summary>Get the position between two  points at normalized t.</summary>
        public static Vector3 GetPositionAt(BezierPoint p1, BezierPoint p2, float t) {
            return GetPositionAlongCurve(p1.position, p2.position, p1.handle2LocalPosition, p2.handle1LocalPosition, t);
        }

        ///----------------------------------------------------------------------------------------------

        //EDITOR
#if UNITY_EDITOR
        void Reset() {
            var p1 = AddPointAt(transform.position + new Vector3(-3, 0, 0));
            p1.handle1Position = new Vector3(-3, 0, -1);
            var p2 = AddPointAt(transform.position + new Vector3(3, 0, 0));
            p2.handle1Position = new Vector3(3, 0, 1);
            SetDirty();
        }

        //...
        void OnDrawGizmos() {
            if ( points.Count > 1 ) {
                for ( int i = 0; i < points.Count - 1; i++ ) {
                    Gizmos.color = Prefs.motionPathsColor;
                    DrawPath(points[i], points[i + 1], resolution);
                    Gizmos.color = Prefs.gizmosColor;
                    DrawVerticalGuide(points[i].position);
                }

                DrawVerticalGuide(points[points.Count - 1].position);
            }

            Gizmos.color = Color.white;
        }

        ///<summary>Draw vertical guides</summary>
        static void DrawVerticalGuide(Vector3 position) {
            var hit = new RaycastHit();
            Vector3 hitPos = new Vector3(position.x, 0, position.z);
            if ( Physics.Linecast(position, position - new Vector3(0, 100, 0), out hit) ) {
                hitPos = hit.point;
            }

            Gizmos.DrawLine(position, hitPos);
        }

        ///<summary>Draw the path</summary>
        static void DrawPath(BezierPoint p1, BezierPoint p2, int resolution) {
            int limit = resolution + 1;
            float _res = resolution;
            var lastPoint = p1.position;
            var currentPoint = Vector3.zero;

            for ( int i = 1; i < limit; i++ ) {
                currentPoint = GetPositionAt(p1, p2, i / _res);
                Gizmos.DrawLine(lastPoint, currentPoint);
                lastPoint = currentPoint;
            }
        }
#endif

    }
}