using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GestureType
{
    ThumbClick,
    ThumbDrag,
    ThumbLongPress,
    ThumbRotate,
    ThumbSlideUp,
    ThumbSlideDown,
    ThumbSlideLeft,
    ThumbSlideRight,
    ForeFingerClick,
    ForeFingerDrag,
    ForeFingerLongPress,
    ForeFingerRotate,
    ForeFingerSlideUp,
    ForeFingerSlideDown,
    ForeFingerSlideLeft,
    ForeFingerSlideRight
}
public enum ObjectSelectType
{
    auto,
    select
}

public class GuideGesture : GuideWidgetBase
{
    private GameObject GestureAnimation;
    private Animator GestureAnimator;
    public GameObject CustomGestureObject;
    public GestureType gesType = GestureType.ThumbClick;
    public float dragduration;

    private GuideGestureData gestureData;
    private AnimationCurve dragCurve;
    private Vector3 dragStartPos;
    private Vector3 dragEndPos;

    private IEnumerator dragTween;
    // void Update(){
    //     if(gestureData.selectedObject!=null){
    //         transform.SetPositionAndRotation(gestureData.selectedObject.transform.position, gestureData.selectedObject.transform.rotation);
    //     }
    // }

    public override void Init(GuideWidgetData data)
    {
        gestureData = data as GuideGestureData;
        if (gestureData != null)
        {
            gestureData.ApplyTransformData(transform);
            if (gestureData.objectSelectType == ObjectSelectType.select && gestureData.selectedObject != null)
            {
                transform.position = gestureData.selectedObject.transform.position;
            }
            if (gestureData.Open)
            {
                if (gestureData.UseCustomGesture == true)
                {
                    GestureAnimator = CustomGestureObject.GetComponent<Animator>();
                    LoadCustomGesture(CustomGestureObject);
                    return;
                }
                dragduration = gestureData.duration;
                gesType = gestureData.gestureType;
                dragCurve = gestureData.dragCurve;
                dragStartPos = gestureData.dragStartPos;
                dragEndPos = gestureData.dragEndPos;
                LoadGesture(gesType);

                GestureAnimator = GestureAnimation.GetComponent<Animator>();
                if (gestureData.gestureType == GestureType.ThumbDrag || gestureData.gestureType == GestureType.ForeFingerDrag)
                {
                    transform.localPosition = dragStartPos;
                }
            }
        }
        else
        {
            LoadGesture(gesType);
        }
    }

    public override List<int> GetControlledInstanceIds()
    {
        Transform[] allChild = gameObject.GetComponentsInChildren<Transform>(true);
        List<int> list = new List<int>();
        foreach (var child in allChild)
        {
            list.Add(child.GetInstanceID());
        }
        Transform startPosControllerTrans = this.transform.Find("StartPosController");
        Transform endPosControllerTrans = this.transform.Find("EndPosController");
        if (startPosControllerTrans != null)
        {
            list.Add(startPosControllerTrans.GetInstanceID());
        }
        if (endPosControllerTrans != null)
        {
            list.Add(endPosControllerTrans.GetInstanceID());
        }
        return list;
    }

    public override void Show()
    {
        PlayAnimation();
    }

    public override void Stop()
    {
        StopAnimation();
    }

    public GameObject LoadGesture(GestureType type)
    {
        GameObject go = GestureAnimation;

        if (type == GestureType.ThumbClick)
        {
            GestureAnimation = Instantiate(ResourceManager.Load<GameObject>("Assets/Res/UI/UXTool/Feature/BeginnerGuide/Gesture/clickPrefab_thumb.prefab"), transform);
        }
        else if (type == GestureType.ThumbDrag)
        {
            GestureAnimation = Instantiate(ResourceManager.Load<GameObject>("Assets/Res/UI/UXTool/Feature/BeginnerGuide/Gesture/dragPrefab_thumb.prefab"), transform);
        }
        else if (type == GestureType.ThumbLongPress)
        {
            GestureAnimation = Instantiate(ResourceManager.Load<GameObject>("Assets/Res/UI/UXTool/Feature/BeginnerGuide/Gesture/longclickPrefab_thumb.prefab"), transform);
        }
        else if (type == GestureType.ThumbRotate)
        {
            GestureAnimation = Instantiate(ResourceManager.Load<GameObject>("Assets/Res/UI/UXTool/Feature/BeginnerGuide/Gesture/rotate_thumb.prefab"), transform);
        }
        else if (type == GestureType.ThumbSlideDown)
        {
            GestureAnimation = Instantiate(ResourceManager.Load<GameObject>("Assets/Res/UI/UXTool/Feature/BeginnerGuide/Gesture/slideDown_thumb.prefab"), transform);
        }
        else if (type == GestureType.ThumbSlideUp)
        {
            GestureAnimation = Instantiate(ResourceManager.Load<GameObject>("Assets/Res/UI/UXTool/Feature/BeginnerGuide/Gesture/slideUp_thumb.prefab"), transform);
        }
        else if (type == GestureType.ThumbSlideLeft)
        {
            GestureAnimation = Instantiate(ResourceManager.Load<GameObject>("Assets/Res/UI/UXTool/Feature/BeginnerGuide/Gesture/slideLeft_thumb.prefab"), transform);
        }
        else if (type == GestureType.ThumbSlideRight)
        {
            GestureAnimation = Instantiate(ResourceManager.Load<GameObject>("Assets/Res/UI/UXTool/Feature/BeginnerGuide/Gesture/slideRight_thumb.prefab"), transform);
        }
        else if (type == GestureType.ForeFingerClick)
        {
            GestureAnimation = Instantiate(ResourceManager.Load<GameObject>("Assets/Res/UI/UXTool/Feature/BeginnerGuide/Gesture/clickPrefab_forefinger.prefab"), transform);
        }
        else if (type == GestureType.ForeFingerDrag)
        {
            GestureAnimation = Instantiate(ResourceManager.Load<GameObject>("Assets/Res/UI/UXTool/Feature/BeginnerGuide/Gesture/dragPrefab_forefinger.prefab"), transform);
        }
        else if (type == GestureType.ForeFingerLongPress)
        {
            GestureAnimation = Instantiate(ResourceManager.Load<GameObject>("Assets/Res/UI/UXTool/Feature/BeginnerGuide/Gesture/longclickPrefab_forefinger.prefab"), transform);
        }
        else if (type == GestureType.ForeFingerRotate)
        {
            GestureAnimation = Instantiate(ResourceManager.Load<GameObject>("Assets/Res/UI/UXTool/Feature/BeginnerGuide/Gesture/rotate_forefinger.prefab"), transform);
        }
        else if (type == GestureType.ForeFingerSlideDown)
        {
            GestureAnimation = Instantiate(ResourceManager.Load<GameObject>("Assets/Res/UI/UXTool/Feature/BeginnerGuide/Gesture/slideDown_forefinger.prefab"), transform);
        }
        else if (type == GestureType.ForeFingerSlideUp)
        {
            GestureAnimation = Instantiate(ResourceManager.Load<GameObject>("Assets/Res/UI/UXTool/Feature/BeginnerGuide/Gesture/slideUp_forefinger.prefab"), transform);
        }
        else if (type == GestureType.ForeFingerSlideLeft)
        {
            GestureAnimation = Instantiate(ResourceManager.Load<GameObject>("Assets/Res/UI/UXTool/Feature/BeginnerGuide/Gesture/slideLeft_forefinger.prefab"), transform);
        }
        else if (type == GestureType.ForeFingerSlideRight)
        {
            GestureAnimation = Instantiate(ResourceManager.Load<GameObject>("Assets/Res/UI/UXTool/Feature/BeginnerGuide/Gesture/slideRight_forefinger.prefab"), transform);
        }

        Object.DestroyImmediate(go);
        return GestureAnimation;
    }
    public void LoadCustomGesture(GameObject obj)
    {
        GameObject go = GestureAnimation;
        GestureAnimation = Instantiate(obj, transform);
        Object.DestroyImmediate(go);
    }

    public void PlayAnimation()
    {
        if (gestureData == null)
        {
            return;
        }
        if (gestureData.UseCustomGesture == true)
        {
            GestureAnimator.Play("Base Layer.start");
            return;
        }
        if (GestureAnimator != null)
        {
            GestureAnimator.Play("start");
        }
        //只有拖动手势需要额外的位移动画
        if (gestureData.gestureType == GestureType.ThumbDrag || gestureData.gestureType == GestureType.ForeFingerDrag)
        {
            if (dragduration > 0)
                StartCoroutine(dragTween = DOMoveLoops(dragEndPos, dragduration, dragCurve));
            else
                StartCoroutine(dragTween = DOMoveLoops(dragEndPos, 3, dragCurve));
            //Debug.Log("dragEndPos" + dragEndPos.ToString());
            // dragTween = transform.DOLocalMove(dragEndPos, 3).SetEase(dragCurve).SetLoops(-1);
        }
    }

    private IEnumerator DOMoveLoops(Vector3 endValue, float duration, AnimationCurve animationCurve)
    {
        Vector3 startPos = transform.position;
        while (true)
        {
            transform.position = startPos;
            StartCoroutine(DOLocalMove(endValue, duration, animationCurve));
            yield return new WaitForSeconds(duration);
        }
    }

    private IEnumerator DOLocalMove(Vector3 endValue, float duration, AnimationCurve animationCurve)
    {
        float startTime = Time.time;
        Vector3 startPos = transform.localPosition;
        while (Time.time - startTime <= duration)
        {
            transform.localPosition = startPos + (endValue - startPos) * animationCurve.Evaluate((Time.time - startTime) / duration);
            yield return null;
        }
    }

    public void StopAnimation()
    {
        if (GestureAnimator != null)
        {
            GestureAnimator.speed = 0;
        }
        //只有拖动手势需要关闭额外的位移动画
        if ((gestureData.gestureType == GestureType.ThumbDrag || gestureData.gestureType == GestureType.ForeFingerDrag) && dragTween != null)
        {
            StopCoroutine(dragTween);
        }
    }
    public void SetCustomGesturePrefab(GameObject go)
    {
        CustomGestureObject = go;
    }
}
