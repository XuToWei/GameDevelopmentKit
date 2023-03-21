// Copyright (C) 2018 KAMGAM e.U. - All rights reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace kamgam.editor.smartuiselection
{
    public static class SmartUiSelection_CanvasGizmo
    {
        [DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.NonSelected)]
        static void DrawGizmo(Canvas canvas, GizmoType gizmoType)
        {
            bool executeInPlayModeCheck = !EditorApplication.isPlaying || (SmartUiSelection_Settings.autoHideDuringPlayback && EditorApplication.isPlaying);
            if (SmartUiSelection_Settings.enableAutoHide && SmartUiSelection_Settings.showAutoHideWarningGizmo && executeInPlayModeCheck)
            {
                if (canvas.renderMode == RenderMode.ScreenSpaceOverlay
                    && canvas.isRootCanvas)
                {
                    Vector3[] rectCorners = new Vector3[4];
                    (canvas.transform as RectTransform).GetWorldCorners(rectCorners);

                    string distance = string.Format( "({0:0}/{1:0})", SmartUiSelection_AutoHide.GetCamToXYPlaneDistance(), SmartUiSelection_Settings.autoHideDistanceThreshold );

#if UNITY_2019_2_OR_NEWER
                    if (SmartUiSelection_AutoHide.IsVisibilityActiveViaReflection() == false)
                    {
                        string text = "SmartUiSelection: The SceneVisibility is turned OFF. The canvases will NOT be hidden. Please turn the SceneVisibility ON (the eye icon).";
                        var style = GUIStyle.none;
                        style.normal.textColor = Color.black;
                        Handles.Label(moveVectorXY(rectCorners[0], 0, 0), text, style);
                        style = GUIStyle.none;
                        style.normal.textColor = Color.red;
                        Handles.Label(moveVectorXY(rectCorners[0], -0.1f, 0.1f), text, style);
                    }
                    else
                    {
#endif
                        if (SmartUiSelection_AutoHide.GetCamToXYPlaneDistance() <= SmartUiSelection_Settings.autoHideDistanceThreshold)
                        {
                            string text = distance + " SmartUiSelection: Canvas WILL be hidden if the mouse enters the 'Scene' view.";
                            var style = GUIStyle.none;
                            style.normal.textColor = Color.black;
                            Handles.Label(moveVectorXY(rectCorners[0], 0, 0), text, style);
                            style = GUIStyle.none;
                            style.normal.textColor = SmartUiSelection_Settings.autoHideWarningTextColor;
                            Handles.Label(moveVectorXY(rectCorners[0], -0.1f, 0.1f), text, style);
                        }
                        else
                        {
                            string text = "SmartUiSelection: Canvas will not be hidden because the distance is above the threshold " + distance + ".";
                            var style = GUIStyle.none;
                            style.normal.textColor = Color.black;
                            Handles.Label(moveVectorXY(rectCorners[0], 0, 0), text, style);
                            style = GUIStyle.none;
                            style.normal.textColor = Color.white;
                            Handles.Label(moveVectorXY(rectCorners[0], -0.1f, 0.1f), text, style);
                        }

                        /*
                        float margin = 0.2f;
                        Gizmos.color = Color.red;
                        Gizmos.DrawLine(moveVectorXY(rectCorners[0], -margin, -margin), moveVectorXY(rectCorners[1], -margin, margin));
                        Gizmos.DrawLine(moveVectorXY(rectCorners[1], -margin, margin), moveVectorXY(rectCorners[2], margin, margin));
                        Gizmos.DrawLine(moveVectorXY(rectCorners[2], margin, margin), moveVectorXY(rectCorners[3], margin, -margin));
                        Gizmos.DrawLine(moveVectorXY(rectCorners[3], margin, -margin), moveVectorXY(rectCorners[0], -margin, -margin));
                        */
#if UNITY_2019_2_OR_NEWER
                    }
#endif
                }
            }
        }

        static Vector3 moveVectorXY(Vector3 vector, float x, float y)
        {
            vector.x += x;
            vector.y += y;
            return vector;
        }
    }
}

#endif