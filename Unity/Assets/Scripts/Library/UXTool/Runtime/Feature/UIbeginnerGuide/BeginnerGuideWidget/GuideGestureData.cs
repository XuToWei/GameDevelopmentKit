
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class GuideGestureData : GuideWidgetData
{
    //使用自定义的引导动画
    public bool UseCustomGesture = false;
    public GameObject GestureObject;

    //手势类型
    public GestureType gestureType = GestureType.ThumbClick;
    //跟随物体
    public ObjectSelectType objectSelectType = ObjectSelectType.auto;
    public GameObject selectedObject;

    /// <summary>
    /// 只有drag类型才需要
    /// </summary>
    public float duration;
    public Vector3 dragStartPos;
    public Vector3 dragEndPos;
    public AnimationCurve dragCurve;
    public string startPosName;
    public string endPosName;


    public override string Serialize()
    {
        UpdateTransformData();
#if UNITY_EDITOR
        UpdateDragPos();
#endif
        string data = JsonUtility.ToJson(this);
        return data;
    }
    public void SetCustomGesturePrefab(GameObject prefab)
    {
        GestureObject = prefab;
    }

    public void SetTarget(GameObject target)
    {
        selectedObject = target;
    }
#if UNITY_EDITOR
    private void UpdateDragPos()
    {
        if (gestureType == GestureType.ThumbDrag || gestureType == GestureType.ForeFingerDrag)
        {
            var startPosControllerTrans = transform.Find(startPosName);
            var endPosControllerTrans = transform.Find(endPosName);
            if (startPosControllerTrans != null && endPosControllerTrans != null)
            {
                dragStartPos = transform.parent.InverseTransformPoint(startPosControllerTrans.position);
                dragEndPos = transform.parent.InverseTransformPoint(endPosControllerTrans.position);
            }
        }
    }

#endif



#if UNITY_EDITOR

    private GameObject dragStartPosController;
    private GameObject dragEndPosController;

    public void EditorInit()
    {
        if (gestureType == GestureType.ThumbDrag || gestureType == GestureType.ForeFingerDrag)
        {
            ShowDragEditorController();
        }
    }

    public void ShowDragEditorController()
    {
        Transform startPosControllerTrans = transform.Find(startPosName);
        Transform endPosControllerTrans = transform.Find(endPosName);

        if (startPosControllerTrans == null || endPosControllerTrans == null)
        {
            GameObject controllerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Res/Editor/UXTool/Feature/BeginnerGuide/Prefab/PosController.prefab");

            dragStartPosController = Instantiate(controllerPrefab, transform);
            dragEndPosController = Instantiate(controllerPrefab, transform);

            dragStartPosController.name = startPosName;
            dragStartPosController.gameObject.hideFlags = HideFlags.DontSave;
            dragStartPosController.transform.position = transform.parent.TransformPoint(dragStartPos);

            dragEndPosController.name = endPosName;
            dragEndPosController.gameObject.hideFlags = HideFlags.DontSave;
            dragEndPosController.transform.position = transform.parent.TransformPoint(dragEndPos);
        }
        else
        {
            dragStartPosController = startPosControllerTrans.gameObject;
            dragEndPosController = endPosControllerTrans.gameObject;
        }
    }

#endif
}
