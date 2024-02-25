using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAdapter : MonoBehaviour
{
    [HideInInspector]
    public Vector2 oldAnchorMin;
    [HideInInspector]
    public Vector2 oldAnchorMax;
    [HideInInspector]
    public Vector2 anchorMin;
    [HideInInspector]
    public Vector2 anchorMax;
    RectTransform Panel;
    Rect LastSafeArea = new Rect(0, 0, 0, 0);

    public static float designAspectRatio = (float)1920 / 1080;

    public static Rect GetSafeArea()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float aspectRatio = screenWidth / screenHeight;
        Rect safeArea = Screen.safeArea;
        if(Application.isPlaying)
        {
            //是否长屏
            if (aspectRatio > designAspectRatio)
            {
                //有些机型左侧没有安全区,这里加个iOS右侧安全区默认值,方便交互统一设计
                if (safeArea.x == 0)
                {
                    safeArea.x += 102;
                    safeArea.width -= 102;
                }

                //有些机型右侧没有安全区,这里加个iOS右侧安全区默认值,方便交互统一设计
                if (safeArea.width + safeArea.x == screenWidth)
                {
                    safeArea.width -= 102;
                }
            }
        }
        
        return safeArea;
    }

    void Awake()
    {
        Panel = GetComponent<RectTransform>();
        oldAnchorMin = Panel.anchorMin;
        oldAnchorMax = Panel.anchorMax;
        Refresh();

    }

    public void Refresh()
    {
        Rect safeArea = GetSafeArea();
        safeArea.height = safeArea.height + safeArea.y;
        safeArea.y = 0;

        if (safeArea != LastSafeArea)
            ApplySafeArea(safeArea);
    }

    void ApplySafeArea(Rect r)
    {
        UnityEngine.Profiling.Profiler.BeginSample($"UXTool UIAdapter {gameObject.name}");
        LastSafeArea = r;

        // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
        anchorMin = r.position;
        anchorMax = r.position + r.size;
        // If not put in update, the screen.width will return the width of the window size rather than the simulator size
        // Quote Doc :
        // In a built application, or when the Device Simulator isn’t active, the simulated classes have the same behavior as their counterparts in the UnityEngine namespace.
#if UNITY_2021_1_OR_NEWER
        anchorMin.x /= UnityEngine.Device.Screen.width;
        anchorMin.y /= UnityEngine.Device.Screen.height;
        anchorMax.x /= UnityEngine.Device.Screen.width;
        anchorMax.y /= UnityEngine.Device.Screen.height;
#else
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
#endif

        Panel.anchorMin = anchorMin;
        Panel.anchorMax = anchorMax;
        UnityEngine.Profiling.Profiler.EndSample();
    }
}