using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAdapterAutoFitScale : MonoBehaviour
{
   
    RectTransform rectTrans;

    void Start()
    {
        AdaptBackGround();
    }

    void CompareBorder(float selfRectValue, float canvasRectValue, ref float scale)
    {
        if (selfRectValue < canvasRectValue && selfRectValue != 0)
        {
            float tmpScale = canvasRectValue / selfRectValue;
            if (scale < tmpScale)
            {
                scale = tmpScale;
            }
        }
    }


    void AdaptBackGround()
    {
        rectTrans = GetComponent<RectTransform>();
        if (rectTrans == null)
        {
            return;
        }

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            return;
        }
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        if (canvasRect == null)
        {
            return;
        }

        if (rectTrans.parent == null)
        {
            return;
        }

        float orginWidth = rectTrans.rect.width;
        float orginHeight = rectTrans.rect.height;

        Vector2 anchor = new Vector2(0.5f, 0.5f);
        rectTrans.anchorMin = anchor;
        rectTrans.anchorMax = anchor;
        Vector2 oldSelfLocalPos = rectTrans.localPosition;
        rectTrans.position = canvasRect.position;
        //计算中心点的偏移
        float centerOffset_x = oldSelfLocalPos.x - rectTrans.localPosition.x;
        float centerOffset_y = oldSelfLocalPos.y - rectTrans.localPosition.y;
        rectTrans.localPosition = oldSelfLocalPos;

        float scale = 1;
        //右边界
        CompareBorder(orginWidth * (1 - rectTrans.pivot.x), canvasRect.rect.width / 2f - centerOffset_x, ref scale);
        //左边界
        CompareBorder(orginWidth * rectTrans.pivot.x, canvasRect.rect.width / 2f + centerOffset_x, ref scale);
        //上下边界
        CompareBorder(orginHeight * (1 - rectTrans.pivot.y), canvasRect.rect.height / 2f - centerOffset_y, ref scale);
        CompareBorder(orginHeight * rectTrans.pivot.y, canvasRect.rect.height / 2f + centerOffset_y, ref scale);

        rectTrans.localScale = new Vector3(scale, scale, 1);
    }
}
