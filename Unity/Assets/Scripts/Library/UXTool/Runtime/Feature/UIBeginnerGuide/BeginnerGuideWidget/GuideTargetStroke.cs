using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1000)]
public class GuideTargetStroke : GuideWidgetBase
{
    private GameObject target;
    public GameObject square;
    public GameObject circle;
    public override void Init(GuideWidgetData data)
    {
        // changed by gdk
        needUpdateTarget = false;
        GuideTargetStrokeData targetStrokeData = data as GuideTargetStrokeData;
        if (targetStrokeData != null)
        {
            targetStrokeData.ApplyTransformData(transform);
            if (targetStrokeData.targetType == TargetType.Target && target != null)
            {
                transform.position = target.transform.position;
                transform.eulerAngles = target.transform.eulerAngles;
                transform.localScale = target.transform.localScale;
                transform.GetComponent<RectTransform>().sizeDelta = target.GetComponent<RectTransform>().sizeDelta;
                // changed by gdk
                needUpdateTarget = true;
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

    // changed by gdk
    private bool needUpdateTarget;
    private Vector3 targetPosition;
    private void LateUpdate()
    {
        if (needUpdateTarget)
        {
            RectTransform rectTransform = transform.GetComponent<RectTransform>();
            RectTransform targetRectTransform = target.GetComponent<RectTransform>();
            rectTransform.position = targetRectTransform.position;
            rectTransform.eulerAngles = targetRectTransform.eulerAngles;
            rectTransform.localScale = targetRectTransform.localScale;
            rectTransform.sizeDelta = targetRectTransform.sizeDelta;
        }
    }
}
