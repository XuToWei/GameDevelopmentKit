using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// GuideWidgetData的基类
/// </summary>
[Serializable]
public abstract class GuideWidgetData : MonoBehaviour
{
    [SerializeField, HideInInspector]
    public bool Saved = false;
    //protected Vector3 Pos;
    [SerializeField, HideInInspector]
    protected Vector3 anchoredPos;
    [SerializeField, HideInInspector]
    public Vector2 offsetMax;
    [SerializeField, HideInInspector]
    public Vector2 offsetMin;
    [SerializeField, HideInInspector]
    public Vector2 anchorMin;
    [SerializeField, HideInInspector]
    public Vector2 anchorMax;
    [SerializeField, HideInInspector]
    public Vector2 pivot;
    [SerializeField, HideInInspector]
    protected Vector3 Scale;
    [SerializeField, HideInInspector]
    protected Vector3 Rotation;
    [SerializeField, HideInInspector]
    protected Vector2 Size;

    public bool Open;

    public void Load(string savedData)
    {
        JsonUtility.FromJsonOverwrite(savedData, this);
    }

    public void UpdateTransformData()
    {
        //Pos = transform.position;
        anchoredPos = GetComponent<RectTransform>().anchoredPosition3D;
        offsetMax = GetComponent<RectTransform>().offsetMax;
        offsetMin = GetComponent<RectTransform>().offsetMin;
        anchorMax = GetComponent<RectTransform>().anchorMax;
        anchorMin = GetComponent<RectTransform>().anchorMin;
        pivot = GetComponent<RectTransform>().pivot;
        Scale = transform.localScale;
        Rotation = transform.eulerAngles;
        Size = GetComponent<RectTransform>().sizeDelta;
        Open = gameObject.activeSelf;
    }
    public void ApplyTransformData(Transform targetTransform)
    {
        if (Saved)
        {
            //targetTransform.position = Pos;
            targetTransform.localScale = Scale;
            targetTransform.eulerAngles = Rotation;
            targetTransform.GetComponent<RectTransform>().sizeDelta = Size;

            targetTransform.GetComponent<RectTransform>().anchoredPosition3D = anchoredPos;
            targetTransform.GetComponent<RectTransform>().offsetMax = offsetMax;
            targetTransform.GetComponent<RectTransform>().offsetMin = offsetMin;
            targetTransform.GetComponent<RectTransform>().anchorMax = anchorMax;
            targetTransform.GetComponent<RectTransform>().anchorMin = anchorMin;
            targetTransform.GetComponent<RectTransform>().pivot = pivot;
        }

        targetTransform.gameObject.SetActive(Open);
    }

    public abstract string Serialize();
}

/// <summary>
/// 以JsonString的形式保存一个Guide中用到的所有的Widget的Data
/// 临时持有一下需要的TargetGameObject
/// </summary>
[Serializable]
public class UIBeginnerGuideData
{
    //本次引导ID
    public string guideID;

    //引导结束类型
    public GuideFinishType guideFinishType;
    public float guideFinishDuration;

    //引导模板
    public GameObject guideTemplatePrefab;

    //黑幕镂空数据
    public string guideHighLightData = "";
    //需要镂空的UI控件
    public GameObject highLightTarget;

    //文本引导数据
    public string guideTextPanelData = "";

    // 手势引导数据
    public string guideGesturePanelData = "";
    //自定义的手势Prefab
    public GameObject GestureObject;
    //手势跟随对象
    public GameObject selectedObject;
    // 手柄动画专用数据
    public string gamePadPanelData = "";

    //Target Stroke相关
    public string targetStrokeData = "";
    //描边框跟随对象
    public GameObject strokeTarget;

    //引导箭头相关
    public string guideArrowLineData = "";

    //用户自定义相关
    public List<string> GuideSelfDefinedData = new List<string>();

    //其他非Widget的GameObject的Transform
    public string CustomTransformDatas;

    //其他非Widget的GameObject的Text
    public string CustomTextFont;
    public string CustomTextDatas;

    //其他非Widget的GameObject的Image
    public string CustomImagesprite;
    public string CustomImageDatas;
    public bool UseOwnPrefab = false;
    public GuideTemplate Template = GuideTemplate.t1;
}

public class UIBeginnerGuideDataList : MonoBehaviour
{
    [SerializeField]
    public string guid;

    public List<UIBeginnerGuideData> guideDataList = new List<UIBeginnerGuideData>();

    //Temp: 临时作为UI出现时触发开关,实际应该都由用户手动调用引导出现
    private void Start()
    {
        //UIBeginnerGuideManager.Instance.AddGuide(this);
        //UIBeginnerGuideManager.Instance.ShowGuideList();
    }
}
