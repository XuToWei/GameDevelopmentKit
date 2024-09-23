using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ThunderFireUnityEx;

public class UIBeginnerGuide : UIBeginnerGuideBase
{
    public GuideHighLight highLightWidget;
    public GuideText textWidget;
    public GuideGesture gestureWidget;
    public UIBeginnerGuideGamePad GamePadWidget;
    public GuideArrowLine arrowLineWidget;
    public GuideTargetStroke targetStrokeWidget;

    [HideInInspector]
    public List<Transform> customObjects;

    private GuideHighLightData guideHighLightData;
    private GuideTextData guideTextData;
    private GuideGestureData guideGestureData;
    private GuideGamePadData gamePadData;
    private GuideArrowLineData guideArrowLineData;
    private GuideTargetStrokeData targetStrokeData;
    private List<GuideSelfDefinedData> selfDefinedDataList;
#if UNITY_EDITOR
    public void EditorInit()
    {
        if (gestureWidget != null)
        {
            if (!string.IsNullOrEmpty(guideData.guideGesturePanelData))
            {
                guideGestureData.EditorInit();
            }
        }
    }
#endif

    public override void Init(UIBeginnerGuideData data)
    {
        base.Init(data);

        if (!string.IsNullOrEmpty(guideData.guideTextPanelData) && textWidget != null)
        {
            guideTextData = GetComponentInChildren<GuideTextData>();
            guideTextData.Load(guideData.guideTextPanelData);
            textWidget.Init(guideTextData);
        }

        if (gestureWidget != null)
        {
            if (!string.IsNullOrEmpty(guideData.guideGesturePanelData))
            {
                guideGestureData = GetComponentInChildren<GuideGestureData>();
                guideGestureData.Load(guideData.guideGesturePanelData);
                guideGestureData.SetCustomGesturePrefab(guideData.GestureObject);
                guideGestureData.SetTarget(guideData.selectedObject);
                gestureWidget.SetCustomGesturePrefab(guideData.GestureObject);
                gestureWidget.Init(guideGestureData);
            }
            else
            {
                gestureWidget.Init(null);
            }
        }

        if (!string.IsNullOrEmpty(guideData.gamePadPanelData) && GamePadWidget != null)
        {
            gamePadData = GetComponentInChildren<GuideGamePadData>();
            gamePadData.Load(guideData.gamePadPanelData);
            //Todo init
        }

        if (!string.IsNullOrEmpty(guideData.targetStrokeData) && targetStrokeWidget != null)
        {
            targetStrokeData = GetComponentInChildren<GuideTargetStrokeData>();
            targetStrokeData.Load(guideData.targetStrokeData);
            if (targetStrokeData.targetType == TargetType.Target && guideData.strokeTarget)
            {
                targetStrokeData.SetTarget(guideData.strokeTarget);
                targetStrokeWidget.SetTarget(guideData.strokeTarget);
            }
            targetStrokeWidget.Init(targetStrokeData);
        }

        if (!string.IsNullOrEmpty(guideData.guideArrowLineData) && arrowLineWidget != null)
        {
            guideArrowLineData = GetComponentInChildren<GuideArrowLineData>();
            guideArrowLineData.Load(guideData.guideArrowLineData);
            arrowLineWidget.Init(guideArrowLineData);
        }

        if (!string.IsNullOrEmpty(guideData.guideHighLightData) && highLightWidget != null)
        {
            guideHighLightData = GetComponentInChildren<GuideHighLightData>();
            guideHighLightData.Load(guideData.guideHighLightData);
            highLightWidget.Init(guideHighLightData);
            if (guideData.highLightTarget)
            {
                guideHighLightData.SetTarget(guideData.highLightTarget);

            }
            highLightWidget.SetTarget(guideData.highLightTarget);

            highLightWidget.SetType(data.guideFinishType);
            highLightWidget.SetID(data.guideID);
        }

        if (!string.IsNullOrEmpty(guideData.gamePadPanelData) && GamePadWidget != null)
        {
            gamePadData = GetComponentInChildren<GuideGamePadData>();
            gamePadData.Load(guideData.gamePadPanelData);
            GamePadWidget.Init(gamePadData);
        }

        if (guideData.GuideSelfDefinedData != null && guideData.GuideSelfDefinedData.Count != 0)
        {
            foreach (string item in guideData.GuideSelfDefinedData)
            {
                GuideSelfDefinedData guideSelfDefinedData = new GuideSelfDefinedData();
                guideSelfDefinedData.Load(item);
                GameObject go = GameObject.Find(transform.name + guideSelfDefinedData.parentPath);
                // 如果父节点的Active为false，就会变成null
                if (go != null)
                {
                    GameObject gameObject = new GameObject(guideSelfDefinedData.name);
                    gameObject.transform.SetParent(go.transform);
                    gameObject.AddComponent<RectTransform>();
                    gameObject.AddComponent<Text>();
                    gameObject.GetComponent<Text>().text = guideSelfDefinedData.text;
                    gameObject.AddComponent<GuideSelfDefinedData>();
                    guideSelfDefinedData = gameObject.GetComponent<GuideSelfDefinedData>();
                    guideSelfDefinedData.Load(item);
                    gameObject.AddComponent<GuideSelfDefined>();
                    gameObject.GetComponent<GuideSelfDefined>().Init(guideSelfDefinedData);
                }
            }
        }

        InitCustomObject();
    }

    private void InitCustomObject()
    {
        if (!string.IsNullOrEmpty(guideData.CustomTransformDatas))
        {
            Dictionary<string, string> transformDatas = JsonUtilityEx.FromJson<string, string>(guideData.CustomTransformDatas);

            foreach (var kvp in transformDatas)
            {
                Transform trans = transform.Find(kvp.Key);
                if (trans != null)
                {
                    GuideTransformData data = trans.GetOrAddComponent<GuideTransformData>();
                    data.Load(kvp.Value);
                    data.ApplyTransformData(trans);
                    Object.DestroyImmediate(data);
                }
            }
        }

        if (!string.IsNullOrEmpty(guideData.CustomTextDatas))
        {
            Dictionary<string, string> textDatas = JsonUtilityEx.FromJson<string, string>(guideData.CustomTextDatas);

            foreach (var kvp in textDatas)
            {
                Transform trans = transform.Find(kvp.Key);
                if (trans != null)
                {
                    Text text = trans.GetComponent<Text>();
                    if (text != null)
                    {
                        JsonUtility.FromJsonOverwrite(kvp.Value, text);
                    }
                }
            }
        }

        if (!string.IsNullOrEmpty(guideData.CustomTextFont))
        {
            Dictionary<string, string> TextFont = JsonUtilityEx.FromJson<string, string>(guideData.CustomTextFont);
            foreach (var kvp in TextFont)
            {
                Transform trans = transform.Find(kvp.Key);
                if (trans != null)
                {
                    Text text = trans.GetComponent<Text>();
                    if (text != null)
                    {
                        text.font = ResourceManager.Load<Font>(TextFont[kvp.Key]);
                    }
                }
            }
        }

        if (!string.IsNullOrEmpty(guideData.CustomImageDatas))
        {
            Dictionary<string, string> imageDatas = JsonUtilityEx.FromJson<string, string>(guideData.CustomImageDatas);

            foreach (var kvp in imageDatas)
            {
                Transform trans = transform.Find(kvp.Key);
                if (trans != null)
                {
                    Image image = trans.GetComponent<Image>();
                    if (image != null)
                    {
                        JsonUtility.FromJsonOverwrite(kvp.Value, image);
                    }
                }
            }
        }

        if (!string.IsNullOrEmpty(guideData.CustomImagesprite))
        {
            Dictionary<string, string> imageSprite = JsonUtilityEx.FromJson<string, string>(guideData.CustomImagesprite);
            foreach (var kvp in imageSprite)
            {
                Transform trans = transform.Find(kvp.Key);
                if (trans != null)
                {
                    Image image = trans.GetComponent<Image>();
                    if (image != null)
                    {
                        image.sprite = ResourceManager.Load<Sprite>(imageSprite[kvp.Key]);
                    }
                }
            }
        }
    }

    public override void Show()
    {
        base.Show();
        gestureWidget?.Show();
        GamePadWidget?.Show();
    }

    public override void Finish()
    {
        base.Finish();
        gestureWidget?.Stop();
        highLightWidget?.Stop();
        textWidget?.Stop();
        arrowLineWidget?.Stop();
        targetStrokeWidget?.Stop();
        GamePadWidget?.Stop();
    }
#if UNITY_EDITOR
    #region Test

    public void HighLightAreaPreview(RectTransform target)
    {

    }
    #endregion
#endif
}
