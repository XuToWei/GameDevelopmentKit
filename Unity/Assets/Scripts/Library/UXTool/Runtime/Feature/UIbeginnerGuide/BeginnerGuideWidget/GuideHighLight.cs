using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public enum HighLightType
{
    Circle,
    Square
}
public class GuideHighLight : GuideWidgetBase, ICanvasRaycastFilter, IPointerClickHandler
{
    private GuideFinishType guideFinishType;
    private string guideID;
    private bool isCircle = true; //rect at first
    private bool isVague = false;
    private bool UseCustomTarget;
    private RectTransform target;
    
    private Vector3 center;     // 镂空区域的中心
    private float width;        // 镂空区域的宽
    private float height;       // 镂空区域的高
    private Canvas canvas;
    
    public GameObject childObject;
    public Material rectMaterial;
    public Material circleMaterial;

    private Vector3[] targetCorners = new Vector3[4];//存储要镂空组件的四个角的数组

    public Vector2 WorldToScreenPoint(Canvas canvas, Vector3 world)
    {
        //把世界坐标转化为屏幕坐标
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, world);

        // 屏幕坐标转换为局部坐标
        //out的是vector2类型，事先声明
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(),
                                            screenPoint, canvas.worldCamera, out localPoint);
        return localPoint;
    }

    public override void Init(GuideWidgetData data)
    {
        GuideHighLightData guideHighLightData = data as GuideHighLightData;
        if (guideHighLightData != null)
        {
            guideHighLightData.ApplyTransformData(transform);
            if (guideHighLightData.highLightType == HighLightType.Circle)
            {
                isCircle = true;
            }
            else isCircle = false;

            isVague = guideHighLightData.isVague;
            UseCustomTarget = guideHighLightData.UseCustomTarget;
            //GetComponent<RectGuide>().target = guideHighLightData.target;
            childObject.transform.localPosition = guideHighLightData.childPos;
            childObject.transform.eulerAngles = guideHighLightData.childRot;
            childObject.transform.localScale = guideHighLightData.childScale;
            childObject.transform.GetComponent<RectTransform>().sizeDelta = guideHighLightData.childSize;
        }

    }

    public override List<int> GetControlledInstanceIds()
    {
        List<int> list = new List<int>();

        return list;
    }

    public void SetType(GuideFinishType type)
    {
        this.guideFinishType = type;
    }

    private void SetRectHighLightArea()
    {
        InitTarget();
        //设置材质的中心点
        rectMaterial.SetVector("_Center", center);
        //设置材质的宽高
        rectMaterial.SetFloat("_SliderX", width);
        rectMaterial.SetFloat("_SliderY", height);
    }
    private void SetCircleHighLightArea()
    {
        InitTarget();
        circleMaterial.SetVector("_Center", center);
        circleMaterial.SetFloat("_SliderX", width);
        circleMaterial.SetFloat("_SliderY", height);

    }
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        if (target == null) { return true; }//点击不了
        // if (RectTransformUtility.RectangleContainsScreenPoint(target, sp))
        // {
        //     return false;
        // }
        return true;


    }
    public void finish()
    {
         //延迟5秒执行
        UIBeginnerGuideManager.Instance.FinishGuide(guideID);
    }
    public void SetTarget(GameObject go)
    {
        if(go==null){
            target = childObject.GetComponent<RectTransform>();
        }
        else{
        if (UseCustomTarget)
        {
            childObject.transform.position = go.transform.position;
            childObject.transform.eulerAngles = go.transform.eulerAngles;
            childObject.GetComponent<RectTransform>().sizeDelta = new Vector2(go.GetComponent<RectTransform>().rect.width, go.GetComponent<RectTransform>().rect.height);
            childObject.transform.localScale = go.transform.localScale;
            target = go.GetComponent<RectTransform>();
        }
        else
        {
            target = childObject.GetComponent<RectTransform>();
        }
        }
        //Debug.Log(target.position);
        InitTarget();
        if (isCircle)
        {
            transform.GetComponent<Image>().material = circleMaterial;
            SetCircleHighLightArea();
        }
        else
        {
            transform.GetComponent<Image>().material = rectMaterial;
            SetRectHighLightArea();
        }

    }
    public void SetID(string id)
    {
        guideID = id;
    }
    private void InitTarget()
    {
        canvas = transform.GetComponentInParent<Canvas>();
        if (target == null) return;
        // 获取中心点
        // GetWorldCorners:在世界空间中得到计算的矩形的角。参数角的数组
        target.GetWorldCorners(targetCorners);

        // 讲四个角的世界坐标转为局部坐标坐标
        for (int i = 0; i < targetCorners.Length; i++)
        {
            targetCorners[i] = WorldToScreenPoint(canvas, targetCorners[i]);
        }

        //计算中心点// 计算宽高
        center.x = targetCorners[0].x + (targetCorners[3].x - targetCorners[0].x) / 2;
        center.y = targetCorners[0].y + (targetCorners[1].y - targetCorners[0].y) / 2;
        width = (targetCorners[3].x - targetCorners[0].x) / 2;
        height = (targetCorners[1].y - targetCorners[0].y) / 2;
    }
    public override void Show()
    {
        if (guideFinishType == GuideFinishType.Strong)
        {

        }
        else if (guideFinishType == GuideFinishType.Middle)
        {

        }
    }

    public override void Stop()
    {
    }
    public void OnPointerClick(PointerEventData data)
    {
        if (guideFinishType == GuideFinishType.Middle)// || guideFinishType == GuideFinishType.Weak)
        {
            UIBeginnerGuideManager.Instance.FinishGuide(guideID);
        }
        // else if (guideFinishType == GuideFinishType.Strong)
        // {
        //     // Vector2 sp = Input.mousePosition;
        //     // Debug.Log(sp);
        //     // //Debug.Log(target.)
        //     // if (RectTransformUtility.RectangleContainsScreenPoint(target, sp))
        //     // {
        //     //     UIBeginnerGuideManager.Instance.FinishGuide(guideID);
        //     // }
        //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //     RaycastHit hit;
        //     if (Physics.Raycast(ray, out hit))
        //     {
        //         Debug.Log(hit.collider.gameObject.name);
        //         // obj = hit.collider.gameObject;
        //         // //通过名字
        //         // if (obj.name.Equals("BeiJiChuan"))
        //         // {
        //         //     Debug.Log("点中" + obj.name);
        //         // }
        //         // //通过标签
        //         // if (obj.tag == "ClicObj")
        //         // {
        //         //     Debug.Log("点中" + obj.name);
        //         // }
        //     }
        // }

    }
}