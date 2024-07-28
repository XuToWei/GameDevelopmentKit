using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAdapterPlatform : MonoBehaviour
{
    public float Mobile_AdapterScale = 1;
    public float PC_AdapterScale = 1;
    public float GameConsole_AdapterScale = 1;

    private Vector3 originScale;
    private float platformScale = 1;
    private bool inited = false;

    private void OnEnable()
    {
        if(!inited)
        {
            var transform = GetComponent<RectTransform>();
            if (transform != null)
            {
                originScale = transform.localScale;
                inited = true;
            }
        }

        InitPlatformScale();
        ApplyPlatformScale();
    }

    private void  InitPlatformScale()
    {
        switch(UXPlatform.GetPlatformType())
        {
            case UXPlatform.PlatformType.PC:
                platformScale = PC_AdapterScale;
                break;
            case UXPlatform.PlatformType.Mobile:
                platformScale = Mobile_AdapterScale;
                break;
            case UXPlatform.PlatformType.Console:
                platformScale = GameConsole_AdapterScale;
                break;
            default:
                platformScale = Mobile_AdapterScale;
                break;
        }
    }
    
    private void ApplyPlatformScale()
    {
        var transform = GetComponent<RectTransform>();
        if (transform != null)
        {
            transform.localScale = originScale * platformScale;
        }
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        if(inited)
        {
            ApplyPlatformScale();
        }
    }
#endif

}

