using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace ThunderFireUITool
{

    [CustomEditor(typeof(GuideGestureData))]
    public class GuideGestureDataEditor : Editor
    {
        SerializedProperty m_duration;
        SerializedProperty m_Open;
        SerializedProperty m_UseCustomGesture;
        SerializedProperty m_GestureType;
        SerializedProperty m_GestureObject;

        SerializedProperty m_DragStartPos;
        SerializedProperty m_DragEndPos;
        SerializedProperty m_DragCurve;

        GuideGesture guide;
        GuideGestureData data;
        SerializedProperty m_objectSelectType;
        SerializedProperty m_selectedObject;

        private bool GestureOpenOld;
        private bool UseCustomGestureOld;
        private int GestureTypeOld;
        private Object GestureObjectOld;

        private GameObject dragStartPosController;
        private GameObject dragEndPosController;

        private SerializedProperty startPosName;
        private SerializedProperty endPosName;
        private void OnEnable()
        {
            m_duration = serializedObject.FindProperty("duration");
            m_Open = serializedObject.FindProperty("Open");
            m_UseCustomGesture = serializedObject.FindProperty("UseCustomGesture");
            m_GestureType = serializedObject.FindProperty("gestureType");
            m_GestureObject = serializedObject.FindProperty("GestureObject");
            m_DragStartPos = serializedObject.FindProperty("dragStartPos");
            m_DragEndPos = serializedObject.FindProperty("dragEndPos");
            m_DragCurve = serializedObject.FindProperty("dragCurve");
            m_objectSelectType = serializedObject.FindProperty("objectSelectType");
            m_selectedObject = serializedObject.FindProperty("selectedObject");

            startPosName = serializedObject.FindProperty("startPosName");

            endPosName = serializedObject.FindProperty("endPosName");

            data = target as GuideGestureData;
            guide = data.gameObject.GetComponent<GuideGesture>();
            if( string.IsNullOrEmpty(AssetDatabase.GetAssetPath(data.gameObject)))
            {
                //Debug.Log(data.gestureType);
                //Instantiate(ResourceManager.Load<GameObject>("Gesture/longclickPrefab_thumb"), (Selection.activeObject as GameObject).transform);
                if(data.transform.childCount==0)
                {
                    GameObject go = guide.LoadGesture(data.gestureType);
                    go.hideFlags = HideFlags.DontSave;
                    if (m_GestureType.intValue == (int)GestureType.ThumbDrag || m_GestureType.intValue == (int)GestureType.ForeFingerDrag)
                    {
                        ShowDragEditorController();
                    }
                }
            }
        }
        
        private void OnDisable()
        {
            //HideDragEditorController();
        }
        /// <summary>
        /// 显示拖动动画的编辑按钮
        /// </summary>
        private void ShowDragEditorController()
        {
            Transform startPosControllerTrans = data.transform.Find(startPosName.stringValue);
            Transform endPosControllerTrans = data.transform.Find(endPosName.stringValue);

            if (startPosControllerTrans == null || endPosControllerTrans == null)
            {
                GameObject controllerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{ThunderFireUIToolConfig.UXEditorFeaturePath}BeginnerGuide/Prefab/PosController.prefab");

                dragStartPosController = Instantiate(controllerPrefab, data.transform);
                dragEndPosController = Instantiate(controllerPrefab, data.transform);

                dragStartPosController.name = startPosName.stringValue;
                dragStartPosController.gameObject.hideFlags = HideFlags.DontSave;
                dragStartPosController.transform.position = data.transform.parent.TransformPoint(m_DragStartPos.vector3Value);

                dragEndPosController.name = endPosName.stringValue;
                dragEndPosController.gameObject.hideFlags = HideFlags.DontSave;
                dragEndPosController.transform.position = data.transform.parent.TransformPoint(m_DragEndPos.vector3Value);
            }
            else
            {
                dragStartPosController = startPosControllerTrans.gameObject;
                dragEndPosController = endPosControllerTrans.gameObject;
            }
        }

        private void HideDragEditorController()
        {
            //因销毁引起的disable时，整个对象都已经没了，不用再删除dragposController了
            if (data == null) return;

            Transform startPosControllerTrans = data.transform.Find(startPosName.stringValue);
            Transform endPosControllerTrans = data.transform.Find(endPosName.stringValue);
            //Debug.Log(startPosName.stringValue);

            if (startPosControllerTrans)
            {
                DestroyImmediate(startPosControllerTrans.gameObject);
            }

            if (endPosControllerTrans)
            {
                DestroyImmediate(endPosControllerTrans.gameObject);
            }

            dragStartPosController = null;
            dragEndPosController = null;
        }
        public override void OnInspectorGUI()
        {
            //EditorGUILayout.PropertyField(m_DragCurve,new GUIContent(EditorLocalization.GetLocalization("GestureData","DragCurve")));
            //只在project窗口中选中时，不显示编辑器
            if (!guide.gameObject.scene.isLoaded)
            {
                return;
            }

            EditorGUI.BeginChangeCheck();

            GestureOpenOld = m_Open.boolValue;
            UseCustomGestureOld = m_UseCustomGesture.boolValue;
            GestureTypeOld = m_GestureType.intValue;
            GestureObjectOld = m_GestureObject.objectReferenceValue;

            serializedObject.Update();
            startPosName.stringValue = EditorLocalization.GetLocalization("GestureData", "StartPosController");
            endPosName.stringValue = EditorLocalization.GetLocalization("GestureData", "EndPosController");


            EditorGUILayout.PropertyField(m_Open, new GUIContent(EditorLocalization.GetLocalization("UIBeginnerGuide", "OpenComponent")));
            EditorGUILayout.PropertyField(m_UseCustomGesture, new GUIContent(EditorLocalization.GetLocalization("GestureData", "UseCustomGesture")));
            if (m_UseCustomGesture.boolValue == false)
            {
                string[] strs = {EditorLocalization.GetLocalization("GestureData","ThumbClick"),EditorLocalization.GetLocalization("GestureData","ThumbDrag"),
                EditorLocalization.GetLocalization("GestureData","ThumbLongPress"),EditorLocalization.GetLocalization("GestureData","ThumbRotate"),
                EditorLocalization.GetLocalization("GestureData","ThumbSlideUp"),EditorLocalization.GetLocalization("GestureData","ThumbSlideDown"),
                EditorLocalization.GetLocalization("GestureData","ThumbSlideLeft"),EditorLocalization.GetLocalization("GestureData","ThumbSlideRight"),
                EditorLocalization.GetLocalization("GestureData","ForeFingerClick"),EditorLocalization.GetLocalization("GestureData","ForeFingerDrag"),
                EditorLocalization.GetLocalization("GestureData","ForeFingerLongPress"),EditorLocalization.GetLocalization("GestureData","ForeFingerRotate"),
                EditorLocalization.GetLocalization("GestureData","ForeFingerSlideUp"),EditorLocalization.GetLocalization("GestureData","ForeFingerSlideDown"),
                EditorLocalization.GetLocalization("GestureData","ForeFingerSlideLeft"),EditorLocalization.GetLocalization("GestureData","ForeFingerSlideRight"),
                };
                m_GestureType.intValue = Utils.EnumPopupLayoutEx(EditorLocalization.GetLocalization("GestureData", "gesture"), typeof(GestureType), m_GestureType.intValue, strs);
                if (m_GestureType.intValue == (int)GestureType.ThumbDrag || m_GestureType.intValue == (int)GestureType.ForeFingerDrag)
                {
                    if (dragStartPosController != null && dragEndPosController != null)
                    {
                        m_DragStartPos.vector3Value = data.transform.parent.InverseTransformPoint(dragStartPosController.transform.position);
                        m_DragEndPos.vector3Value = data.transform.parent.InverseTransformPoint(dragEndPosController.transform.position);
                    }
                    //SceneHierarchyUtility.SetExpanded(guide.gameObject, true);

                    EditorGUILayout.PropertyField(m_duration, new GUIContent(EditorLocalization.GetLocalization("GestureData", "duration")));
                    EditorGUILayout.PropertyField(m_DragCurve, new GUIContent(EditorLocalization.GetLocalization("GestureData", "DragCurve")));


                }


            }
            else if (m_UseCustomGesture.boolValue == true)
            {
                EditorGUILayout.PropertyField(m_GestureObject, new GUIContent(EditorLocalization.GetLocalization("GestureData", "customGesture")));
            }
            string[] labels = { EditorLocalization.GetLocalization("GestureData", "auto"), EditorLocalization.GetLocalization("GestureData", "select") };
            m_objectSelectType.intValue = Utils.EnumPopupLayoutEx(EditorLocalization.GetLocalization("GestureData", "objecttype"), typeof(ObjectSelectType),
            m_objectSelectType.intValue, labels);
            //EditorGUILayout.PropertyField(m_objectSelectType,new GUIContent(EditorLocalization.GetLocalization("GestureData", "objecttype")));
            if (m_objectSelectType.intValue == (int)ObjectSelectType.select)
            {
                EditorGUILayout.PropertyField(m_selectedObject, new GUIContent(EditorLocalization.GetLocalization("GestureData", "selectedObject")));
                if (m_selectedObject.objectReferenceValue != null)
                {
                    guide.gameObject.transform.SetPositionAndRotation(((GameObject)m_selectedObject.objectReferenceValue).transform.position, ((GameObject)m_selectedObject.objectReferenceValue).transform.rotation);
                }
            }
            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                if (GestureOpenOld != m_Open.boolValue)
                {
                    data.gameObject.SetActive(m_Open.boolValue);
                }
                if (m_UseCustomGesture.boolValue == true && GestureObjectOld != m_GestureObject.objectReferenceValue)
                {
                    guide.LoadCustomGesture((GameObject)m_GestureObject.objectReferenceValue);
                }
                if (GestureTypeOld != m_GestureType.intValue)
                {
                    GameObject go = guide.LoadGesture((GestureType)m_GestureType.intValue);
                    go.hideFlags = HideFlags.DontSave;

                    if (m_GestureType.intValue == (int)GestureType.ThumbDrag || m_GestureType.intValue == (int)GestureType.ForeFingerDrag)
                    {
                        ShowDragEditorController();
                    }
                    else
                    {
                        HideDragEditorController();
                    }
                }
            }
        }
    }
}
