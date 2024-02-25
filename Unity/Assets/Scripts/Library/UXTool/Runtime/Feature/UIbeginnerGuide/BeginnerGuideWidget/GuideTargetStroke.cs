using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideTargetStroke : GuideWidgetBase
{
    private GameObject target;
    public GameObject square;
    public GameObject circle;
    public override void Init(GuideWidgetData data)
    {
        GuideTargetStrokeData targetStrokeData = data as GuideTargetStrokeData;
        if (targetStrokeData != null)
        {
            targetStrokeData.ApplyTransformData(transform); 
            if (targetStrokeData.targetType == TargetType.Target && target != null) 
            {
                transform.position = target.transform.position; 
                transform.eulerAngles= target.transform.eulerAngles;
                transform.localScale = target.transform.localScale;
                transform.GetComponent<RectTransform>().sizeDelta = target.GetComponent<RectTransform>().sizeDelta;
            } 
            square.SetActive(targetStrokeData.strokeType == StrokeType.Square); 
            circle.SetActive(targetStrokeData.strokeType == StrokeType.Circle);
            square.GetComponent<Animator>().enabled = targetStrokeData.playAnimator;
            circle.GetComponent<Animator>().enabled = targetStrokeData.playAnimator;
        }
    }

    public override List<int> GetControlledInstanceIds()
    {
        List<int> list = new List<int>();

        return list;
    }


    public void SetTarget(GameObject go)
    {
        target = go;
    }
    public override void Show()
    {
    }
    public override void Stop()
    {
    }
}
