using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEditor;


/// <summary>
/// 材质描边组件。
/// </summary>
[DisallowMultipleComponent]
public sealed class UXOutline : BaseMeshEffect
{
    public Material outlineMaterial;

    #region Mono

    protected override void Awake()
    {
        base.Awake();
#if UNITY_EDITOR
        outlineMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/UXTools/Res/UX-GUI/Res/Material/Outline.mat");
#endif
        Font.textureRebuilt += OnFontRebuilt;
    }

    //字体扩容的时候unity有bug，不会赋值_MainTex_TexelSize，需要手动dirty下
    private void OnFontRebuilt(Font font)
    {
        if (this == null || graphic == null)
        {
            return;
        }

        //TODO 后面改成修改源码的方式
        Text text = graphic as Text;
        if (text == null)
        {
            return;
        }

        Texture tex = text.mainTexture;
        graphic.canvasRenderer.SetTexture(null);
        graphic.canvasRenderer.SetTexture(tex);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Font.textureRebuilt -= OnFontRebuilt;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        RefreshGraphic();
    }

    protected override void OnDisable()
    {
        if (graphic)
            graphic.material = null;
        base.OnDisable();
    }

    private void RefreshGraphic()
    {
        if (graphic)
        {
            graphic.material = outlineMaterial;
            graphic.SetVerticesDirty();
        }
    }

    private void RefreshCanvasSettings()
    {
        if (graphic)
        {
            var canvas = graphic.canvas;
            if (canvas)
            {
                canvas.additionalShaderChannels = (AdditionalCanvasShaderChannels)(~0);
            }
        }
    }

    #endregion

    #region Outline

    

#if ODIN_INSPECTOR
    [LabelText("Color")]
#endif
    [SerializeField] private Color m_EffectColor = new Color(0, 0, 0, 1);

    /// <summary>
    /// 描边颜色。
    /// </summary>
    public Color effectColor
    {
        get { return m_EffectColor; }
        set
        {
            if (m_EffectColor != value)
            {
                m_EffectColor = value;
                if (graphic != null)
                    graphic.SetVerticesDirty();
            }
        }
    }

  #if ODIN_INSPECTOR
    [LabelText("Width")]
    [Range(0, 50)]
#endif
    [SerializeField]
    private float m_EffectWidth = 2;

    /// <summary>
    /// 描边宽度。
    /// </summary>
    public float effectWidth
    {
        get { return m_EffectWidth; }
        set
        {
            if (m_EffectWidth != value)
            {
                m_EffectWidth = value;
                if (graphic != null)
                    graphic.SetVerticesDirty();
            }
        }
    }

  #if ODIN_INSPECTOR
    [LabelText("Expand")]
    [Range(0, 50)]
#endif
    [SerializeField]
    private float m_EffectExpand;

    /// <summary>
    /// 描边扩展。
    /// </summary>
    public float effectExpand
    {
        get { return m_EffectExpand; }
        set
        {
            if (m_EffectExpand != value)
            {
                m_EffectExpand = value;
                if (graphic != null)
                    graphic.SetVerticesDirty();
            }
        }
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
            return;

        RefreshCanvasSettings();
        ModifyTextMesh(vh);
    }

    private float oldPosX = -99999;
    private float offsetX = 0;
    private void ModifyTextMesh(VertexHelper vh)
    {
        var col_rg = new Vector2(effectColor.r, effectColor.g);
        var col_ba = new Vector4(0, 0, effectColor.b, effectColor.a);
        if (graphic == null)
        {
            Debug.LogError("UMatOutline graphic == null");
            return;
        }
        Text text = graphic as Text;
        var realSize = text.cachedTextGenerator.fontSizeUsedForBestFit;
        var targetSize = text.fontSize;

        var rate = targetSize != 0?(float)realSize/(float)targetSize:0;
        var normal = new Vector3(0, 0, effectWidth*rate);
        //var normal = new Vector3(0, 0, effectWidth);
        var v1 = default(UIVertex);
        var v2 = default(UIVertex);
        var v3 = default(UIVertex);
        var v4 = default(UIVertex);
        // oldPosX = -99999;
        // offsetX = 0;
        
        for (int i = 0, len = vh.currentVertCount - 3; i < len; i += 4)
        {

            vh.PopulateUIVertex(ref v1, i);
            vh.PopulateUIVertex(ref v2, i + 1);
            vh.PopulateUIVertex(ref v3, i + 2);
            vh.PopulateUIVertex(ref v4, i + 3);

            var posCenter = Vector2.Lerp(v1.position, v3.position, 0.5f);
            Vector2 uvX, uvY;
            float triX, triY;
            Vector2 pos1 = v1.position;
            Vector2 pos2 = v2.position;
            Vector2 pos3 = v3.position;
            Vector2 pos4 = v4.position;
            float v2x = pos2.x;
            //文本顶点从左上开始
            triX = pos2.x - pos1.x;
            triY = pos3.y - pos2.y;
            uvX = v2.uv0 - v1.uv0;
            uvY = v3.uv0 - v2.uv0;

            //计算原始UV框
            var uvMin = Min(v1.uv0, v2.uv0, v3.uv0, v4.uv0);
            var uvMax = Max(v1.uv0, v2.uv0, v3.uv0, v4.uv0);

            v1.uv1 = uvMin;
            v1.uv2 = uvMax;
            v1.uv3 = col_rg;
            v1.tangent = col_ba;
            v1.normal = normal;

            v2.uv1 = uvMin;
            v2.uv2 = uvMax;
            v2.uv3 = col_rg;
            v2.tangent = col_ba;
            v2.normal = normal;

            v3.uv1 = uvMin;
            v3.uv2 = uvMax;
            v3.uv3 = col_rg;
            v3.tangent = col_ba;
            v3.normal = normal;

            v4.uv1 = uvMin;
            v4.uv2 = uvMax;
            v4.uv3 = col_rg;
            v4.tangent = col_ba;
            v4.normal = normal;

            // var offset = oldPosX - pos1.x;
            // if (offset>0){
            //     offsetX += offset;
            // }
            

            v1 = SetNewPosAndUV(v1, effectExpand*rate, posCenter, triX, triY, uvX, uvY, offsetX);
            v2 = SetNewPosAndUV(v2, effectExpand*rate, posCenter, triX, triY, uvX, uvY, offsetX);
            v3 = SetNewPosAndUV(v3, effectExpand*rate, posCenter, triX, triY, uvX, uvY, offsetX);
            v4 = SetNewPosAndUV(v4, effectExpand*rate, posCenter, triX, triY, uvX, uvY, offsetX);

            oldPosX = v2x;
            vh.SetUIVertex(v1, i);
            vh.SetUIVertex(v2, i + 1);
            vh.SetUIVertex(v3, i + 2);
            vh.SetUIVertex(v4, i + 3);
        }
    }

    // private static readonly System.Func<UIVertex, float, Vector2, float, float, Vector2, Vector2, UIVertex>[]
    //     s_SlicedVert1Func = new System.Func<UIVertex, float, Vector2, float, float, Vector2, Vector2, UIVertex>[]
    //     {
    //         SetNewPosAndUV,
    //         SetNewPosAndUV_X,
    //         SetNewPosAndUV_X,
    //         SetNewPosAndUV_Y,
    //         SetNewUV,
    //         SetNewUV,
    //         SetNewPosAndUV_Y,
    //         SetNewUV,
    //         SetNewUV,
    //     };

    // private static readonly System.Func<UIVertex, float, Vector2, float, float, Vector2, Vector2, UIVertex>[]
    //     s_SlicedVert2Func = new System.Func<UIVertex, float, Vector2, float, float, Vector2, Vector2, UIVertex>[]
    //     {
    //         SetNewPosAndUV_X,
    //         SetNewPosAndUV_X,
    //         SetNewPosAndUV,
    //         SetNewUV,
    //         SetNewUV,
    //         SetNewPosAndUV_Y,
    //         SetNewUV,
    //         SetNewUV,
    //         SetNewPosAndUV_Y,
    //     };

    // private static readonly System.Func<UIVertex, float, Vector2, float, float, Vector2, Vector2, UIVertex>[]
    //     s_SlicedVert3Func = new System.Func<UIVertex, float, Vector2, float, float, Vector2, Vector2, UIVertex>[]
    //     {
    //         SetNewUV,
    //         SetNewUV,
    //         SetNewPosAndUV_Y,
    //         SetNewUV,
    //         SetNewUV,
    //         SetNewPosAndUV_Y,
    //         SetNewPosAndUV_X,
    //         SetNewPosAndUV_X,
    //         SetNewPosAndUV,
    //     };

    // private static readonly System.Func<UIVertex, float, Vector2, float, float, Vector2, Vector2, UIVertex>[]
    //     s_SlicedVert4Func = new System.Func<UIVertex, float, Vector2, float, float, Vector2, Vector2, UIVertex>[]
    //     {
    //         SetNewPosAndUV_Y,
    //         SetNewUV,
    //         SetNewUV,
    //         SetNewPosAndUV_Y,
    //         SetNewUV,
    //         SetNewUV,
    //         SetNewPosAndUV,
    //         SetNewPosAndUV_X,
    //         SetNewPosAndUV_X,
    //     };

    

    private static readonly List<UIVertex> s_VetexList = new List<UIVertex>();
    

    #endregion

    #region Utils

    private static UIVertex SetNewPosAndUV(UIVertex pVertex, float pOutLineWidth, Vector2 pPosCenter, float pTriangleX,
        float pTriangleY, Vector2 pUVX, Vector2 pUVY, float oldX)
    {
        //Position
        Vector3 pos = pVertex.position;
        float posXOffset = pos.x > pPosCenter.x ? pOutLineWidth : -pOutLineWidth;
        float posYOffset = pos.y > pPosCenter.y ? pOutLineWidth : -pOutLineWidth;
        pos.x += posXOffset;
        pos.y += posYOffset;
        
        // pos.x += oldX;
        //限制最小Pos
        // if (oldX!=-99999 && pos.x < oldX) {
        //     pos.x = oldX;
        //     posXOffset = pos.x - pVertex.position.x;
        // }
        pVertex.position = pos;
        //UV
        Vector2 uv = pVertex.uv0;
        uv += pUVX / pTriangleX * posXOffset;
        uv += pUVY / pTriangleY * posYOffset;
        pVertex.uv0 = uv;
        return pVertex;
    }

    private static UIVertex SetNewPosAndUV_X(UIVertex pVertex, float pOutLineWidth, Vector2 pPosCenter,
        float pTriangleX, float pTriangleY, Vector2 pUVX, Vector2 pUVY)
    {
        //Position
        Vector3 pos = pVertex.position;
        float posXOffset = pos.x > pPosCenter.x ? pOutLineWidth : -pOutLineWidth;
        float posYOffset = pos.y > pPosCenter.y ? pOutLineWidth : -pOutLineWidth;
        pos.x += posXOffset;
        pVertex.position = pos;
        //UV
        Vector2 uv = pVertex.uv0;
        uv += pUVX / pTriangleX * posXOffset;
        pVertex.uv0 = uv;
        return pVertex;
    }

    private static UIVertex SetNewPosAndUV_Y(UIVertex pVertex, float pOutLineWidth, Vector2 pPosCenter,
        float pTriangleX, float pTriangleY, Vector2 pUVX, Vector2 pUVY)
    {
        //Position
        Vector3 pos = pVertex.position;
        float posXOffset = pos.x > pPosCenter.x ? pOutLineWidth : -pOutLineWidth;
        float posYOffset = pos.y > pPosCenter.y ? pOutLineWidth : -pOutLineWidth;
        pos.y += posYOffset;
        pVertex.position = pos;
        //UV
        Vector2 uv = pVertex.uv0;
        uv += pUVY / pTriangleY * posYOffset;
        pVertex.uv0 = uv;
        return pVertex;
    }

    private static UIVertex SetNewUV(UIVertex pVertex, float pOutLineWidth, Vector2 pPosCenter, float pTriangleX,
        float pTriangleY, Vector2 pUVX, Vector2 pUVY)
    {
        return pVertex;
    }

    private static float Min(float pA, float pB, float pC, float pD)
    {
        return Mathf.Min(Mathf.Min(Mathf.Min(pA, pB), pC), pD);
    }

    private static float Max(float pA, float pB, float pC, float pD)
    {
        return Mathf.Max(Mathf.Max(Mathf.Max(pA, pB), pC), pD);
    }

    private static Vector2 Min(Vector2 pA, Vector2 pB, Vector2 pC, Vector2 pD)
    {
        return new Vector2(Min(pA.x, pB.x, pC.x, pD.x), Min(pA.y, pB.y, pC.y, pD.y));
    }

    private static Vector2 Max(Vector2 pA, Vector2 pB, Vector2 pC, Vector2 pD)
    {
        return new Vector2(Max(pA.x, pB.x, pC.x, pD.x), Max(pA.y, pB.y, pC.y, pD.y));
    }

    private static UIVertex SetNewPosAndUV(UIVertex pVertex, float pOutLineWidth, Vector2 pPosCenter,
        Vector2 pTriangleX, Vector2 pTriangleY, Vector2 pUVX, Vector2 pUVY, Vector2 pUVOriginMin, Vector2 pUVOriginMax)
    {
        //Position
        Vector3 pos = pVertex.position;
        float posXOffset = pos.x > pPosCenter.x ? pOutLineWidth : -pOutLineWidth;
        float posYOffset = pos.y > pPosCenter.y ? pOutLineWidth : -pOutLineWidth;
        pos.x += posXOffset;
        pos.y += posYOffset;
        pVertex.position = pos;
        //UV
        Vector2 uv = pVertex.uv0;
        uv += pUVX / pTriangleX.magnitude * posXOffset * (Vector2.Dot(pTriangleX, Vector2.right) > 0 ? 1 : -1);
        uv += pUVY / pTriangleY.magnitude * posYOffset * (Vector2.Dot(pTriangleY, Vector2.up) > 0 ? 1 : -1);
        pVertex.uv0 = uv;
        pVertex.uv1 = pUVOriginMin;
        pVertex.uv2 = pUVOriginMax;
        return pVertex;
    }

    private static float Min(float pA, float pB, float pC)
    {
        return Mathf.Min(Mathf.Min(pA, pB), pC);
    }

    private static float Max(float pA, float pB, float pC)
    {
        return Mathf.Max(Mathf.Max(pA, pB), pC);
    }

    private static Vector2 Min(Vector2 pA, Vector2 pB, Vector2 pC)
    {
        return new Vector2(Min(pA.x, pB.x, pC.x), Min(pA.y, pB.y, pC.y));
    }

    private static Vector2 Max(Vector2 pA, Vector2 pB, Vector2 pC)
    {
        return new Vector2(Max(pA.x, pB.x, pC.x), Max(pA.y, pB.y, pC.y));
    }

    #endregion

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        if (!IsActive())
            return;
        RefreshGraphic();
        RefreshCanvasSettings();
    }
#endif
}
