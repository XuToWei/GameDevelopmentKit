using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIBeginnerGuideManager : MonoBehaviour
{
    private static UIBeginnerGuideManager instance;
    public static UIBeginnerGuideManager Instance
    {
        get
        {
            return instance;
        }
    }
    public bool isPreviewing;

    private List<UIBeginnerGuideDataList> guideDataList = new List<UIBeginnerGuideDataList>();
    private UIBeginnerGuideDataList curGuideList;
    private UIBeginnerGuideData curGuideData;
    private UIBeginnerGuide curGuide;
    private string targetID;

    // private bool guideShowing = false;
    // private bool GuideShowing { get { return guideShowing; } }
    private void Awake()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        instance = null;
    }
    public void SetGuideID(string id)
    {
        if (isPreviewing)
        {
            return;
        }
        targetID = id;
    }
    public void AddGuideList(UIBeginnerGuideDataList datalist)
    {
        if (isPreviewing)
        {
            return;
        }
        int listIndex = guideDataList.IndexOf(datalist);
        if (listIndex == -1)
        {
            guideDataList.Add(datalist);
        }
        // if (GuideShowing)
        //     return;

        //ShowGuideList(datalist);
    }
    public void ClearGuideList()
    {
        guideDataList.Clear();
    }
    public void RemoveGuideList(UIBeginnerGuideDataList dataList)
    {
        guideDataList.Remove(dataList);
    }
    public void ShowGuideList()
    {
        if (isPreviewing)
        {
            return;
        }
        if (guideDataList.Count != 0)
        {
            ShowGuideList(guideDataList[0]);
        }
    }
    public void ShowGuideList(UIBeginnerGuideDataList datalist)
    {
        if (isPreviewing)
        {
            return;
        }
        int listIndex = guideDataList.IndexOf(datalist);
        if (listIndex == -1)
        {
            return;
        }

        curGuideList = datalist;
        if (curGuideList.guideDataList.Count != 0)
        {
            if (string.IsNullOrEmpty(targetID))
            {
                ShowGuide(curGuideList.guideDataList.First());
            }
            else
            {
                var targetGuideList = curGuideList.guideDataList.Where(data => data.guideID == targetID).ToList();
                if (targetGuideList.Count != 0)
                {
                    targetID = null;
                    var targetGuide = targetGuideList.First();
                    if (targetGuide == null)
                    {
                        StartNextGuide();
                    }
                    ShowGuide(targetGuide);
                }
            }
        }
        else
        {
            StartNextGuide();
        }
    }
    public void ShowGuideList(UIBeginnerGuideDataList datalist, string guideID)
    {
        if (isPreviewing)
        {
            return;
        }
        targetID = guideID;
        int listIndex = guideDataList.IndexOf(datalist);
        if (listIndex == -1)
        {
            return;
        }
        curGuideList = datalist;
        if (curGuideList.guideDataList.Count != 0)
        {
            if (string.IsNullOrEmpty(targetID))
            {
                ShowGuide(curGuideList.guideDataList.First());
            }
            else
            {
                var targetGuideList = curGuideList.guideDataList.Where(data => data.guideID == targetID).ToList();
                if (targetGuideList.Count != 0)
                {
                    targetID = null;
                    var targetGuide = targetGuideList.First();
                    if (targetGuide == null)
                    {
                        StartNextGuide();
                    }
                    ShowGuide(targetGuide);
                }
            }
        }
        else
        {
            StartNextGuide();
        }
    }
    private void ShowGuide(UIBeginnerGuideData data)
    {
        curGuideData = data;

        var guideGo = Instantiate(curGuideData.guideTemplatePrefab, curGuideList.transform);
        curGuide = guideGo.GetComponent<UIBeginnerGuide>();
        curGuide.Init(curGuideData);
        curGuide.Show();
        if (curGuideData.guideFinishType == GuideFinishType.Weak)
        {
            StartCoroutine(RegisterAutoFinish(curGuideData.guideFinishDuration, curGuideData.guideID));
        }
    }
    public void FinishGuide(string guideId)
    {
        if (curGuideData.guideID == guideId)
        {
            curGuide.Finish();
            DestroyImmediate(curGuide.gameObject);
            StartNextGuide();
        }
    }
    public void FinishGuide()
    {
        curGuide.Finish();
        DestroyImmediate(curGuide.gameObject);
        StartNextGuide();
    }
    private IEnumerator RegisterAutoFinish(float duration, string ID)
    {
        yield return new WaitForSeconds(duration);
        if (curGuideData != null && ID == curGuideData.guideID)
            FinishGuide(curGuideData.guideID);
    }
    private void StartNextGuide()
    {
        int index = curGuideList.guideDataList.IndexOf(curGuideData);

        if (index < curGuideList.guideDataList.Count - 1)
        {
            //一个List没完成,只切换data
            index++;
            ShowGuide(curGuideList.guideDataList[index]);
        }
        else
        {
            int listIndex = guideDataList.IndexOf(curGuideList);
            if (listIndex < guideDataList.Count - 1)
            {
                //还有剩余的guideDataList没完成,切换到下一个List
                listIndex++;
                ShowGuideList(guideDataList[listIndex]);
            }
            else
            {
                //也没有其他的guideDataList了,结束引导,等待新的guidedatalist
                //guideShowing = false;
                curGuide = null;
                curGuideData = null;
            }
        }
    }
}
