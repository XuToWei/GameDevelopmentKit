#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

using Object = UnityEngine.Object;

namespace ThunderFireUITool
{
    [CustomEditor(typeof(RectTransform))]
    [CanEditMultipleObjects]
    public class LayoutEditor : DecoratorEditor
    {
        private IMGUIContainer imNormalInspector;
        private static VisualElement BarFromUXML;
        private Vector3[] origin_position, origin_scale;
        private Rect origin_rect;
        private Vector2 origin_mouse;
        private Quaternion[] origin_rotation;
        private GameObject[] cloneObj;
        private int OnAltPress = 0; //0代表没有按alt，1在鼠标点击之前按了alt，2代表在鼠标点击之后按了alt
        private bool OnShiftPress = false;

        private bool OnDrag = false;
        public LayoutEditor() : base("RectTransformEditor") { }

        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();
            //初始化原始Inspector容器
            imNormalInspector = new IMGUIContainer(() =>
            {
                if (targets != null && targets[0] != null)
                    base.OnInspectorGUI();
            });

            if (SwitchSetting.CheckValid(SwitchSetting.SwitchType.AlignSnap))
            {
                VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ThunderFireUIToolConfig.UIBuilderPath + "rectTransformButton.uxml");
                BarFromUXML = visualTree.CloneTree();
                container.Add(BarFromUXML);

                BarFromUXML.style.left = -20;
                BarFromUXML.style.width = 10000;

                //先绘制对齐工具按钮
                bool canAlign = AlignLogic.CanAlign();
                int index = 0;
                foreach (AlignType alignType in AlignType.GetValues(typeof(AlignType)))
                {
                    string name = "UI_Align_" + alignType.ToString();
                    VisualElement button = BarFromUXML.Q<VisualElement>(name);
                    button.tooltip = EditorLocalization.GetLocalization(AlignLogic.GetStringKeyByAlignType(alignType));
                    button.SetEnabled(canAlign);
                    if (!canAlign)
                    {
                        button.style.backgroundColor = ThunderFireUIToolConfig.disableColor;
                    }

                    button.RegisterCallback((MouseDownEvent e) =>
                    {
                        AlignLogic.Align(alignType);
                    });
                    RegisterMouseHover(button);
                    index++;
                }
                bool canGrid = AlignLogic.CanGrid();
                //绘制阵列工具按钮 和对齐按钮在同一行
                foreach (GridType gridType in GridType.GetValues(typeof(GridType)))
                {
                    string name = "UI_Grid_" + gridType.ToString();
                    VisualElement button = BarFromUXML.Q<VisualElement>(name);
                    button.tooltip = EditorLocalization.GetLocalization(AlignLogic.GetStringKeyByGridType(gridType));
                    button.SetEnabled(canGrid);
                    if (!canGrid)
                    {
                        button.style.backgroundColor = ThunderFireUIToolConfig.disableColor;
                    }
                    button.RegisterCallback((MouseDownEvent e) =>
                    {
                        AlignLogic.Grid(gridType);
                    });
                    RegisterMouseHover(button);
                    index++;
                }

                imNormalInspector.style.top = 40;
                container.style.height = 250f;
            }

            container.Add(imNormalInspector);

            //container.style.backgroundColor = Color.white;
            return container;
        }

        private void RegisterMouseHover(VisualElement button)
        {
            button.RegisterCallback((PointerOverEvent e) =>
            {
                if (button.enabledSelf)
                {
                    button.style.backgroundColor = ThunderFireUIToolConfig.hoverColor;
                }
            });
            button.RegisterCallback((PointerOutEvent e) =>
            {
                if (button.enabledSelf)
                {
                    button.style.backgroundColor = ThunderFireUIToolConfig.normalColor;
                }
            });
        }

        public override void OnSceneGUI()
        {
            CallInspectorMethod("OnSceneGUI");
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                OnMouseDown();
            }

            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                OnMouseUp();
            }

            if (Event.current.type == EventType.MouseDrag && OnDrag)
            {
                //LocationLineLogic.Instance.SnapToLocationLine();
                LocationLineLogic.Instance.EnableSnap = true;
                if (SwitchSetting.CheckValid(SwitchSetting.SwitchType.QuickCopy) && OnAltPress == 2)
                {
                    Vector2 vec = Event.current.mousePosition;
                    vec.y = Camera.current.pixelHeight - vec.y;
                    vec = Camera.current.ScreenToWorldPoint(vec);
                    if (!OnShiftPress)
                    {
                        for (int i = 0; i < origin_position.Length; i++)
                        {
                            Selection.transforms[i].position = new Vector2(origin_position[i].x, origin_position[i].y) + vec - origin_mouse;
                        }
                    }
                    else
                    {
                        if (Mathf.Abs(Selection.transforms[0].position.x - origin_position[0].x) <= Mathf.Abs(Selection.transforms[0].position.y - origin_position[0].y))
                        {
                            for (int i = 0; i < origin_position.Length; i++)
                            {
                                Selection.transforms[i].position = new Vector2(origin_position[i].x, origin_position[i].y + vec.y - origin_mouse.y);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < origin_position.Length; i++)
                            {
                                Selection.transforms[i].position = new Vector2(origin_position[i].x + vec.x - origin_mouse.x, origin_position[i].y);
                            }
                        }
                    }
                }
            }
            else
            {
                LocationLineLogic.Instance.EnableSnap = false;
            }
            if (SwitchSetting.CheckValid(SwitchSetting.SwitchType.QuickCopy) && Event.current.type == EventType.KeyDown &&
            OnAltPress == 0 && (Event.current.keyCode == KeyCode.LeftAlt || Event.current.keyCode == KeyCode.RightAlt))
            {
                OnAltPress = 1;
                if (OnDrag)
                {
                    //判断图片是变形还是移动，没有变形且移动位置才会复制
                    Rect cur_rect = Selection.transforms[0].GetComponent<RectTransform>().rect;
                    if (cur_rect.width == origin_rect.width && cur_rect.height == origin_rect.height && Selection.transforms[0].position != origin_position[0])
                    {
                        OnAltPress = 2;
                        cloneObj = new GameObject[origin_position.Length];
                        for (int i = 0; i < origin_position.Length; i++)
                        {
                            cloneObj[i] = Instantiate<GameObject>(Selection.transforms[i].gameObject, origin_position[i], origin_rotation[i]);
                            Undo.RegisterCreatedObjectUndo(cloneObj[i], "");
                            Undo.SetTransformParent(cloneObj[i].transform, Selection.transforms[i].parent, "");
                            cloneObj[i].transform.localScale = origin_scale[i];
                            cloneObj[i].transform.SetSiblingIndex(Selection.transforms[i].GetSiblingIndex());
                            cloneObj[i].name = Selection.transforms[i].gameObject.name;
                        }
                    }
                }

            }
            if (Event.current.type == EventType.KeyUp &&
            (Event.current.keyCode == KeyCode.LeftAlt || Event.current.keyCode == KeyCode.RightAlt))
            {
                if (cloneObj != null && OnDrag)
                {
                    for (int i = 0; i < cloneObj.Length; i++)
                    {
                        Undo.DestroyObjectImmediate(cloneObj[i]);
                    }
                    cloneObj = null;
                }
                OnAltPress = 0;
            }

            if (Event.current.type == EventType.KeyDown && !OnShiftPress && (Event.current.keyCode == KeyCode.LeftShift || Event.current.keyCode == KeyCode.RightShift))
            {
                OnShiftPress = true;
            }
            if (Event.current.type == EventType.KeyUp && (Event.current.keyCode == KeyCode.LeftShift || Event.current.keyCode == KeyCode.RightShift))
            {
                OnShiftPress = false;
            }

            if (Event.current.type == EventType.KeyDown && SwitchSetting.CheckValid(SwitchSetting.SwitchType.MovementShortcuts))
            {
                switch (Event.current.keyCode)
                {
                    case KeyCode.UpArrow:
                        Undo.RecordObjects(Selection.transforms, "Move GameObjects");
                        foreach (var trans in Selection.transforms)
                        {
                            trans.position = new Vector3(trans.position.x, trans.position.y + 1, trans.position.z);
                        }
                        Event.current.Use();
                        break;
                    case KeyCode.DownArrow:
                        Undo.RecordObjects(Selection.transforms, "Move GameObjects");
                        foreach (var trans in Selection.transforms)
                        {
                            trans.position = new Vector3(trans.position.x, trans.position.y - 1, trans.position.z);
                        }
                        Event.current.Use();
                        break;
                    case KeyCode.LeftArrow:
                        Undo.RecordObjects(Selection.transforms, "Move GameObjects");
                        foreach (var trans in Selection.transforms)
                        {
                            trans.position = new Vector3(trans.position.x - 1, trans.position.y, trans.position.z);
                        }
                        Event.current.Use();
                        break;
                    case KeyCode.RightArrow:
                        Undo.RecordObjects(Selection.transforms, "Move GameObjects");
                        foreach (var trans in Selection.transforms)
                        {
                            trans.position = new Vector3(trans.position.x + 1, trans.position.y, trans.position.z);
                        }
                        Event.current.Use();
                        break;
                    default:
                        break;
                }
            }
        }

        public void OnMouseDown()
        {
            if (Selection.transforms.Length > 0 && Selection.transforms[0] == target)
            {
                origin_position = new Vector3[Selection.transforms.Length];
                origin_scale = new Vector3[Selection.transforms.Length];
                origin_rotation = new Quaternion[Selection.transforms.Length];
                origin_rect = Selection.transforms[0].GetComponent<RectTransform>().rect;
                for (int i = 0; i < origin_position.Length; i++)
                {
                    origin_position[i] = Selection.transforms[i].position;
                    origin_rotation[i] = Selection.transforms[i].rotation;
                    origin_scale[i] = Selection.transforms[i].localScale;
                }
                origin_mouse = Event.current.mousePosition;
                origin_mouse.y = Camera.current.pixelHeight - origin_mouse.y;
                origin_mouse = Camera.current.ScreenToWorldPoint(origin_mouse);
                OnDrag = true;
            }
        }

        public void OnMouseUp()
        {
            OnDrag = false;
            if (cloneObj != null)
            {
                for (int i = 0; i < origin_position.Length; i++)
                {
                    Undo.IncrementCurrentGroup();
                    Undo.RecordObject(Selection.transforms[i].gameObject, "Rename GameObject");
                    Selection.transforms[i].gameObject.name = GameObjectUtility.GetUniqueNameForSibling(Selection.transforms[i].parent, Selection.transforms[i].gameObject.name);
                }
                Undo.CollapseUndoOperations(Undo.GetCurrentGroup() - Selection.transforms.Length);
                cloneObj = null;
            }
        }

        private Rect GetWorldRect(RectTransform rectTransform)
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            float width = Math.Abs(Vector2.Distance(corners[0], corners[3]));
            float height = Math.Abs(Vector2.Distance(corners[0], corners[1]));
            return new Rect(corners[0], new Vector2(width, height));
        }

        private bool IsClickIn(Vector2 mousePosition)
        {
            Vector2 position = new Vector2(mousePosition.x, SceneView.lastActiveSceneView.camera.pixelHeight - mousePosition.y);
            RectTransform rect = target as RectTransform;
            if (rect != null)
            {
                //Rect worldRect = GetWorldRect(rect);
                //Vector3 worldPosition = SceneView.lastActiveSceneView.camera.ScreenToWorldPoint(position);
                //bool inArea = worldRect.Contains(worldPosition);
                //
                //Transform parent = FindContainerLogic.GetObjectParent(new GameObject[] { rect.gameObject } );
                ////RectTransformUtility.ScreenPointToLocalPointInRectangle(rect.parent., position, Camera.main, out Vector2 worldPosition1);
                //Vector3 worldPosition1 = SceneView.lastActiveSceneView.camera.ScreenToWorldPoint(position);
                //Vector2 localPos = parent.InverseTransformPoint(worldPosition1);
                //bool inArea2 = rect.rect.Contains(localPos);

                bool inArea = RectTransformUtility.RectangleContainsScreenPoint(rect, position, SceneView.lastActiveSceneView.camera);
                return inArea;
            }
            return false;
        }
    }

    //别直接删除，有东西没移上去
    /*
    [CustomEditor(typeof(RectTransform))]
    [CanEditMultipleObjects]
    public class LayoutEditor : Editor
    {

        private IMGUIContainer imNormalInspector;
        private Editor rectTransformEditor;
        private static VisualElement BarFromUXML;

        private void OnEnable()
        {
            //反射获取不公开的RectTransformEditor类
            //rectTransformEditor = CreateEditor(targets, typeof(Editor).Assembly.GetType("UnityEditor.RectTransformEditor"));
            rectTransformEditor = Utils.GetEditor(targets, "UnityEditor.RectTransformEditor");

            //初始化原始Inspector容器
            imNormalInspector = new IMGUIContainer(() =>
            {
                //利用反射获取RectTransformEditor类 绘制出和原Inspector一致的界面
                if (rectTransformEditor != null && targets != null && targets[0] != null)
                    rectTransformEditor.OnInspectorGUI();
            });
        }

        private void OnDisable()
        {
            if (rectTransformEditor != null)
            {
                DestroyImmediate(rectTransformEditor);
            }
            imNormalInspector.Dispose();
        }


        public override VisualElement CreateInspectorGUI()
        {

            //这是一段获取对象属性并绘制Inspector的代码 
            //该方法绘制的Inspector样式是为每个属性单独绘制的, 和默认样式区别较大，说不定以后可以用到
            //IMGUIContainer container = new IMGUIContainer();
            //var iterator = serializedObject.GetIterator();
            //if (iterator.NextVisible(true))
            //{
            //    do
            //    {
            //        var propertyField = new PropertyField(iterator.Copy()) { name = "PropertyField:" + iterator.propertyPath };
            //
            //        if (iterator.propertyPath == "m_Script" && serializedObject.targetObject != null)
            //            propertyField.SetEnabled(value: false);
            //
            //        container.Add(propertyField);
            //    }
            //    while (iterator.NextVisible(false));
            //}

            var container = new VisualElement();
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ThunderFireUIToolConfig.UIBuilderPath + "rectTransformButton.uxml");
            BarFromUXML = visualTree.CloneTree();
            container.Add(BarFromUXML);

            BarFromUXML.style.left = -20;
            BarFromUXML.style.width = 10000;

            //先绘制对齐工具按钮
            bool canAlign = AlignLogic.CanAlign();
            int index = 0;
            foreach (AlignType alignType in AlignType.GetValues(typeof(AlignType)))
            {
                string name = "UI_Align_" + alignType.ToString();
                EditorUIUtil.CreateUIEButton(BarFromUXML.Q<VisualElement>(name), () => { AlignLogic.Align(alignType); }, EditorLocalization.GetLocalization(AlignLogic.GetStringKeyByAlignType(alignType)), name, name + "_S");
                BarFromUXML.Q<VisualElement>(name).SetEnabled(canAlign);
                index++;
            }

            bool canGrid = AlignLogic.CanGrid();
            //绘制阵列工具按钮 和对齐按钮在同一行
            foreach (GridType gridType in GridType.GetValues(typeof(GridType)))
            {
                string name = "UI_Grid_" + gridType.ToString();
                EditorUIUtil.CreateUIEButton(BarFromUXML.Q<VisualElement>(name), () => { AlignLogic.Grid(gridType); }, EditorLocalization.GetLocalization(AlignLogic.GetStringKeyByGridType(gridType)), name, name + "_S");
                BarFromUXML.Q<VisualElement>(name).SetEnabled(canGrid);
                index++;
            }


            //将用于绘制原Inspector的区域的Container加入到root下
            //container.Add(base.CreateInspectorGUI());
            container.Add(imNormalInspector);
            imNormalInspector.style.top = 40;

            container.style.height = 250f;
            //container.style.backgroundColor = Color.white;

            return container;
        }
    }
    */
}

#endif