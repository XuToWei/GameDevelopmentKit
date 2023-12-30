/* Inspired by Bezier Curve Editor by Arkham Interactive */

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Slate
{

    [CustomEditor(typeof(BezierPath))]
    public class BezierPathEditor : Editor
    {
        private BezierPoint selectedPoint;
        private BezierPath path { get { return (BezierPath)target; } }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            if ( GUILayout.Button("Add Point") ) {
                var last = path.points.LastOrDefault();
                if ( last != null ) {
                    path.AddPointAt(last.position + Vector3.one);
                } else {
                    path.AddPointAt(path.transform.position + Vector3.one);
                }
                SceneView.RepaintAll();
            }
        }

        void OnSceneGUI() {
            for ( int i = 0; i < path.pointCount; i++ ) {
                DrawPointSceneGUI(path[i], i);
            }
        }

        void DrawPointSceneGUI(BezierPoint point, int index) {
            Handles.BeginGUI();
            var e = Event.current;

            Handles.Label(point.position + new Vector3(0, HandleUtility.GetHandleSize(point.position) * 0.4f, 0), index.ToString());
            Handles.color = Prefs.gizmosColor;
            var pointGUISize = HandleUtility.GetHandleSize(point.position) * 0.1f;
            if ( e.type == EventType.MouseDown ) {

                var screenPos = HandleUtility.WorldToGUIPoint(point.position);
                var nextPoint = index < path.points.Count - 1 ? path[index + 1] : null;
                var previousPoint = index > 0 ? path[index - 1] : null;

                if ( ( (Vector2)screenPos - e.mousePosition ).magnitude < 13 ) {

                    if ( e.button == 0 ) {
                        selectedPoint = point;
                        SceneView.RepaintAll();
                    }

                    if ( e.button == 1 ) {
                        path.SetDirty();
                        var menu = new GenericMenu();
                        if ( nextPoint != null ) {
                            menu.AddItem(new GUIContent("Add Point After"), false, () => { path.AddPointAt(BezierPath.GetPositionAt(point, nextPoint, 0.5f), index + 1); });
                        }
                        if ( previousPoint != null ) {
                            menu.AddItem(new GUIContent("Add Point Before"), false, () => { path.AddPointAt(BezierPath.GetPositionAt(previousPoint, point, 0.5f), index); });
                        }
                        menu.AddItem(new GUIContent("Tangent/Connected"), false, () => { point.handleStyle = BezierPoint.HandleStyle.Connected; });
                        menu.AddItem(new GUIContent("Tangent/Broken"), false, () => { point.handleStyle = BezierPoint.HandleStyle.Broken; });
                        if ( path.points.Count > 2 ) {
                            menu.AddSeparator("/");
                            menu.AddItem(new GUIContent("Delete"), false, () => { path.RemovePoint(point); });
                        }
                        menu.ShowAsContext();
                        e.Use();
                    }
                }
            }

            var newPosition = point.position;
            if ( point == selectedPoint ) {
                newPosition = Handles.PositionHandle(point.position, Quaternion.identity);
            }
            var fmh_82_52_638395659275204169 = Quaternion.identity; Handles.FreeMoveHandle(point.position, pointGUISize, Vector3.zero, Handles.RectangleHandleCap);

            if ( newPosition != point.position ) {
                point.position = newPosition;
                path.SetDirty();
            }


            var fmh_90_76_638395659275241463 = Quaternion.identity; var newGlobal1 = Handles.FreeMoveHandle(point.handle1Position, HandleUtility.GetHandleSize(point.handle1Position) * 0.075f, Vector3.zero, Handles.CircleHandleCap);
            if ( selectedPoint == point ) { newGlobal1 = Handles.PositionHandle(newGlobal1, Quaternion.identity); }

            if ( point.handle1Position != newGlobal1 ) {
                point.handle1Position = newGlobal1;
                path.SetDirty();
            }

            var fmh_98_76_638395659275246382 = Quaternion.identity; var newGlobal2 = Handles.FreeMoveHandle(point.handle2Position, HandleUtility.GetHandleSize(point.handle2Position) * 0.075f, Vector3.zero, Handles.CircleHandleCap);
            if ( selectedPoint == point ) { newGlobal2 = Handles.PositionHandle(newGlobal2, Quaternion.identity); }

            if ( point.handle2Position != newGlobal2 ) {
                point.handle2Position = newGlobal2;
                path.SetDirty();
            }

            Handles.DrawLine(point.position, point.handle1Position);
            Handles.DrawLine(point.position, point.handle2Position);

            Handles.EndGUI();
            Handles.color = Color.white;
        }
    }
}

#endif