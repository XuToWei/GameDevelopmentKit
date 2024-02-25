#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using UnityEngine.UIElements;
using System;

namespace ThunderFireUITool
{
    /// <summary>
    /// Snap相关的逻辑和全局变量
    /// </summary>
    public class SnapLogic
    {
        //屏幕空间吸附距离，后续可以添加到设置中之类
        public static float SnapSceneDistance = 8f;
        /// <summary>
        /// 间距吸附的可接受误差
        /// </summary>
        private static float SnapEps = 0.5f;
        /// <summary>
        /// 当前帧物体最终吸附的位置
        /// </summary>
        public static Vector3 ObjFinalPos;
        /// <summary>
        /// 表示本次EditorApplication.update吸附到的辅助线距离
        /// Vert代表竖直，Horiz代表水平
        /// </summary>
        public static float SnapLineDisVert, SnapLineDisHoriz;
        /// <summary>
        /// 表示本次EditorApplication.update吸附到的边缘距离
        /// Vert代表竖直，Horiz代表水平
        /// </summary>
        public static float SnapEdgeDisVert, SnapEdgeDisHoriz;
        /// <summary>
        /// 表示本次EditorApplication.update吸附到的Interval距离
        /// Vert代表竖直，Horiz代表水平
        /// </summary>
        public static float SnapIntervalDisVert, SnapIntervalDisHoriz;
        //TODO 可以改成只在滚轮滚动之后更新几次
        public static float SnapWorldDistance
        {
            get
            {
                SceneView sceneView = SceneView.lastActiveSceneView;
                Vector3 v1 = sceneView.camera.ScreenToWorldPoint(new Vector3(SnapSceneDistance, 0, 0));
                Vector3 v2 = sceneView.camera.ScreenToWorldPoint(new Vector3(0, 0, 0));
                return Mathf.Abs(v1.x - v2.x);
            }
        }
        public static float SnapEpsDistance
        {
            get
            {
                SceneView sceneView = SceneView.lastActiveSceneView;
                Vector3 v1 = sceneView.camera.ScreenToWorldPoint(new Vector3(SnapEps, 0, 0));
                Vector3 v2 = sceneView.camera.ScreenToWorldPoint(new Vector3(0, 0, 0));
                return Mathf.Abs(v1.x - v2.x);
            }
        }
    }

}
#endif