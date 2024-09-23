#if UNITY_EDITOR
using UnityEngine;

//Editor中预览使用
public class UIBeginnerGuidePreviewLauncher : MonoBehaviour
{
    public UIBeginnerGuideDataList guideList;
    public string previewGuideId;

    void Start()
    {
        UIBeginnerGuideManager.Instance.ClearGuideList();
        UIBeginnerGuideManager.Instance.AddGuideList(guideList);
        UIBeginnerGuideManager.Instance.SetGuideID(previewGuideId);
        UIBeginnerGuideManager.Instance.ShowGuideList();
        UIBeginnerGuideManager.Instance.isPreviewing = true;
    }
}
#endif