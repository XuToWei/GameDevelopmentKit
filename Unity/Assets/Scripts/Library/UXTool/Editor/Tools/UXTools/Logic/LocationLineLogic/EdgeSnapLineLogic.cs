#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using ThunderFireUnityEx;

namespace ThunderFireUITool
{
    /// <summary>
    /// 处理边缘吸附
    /// </summary>
    public class EdgeSnapLineLogic : UXSingleton<EdgeSnapLineLogic>
    {
        private VisualLineManager m_VisualManager;
        private GameObject m_SelectedObject;
        private List<Rect> m_Rects;

        private struct Rect
        {
            /// <summary>
            /// 长度为6，上中下左中右
            /// </summary>
            public float[] pos;

            public Rect(RectTransform trans)
            {
                pos = new float[6];
                pos[0] = (float)Math.Round((double)trans.GetTopWorldPosition(), 1);
                pos[2] = (float)Math.Round((double)trans.GetBottomWorldPosition(), 1);
                pos[3] = (float)Math.Round((double)trans.GetLeftWorldPosition(), 1);
                pos[5] = (float)Math.Round((double)trans.GetRightWorldPosition(), 1);
                pos[4] = (float)Math.Round((double)trans.position.x, 1);
                pos[1] = (float)Math.Round((double)trans.position.y, 1);
            }
        }

        public override void Init()
        {
            m_VisualManager = new VisualLineManager();
            m_VisualManager.Init();
            m_Rects = new List<Rect>();

            ResetAll();

            EditorApplication.hierarchyChanged += ResetAll;
            Selection.selectionChanged += ResetAll;
            EditorApplication.update += ListenMoving;
        }
        public void InitAfter()
        {
            EditorApplication.update += SnapToFinalPos;
        }

        public void CloseBefore()
        {
            EditorApplication.update -= SnapToFinalPos;
        }
        public override void Close()
        {
            EditorApplication.hierarchyChanged -= ResetAll;
            Selection.selectionChanged -= ResetAll;
            EditorApplication.update -= ListenMoving;

            m_VisualManager?.RemoveAll();
            m_VisualManager = null;
            Instance.Release();
        }

        private void ResetAll()
        {
            m_VisualManager.RemoveAll();
            if (Selection.gameObjects.Length == 1 && EditorLogic.ObjectFit(Selection.activeGameObject))
            {
                m_SelectedObject = Selection.activeGameObject;
                m_SelectedObject.transform.hasChanged = false;
            }
            else
            {
                m_SelectedObject = null;
                return;
            }

            m_Rects.Clear();
            RectTransform[] allObjects;
            var prefabStage = PrefabStageUtils.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                allObjects = prefabStage.prefabContentsRoot.GetComponentsInChildren<RectTransform>();
            }
            else
            {
                allObjects = UnityEngine.Object.FindObjectsOfType<RectTransform>();
            }
            foreach (RectTransform item in allObjects)
            {
                if (EditorLogic.ObjectFit(item.gameObject) && item.gameObject != m_SelectedObject && !item.IsChildOf(m_SelectedObject.transform))
                {
                    m_Rects.Add(new Rect(item));
                }
            }
        }

        /// <summary>
        /// 核心逻辑
        /// </summary>
        /// <param name="eps">eps=0表示吸附最终位置（需要画提示线）</param>
        private void FindEdges(float eps)
        {
            if (eps == 0)
            {
                m_VisualManager.RemoveAll();
            }
            Rect objRect = new Rect(m_SelectedObject.GetComponent<RectTransform>());

            foreach (Rect rect in m_Rects)
            {
                if (eps != 0)
                {
                    float dis = Mathf.Infinity;
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (Mathf.Abs(dis) > Mathf.Abs(rect.pos[i] - objRect.pos[j]))
                            {
                                dis = rect.pos[i] - objRect.pos[j];
                            }
                        }
                    }
                    if (Mathf.Abs(dis) < eps && Mathf.Abs(dis) < Mathf.Abs(SnapLogic.SnapEdgeDisVert))
                    {
                        SnapLogic.SnapEdgeDisVert = dis;
                    }
                    dis = Mathf.Infinity;
                    for (int i = 3; i < 6; i++)
                    {
                        for (int j = 3; j < 6; j++)
                        {
                            if (Mathf.Abs(dis) > Mathf.Abs(rect.pos[i] - objRect.pos[j]))
                            {
                                dis = rect.pos[i] - objRect.pos[j];
                            }
                        }
                    }
                    if (Mathf.Abs(dis) < eps && Mathf.Abs(dis) < Mathf.Abs(SnapLogic.SnapEdgeDisHoriz))
                    {
                        SnapLogic.SnapEdgeDisHoriz = dis;
                    }
                }
                else
                {
                    float minX = Math.Min(rect.pos[3], objRect.pos[3]);
                    float maxX = Math.Max(rect.pos[5], objRect.pos[5]);
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (rect.pos[i] == objRect.pos[j])
                            {
                                m_VisualManager.AddHorizLine(minX, maxX, rect.pos[i], false);
                            }
                        }
                    }
                    float minY = Math.Min(rect.pos[2], objRect.pos[2]);
                    float maxY = Math.Max(rect.pos[0], objRect.pos[0]);
                    for (int i = 3; i < 6; i++)
                    {
                        for (int j = 3; j < 6; j++)
                        {
                            if (rect.pos[i] == objRect.pos[j])
                            {
                                m_VisualManager.AddVertLine(rect.pos[i], minY, maxY, false);
                            }
                        }
                    }
                }
            }
        }

        private void ListenMoving()
        {
            if (m_SelectedObject != null && m_SelectedObject.GetComponent<RectTransform>().position != SnapLogic.ObjFinalPos && LocationLineLogic.Instance.EnableSnap)
            {
                SnapLogic.SnapEdgeDisHoriz = SnapLogic.SnapEdgeDisVert = Mathf.Infinity;
                FindEdges(SnapLogic.SnapWorldDistance);
            }
        }

        private void SnapToFinalPos()
        {
            if (m_SelectedObject == null) return;
            RectTransform rectTransform = m_SelectedObject.GetComponent<RectTransform>();
            if (rectTransform == null) return;
            if (rectTransform.position != SnapLogic.ObjFinalPos && LocationLineLogic.Instance.EnableSnap)
            {
                Vector3 vec = rectTransform.position;
                if (Math.Abs(SnapLogic.SnapEdgeDisHoriz) <= Math.Abs(SnapLogic.SnapIntervalDisHoriz) &&
                Math.Abs(SnapLogic.SnapEdgeDisHoriz) < Math.Abs(SnapLogic.SnapLineDisHoriz))
                {
                    vec.x += SnapLogic.SnapEdgeDisHoriz;
                }
                if (Math.Abs(SnapLogic.SnapEdgeDisVert) <= Math.Abs(SnapLogic.SnapIntervalDisVert) &&
                Math.Abs(SnapLogic.SnapEdgeDisVert) < Math.Abs(SnapLogic.SnapLineDisVert))
                {
                    vec.y += SnapLogic.SnapEdgeDisVert;
                }
                rectTransform.position = vec;
                FindEdges(0);
            }
        }
    }
}
#endif