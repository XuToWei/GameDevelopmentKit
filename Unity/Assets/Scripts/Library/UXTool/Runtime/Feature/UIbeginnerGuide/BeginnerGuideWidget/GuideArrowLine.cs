using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LineType{
    NoLines,
    OneLine,
    TwoLine
}
public class GuideArrowLine : GuideWidgetBase
{
    private LineType lineType;
    public GameObject arrow;
    public GameObject oneLine;
    public GameObject twoLine;
    public GameObject oneLineArrow;
    public GameObject twoLineArrow;
    public override void Init(GuideWidgetData data)
    {
        GuideArrowLineData arrowLineData = data as GuideArrowLineData;
        //throw new System.NotImplementedException();
        if (arrowLineData != null)
        {
            arrowLineData.ApplyTransformData(transform);
            lineType = arrowLineData.lineType;
            SetChild();
            if(arrowLineData.smallArrowData!=""){
                Transform obj; 
                if(arrowLineData.lineType==LineType.OneLine){
                    obj = oneLineArrow.transform;
                }
                else obj=twoLineArrow.transform;
                SmallArrowData arrowData = obj.GetComponent<SmallArrowData>();
                arrowData.Load(arrowLineData.smallArrowData);
                arrowData.ApplyTransformData(obj);
            }
        }
    }

    public override List<int> GetControlledInstanceIds()
    {
        List<int> list = new List<int>();

        return list;
    }

    public void SetChild()
    {
        arrow.SetActive(false);
        oneLine.SetActive(false);
        twoLine.SetActive(false);
        switch (lineType){
            case LineType.NoLines:
                arrow.SetActive(true);
                break;
            case LineType.OneLine:
                oneLine.SetActive(true);
                break;
            case LineType.TwoLine:
                twoLine.SetActive(true);
                break;
        }
    }

    public override void Show()
    {
        
    }

    public override void Stop()
    {
       
    }
    public void changeAct(int i, int j){
        switch (i){
            case 0:
                arrow.SetActive(true);
                break;
            case 1:
                oneLine.SetActive(true);
                break;
            case 2:
                twoLine.SetActive(true);
                break;
        }
        switch (j){
            case 0:
                arrow.SetActive(false);
                break;
            case 1:
                oneLine.SetActive(false);
                break;
            case 2:
                twoLine.SetActive(false);
                break;
        }
    }
}
