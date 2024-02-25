using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GuideFinishType
{
    Strong, //强引导,黑幕镂空,必须点击镂空区域结束
    Middle, //中引导,黑幕镂空,点击任意区域结束
    Weak    //弱引导,无黑幕，设定时间过后自动结束
}
public enum GuideTemplate{
    t0,
    t1,
    // t2,
    // t3,
    // t4
}

/// <summary>
/// 所有引导模板上挂的UIBeginnerGuide的基类
/// 新建的引导模板Prefab上都应该挂一个派生出的UIBeginnerGuide类型
/// </summary>
public class UIBeginnerGuideBase : MonoBehaviour
{
    protected UIBeginnerGuideData guideData;
    protected string guideId;
    protected GuideFinishType guideFinishType;
    protected float guideFinishDuration;
    protected GameObject guidePrefab;
    public virtual void Init(UIBeginnerGuideData data)
    {
        guideData = data;

        guideId = guideData.guideID;
        guideFinishType = guideData.guideFinishType;
        guideFinishDuration = guideData.guideFinishDuration;
        guidePrefab = guideData.guideTemplatePrefab;
    }

    public virtual void Show()
    {
        
    }

    public virtual void Finish()
    {
    }
}
