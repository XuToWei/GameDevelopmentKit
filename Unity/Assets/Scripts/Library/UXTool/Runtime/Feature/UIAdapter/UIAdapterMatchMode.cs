using UnityEngine;
using UnityEngine.UI;

public class UIAdapterMatchMode : MonoBehaviour
{
    //确保1920/1080比例下按高度适配,不能取整
    public static float designAspectRatio = 1.77f; // (float)1920 / 1080 = 1.777; (float)1334/750 = 1.778
    void Awake()
    {
        CanvasScaler canvasScaler = GetComponent<CanvasScaler>();
        if (canvasScaler == null) return;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float aspectRatio = screenWidth / screenHeight;
        if (aspectRatio >= designAspectRatio)
        {
            //长屏
            canvasScaler.matchWidthOrHeight = 1;
        }
        else
        {
            //短屏
            canvasScaler.matchWidthOrHeight = 0;
        }
    }
}