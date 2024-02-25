#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using UnityEngine.UIElements;
using System.Collections.Generic;
using ThunderFireUnityEx;

namespace ThunderFireUITool
{
    public enum LocationLineDirection
    {
        Vertical,
        Horizontal
    }

    public class LineManipulator : MouseManipulator
    {
        public LineManipulator(Action handler)
        {
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.TrickleDown);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp, TrickleDown.TrickleDown);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.TrickleDown);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp, TrickleDown.TrickleDown);
        }

        public void OnMouseDown(MouseDownEvent e)
        {
        }

        private void OnMouseUp(MouseUpEvent e)
        {
        }
    }

    public class CustomClickable : Clickable
    {
        public CustomClickable(Action handler) : base(handler) { }
        public CustomClickable(Action<EventBase> handler) : base(handler) { }
        public CustomClickable(Action handler, long delay, long interval) : base(handler, delay, interval) { }

        public event System.Action<PointerEnterEvent> OnPointerEnterAction;
        public event System.Action<PointerDownEvent> OnPointerDownAction;
        public event System.Action<PointerMoveEvent> OnPointerMoveAction;
        public event System.Action<PointerUpEvent> OnPointerUpAction;
        public event System.Action<PointerLeaveEvent> OnPointerLeaveAction;

        public float t;
        public Vector2 lastPointerPosition
        {
            get { return lastMousePosition; }
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerEnterEvent>(OnPointerEnter);
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
            target.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
            base.RegisterCallbacksOnTarget();
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerEnterEvent>(OnPointerEnter);
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            target.UnregisterCallback<PointerLeaveEvent>(OnPointerLeave);
            base.UnregisterCallbacksFromTarget();
        }

        private void OnPointerEnter(PointerEnterEvent e)
        {
            OnPointerEnterAction?.Invoke(e);
        }
        private void OnPointerDown(PointerDownEvent e)
        {
            OnPointerDownAction?.Invoke(e);
        }
        private void OnPointerMove(PointerMoveEvent e)
        {
            OnPointerMoveAction?.Invoke(e);
        }
        private void OnPointerUp(PointerUpEvent e)
        {
            OnPointerUpAction?.Invoke(e);
        }
        private void OnPointerLeave(PointerLeaveEvent e)
        {
            OnPointerLeaveAction?.Invoke(e);
        }
    }

    public class LocationLine : VisualElement
    {
        public int id;
        public Vector3 worldPostion;
        public LocationLineDirection direction;
        protected VisualElement m_Line;
        protected bool m_Selected;

        protected readonly Color m_MyBlue = new Color(0.4f, 0.8f, 1f);
        protected readonly Color m_MyRed = new Color(1f, 0.3f, 0.3f);

        private CustomClickable m_Clickable;

        protected static List<RectEx> m_Rects;

        protected struct RectEx
        {
            /// <summary>
            /// 长度为4，上下左右
            /// </summary>
            public float[] pos;

            public RectEx(RectTransform trans)
            {
                pos = new float[4];
                pos[0] = (float)Math.Round((double)trans.GetTopWorldPosition(), 1);
                pos[1] = (float)Math.Round((double)trans.GetBottomWorldPosition(), 1);
                pos[2] = (float)Math.Round((double)trans.GetLeftWorldPosition(), 1);
                pos[3] = (float)Math.Round((double)trans.GetRightWorldPosition(), 1);
            }
        }

        public CustomClickable clickable
        {
            get
            {
                return m_Clickable;
            }
            set
            {
                if (m_Clickable != null && m_Clickable.target == this)
                {
                    this.RemoveManipulator(m_Clickable);
                }

                m_Clickable = value;

                if (m_Clickable != null)
                {
                    this.AddManipulator(m_Clickable);
                }
            }
        }

        public LocationLine()
        {
            SceneView sceneView = SceneView.lastActiveSceneView;
            worldPostion = sceneView.camera.ScreenToWorldPoint(new Vector3(sceneView.camera.pixelWidth / 2, (sceneView.camera.pixelHeight - 40) / 2, 0));
            style.backgroundColor = new Color(0, 0, 0, 0);

            Action action = null;

            clickable = new CustomClickable(action);
            clickable.OnPointerEnterAction += OnMouseOver;
            clickable.OnPointerDownAction += OnMouseDown;
            clickable.OnPointerMoveAction += OnMouseMove;
            clickable.OnPointerUpAction += OnMouseUp;
            clickable.OnPointerLeaveAction += OnMouseOut;

            style.left = sceneView.camera.pixelWidth / 2;
            style.bottom = sceneView.camera.pixelHeight / 2;
        }

        public static void Init()
        {
            m_Rects = new List<RectEx>();
            //ResetAll();
            //EditorApplication.hierarchyChanged += ResetAll;
        }

        public static void Close()
        {
            //EditorApplication.hierarchyChanged -= ResetAll;
        }

        private static void ResetAll()
        {
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
                if (EditorLogic.ObjectFit(item.gameObject))
                {
                    m_Rects.Add(new RectEx(item));
                }
            }
        }

        public void OnMouseOver(PointerEnterEvent evt)
        {
            RecoverCursor();
            SetCursor();
        }
        public void OnMouseDown(PointerDownEvent evt)
        {
            if (evt.button == 0)
            {
                m_Selected = true;
                ResetAll();
            }
            else if (evt.button == 1)
            {
#if UNITY_2020_1_OR_NEWER
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_删除)), false, () =>
                {
                    LocationLineLogic.Instance.RemoveLine(this);
                });
                menu.ShowAsContext();
                Event.current.Use();
                evt.StopImmediatePropagation();
#endif
            }
            UpdateLineState();
        }
        public void OnMouseUp(PointerUpEvent evt)
        {
            if (evt.button == 0 && m_Selected)
            {
                m_Selected = false;
                RecoverCursor();
                LocationLineLogic.Instance.ModifyLine(this);
            }
            UpdateLineState();
        }
        public void OnMouseMove(PointerMoveEvent evt)
        {
            if (m_Selected)
            {
                OnDrag(Event.current.mousePosition);
                Event.current.Use();
            }
        }
        public void OnMouseOut(PointerLeaveEvent evt)
        {
            if (m_Selected)
            {
                OnDrag(Event.current.mousePosition);
            }
            else
            {
                RecoverCursor();
            }
        }

        private UXCursorType GetCursorType()
        {
            if (direction == LocationLineDirection.Horizontal)
            {
                return UXCursorType.Updown;
            }
            else
            {
                return UXCursorType.Leftright;
            }

        }
        private void SetCursor()
        {
            //Cursor.visible = false;
            //UXCustomSceneView.AddDelegate(ChangeCursor);
            UXSceneViewCursor.Instance.SetCursor(GetCursorType());
        }
        private void RecoverCursor()
        {
            //Cursor.visible = true;
            //UXCustomSceneView.RemoveDelegate(ChangeCursor);
            UXSceneViewCursor.Instance.SetCursor(UXCursorType.None);
        }
        private void ChangeCursor(SceneView sceneView)
        {
            //Handles.BeginGUI();
            //GUI.DrawTexture(new Rect(Event.current.mousePosition.x - m_CursorTexture.width / 2, Event.current.mousePosition.y - m_CursorTexture.height / 2, m_CursorTexture.width, m_CursorTexture.height), m_CursorTexture);
            //Handles.EndGUI();
            //HandleUtility.AddDefaultControl(0);
            //sceneView.Repaint();
            EditorGUIUtility.AddCursorRect(new Rect(0, 0, 500, 500), MouseCursor.ResizeVertical);
        }

        private void UpdateLineState()
        {
            if (!m_Selected)
            {
                m_Line.style.backgroundColor = m_MyBlue;
            }
            else
            {
                m_Line.style.backgroundColor = m_MyRed;
            }
        }

        protected virtual void OnDrag(Vector2 mousePosition)
        {

        }
        public virtual void UpdateLineScreenViewPos(SceneView sceneView)
        {

        }
    }
    public class HorizontalLocationLine : LocationLine
    {
        public HorizontalLocationLine()
        {
            direction = LocationLineDirection.Horizontal;
            m_Line = new VisualElement();
            m_Line.style.position = Position.Absolute;
            m_Line.style.height = 2;
            m_Line.style.left = 0;
            m_Line.style.right = 0;
            m_Line.style.alignSelf = Align.Center;
            m_Line.style.backgroundColor = m_MyBlue;
            style.flexDirection = FlexDirection.Row;

            style.position = Position.Absolute;
            style.flexDirection = FlexDirection.Row;
            style.height = 10;
            style.right = 0;
            style.left = 0;
            Add(m_Line);
        }
        protected override void OnDrag(Vector2 mousePosition)
        {
            mousePosition.y = mousePosition.y - LocationLineLogic.sceneviewOffset;

            //SceneView的(0,0)在左上角, 所以用SceneView的高减去mousePosition.y 才是距离底部的高度
            float y = SceneView.lastActiveSceneView.camera.pixelHeight - mousePosition.y;

            style.bottom = y - style.height.value.value / 2;

            Vector3 mousePos = SceneView.lastActiveSceneView.camera.ScreenToWorldPoint(new Vector3(0, y, 0));
            float minDis = Mathf.Infinity;
            foreach (var rect in m_Rects)
            {
                if (Mathf.Abs(minDis) > Mathf.Abs(rect.pos[0] - mousePos.y))
                {
                    minDis = rect.pos[0] - mousePos.y;
                }
                if (Mathf.Abs(minDis) > Mathf.Abs(rect.pos[1] - mousePos.y))
                {
                    minDis = rect.pos[1] - mousePos.y;
                }
            }
            if (Mathf.Abs(minDis) < SnapLogic.SnapWorldDistance)
            {
                worldPostion = mousePos + new Vector3(0, minDis, 0);
                style.bottom = SceneView.lastActiveSceneView.camera.WorldToScreenPoint(worldPostion).y - style.height.value.value / 2;
            }
            else
            {
                worldPostion = mousePos;
            }
            SceneView.lastActiveSceneView.Repaint();
        }
        public override void UpdateLineScreenViewPos(SceneView sceneView)
        {
            if (!m_Selected)
            {
                style.bottom = sceneView.camera.WorldToScreenPoint(worldPostion).y - style.height.value.value / 2;
            }
        }
    }
    public class VerticalLocationLine : LocationLine
    {
        public VerticalLocationLine()
        {
            direction = LocationLineDirection.Vertical;
            m_Line = new VisualElement();
            m_Line.style.position = Position.Absolute;
            m_Line.style.alignSelf = Align.Center;
            m_Line.style.top = 0;
            m_Line.style.bottom = 0;
            m_Line.style.width = 2;
            m_Line.style.backgroundColor = m_MyBlue;
            this.style.position = Position.Absolute;
            this.style.width = 10;
            this.style.bottom = 0;
#if !UNITY_2021_3_OR_NEWER
            this.style.top = Utils.GetSceneViewToolbarHeight();
#else
            this.style.top = 0;
#endif
            this.Add(m_Line);
        }

        protected override void OnDrag(Vector2 mousePosition)
        {
            style.left = mousePosition.x - style.width.value.value / 2;

            float screenLeft = style.left.value.value + style.width.value.value / 2;
            Vector3 mousePos = SceneView.lastActiveSceneView.camera.ScreenToWorldPoint(new Vector3(mousePosition.x, 0, 0));

            float minDis = Mathf.Infinity;
            foreach (var rect in m_Rects)
            {
                if (Mathf.Abs(minDis) > Mathf.Abs(rect.pos[2] - mousePos.x))
                {
                    minDis = rect.pos[2] - mousePos.x;
                }
                if (Mathf.Abs(minDis) > Mathf.Abs(rect.pos[3] - mousePos.x))
                {
                    minDis = rect.pos[3] - mousePos.x;
                }
            }
            if (Mathf.Abs(minDis) < SnapLogic.SnapWorldDistance)
            {
                worldPostion = mousePos + new Vector3(minDis, 0, 0);
                style.left = SceneView.lastActiveSceneView.camera.WorldToScreenPoint(worldPostion).x - style.width.value.value / 2;
            }
            else
            {
                worldPostion = mousePos;
            }
            SceneView.lastActiveSceneView.Repaint();
        }

        //用来更新 SceneView的拖动或者滚轮缩放后 辅助线在SceneView的位置
        public override void UpdateLineScreenViewPos(SceneView sceneView)
        {
            if (!m_Selected)
            {
                style.left = sceneView.camera.WorldToScreenPoint(worldPostion).x - style.width.value.value / 2;
            }
        }
    }
}
#endif