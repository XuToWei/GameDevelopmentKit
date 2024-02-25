#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using ThunderFireUnityEx;

namespace ThunderFireUITool
{
    /// <summary>
    /// 处理间距吸附
    /// </summary>
    public class IntervalLineLogic : UXSingleton<IntervalLineLogic>
    {
        private VisualLineManager m_VisualManager;
        private GameObject m_SelectedObject;
        /// <summary>
        /// 记录所有物体的上下左右边界值
        /// </summary>
        private List<Rect> m_Rects;
        /// <summary>
        /// 记录所有Tag
        /// </summary>
        private TagList m_Tags;

        /// <summary>
        /// 类似于RectTransform，写了一个方便排序的版本
        /// </summary>
        private struct Rect
        {
            public float top, bottom, left, right;

            public Rect(RectTransform trans)
            {
                top = trans.GetTopWorldPosition();
                bottom = trans.GetBottomWorldPosition();
                left = trans.GetLeftWorldPosition();
                right = trans.GetRightWorldPosition();
            }
        }

        /// <summary>
        /// Interval的核心，给可能产生答案的区间打Tag
        /// 左端点打加(ADD)Tag，右端点打减(DELETE)Tag
        /// </summary>
        private struct Tag
        {
            public enum TagType
            {
                ADD,
                DELETE
            }
            public TagType type;
            /// <summary>
            /// Tag的横坐标/纵坐标
            /// </summary>
            public float pos;
            /// <summary>
            /// 该Tag对应的上一个物体的Rect
            /// </summary>
            public Rect pre;
            /// <summary>
            /// Tag根物体的边界坐标
            /// </summary>
            public float root;

            public Tag(TagType t, float p, Rect r, float rt)
            {
                type = t;
                pos = p;
                pre = r;
                root = rt;
            }
        }
        /// <summary>
        /// 用来使SortedList支持重复Key值
        /// </summary>
        /// <typeparam name="TKey">Key类型</typeparam>
        private class DuplicateKeyComparer<TKey> : IComparer<TKey> where TKey : IComparable
        {
            public int Compare(TKey x, TKey y)
            {
                int result = x.CompareTo(y);
                if (result == 0)
                    return 1;
                else
                    return result;
            }
        }
        /// <summary>
        /// 使用SortedList实现了一个元素可重复的优先队列
        /// </summary>
        private struct TagList
        {
            public SortedList<float, Tag> tags;

            public TagList(int u)
            {
                tags = new SortedList<float, Tag>(new DuplicateKeyComparer<float>());
            }
            public void AddTag(Tag t)
            {
                tags.Add(t.pos, t);
            }
            public void AddTag(Tag.TagType t, float p, Rect r, float rt)
            {
                Tag tag = new Tag(t, p, r, rt);
                tags.Add(p, tag);
            }
            public Tag? GetMinTag()
            {
                if (tags.Count == 0) return null;
                return tags.Values[0];
            }
            public Tag? GetMaxTag()
            {
                if (tags.Count == 0) return null;
                return tags.Values[tags.Count - 1];
            }
            public void PopMinTag()
            {
                if (tags.Count == 0) return;
                tags.RemoveAt(0);
            }
            public void PopMaxTag()
            {
                if (tags.Count == 0) return;
                tags.RemoveAt(tags.Count - 1);
            }
            public void Clear()
            {
                tags.Clear();
            }
        }

        public override void Init()
        {
            m_VisualManager = new VisualLineManager();
            m_VisualManager.Init();
            m_Rects = new List<Rect>();
            m_Tags = new TagList(0);

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
            SnapLogic.ObjFinalPos = m_SelectedObject.GetComponent<RectTransform>().position;

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
        private void FindInterval(float eps)
        {
            if (eps == 0)
            {
                m_VisualManager.RemoveAll();
            }
            Rect objRect = new Rect(m_SelectedObject.GetComponent<RectTransform>());
            float ep2 = SnapLogic.SnapEpsDistance;
            eps += ep2;
            int[] flags = new int[m_Rects.Count];

            // 处理SelectedObject在最左边的情况
            m_Tags.Clear();
            TagList nowTags = new TagList(0);
            m_Rects.Sort((u1, u2) => u1.left.CompareTo(u2.left));
            for (int i = 0; i < m_Rects.Count; i++)
            {
                Rect rect = m_Rects[i];
                flags[i] = 0;
                if (rect.left <= objRect.right) continue;
                while (true)
                {
                    Tag? ret = m_Tags.GetMinTag();
                    if (ret == null || ret?.pos > rect.left) break;
                    Tag tag = ret ?? new Tag();
                    if (tag.type == Tag.TagType.ADD)
                    {
                        nowTags.AddTag(tag);
                    }
                    else
                    {
                        nowTags.PopMinTag();
                    }
                    m_Tags.PopMinTag();
                }
                foreach (var tag in nowTags.tags.Values)
                {
                    if (rect.top >= tag.pre.bottom && rect.bottom <= tag.pre.top)
                    {
                        flags[i] = 1;
                        m_Tags.AddTag(Tag.TagType.ADD, rect.right + rect.left - tag.pre.right - ep2, rect, tag.root);
                        m_Tags.AddTag(Tag.TagType.DELETE, rect.right + rect.left - tag.pre.right + ep2, rect, tag.root);
                        if (eps != ep2)
                        {
                            float dis = tag.root - rect.left + tag.pre.right - objRect.right;
                            if (Mathf.Abs(dis) < eps && Mathf.Abs(dis) < Mathf.Abs(SnapLogic.SnapIntervalDisHoriz))
                            {
                                SnapLogic.SnapIntervalDisHoriz = dis;
                            }
                        }
                        else
                        {
                            float mid = (Math.Min(rect.top, tag.pre.top) + Math.Max(rect.bottom, tag.pre.bottom)) / 2;
                            m_VisualManager.AddHorizLine(tag.pre.right, rect.left, mid);
                            if (tag.pre.left == tag.root)
                            {
                                mid = (Math.Min(tag.pre.top, objRect.top) + Math.Max(tag.pre.bottom, objRect.bottom)) / 2;
                                m_VisualManager.AddHorizLine(objRect.right, tag.pre.left, mid);
                            }
                        }
                    }
                }
                if (rect.top < objRect.bottom || rect.bottom > objRect.top || flags[i] == 1) continue;
                m_Tags.AddTag(Tag.TagType.ADD, rect.left - objRect.right + rect.right - eps, rect, rect.left);
                m_Tags.AddTag(Tag.TagType.DELETE, rect.left - objRect.right + rect.right + eps, rect, rect.left);
            }
            // 处理SelectedObject在中间的tag
            m_Tags.Clear();
            for (int i = 0; i < m_Rects.Count; i++)
            {
                Rect rect = m_Rects[i];
                if (rect.left <= objRect.right) continue;
                if (rect.top < objRect.bottom || rect.bottom > objRect.top) continue;
                if (flags[i] == 1) continue;
                m_Tags.AddTag(Tag.TagType.ADD, objRect.left - rect.left + objRect.right + eps * 2, rect, rect.left);
                m_Tags.AddTag(Tag.TagType.DELETE, objRect.left - rect.left + objRect.right - eps * 2, rect, rect.left);
            }
            // 处理SelectedObject在最右边的情况，代码类似最左边
            nowTags.Clear();
            m_Rects.Sort((u1, u2) => u2.right.CompareTo(u1.right));
            for (int i = 0; i < m_Rects.Count; i++)
            {
                Rect rect = m_Rects[i];
                flags[i] = 0;
                if (rect.right >= objRect.left) continue;
                while (true)
                {
                    Tag? ret = m_Tags.GetMaxTag();
                    if (ret == null || ret?.pos < rect.right) break;
                    Tag tag = ret ?? new Tag();
                    if (tag.type == Tag.TagType.ADD)
                    {
                        nowTags.AddTag(tag);
                    }
                    else
                    {
                        nowTags.PopMaxTag();
                    }
                    m_Tags.PopMaxTag();
                }
                foreach (var tag in nowTags.tags.Values)
                {
                    // 说明这是一个SelectedObject在中间的tag
                    if (tag.pre.left > objRect.right)
                    {
                        if (rect.top < objRect.bottom || rect.bottom > objRect.top) continue;
                        if (eps != ep2)
                        {
                            float dis = (tag.pre.left + rect.right - (objRect.left + objRect.right)) / 2;
                            if (Mathf.Abs(dis) < eps && Mathf.Abs(dis) < Mathf.Abs(SnapLogic.SnapIntervalDisHoriz))
                            {
                                SnapLogic.SnapIntervalDisHoriz = dis;
                            }
                        }
                        else
                        {
                            float mid = (Math.Min(rect.top, objRect.top) + Math.Max(rect.bottom, objRect.bottom)) / 2;
                            m_VisualManager.AddHorizLine(rect.right, objRect.left, mid);
                            mid = (Math.Min(tag.pre.top, objRect.top) + Math.Max(tag.pre.bottom, objRect.bottom)) / 2;
                            m_VisualManager.AddHorizLine(objRect.right, tag.pre.left, mid);
                        }
                    }
                    else if (rect.top >= tag.pre.bottom && rect.bottom <= tag.pre.top)
                    {
                        flags[i] = 1;
                        m_Tags.AddTag(Tag.TagType.ADD, rect.left - tag.pre.left + rect.right + ep2, rect, tag.root);
                        m_Tags.AddTag(Tag.TagType.DELETE, rect.left - tag.pre.left + rect.right - ep2, rect, tag.root);
                        if (eps != ep2)
                        {
                            float dis = tag.root + tag.pre.left - rect.right - objRect.left;
                            if (Mathf.Abs(dis) < eps && Mathf.Abs(dis) < Mathf.Abs(SnapLogic.SnapIntervalDisHoriz))
                            {
                                SnapLogic.SnapIntervalDisHoriz = dis;
                            }
                        }
                        else
                        {
                            float mid = (Math.Min(rect.top, tag.pre.top) + Math.Max(rect.bottom, tag.pre.bottom)) / 2;
                            m_VisualManager.AddHorizLine(rect.right, tag.pre.left, mid);
                            if (tag.pre.right == tag.root)
                            {
                                mid = (Math.Min(tag.pre.top, objRect.top) + Math.Max(tag.pre.bottom, objRect.bottom)) / 2;
                                m_VisualManager.AddHorizLine(tag.pre.right, objRect.left, mid);
                            }
                        }
                    }
                }
                if (rect.top < objRect.bottom || rect.bottom > objRect.top || flags[i] == 1) continue;
                m_Tags.AddTag(Tag.TagType.ADD, rect.left - objRect.left + rect.right + eps, rect, rect.right);
                m_Tags.AddTag(Tag.TagType.DELETE, rect.left - objRect.left + rect.right - eps, rect, rect.right);
            }
            // 处理SelectedObject在最下边的情况
            m_Tags.Clear();
            nowTags.Clear();
            m_Rects.Sort((u1, u2) => u1.bottom.CompareTo(u2.bottom));
            for (int i = 0; i < m_Rects.Count; i++)
            {
                Rect rect = m_Rects[i];
                flags[i] = 0;
                if (rect.bottom <= objRect.top) continue;
                while (true)
                {
                    Tag? ret = m_Tags.GetMinTag();
                    if (ret == null || ret?.pos > rect.bottom) break;
                    Tag tag = ret ?? new Tag();
                    if (tag.type == Tag.TagType.ADD)
                    {
                        nowTags.AddTag(tag);
                    }
                    else
                    {
                        nowTags.PopMinTag();
                    }
                    m_Tags.PopMinTag();
                }
                foreach (var tag in nowTags.tags.Values)
                {
                    if (rect.right >= tag.pre.left && rect.left <= tag.pre.right)
                    {
                        flags[i] = 1;
                        m_Tags.AddTag(Tag.TagType.ADD, rect.top + rect.bottom - tag.pre.top - ep2, rect, tag.root);
                        m_Tags.AddTag(Tag.TagType.DELETE, rect.top + rect.bottom - tag.pre.top + ep2, rect, tag.root);
                        if (eps != ep2)
                        {
                            float dis = tag.root - rect.bottom + tag.pre.top - objRect.top;
                            if (Mathf.Abs(dis) < eps && Mathf.Abs(dis) < Mathf.Abs(SnapLogic.SnapIntervalDisVert))
                            {
                                SnapLogic.SnapIntervalDisVert = dis;
                            }
                        }
                        else
                        {
                            float mid = (Math.Min(rect.right, tag.pre.right) + Math.Max(rect.left, tag.pre.left)) / 2;
                            m_VisualManager.AddVertLine(mid, tag.pre.top, rect.bottom);
                            if (tag.pre.bottom == tag.root)
                            {
                                mid = (Math.Min(tag.pre.right, objRect.right) + Math.Max(tag.pre.left, objRect.left)) / 2;
                                m_VisualManager.AddVertLine(mid, objRect.top, tag.pre.bottom);
                            }
                        }
                    }
                }
                if (rect.right < objRect.left || rect.left > objRect.right || flags[i] == 1) continue;
                m_Tags.AddTag(Tag.TagType.ADD, rect.bottom - objRect.top + rect.top - eps, rect, rect.bottom);
                m_Tags.AddTag(Tag.TagType.DELETE, rect.bottom - objRect.top + rect.top + eps, rect, rect.bottom);
            }
            // 处理SelectedObject在中间的tag
            m_Tags.Clear();
            for (int i = 0; i < m_Rects.Count; i++)
            {
                Rect rect = m_Rects[i];
                if (rect.bottom <= objRect.top) continue;
                if (rect.right < objRect.left || rect.left > objRect.right) continue;
                if (flags[i] == 1) continue;
                m_Tags.AddTag(Tag.TagType.ADD, objRect.bottom - rect.bottom + objRect.top + eps * 2, rect, rect.bottom);
                m_Tags.AddTag(Tag.TagType.DELETE, objRect.bottom - rect.bottom + objRect.top - eps * 2, rect, rect.bottom);
            }
            // 处理SelectedObject在最上边的情况，代码类似最下边
            nowTags.Clear();
            m_Rects.Sort((u1, u2) => u2.top.CompareTo(u1.top));
            for (int i = 0; i < m_Rects.Count; i++)
            {
                Rect rect = m_Rects[i];
                flags[i] = 0;
                if (rect.top >= objRect.bottom) continue;
                while (true)
                {
                    Tag? ret = m_Tags.GetMaxTag();
                    if (ret == null || ret?.pos < rect.top) break;
                    Tag tag = ret ?? new Tag();
                    if (tag.type == Tag.TagType.ADD)
                    {
                        nowTags.AddTag(tag);
                    }
                    else
                    {
                        nowTags.PopMaxTag();
                    }
                    m_Tags.PopMaxTag();
                }
                foreach (var tag in nowTags.tags.Values)
                {
                    // 说明这是一个SelectedObject在中间的tag
                    if (tag.pre.bottom > objRect.top)
                    {
                        if (rect.right < objRect.left || rect.left > objRect.right) continue;
                        if (eps != ep2)
                        {
                            float dis = (tag.pre.bottom + rect.top - (objRect.bottom + objRect.top)) / 2;
                            if (Mathf.Abs(dis) < eps && Mathf.Abs(dis) < Mathf.Abs(SnapLogic.SnapIntervalDisVert))
                            {
                                SnapLogic.SnapIntervalDisVert = dis;
                            }
                        }
                        else
                        {
                            float mid = (Math.Min(rect.right, objRect.right) + Math.Max(rect.left, objRect.left)) / 2;
                            m_VisualManager.AddVertLine(mid, rect.top, objRect.bottom);
                            mid = (Math.Min(tag.pre.right, objRect.right) + Math.Max(tag.pre.left, objRect.left)) / 2;
                            m_VisualManager.AddVertLine(mid, objRect.top, tag.pre.bottom);
                        }
                    }
                    else if (rect.right >= tag.pre.left && rect.left <= tag.pre.right)
                    {
                        flags[i] = 1;
                        m_Tags.AddTag(Tag.TagType.ADD, rect.bottom - tag.pre.bottom + rect.top + ep2, rect, tag.root);
                        m_Tags.AddTag(Tag.TagType.DELETE, rect.bottom - tag.pre.bottom + rect.top - ep2, rect, tag.root);
                        if (eps != ep2)
                        {
                            float dis = tag.root + tag.pre.bottom - rect.top - objRect.bottom;
                            if (Mathf.Abs(dis) < eps && Mathf.Abs(dis) < Mathf.Abs(SnapLogic.SnapIntervalDisVert))
                            {
                                SnapLogic.SnapIntervalDisVert = dis;
                            }
                        }
                        else
                        {
                            float mid = (Math.Max(rect.left, tag.pre.left) + Math.Min(rect.right, tag.pre.right)) / 2;
                            m_VisualManager.AddVertLine(mid, rect.top, tag.pre.bottom);
                            if (tag.pre.top == tag.root)
                            {
                                mid = (Math.Min(tag.pre.right, objRect.right) + Math.Max(tag.pre.left, objRect.left)) / 2;
                                m_VisualManager.AddVertLine(mid, tag.pre.top, objRect.bottom);
                            }
                        }
                    }
                }
                if (rect.right < objRect.left || rect.left > objRect.right || flags[i] == 1) continue;
                m_Tags.AddTag(Tag.TagType.ADD, rect.bottom - objRect.bottom + rect.top + eps, rect, rect.top);
                m_Tags.AddTag(Tag.TagType.DELETE, rect.bottom - objRect.bottom + rect.top - eps, rect, rect.top);
            }
        }

        private void ListenMoving()
        {
            if (m_SelectedObject != null && m_SelectedObject.GetComponent<RectTransform>().position != SnapLogic.ObjFinalPos && LocationLineLogic.Instance.EnableSnap)
            {
                SnapLogic.SnapIntervalDisHoriz = SnapLogic.SnapIntervalDisVert = Mathf.Infinity;
                FindInterval(SnapLogic.SnapWorldDistance);
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
                if (Math.Abs(SnapLogic.SnapIntervalDisHoriz) < Math.Abs(SnapLogic.SnapEdgeDisHoriz) &&
                Math.Abs(SnapLogic.SnapIntervalDisHoriz) < Math.Abs(SnapLogic.SnapLineDisHoriz))
                {
                    vec.x += SnapLogic.SnapIntervalDisHoriz;
                }
                if (Math.Abs(SnapLogic.SnapIntervalDisVert) < Math.Abs(SnapLogic.SnapEdgeDisVert) &&
                Math.Abs(SnapLogic.SnapIntervalDisVert) < Math.Abs(SnapLogic.SnapLineDisVert))
                {
                    vec.y += SnapLogic.SnapIntervalDisVert;
                }
                rectTransform.position = vec;
                FindInterval(0);
                SnapLogic.ObjFinalPos = rectTransform.position;
            }
        }
    }

    /// <summary>
    /// 画出间距提示线
    /// </summary>
    public class VisualLineManager
    {
        private List<LineHoriz> m_HorizLines;
        private List<LineVert> m_VertLines;
        private bool m_MousePressed;
        private readonly static Color m_MyBlue = new Color(0.1215686f, 0.4980392f, 0.8705883f);

        private class LineBase
        {
            public VisualElement visual;
            /// <summary>
            /// 是否需要绘制间距数字
            /// </summary>
            public bool needNum;

            public LineBase()
            {
                visual = new VisualElement();
                visual.style.position = Position.Absolute;
                visual.style.backgroundColor = m_MyBlue;
                visual.style.flexDirection = FlexDirection.Row;
                Label label = new Label();
                label.name = "lengthLabel";
                label.style.alignSelf = Align.Center;
                label.style.backgroundColor = m_MyBlue;
                label.style.color = Color.white;
                label.style.position = Position.Absolute;
                label.style.unityTextAlign = TextAnchor.MiddleCenter;
                visual.Add(label);
            }
        }
        private class LineHoriz : LineBase
        {

            public float x1, x2;
            public float y;

            public LineHoriz(float u1, float u2, float u3, bool f) : base()
            {
                x1 = u1;
                x2 = u2;
                y = u3;
                needNum = f;
            }
        }
        private class LineVert : LineBase
        {
            public float x;
            public float y1, y2;

            public LineVert(float u1, float u2, float u3, bool f) : base()
            {
                x = u1;
                y1 = u2;
                y2 = u3;
                needNum = f;
            }
        }

        public void Init()
        {
            m_HorizLines = new List<LineHoriz>();
            m_VertLines = new List<LineVert>();
            m_MousePressed = false;

            SceneView.duringSceneGui += (SceneView sceneView) =>
            {
                if (Event.current.type == EventType.MouseDown)
                {
                    m_MousePressed = true;
                }
                else if (Event.current.type == EventType.MouseUp)
                {
                    m_MousePressed = false;
                }
            };
            EditorApplication.update += () =>
            {
                if (m_MousePressed)
                {
                    DrawLines();
                }
                else
                {
                    RemoveAll();
                }
            };
        }

        public void AddHorizLine(float u1, float u2, float u3, bool u4 = true)
        {
            m_HorizLines?.Add(new LineHoriz(u1, u2, u3, u4));
        }
        public void AddVertLine(float u1, float u2, float u3, bool u4 = true)
        {
            m_VertLines?.Add(new LineVert(u1, u2, u3, u4));
        }

        private void DrawLines()
        {
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (m_HorizLines != null)
            {
                foreach (var line in m_HorizLines)
                {
                    Vector3 vec1 = sceneView.camera.WorldToScreenPoint(new Vector3(line.x1, line.y, 0));
                    Vector3 vec2 = sceneView.camera.WorldToScreenPoint(new Vector3(line.x2, line.y, 0));
                    line.visual.style.left = vec1.x;
                    line.visual.style.bottom = vec1.y;
                    line.visual.style.height = 1;
                    line.visual.style.right = sceneView.camera.pixelWidth - vec2.x;
                    line.visual.style.justifyContent = Justify.Center;
                    if (line.needNum)
                    {
                        Label label = line.visual.Q<Label>("lengthLabel");
                        label.text = (line.x2 - line.x1).ToString("0.#");
                        label.style.top = 3f;
                    }
                    sceneView.rootVisualElement.Add(line.visual);
                }
            }
            if (m_VertLines != null)
            {
                foreach (var line in m_VertLines)
                {
                    Vector3 vec1 = sceneView.camera.WorldToScreenPoint(new Vector3(line.x, line.y1, 0));
                    Vector3 vec2 = sceneView.camera.WorldToScreenPoint(new Vector3(line.x, line.y2, 0));
                    line.visual.style.bottom = vec1.y;
                    line.visual.style.left = vec2.x;
                    line.visual.style.width = 1;
                    line.visual.style.height = vec2.y - vec1.y;
                    if (line.needNum)
                    {
                        Label label = line.visual.Q<Label>("lengthLabel");
                        label.text = (line.y2 - line.y1).ToString("0.#");
                        label.style.left = 3f;
                    }
                    sceneView.rootVisualElement.Add(line.visual);
                }
            }
        }

        public void RemoveAll()
        {
            if (m_HorizLines != null)
            {
                foreach (var line in m_HorizLines)
                {
                    line.visual.RemoveFromHierarchy();
                }
                m_HorizLines.Clear();
            }
            if (m_VertLines != null)
            {
                foreach (var line in m_VertLines)
                {
                    line.visual.RemoveFromHierarchy();
                }
                m_VertLines.Clear();
            }
        }
    }
}
#endif