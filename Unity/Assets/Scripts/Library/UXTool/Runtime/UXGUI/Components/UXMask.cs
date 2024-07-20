using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UXMask : Mask
{
    [SerializeField]
    private bool m_IsReverseMask = false;
    public bool IsReverseMask
    {
        get { return m_IsReverseMask; }
        set
        {
            if (m_IsReverseMask == value)
                return;

            m_IsReverseMask = value;
            if (graphic != null)
                graphic.SetMaterialDirty();
        }
    }

    [SerializeField]
    private bool m_UseSpriteAlpha = false;
    public bool UseSpriteAlpha
    {
        get { return m_UseSpriteAlpha; }
        set
        {
            if (m_UseSpriteAlpha == value)
                return;
            m_UseSpriteAlpha = value;

            if (graphic != null)
            {
                SetSpriteAlpha();
                graphic.SetMaterialDirty();
            }


        }
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        SetSpriteAlpha();

    }
    public void SetSpriteAlpha()
    {

        var rect = GetComponent<RectTransform>();
        Rect rectInScreen = RectTransformToScreenSpace(rect);
        var image = GetComponent<Image>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var childTrans = transform.GetChild(i);
            var childImage = childTrans.GetComponent<UXImage>();
            var childMat = new Material(childImage.material);
            if (!m_UseSpriteAlpha)
            {
                childMat.SetFloat("_IsCalAlpha", 0);
                childMat.DisableKeyword("IS_CALALPHA");
                childImage.material = childMat;
                continue;
            }
            childMat.SetFloat("_IsCalAlpha", 1);
            childMat.EnableKeyword("IS_CALALPHA");
            Rect childRectInScreen = RectTransformToScreenSpace(childTrans.GetComponent<RectTransform>());
            Rect overlap = RectOverlap(rectInScreen, childRectInScreen);
            if (overlap != Rect.zero)
            {

                Texture2D lowerTexture = image.sprite.texture as Texture2D;
                childMat.SetTexture("_AlphaTex", lowerTexture);
                var selfUmin = (overlap.xMin - childRectInScreen.xMin) / childRectInScreen.width;
                var selfUmax = (overlap.xMax - childRectInScreen.xMin) / childRectInScreen.width;
                var selfVmin = (overlap.yMin - childRectInScreen.yMin) / childRectInScreen.height;
                var selfVmax = (overlap.yMax - childRectInScreen.yMin) / childRectInScreen.height;

                var alphaUmin = (overlap.xMin - rectInScreen.xMin) / rectInScreen.width;
                var alphaUmax = (overlap.xMax - rectInScreen.xMin) / rectInScreen.width;
                var alphaVmin = (overlap.yMin - rectInScreen.yMin) / rectInScreen.height;
                var alphaVmax = (overlap.yMax - rectInScreen.yMin) / rectInScreen.height;
                childMat.SetVector("_SelfUV", new Vector4(selfUmin, selfVmin, selfUmax, selfVmax));
                childMat.SetVector("_AlphaUV", new Vector4(alphaUmin, alphaVmin, alphaUmax, alphaVmax));
                childImage.material = childMat;
            }
        }
    }
    Rect RectTransformToScreenSpace(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        Rect screenSpaceRect = new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);
        return screenSpaceRect;
    }
    Rect RectOverlap(Rect rect1, Rect rect2)
    {
        float x1 = Mathf.Max(rect1.x, rect2.x);
        float y1 = Mathf.Max(rect1.y, rect2.y);
        float x2 = Mathf.Min(rect1.x + rect1.width, rect2.x + rect2.width);
        float y2 = Mathf.Min(rect1.y + rect1.height, rect2.y + rect2.height);
        return new Rect(x1, y1, Mathf.Max(0, x2 - x1), Mathf.Max(0, y2 - y1));
    }
}
