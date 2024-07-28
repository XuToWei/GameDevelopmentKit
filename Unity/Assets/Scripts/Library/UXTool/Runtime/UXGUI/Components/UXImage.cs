
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.Rendering;

namespace UnityEngine.UI
{

    public class UXImage : Image, ILocalization
    {
        [SerializeField]
        private bool m_ignoreLocalization = true;
        public bool ignoreLocalization { get { return m_ignoreLocalization; } set { m_ignoreLocalization = value; } }

        public new Sprite sprite
        {
            get
            {
                return base.sprite;
            }
            set
            {
                base.sprite = value;
                ChangeLanguage(LocalizationHelper.GetLanguage());
                SetMaterialUV();
            }
        }

        [HideInInspector]
        public string origin_name;
        public enum ColorType
        {
            Solid_Color,
            Gradient_Color
        }
        [SerializeField] public ColorType m_ColorType = ColorType.Solid_Color;

        [SerializeField]
        private Gradient m_GradientColor = new Gradient()
        {
            colorKeys = new GradientColorKey[2] {
        // Add your colour and specify the stop point
                new GradientColorKey(new Color(0, 0, 0), 0),
                new GradientColorKey(new Color(1, 1, 1), 1)
            },
            // This sets the alpha to 1 at both ends of the gradient
            alphaKeys = new GradientAlphaKey[2] {
                new GradientAlphaKey(1, 0),
                new GradientAlphaKey(1, 1)
            }
        };
        public Gradient gradient
        {
            get { return m_GradientColor; }
            set { m_GradientColor = value; SetVerticesDirty(); }
        }
        public enum GradientDirection
        {
            Vertical,
            Horizontal
        }
        [SerializeField] private GradientDirection m_Direction = GradientDirection.Vertical;
        public GradientDirection Direction
        {
            get { return m_Direction; }
            set { m_Direction = Direction; }
        }

        #region Material && Effect
        private static Material s_UXImageMaterial;

        private enum MaterialType
        {
            InstanceUXImageMat,
            StaticUXImageMat,
            OtherMat
        }

        private MaterialType m_MatType = MaterialType.OtherMat;

        //UI Shader Effect Toggle
        [SerializeField]
        private bool m_LinearToGamma;
        [SerializeField]
        private bool m_GammaToLinear;
        [SerializeField]
        private bool m_EnableGrey;
        [SerializeField]
        private bool m_EnableRadiusCorner;
        [SerializeField]
        private bool m_EnableDistortion;

        //Grey Params
        [SerializeField]
        private float m_Contrast = 1;
        [SerializeField]
        private float m_Saturation = 0;

        //RadiusCorner Params
        public Vector4 m_CornerRadius = new Vector4(10f, 10f, 10f, 10f);

        //Distortion Params
        [SerializeField]
        private Vector2 m_BottomLeftDistortion = Vector2.zero;
        [SerializeField]
        private Vector2 m_BottomRightDistortion = Vector2.right;
        [SerializeField]
        private Vector2 m_TopLeftDistortion = Vector2.up;
        [SerializeField]
        private Vector2 m_TopRightDistortion = Vector2.one;

        // Vector2.right rotated clockwise by 45 degrees
        private static readonly Vector2 wNorm = new Vector2(.7071068f, -.7071068f);
        // Vector2.right rotated counter-clockwise by 45 degrees
        private static readonly Vector2 hNorm = new Vector2(.7071068f, .7071068f);
        // xy - position,
        // zw - halfSize
        [HideInInspector, SerializeField] private Vector4 rect2props;
        [HideInInspector, SerializeField] private MaskableGraphic image;


        private static readonly int EnableGrey = Shader.PropertyToID("_EnableGrey");
        private static readonly int Contrast = Shader.PropertyToID("_Contrast");
        private static readonly int Saturation = Shader.PropertyToID("_Saturation");

        private static readonly int LinearToGamma = Shader.PropertyToID("_LinearToGamma");
        private static readonly int GammaToLinear = Shader.PropertyToID("_GammaToLinear");

        private static readonly int RadiusCorner = Shader.PropertyToID("_RadiusCorner");
        private static readonly int prop_halfSize = Shader.PropertyToID("_halfSize");
        private static readonly int prop_radiuses = Shader.PropertyToID("_r");
        private static readonly int prop_rect2props = Shader.PropertyToID("_rect2props");

        private static readonly int Distortion = Shader.PropertyToID("_Distortion");
        private static readonly int BottomDistortion = Shader.PropertyToID("_BottomDistortion");
        private static readonly int TopDistortion = Shader.PropertyToID("_TopDistortion");

        public void InitMaterial()
        {
            if (material == null || material.name == "Default UI Material")
            {
                //没有手动设置过Material的都设置成UXImage
                if (Application.isPlaying)
                {
                    if (m_LinearToGamma || m_GammaToLinear || m_EnableGrey || m_EnableRadiusCorner || m_EnableDistortion)
                    {
                        SetInstanceMaterial();
                    }
                    else
                    {
                        //运行时 没有设置特殊效果时使用公用Material 方便合批
                        SetStaticMaterial();
                    }
                }
                else
                {
                    SetInstanceMaterial();
                }
            }

            if (material != null && material.name.StartsWith("UXImage"))
            {
                SetMaterialUV();
                SetMaterialColorSpace();
                SetMaterialGrey();
                SetMaterialRadiusCorner();
            }
        }

        private void SetInstanceMaterial()
        {
            material = new Material(s_UXImageMaterial
                                    ? s_UXImageMaterial
                                    : s_UXImageMaterial = new Material(ResourceManager.Load<Shader>("Assets/Res/UI/UXTool/GUI/Shader/UXImage.shader")) { name = "UXImage" });
            material.hideFlags = HideFlags.HideAndDontSave;

            m_MatType = MaterialType.InstanceUXImageMat;
        }
        private void SetStaticMaterial()
        {
            material = s_UXImageMaterial
                                    ? s_UXImageMaterial
                                    : s_UXImageMaterial = new Material(ResourceManager.Load<Shader>("Assets/Res/UI/UXTool/GUI/Shader/UXImage.shader")) { name = "UXImage" };
            m_MatType = MaterialType.StaticUXImageMat;
        }
        private void SetMaterialUV()
        {
            if (material != null && material.name.StartsWith("UXImage"))
            {
                if (overrideSprite != null)
                {
                    Vector4 result = new Vector4(
                        overrideSprite.textureRect.position.x,
                        overrideSprite.textureRect.position.y,
                        overrideSprite.textureRect.size.x,
                        overrideSprite.textureRect.size.y
                    );

                    materialForRendering.SetVector("_AtlasPosition", result);
                    materialForRendering.SetFloat("_hasTex", 1);

                }
                else
                {
                    materialForRendering.SetFloat("_hasTex", 0);
                }
            }
        }
        private void SetMaterialColorSpace()
        {
            int toGamma = m_LinearToGamma ? 1 : 0;
            material.SetFloat(LinearToGamma, toGamma);
            if (m_LinearToGamma)
            {
                material.EnableKeyword("LINEAR_TO_GAMMA");
                material.DisableKeyword("GAMMA_TO_LINEAR");
            }

            int toLinear = m_GammaToLinear ? 1 : 0;
            material.SetFloat(GammaToLinear, toLinear);
            if (m_GammaToLinear)
            {
                material.EnableKeyword("GAMMA_TO_LINEAR");
                material.DisableKeyword("LINEAR_TO_GAMMA");
            }
        }
        private void SetMaterialGrey()
        {
            if (m_EnableGrey)
            {
                material.SetFloat(EnableGrey, 1);
                material.EnableKeyword("ENABLE_GREY");
                material.SetFloat(Contrast, m_Contrast);
                material.SetFloat(Saturation, m_Saturation);
            }
            else
            {
                material.SetFloat(EnableGrey, 0);
                material.DisableKeyword("ENABLE_GREY");
            }
        }
        private void SetMaterialRadiusCorner()
        {
            if (m_EnableRadiusCorner)
            {
                materialForRendering.SetFloat(RadiusCorner, 1);
                materialForRendering.EnableKeyword("RADIUS_CORNER");
                RefreshRadius();
            }
        }

        public void SetGrey(bool bGrey)
        {
            if (m_MatType == MaterialType.OtherMat) return;
            m_EnableGrey = bGrey;
            if (m_MatType == MaterialType.StaticUXImageMat)
            {
                SetInstanceMaterial();
            }
            SetMaterialGrey();
        }

        public void RefreshRadius()
        {
            if (material != null && material.name.StartsWith("UXImage"))
            {
                var rect = ((RectTransform)transform).rect;
                RecalculateProps(rect.size);
                materialForRendering.SetVector(prop_rect2props, rect2props);
                materialForRendering.SetVector(prop_halfSize, rect.size * .5f);
                materialForRendering.SetVector(prop_radiuses, m_CornerRadius);
            }
        }

        private void RecalculateProps(Vector2 size)
        {
            // Vector that goes from left to right sides of rect2
            var aVec = new Vector2(size.x, -size.y + m_CornerRadius.x + m_CornerRadius.z);

            // Project vector aVec to wNorm to get magnitude of rect2 width vector
            var halfWidth = Vector2.Dot(aVec, wNorm) * .5f;
            rect2props.z = halfWidth;


            // Vector that goes from bottom to top sides of rect2
            var bVec = new Vector2(size.x, size.y - m_CornerRadius.w - m_CornerRadius.y);

            // Project vector bVec to hNorm to get magnitude of rect2 height vector
            var halfHeight = Vector2.Dot(bVec, hNorm) * .5f;
            rect2props.w = halfHeight;


            // Vector that goes from left to top sides of rect2
            var efVec = new Vector2(size.x - m_CornerRadius.x - m_CornerRadius.y, 0);

            // Vector that goes from point E to point G, which is top-left of rect2
            var egVec = hNorm * Vector2.Dot(efVec, hNorm);

            // Position of point E relative to center of coord system
            var ePoint = new Vector2(m_CornerRadius.x - (size.x / 2), size.y / 2);

            // Origin of rect2 relative to center of coord system
            // ePoint + egVec == vector to top-left corner of rect2
            // wNorm * halfWidth + hNorm * -halfHeight == vector from top-left corner to center
            var origin = ePoint + egVec + wNorm * halfWidth + hNorm * -halfHeight;
            rect2props.x = origin.x;
            rect2props.y = origin.y;
        }

        #endregion

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            if (enabled && material != null && material.name.StartsWith("UXImage"))
            {
                RefreshRadius();
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            SetMaterialUV();
        }
#endif

        protected override void OnEnable()
        {
            base.OnEnable();

            InitMaterial();
        }

        protected override void Start()
        {
            base.Start();
            if (!Application.isPlaying) return;
            if (sprite != null)
            {
                origin_name = sprite.name;
                ChangeLanguage(LocalizationHelper.GetLanguage());
            }
        }

        public void ChangeLanguage(LocalizationHelper.LanguageType language)
        {
            int languageIndex = (int)language;
            if (languageIndex >= 0 && sprite != null && !ignoreLocalization)
            {
                base.sprite = ResourceManager.Load<Sprite>($"{UXGUIConfig.LocalizationFolder}/{language}/{origin_name}.png");
                if (sprite == null)
                {
                    base.sprite = ResourceManager.Load<Sprite>(UXGUIConfig.UXGUINeedReplaceSpritePathReplace);
                }
            }
        }

        //这个用于标记属于哪个镜像区域
        public enum FlipPart
        {
            Part1 = 0,
            Part2 = 1,
            Part3 = 2,
            Part4 = 3,
        }

        public enum FlipDirection
        {
            None = 0,
            Horziontal = 1,
            Vertical = 2,
            FourCorner = 3,
            HorizontalNotCopy = 4,
            VerticalNotCopy = 5,
            DiagonalNotCopy = 6,
        }

        public enum FlipMode
        {
            None = 0,
            Horziontal = 1,
            Vertical = 2,
            FourCorner = 3,
        }

        public enum FlipEdge
        {
            None = -1,
            Left = 0,
            HorzMiddle = 1,
            Right = 2,
            Up = 3,
            VertMiddle = 4,
            Down = 5
        }

        public enum FlipEdgeHorizontal
        {
            Left = 0,
            Middle = 1,
            Right = 2,

        }

        public enum FlipEdgeVertical
        {
            Up = 3,
            Middle = 4,
            Down = 5
        }

        public enum FlipFillCenter
        {
            LeftTop,
            RightTop,
            RightBottom,
            LeftBottom
        }


        public FlipMode m_OriginFlipMode = FlipMode.None;

        public FlipMode m_FlipMode = FlipMode.None;
        public FlipMode flipMode { get { return m_FlipMode; } set { SetVerticesDirty(); } }

        public bool m_FlipWithCopy = true;
        public bool flipWithCopy { get { return m_FlipWithCopy; } set { SetVerticesDirty(); } }

        public FlipEdge flipEdge
        {
            get
            {
                if (m_FlipMode == FlipMode.Horziontal)
                {
                    return (FlipEdge)(int)m_FlipEdgeHorizontal;
                }
                if (m_FlipMode == FlipMode.Vertical)
                {
                    return (FlipEdge)(int)m_FlipEdgeVertical;
                }
                return FlipEdge.None;

            }
            set { SetVerticesDirty(); }
        }

        public FlipEdgeHorizontal m_FlipEdgeHorizontal = FlipEdgeHorizontal.Right;
        public FlipEdgeHorizontal flipEdgeHorizontal { get { return m_FlipEdgeHorizontal; } set { SetVerticesDirty(); } }

        public FlipEdgeVertical m_FlipEdgeVertical = FlipEdgeVertical.Down;
        public FlipEdgeVertical flipEdgeVertical { get { return m_FlipEdgeVertical; } set { SetVerticesDirty(); } }


        public FlipFillCenter m_FlipFillCenter = FlipFillCenter.LeftBottom;
        public FlipFillCenter flipFillCenter { get { return m_FlipFillCenter; } set { SetVerticesDirty(); } }


        [SerializeField]
        public FlipDirection m_FlipDirection = FlipDirection.FourCorner;
        public FlipDirection flipDirection { get { return m_FlipDirection; } set { SetVerticesDirty(); } }

        static Rect rect = new Rect();
        static UIVertex vert = new UIVertex();
        private static readonly Vector4 s_DefaultTangent = new Vector4(1.0f, 0.0f, 0.0f, -1.0f);
        private static readonly Vector3 s_DefaultNormal = Vector3.back;


        /// Image's dimensions used for drawing. X = left, Y = bottom, Z = right, W = top.
        private Vector4 GetDrawingDimensions(bool shouldPreserveAspect)
        {
            var padding = overrideSprite == null ? Vector4.zero : Sprites.DataUtility.GetPadding(overrideSprite);
            var size = overrideSprite == null ? new Vector2(rectTransform.rect.width, rectTransform.rect.height) : new Vector2(overrideSprite.rect.width, overrideSprite.rect.height);

            Rect r = GetPixelRectByFlipDirection(flipMode, flipWithCopy, flipEdge, flipFillCenter);

            int spriteW = Mathf.RoundToInt(size.x);
            int spriteH = Mathf.RoundToInt(size.y);

            var v = new Vector4(
                    padding.x / spriteW,
                    padding.y / spriteH,
                    (spriteW - padding.z) / spriteW,
                    (spriteH - padding.w) / spriteH);

            if (shouldPreserveAspect && size.sqrMagnitude > 0.0f)
            {
                PreserveSpriteAspectRatio(ref r, size);
            }

            v = new Vector4(
                    r.x + r.width * v.x,
                    r.y + r.height * v.y,
                    r.x + r.width * v.z,
                    r.y + r.height * v.w
                    );

            return v;
        }

        private void ResizeByFlip()
        {
            if (flipMode == FlipMode.Horziontal && flipWithCopy)
            {
                RectTransform trans = transform as RectTransform;
                trans.sizeDelta = new Vector2(trans.sizeDelta.x * 2, trans.sizeDelta.y);
            }
            if (flipMode == FlipMode.Vertical && flipWithCopy)
            {
                RectTransform trans = transform as RectTransform;
                trans.sizeDelta = new Vector2(trans.sizeDelta.x, trans.sizeDelta.y * 2);
            }
            if (flipMode == FlipMode.FourCorner)
            {
                RectTransform trans = transform as RectTransform;
                trans.sizeDelta = new Vector2(trans.sizeDelta.x * 2, trans.sizeDelta.y * 2);
            }

        }
        protected void OnPopulateMesh1(VertexHelper toFill)
        {
            //ResizeByFlip();

            //if (overrideSprite == null)
            //{
            //    //support pure color fill
            //    switch (m_ColorFillType)
            //    {
            //        case ColorFillType.None:
            //            GenerateEmpytSprite(toFill);
            //            break;
            //        case ColorFillType.Filled:
            //            GenerateFilledSprite(toFill, preserveAspect);
            //            break;
            //    }
            //}
            //else
            if (overrideSprite == null)
            {
                base.OnPopulateMesh(toFill);
                return;
            }
            {
                UnityEngine.Profiling.Profiler.BeginSample(GetType().Name + ".OnPopulateMesh:" + type);
                switch (type)
                {
                    case Type.Simple:
                        if (!useSpriteMesh)
                        {
                            UnityEngine.Profiling.Profiler.BeginSample(GetType().Name + ".OnPopulateMesh:" + type + ", GenerateSimpleSprite");
                            GenerateSimpleSprite(toFill, preserveAspect);
                            UnityEngine.Profiling.Profiler.EndSample();
                        }
                        else
                        {
                            UnityEngine.Profiling.Profiler.BeginSample(GetType().Name + ".OnPopulateMesh:" + type + ", GenerateSprite");
                            GenerateSprite(toFill, preserveAspect);
                            UnityEngine.Profiling.Profiler.EndSample();
                        }
                        break;
                    case Type.Sliced:
                        GenerateSlicedSprite(toFill);
                        break;
                    case Type.Tiled:
                        GenerateTiledSprite(toFill);
                        break;
                    case Type.Filled:
                        GenerateFilledSprite(toFill, preserveAspect);
                        break;
                }
                UnityEngine.Profiling.Profiler.EndSample();
            }

            rect = GetDrawPixelAdjustedRect();

            if (flipMode == FlipMode.Horziontal)
            {
                if (flipWithCopy)
                {
                    CopyImage(toFill);
                    Rect src = GetPixelRectByFlipDirection(FlipMode.Horziontal, flipWithCopy, flipEdge, flipFillCenter);
                    Rect target = GetCopyRectByFlipDirection(src, FlipMode.Horziontal, flipEdge);
                    //Rect target = new Rect(src.position - new Vector2(src.width, 0), src.size);
                    RemapImage(toFill, FlipMode.Horziontal, toFill.currentVertCount / 2, toFill.currentVertCount, src.xMin, src.xMax, target.xMax, target.xMin);
                }
                else
                {
                    RemapImage(toFill, FlipMode.Horziontal, 0, toFill.currentVertCount, rect.xMin, rect.xMax, rect.xMax, rect.xMin);
                }
            }

            if (flipMode == FlipMode.Vertical)
            {
                if (flipWithCopy)
                {
                    CopyImage(toFill);
                    Rect src = GetPixelRectByFlipDirection(FlipMode.Vertical, flipWithCopy, flipEdge, flipFillCenter);
                    Rect target = GetCopyRectByFlipDirection(src, FlipMode.Vertical, flipEdge);
                    RemapImage(toFill, FlipMode.Vertical, toFill.currentVertCount / 2, toFill.currentVertCount, src.yMin, src.yMax, target.yMax, target.yMin);
                }
                else
                {
                    RemapImage(toFill, FlipMode.Vertical, 0, toFill.currentVertCount, rect.xMin, rect.xMax, rect.xMax, rect.xMin);
                }
            }

            if (flipMode == FlipMode.FourCorner)
            {
                //先水平拷贝
                CopyImage(toFill);
                Rect src = GetPixelRectByFlipDirection(FlipMode.FourCorner, true, flipEdge, flipFillCenter);
                Rect target = GetCopyRectByFlipCenter(src, FlipMode.Horziontal, flipFillCenter);
                //Rect target = new Rect(src.position + new Vector2(src.width, 0), src.size);
                RemapImage(toFill, FlipMode.Horziontal, toFill.currentVertCount / 2, toFill.currentVertCount, src.xMin, src.xMax, target.xMax, target.xMin);

                //再垂直拷贝
                CopyImage(toFill);
                Rect src1 = GetPixelRectByFlipDirection(FlipMode.FourCorner, true, flipEdge, flipFillCenter);
                Rect target1 = GetCopyRectByFlipCenter(src1, FlipMode.Vertical, flipFillCenter);
                RemapImage(toFill, FlipMode.Vertical, toFill.currentVertCount / 2, toFill.currentVertCount, src1.yMin, src1.yMax, target1.yMax, target1.yMin);
            }

            m_OriginFlipMode = m_FlipMode;
            /*
            if (flipDirection == FlipDirection.HorizontalNotCopy || flipDirection == FlipDirection.DiagonalNotCopy)
            {
                RemapImage(toFill, FlipDirection.Horziontal, 0, toFill.currentVertCount, rect.xMin, rect.xMax, rect.xMax, rect.xMin);
            }

            if (flipDirection == FlipDirection.VerticalNotCopy || flipDirection == FlipDirection.DiagonalNotCopy)
            {
                RemapImage(toFill, FlipDirection.Vertical, 0, toFill.currentVertCount, rect.yMin, rect.yMax, rect.yMax, rect.yMin);
            }

            if (flipDirection == FlipDirection.Horziontal || flipDirection == FlipDirection.FourCorner)
            {
                CopyImage(toFill);
                Rect src = GetPixelRectByFlipDirection(FlipDirection.Horziontal);
                Rect target = new Rect(src.position + new Vector2(src.width, 0), src.size);
                RemapImage(toFill, FlipDirection.Horziontal, toFill.currentVertCount / 2, toFill.currentVertCount, src.xMin, src.xMax, target.xMax, target.xMin);
            }
            if (flipDirection == FlipDirection.Vertical || flipDirection == FlipDirection.FourCorner)
            {
                CopyImage(toFill);
                Rect src = GetPixelRectByFlipDirection(FlipDirection.Vertical);
                Rect target = new Rect(src.position - new Vector2(0, src.height), src.size);
                RemapImage(toFill, FlipDirection.Vertical, toFill.currentVertCount / 2, toFill.currentVertCount, src.yMin, src.yMax, target.yMax, target.yMin);
            }
            */
        }

        #region Flip 
        private FlipPart GetFlipPart(int index, int vertCount)
        {
            switch (flipDirection)
            {
                case FlipDirection.None:
                    return FlipPart.Part1;
                case FlipDirection.Horziontal:
                case FlipDirection.Vertical:
                    return index < vertCount / 2 ? FlipPart.Part1 : FlipPart.Part2;
                case FlipDirection.FourCorner:
                    if (index < vertCount / 4)
                    {
                        return FlipPart.Part1;
                    }
                    else if (index < vertCount / 2)
                    {
                        return FlipPart.Part2;
                    }
                    else if (index < vertCount * 3 / 4)
                    {
                        return FlipPart.Part3;
                    }
                    else
                    {
                        return FlipPart.Part4;
                    }
            }
            return FlipPart.Part1;
        }

        //TODO: 改变加点规则
        private void CopyImage(VertexHelper toFill)
        {
            int count = toFill.currentVertCount;

            for (int i = 0; i < count; ++i)
            {
                toFill.PopulateUIVertex(ref vert, i % count);
                toFill.AddVert(vert);
            }
            for (int i = count; i < 2 * count - 2; i += 4)
            {
                toFill.AddTriangle(i, i + 1, i + 2);
                toFill.AddTriangle(i + 2, i + 3, i);
            }
        }

        private void RemapImage(VertexHelper toFill, FlipMode flipMode, int indexMin, int indexMax, float Min1, float Max1, float Min2, float Max2)
        {
            for (int i = indexMin; i < indexMax; i++)
            {
                toFill.PopulateUIVertex(ref vert, i);
                RemapVertex(ref vert, flipMode, Min1, Max1, Min2, Max2);
                toFill.SetUIVertex(vert, i);
            }
        }
        public void RemapVertex(ref UIVertex vertex, FlipMode flipMode, float Min1, float Max1, float Min2, float Max2)
        {
            if(Min1 == Max1)
            {
                return;
            }
            Vector2 position = vertex.position;
            float k = (Min2 - Max2) / (Min1 - Max1);
            float b = Min2 - Min1 * k;
            //水平方向，左侧图像方向不变，右侧图像翻转
            if (flipMode == FlipMode.Horziontal)
            {
                vertex.position = new Vector2(position.x * k + b, position.y);
            }
            //垂直方向，上方图像方向不变，下方图像翻转
            if (flipMode == FlipMode.Vertical)
            {
                vertex.position = new Vector2(position.x, position.y * k + b);
            }
        }
        Rect GetDrawPixelAdjustedRect()
        {
            Rect rect = GetPixelAdjustedRect();
            return rect;
        }

        private Rect GetPixelRectByFlipDirection(FlipMode flipMode, bool copy, FlipEdge flipEdge, FlipFillCenter fillCenter)
        {
            Rect rect = GetDrawPixelAdjustedRect();
            return ModifyRectByFlipDirection(rect, flipMode, copy, flipEdge, fillCenter);
        }

        private Rect GetRectByFlipDirection(FlipMode flipMode, bool copy, FlipEdge flipEdge, FlipFillCenter fillCenter)
        {
            Rect rect = rectTransform.rect;
            return ModifyRectByFlipDirection(rect, flipMode, copy, flipEdge, fillCenter);
        }

        /// <summary>
        /// 修改原来的Rect
        /// 如果 flipMode == Horziontal 或 Vertical, 根据copy, flipEdge来修改原本的Rect
        /// 如果 flipMode == FourCorner, 根据flipFillCenter 来修改
        /// </summary>
        /// <param name="rect">Image原始Rect</param>
        private Rect ModifyRectByFlipDirection(Rect rect, FlipMode flipMode, bool copy, FlipEdge flipEdge, FlipFillCenter fillCenter)
        {
            if (flipMode == FlipMode.Horziontal)
            {
                if (copy)
                {
                    if (flipEdge == FlipEdge.Left)
                    {
                        rect = new Rect(rect.center.x, rect.yMin, rect.width / 2, rect.height);
                    }
                    if (flipEdge == FlipEdge.Right)
                    {
                        rect = new Rect(rect.xMin, rect.yMin, rect.width / 2, rect.height);
                    }
                }
            }

            if (flipMode == FlipMode.Vertical)
            {
                if (copy)
                {

                    if (flipEdge == FlipEdge.Up)
                    {
                        rect = new Rect(rect.xMin, rect.yMin, rect.width, rect.height / 2);
                    }
                    if (flipEdge == FlipEdge.Down)
                    {
                        rect = new Rect(rect.xMin, rect.center.y, rect.width, rect.height / 2);
                    }
                }
            }

            if (flipMode == FlipMode.FourCorner)
            {
                if (fillCenter == FlipFillCenter.LeftBottom)
                {
                    rect = new Rect(rect.center.x, rect.center.y, rect.width / 2, rect.height / 2);
                }

                if (fillCenter == FlipFillCenter.LeftTop)
                {
                    rect = new Rect(rect.center.x, rect.yMin, rect.width / 2, rect.height / 2);
                }

                if (fillCenter == FlipFillCenter.RightTop)
                {
                    rect = new Rect(rect.xMin, rect.yMin, rect.width / 2, rect.height / 2);
                }

                if (fillCenter == FlipFillCenter.RightBottom)
                {
                    rect = new Rect(rect.xMin, rect.center.y, rect.width / 2, rect.height / 2);
                }
            }

            return rect;
        }

        /// <summary>
        /// 获取Copy出来的Rect
        /// 这里Src应该是Modify过的原Rect
        /// 只处理flipMode == Horziontal 或 Vertical的情况
        /// </summary>
        /// <param name="src"></param>
        /// <param name="flipMode"></param>
        /// <param name="flipEdge"></param>
        /// <returns></returns>
        private Rect GetCopyRectByFlipDirection(Rect src, FlipMode flipMode, FlipEdge flipEdge)
        {
            Rect target = new Rect();
            if (flipMode == FlipMode.None)
            {
                target = src;
            }

            if (flipMode == FlipMode.Horziontal)
            {
                if (flipEdge == FlipEdge.Left)
                {
                    target = new Rect(src.position - new Vector2(src.width, 0), src.size);
                }
                if (flipEdge == FlipEdge.HorzMiddle)
                {
                    target = src;
                }
                if (flipEdge == FlipEdge.Right)
                {
                    target = new Rect(src.position + new Vector2(src.width, 0), src.size);
                }
            }

            if (flipMode == FlipMode.Vertical)
            {
                if (flipEdge == FlipEdge.Up)
                {
                    target = new Rect(src.position + new Vector2(0, src.height), src.size);
                }
                if (flipEdge == FlipEdge.VertMiddle)
                {
                    target = src;
                }
                if (flipEdge == FlipEdge.Down)
                {
                    target = new Rect(src.position - new Vector2(0, src.height), src.size);
                }
            }

            return target;
        }

        /// <summary>
        /// 获取Copy出来的Rect， flipMode == FourCorner时专用
        /// flipMode == FourCorner时会转换成两次Copy
        /// 分别调用GetCopyRectByFlipDirection
        /// 这里Src应该是Modify过的原Rect
        /// </summary>
        /// <param name="src"></param>
        /// <param name="flipMode"></param>
        /// <param name="fillCenter"></param>
        /// <returns></returns>
        private Rect GetCopyRectByFlipCenter(Rect src, FlipMode flipMode, FlipFillCenter fillCenter)
        {
            Rect target = new Rect();
            if (flipMode == FlipMode.Horziontal)
            {
                if (fillCenter == FlipFillCenter.LeftBottom || fillCenter == FlipFillCenter.LeftTop)
                {
                    target = GetCopyRectByFlipDirection(src, FlipMode.Horziontal, FlipEdge.Left);
                }
                else
                {
                    target = GetCopyRectByFlipDirection(src, FlipMode.Horziontal, FlipEdge.Right);
                }
            }

            if (flipMode == FlipMode.Vertical)
            {
                if (fillCenter == FlipFillCenter.LeftTop || fillCenter == FlipFillCenter.RightTop)
                {
                    target = GetCopyRectByFlipDirection(src, FlipMode.Vertical, FlipEdge.Up);
                }
                else
                {
                    target = GetCopyRectByFlipDirection(src, FlipMode.Vertical, FlipEdge.Down);
                }
            }
            return target;
        }

        private void PreserveSpriteAspectRatio(ref Rect r, Vector2 size)
        {
            var spriteRatio = size.x / size.y;
            var rectRatio = r.width / r.height;

            if (spriteRatio > rectRatio)
            {
                var oldHeight = r.height;
                r.height = r.width * (1.0f / spriteRatio);
                r.y += (oldHeight - r.height) * rectTransform.pivot.y;
            }
            else
            {
                var oldWidth = r.width;
                r.width = r.height * spriteRatio;
                r.x += (oldWidth - r.width) * rectTransform.pivot.x;
            }
        }

        private void GenerateSimpleSprite(VertexHelper vh, bool lPreserveAspect)
        {
            Vector4 v = GetDrawingDimensions(lPreserveAspect);
            var uv = (overrideSprite != null) ? Sprites.DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;
            vh.Clear();

            AddQuad(vh, new Vector2(v.x, v.y), new Vector2(v.z, v.w), color, new Vector2(uv.x, uv.y), new Vector2(uv.z, uv.w), Vector2.zero, Vector2.one);
        }

        #endregion

        #region  gradient
        public class Vert2D   //描述alphakey和colorkey的类
        {
            public Vector3 position = default;
            public Color color = Color.white;
            public Vector2 uv0 = default;
            public int type;//1=alphakey,2=colorkey, 0=两端
            public Vert2D(int type)
            {
                this.type = type;
            }
            public Vert2D(UIVertex vertex)
            {
                type = 0;
            }
            public int CompareToVertically(Vert2D vert)
            {
                if (this.uv0.y > vert.uv0.y)
                    return 1;
                else if (this.uv0.y == vert.uv0.y)
                {
                    if (this.uv0.x <= vert.uv0.x)
                    {
                        return -1;
                    }
                    else return 1;
                }
                else
                    return -1;
            }
            public int CompareToHorizontally(Vert2D vert)
            {
                if (this.uv0.x > vert.uv0.x)
                    return 1;
                else if (this.uv0.x == vert.uv0.x)
                {
                    if (this.uv0.y <= vert.uv0.y)
                    {
                        return -1;
                    }
                    else return 1;
                }
                else
                    return -1;
            }
            // public void SetColor(byte r, byte g, byte b){
            //     this.color.r = r;
            //     this.color.g = g;
            //     this.color.b = b;
            // }
            public void SetColor(Color col)
            {
                this.color.r = col.r;
                this.color.g = col.g;
                this.color.b = col.b;
            }
            public void SetAlpha(float a)
            {
                this.color.a = a;
            }
            public void SetUV(float u, float v)
            {
                this.uv0.x = u;
                this.uv0.y = v;
            }
            public void SetPosition(Vector3 pos)
            {
                this.position.x = pos.x;
                this.position.y = pos.y;
                this.position.z = pos.z;
            }
        }
        public class Comparer : IEqualityComparer<Vert2D>
        {
            public bool Equals(Vert2D x, Vert2D y)
            {
                //这里定义比较的逻辑
                return x.uv0 == y.uv0;
            }

            public int GetHashCode(Vert2D obj)
            {
                //返回字段的HashCode，只有HashCode相同才会去比较
                return obj.uv0.GetHashCode();
            }
        }

        void PositionLerp(List<Vert2D> vlist, Vector2 minpos, Vector2 maxpos)
        {
            if (m_Direction == GradientDirection.Horizontal)
            {
                for (int i = 0; i < vlist.Count; i++)
                {
                    Vector3 temppos = Vector3.Lerp(minpos, maxpos, vlist[i].uv0.x);
                    if (vlist[i].uv0.y == 1)
                    {
                        temppos.y = maxpos.y;
                    }
                    else
                    {
                        temppos.y = minpos.y;
                    }
                    vlist[i].SetPosition(temppos);
                }
            }
            else if (m_Direction == GradientDirection.Vertical)
            {
                for (int i = 0; i < vlist.Count; i++)
                {
                    Vector3 temppos = Vector3.Lerp(minpos, maxpos, vlist[i].uv0.y);
                    if (vlist[i].uv0.x == 1)
                    {
                        temppos.x = maxpos.x;
                    }
                    else
                    {
                        temppos.x = minpos.x;
                    }
                    vlist[i].SetPosition(temppos);
                }
            }
        }
        void UVShrink(List<Vert2D> vlist, Vector2 uvmin, Vector2 uvmax)
        {
            foreach (Vert2D v in vlist)
            {
                v.uv0.x = (uvmax.x - uvmin.x) * v.uv0.x + uvmin.x;
                v.uv0.y = (uvmax.y - uvmin.y) * v.uv0.y + uvmin.y;
            }
        }
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (m_ColorType == ColorType.Solid_Color)
            {
                OnPopulateMesh1(toFill);
            }
            else if (m_ColorType == ColorType.Gradient_Color)
            {
                //根据Gradient,新建mesh
                GradientAlphaKey[] alpha = m_GradientColor.alphaKeys;
                GradientColorKey[] colorkey = m_GradientColor.colorKeys;
                List<Vert2D> vertList = new List<Vert2D>();

                if (m_Direction == GradientDirection.Vertical)
                {
                    //垂直分割
                    Rect rect = GetPixelAdjustedRect();
                    Vert2D v0 = new Vert2D(0);
                    v0.SetColor(colorkey[colorkey.Length - 1].color);
                    v0.SetAlpha(alpha[alpha.Length - 1].alpha);
                    v0.SetUV(0, 0);
                    Vector3 vec = new Vector3(rect.x, rect.y, 0);
                    v0.SetPosition(vec);
                    vertList.Add(v0);

                    Vert2D v1 = new Vert2D(0);
                    v1.SetColor(colorkey[colorkey.Length - 1].color);
                    v1.SetAlpha(alpha[alpha.Length - 1].alpha);
                    v1.SetUV(1, 0);
                    vec = new Vector3(rect.x + rect.width, rect.y, 0);
                    v1.SetPosition(vec);
                    vertList.Add(v1);

                    Vert2D v2 = new Vert2D(0);
                    v2.SetColor(colorkey[0].color);
                    v2.SetAlpha(alpha[0].alpha);
                    v2.SetUV(1, 1);
                    vec = new Vector3(rect.x + rect.width, rect.y + rect.height, 0);
                    v2.SetPosition(vec);
                    vertList.Add(v2);

                    Vert2D v3 = new Vert2D(0);
                    v3.SetColor(colorkey[0].color);
                    v3.SetAlpha(alpha[0].alpha);
                    v3.SetUV(0, 1);
                    vec = new Vector3(rect.x, rect.y + rect.height, 0);
                    v3.SetPosition(vec);
                    vertList.Add(v3);

                    for (int i = 0; i < alpha.Length; i++)
                    {
                        if (alpha[i].time == 0 || alpha[i].time == 1)
                        {
                            continue;
                        }
                        Vert2D v = new Vert2D(1);
                        v.SetAlpha(alpha[i].alpha);
                        v.SetUV(0, 1 - alpha[i].time);
                        vertList.Add(v);
                        Vert2D v_ = new Vert2D(1);
                        v_.SetAlpha(alpha[i].alpha);
                        v_.SetUV(1, 1 - alpha[i].time);
                        vertList.Add(v_);
                    }
                    for (int i = 0; i < colorkey.Length; i++)
                    {
                        if (colorkey[i].time == 0 || colorkey[i].time == 1)
                        {
                            continue;
                        }
                        Vert2D v = new Vert2D(2);
                        v.SetColor(colorkey[i].color);
                        v.SetUV(1, 1 - colorkey[i].time);
                        vertList.Add(v);
                        Vert2D v_ = new Vert2D(2);
                        v_.SetColor(colorkey[i].color);
                        v_.SetUV(0, 1 - colorkey[i].time);
                        vertList.Add(v_);
                    }
                    //排序，addvert
                    vertList.Sort((x, y) =>
                    {
                        return x.CompareToVertically(y);
                    });

                    //给未填充数据的点填充数据
                    int nowtype = vertList[1].type;
                    int lasttypepos = 0;
                    int nexttypepos = 1;
                    for (int i = 1; i < vertList.Count - 1; i++)
                    {
                        nowtype = vertList[i].type;
                        if (i >= nexttypepos)
                        {
                            //去找下一个type
                            lasttypepos = i - 1;
                            for (int j = i + 1; j < vertList.Count; j++)
                            {
                                if (vertList[j].type != nowtype)
                                {
                                    nexttypepos = j;
                                    break;
                                }
                            }
                        }
                        //插值更新color和alpha
                        if (nowtype == 1)
                        {
                            //是alpha点，要更新color
                            Color col;
#if UNITY_2022_2_OR_NEWER
                            if (m_GradientColor.mode == GradientMode.Blend || m_GradientColor.mode == GradientMode.PerceptualBlend)
#else
                            if (m_GradientColor.mode == GradientMode.Blend)
#endif
                            {

                                if ((vertList[nexttypepos].uv0.y - vertList[lasttypepos].uv0.y) == 0)
                                {
                                    col = new Color(vertList[lasttypepos].color.r, vertList[lasttypepos].color.g, vertList[lasttypepos].color.b, vertList[lasttypepos].color.a);
                                }
                                else
                                {
                                    col = Color.Lerp(vertList[lasttypepos].color, vertList[nexttypepos].color,
                                                (vertList[i].uv0.y - vertList[lasttypepos].uv0.y) / (vertList[nexttypepos].uv0.y - vertList[lasttypepos].uv0.y));
                                }
                            }
                            else
                            {
                                col = vertList[nexttypepos].color;
                            }
                            vertList[i].SetColor(col);
                        }
                        else if (nowtype == 2)
                        {
                            Color col;
#if UNITY_2022_2_OR_NEWER
                            if (m_GradientColor.mode == GradientMode.Blend || m_GradientColor.mode == GradientMode.PerceptualBlend)
#else
                            if (m_GradientColor.mode == GradientMode.Blend)
#endif
                            {
                                if ((vertList[nexttypepos].uv0.y - vertList[lasttypepos].uv0.y) == 0)
                                {
                                    col = new Color(vertList[lasttypepos].color.r, vertList[lasttypepos].color.g, vertList[lasttypepos].color.b, vertList[lasttypepos].color.a);
                                }
                                else
                                {
                                    col = Color.Lerp(vertList[lasttypepos].color, vertList[nexttypepos].color,
                                                (vertList[i].uv0.y - vertList[lasttypepos].uv0.y) / (vertList[nexttypepos].uv0.y - vertList[lasttypepos].uv0.y));
                                }
                            }
                            else
                            {
                                col = vertList[nexttypepos].color;
                            }
                            vertList[i].SetAlpha(col.a);
                        }
                    }
                    //去重
                    vertList = vertList.Distinct(new Comparer()).ToList();
                    PositionLerp(vertList, new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y + rect.height));


                }
                else if (m_Direction == GradientDirection.Horizontal)
                {
                    //水平分割 
                    Rect rect = GetPixelAdjustedRect();
                    Vert2D v0 = new Vert2D(0);
                    v0.SetColor(colorkey[0].color);
                    v0.SetAlpha(alpha[0].alpha);
                    v0.SetUV(0, 0);
                    Vector3 vec = new Vector3(rect.x, rect.y, 0);
                    v0.SetPosition(vec);
                    vertList.Add(v0);

                    Vert2D v3 = new Vert2D(0);
                    v3.SetColor(colorkey[0].color);
                    v3.SetAlpha(alpha[0].alpha);
                    v3.SetUV(0, 1);
                    vec = new Vector3(rect.x, rect.y + rect.height, 0);
                    v3.SetPosition(vec);
                    vertList.Add(v3);

                    Vert2D v2 = new Vert2D(0);
                    v2.SetColor(colorkey[colorkey.Length - 1].color);
                    v2.SetAlpha(alpha[alpha.Length - 1].alpha);
                    v2.SetUV(1, 1);
                    vec = new Vector3(rect.x + rect.width, rect.y + rect.height, 0);
                    v2.SetPosition(vec);
                    vertList.Add(v2);

                    Vert2D v1 = new Vert2D(0);
                    v1.SetColor(colorkey[colorkey.Length - 1].color);
                    v1.SetAlpha(alpha[alpha.Length - 1].alpha);
                    v1.SetUV(1, 0);
                    vec = new Vector3(rect.x + rect.width, rect.y, 0);
                    v1.SetPosition(vec);
                    vertList.Add(v1);


                    for (int i = 0; i < alpha.Length; i++)
                    {
                        if (alpha[i].time == 0 || alpha[i].time == 1)
                        {
                            continue;
                        }
                        Vert2D v = new Vert2D(1);
                        v.SetAlpha(alpha[i].alpha);
                        v.SetUV(alpha[i].time, 0);
                        vertList.Add(v);
                        Vert2D v_ = new Vert2D(1);
                        v_.SetAlpha(alpha[i].alpha);
                        v_.SetUV(alpha[i].time, 1);
                        vertList.Add(v_);
                    }
                    for (int i = 0; i < colorkey.Length; i++)
                    {
                        if (colorkey[i].time == 0 || colorkey[i].time == 1)
                        {
                            continue;
                        }
                        Vert2D v = new Vert2D(2);
                        v.SetColor(colorkey[i].color);
                        v.SetUV(colorkey[i].time, 1);
                        vertList.Add(v);
                        Vert2D v_ = new Vert2D(2);
                        v_.SetColor(colorkey[i].color);
                        v_.SetUV(colorkey[i].time, 0);
                        vertList.Add(v_);
                    }
                    //排序，addvert
                    vertList.Sort((x, y) =>
                    {
                        return x.CompareToHorizontally(y);
                    });

                    int nowtype = vertList[1].type;
                    int lasttypepos = 0;
                    int nexttypepos = 1;
                    for (int i = 1; i < vertList.Count - 1; i++)
                    {
                        nowtype = vertList[i].type;
                        if (i >= nexttypepos)
                        {
                            //去找下一个type
                            lasttypepos = i - 1;
                            for (int j = i + 1; j < vertList.Count; j++)
                            {
                                if (vertList[j].type != nowtype)
                                {
                                    nexttypepos = j;
                                    break;
                                }
                            }

                        }
                        //插值
                        if (nowtype == 1)
                        {
                            //是alpha点，要更新color
                            Color col;
#if UNITY_2022_2_OR_NEWER
                            if (m_GradientColor.mode == GradientMode.Blend || m_GradientColor.mode == GradientMode.PerceptualBlend)
#else
                            if (m_GradientColor.mode == GradientMode.Blend)
#endif
                            {
                                if ((vertList[nexttypepos].uv0.x - vertList[lasttypepos].uv0.x) == 0)
                                {
                                    col = new Color(vertList[lasttypepos].color.r, vertList[lasttypepos].color.g, vertList[lasttypepos].color.b, vertList[lasttypepos].color.a);
                                }
                                else
                                {
                                    col = Color.Lerp(vertList[lasttypepos].color, vertList[nexttypepos].color,
                                                (vertList[i].uv0.x - vertList[lasttypepos].uv0.x) / (vertList[nexttypepos].uv0.x - vertList[lasttypepos].uv0.x));
                                }
                            }
                            else
                            {
                                col = vertList[nexttypepos].color;
                            }
                            vertList[i].SetColor(col);
                        }
                        else if (nowtype == 2)
                        {
                            Color col;
#if UNITY_2022_2_OR_NEWER
                            if (m_GradientColor.mode == GradientMode.Blend || m_GradientColor.mode == GradientMode.PerceptualBlend)
#else
                            if (m_GradientColor.mode == GradientMode.Blend)
#endif
                            {
                                if ((vertList[nexttypepos].uv0.x - vertList[lasttypepos].uv0.x) == 0)
                                {
                                    col = new Color(vertList[lasttypepos].color.r, vertList[lasttypepos].color.g, vertList[lasttypepos].color.b, vertList[lasttypepos].color.a);
                                }
                                else
                                {
                                    col = Color.Lerp(vertList[lasttypepos].color, vertList[nexttypepos].color,
                                                (vertList[i].uv0.x - vertList[lasttypepos].uv0.x) / (vertList[nexttypepos].uv0.x - vertList[lasttypepos].uv0.x));
                                }
                            }
                            else
                            {
                                col = vertList[nexttypepos].color;
                            }
                            vertList[i].SetAlpha(col.a);
                        }
                    }
                    vertList = vertList.Distinct(new Comparer()).ToList();
                    PositionLerp(vertList, new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y + rect.height));
                }

                //重写四个函数

                switch (type)
                {
                    case Type.Simple:
                        //因为UseSpriteMesh和渐变色加点冲突，因此放弃使用
                        if (!useSpriteMesh)
                        {
                            GenerateSimpleSprite(toFill, vertList, preserveAspect);
                        }
                        else
                            GenerateSprite(toFill, preserveAspect);
                        break;
                    case Type.Sliced:
                        GenerateSlicedSprite(toFill, vertList);
                        break;
                        // case Type.Tiled:
                        //     GenerateTiledSprite(toFill);
                        //     break;
                        // case Type.Filled:
                        //     GenerateFilledSprite(toFill, preserveAspect);
                        //     break;
                }
                // rect = GetDrawPixelAdjustedRect();

                // if (flipMode == FlipMode.Horziontal)
                // {
                //     if (flipWithCopy)
                //     {
                //         CopyImage(toFill);
                //         Rect src = GetPixelRectByFlipDirection(FlipMode.Horziontal, flipWithCopy, flipEdge, flipFillCenter);
                //         Rect target = GetCopyRectByFlipDirection(src, FlipMode.Horziontal, flipEdge);
                //         //Rect target = new Rect(src.position - new Vector2(src.width, 0), src.size);
                //         RemapImage(toFill, FlipMode.Horziontal, toFill.currentVertCount / 2, toFill.currentVertCount, src.xMin, src.xMax, target.xMax, target.xMin);
                //     }
                //     else
                //     {
                //         RemapImage(toFill, FlipMode.Horziontal, 0, toFill.currentVertCount, rect.xMin, rect.xMax, rect.xMax, rect.xMin);
                //     }
                // }

                // if (flipMode == FlipMode.Vertical)
                // {
                //     if (flipWithCopy)
                //     {
                //         CopyImage(toFill);
                //         Rect src = GetPixelRectByFlipDirection(FlipMode.Vertical, flipWithCopy, flipEdge, flipFillCenter);
                //         Rect target = GetCopyRectByFlipDirection(src, FlipMode.Vertical, flipEdge);
                //         RemapImage(toFill, FlipMode.Vertical, toFill.currentVertCount / 2, toFill.currentVertCount, src.yMin, src.yMax, target.yMax, target.yMin);
                //     }
                //     else
                //     {
                //         RemapImage(toFill, FlipMode.Vertical, 0, toFill.currentVertCount, rect.xMin, rect.xMax, rect.xMax, rect.xMin);
                //     }
                // }

                // if (flipMode == FlipMode.FourCorner)
                // {
                //     //先水平拷贝
                //     CopyImage(toFill);
                //     Rect src = GetPixelRectByFlipDirection(FlipMode.FourCorner, true, flipEdge, flipFillCenter);
                //     Rect target = GetCopyRectByFlipCenter(src, FlipMode.Horziontal, flipFillCenter);
                //     //Rect target = new Rect(src.position + new Vector2(src.width, 0), src.size);
                //     RemapImage(toFill, FlipMode.Horziontal, toFill.currentVertCount / 2, toFill.currentVertCount, src.xMin, src.xMax, target.xMax, target.xMin);

                //     //再垂直拷贝
                //     CopyImage(toFill);
                //     Rect src1 = GetPixelRectByFlipDirection(FlipMode.FourCorner, true, flipEdge, flipFillCenter);
                //     Rect target1 = GetCopyRectByFlipCenter(src1, FlipMode.Vertical, flipFillCenter);
                //     RemapImage(toFill, FlipMode.Vertical, toFill.currentVertCount / 2, toFill.currentVertCount, src1.yMin, src1.yMax, target1.yMax, target1.yMin);
                // }

                // m_OriginFlipMode = m_FlipMode;
                //将color设置为white，函数结束将原color设置回去

                // rect = GetPixelAdjustedRect();
                // for (int i = 0; i < toFill.currentVertCount; ++i)
                // {

                //     toFill.PopulateUIVertex(ref vert, i);

                //     //SetGradientColor(ref vert, rect, flipDirection, GetFlipPart(i, toFill.currentVertCount));
                //     toFill.SetUIVertex(vert, i);
                // }

                //根据gradient的值，赋color

            }

            if (m_EnableDistortion != materialForRendering.IsKeywordEnabled("DISTORTION"))
            {
                if (m_EnableDistortion)
                {
                    materialForRendering.SetFloat(Distortion, 1);
                    materialForRendering.EnableKeyword("DISTORTION");
                }
                else
                {
                    materialForRendering.SetFloat(Distortion, 0);
                    materialForRendering.DisableKeyword("DISTORTION");
                }
            }
            if (m_EnableDistortion)
            {
                materialForRendering.SetVector(BottomDistortion, new Vector4(m_BottomLeftDistortion.x, m_BottomLeftDistortion.y, m_BottomRightDistortion.x, m_BottomRightDistortion.y));
                materialForRendering.SetVector(TopDistortion, new Vector4(m_TopRightDistortion.x, m_TopRightDistortion.y, m_TopLeftDistortion.x, m_TopLeftDistortion.y));
            }
        }

        void GenerateSimpleSprite(VertexHelper vh, List<Vert2D> vertlist, bool lPreserveAspect)
        {
            if (overrideSprite == null)
            {
                vh.Clear();
                AddQuads(vh, vertlist);
                return;
            }
            Vector4 v = GetDrawingDimensions(lPreserveAspect);
            var uv = (overrideSprite != null) ? Sprites.DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;
            PositionLerp(vertlist, new Vector2(v.x, v.y), new Vector2(v.z, v.w));
            UVShrink(vertlist, new Vector2(uv.x, uv.y), new Vector2(uv.z, uv.w));
            vh.Clear();
            AddQuads(vh, vertlist);
        }




        private void GenerateSprite(VertexHelper vh, bool lPreserveAspect)
        {
            var spriteSize = new Vector2(overrideSprite.rect.width, overrideSprite.rect.height);

            // Covert sprite pivot into normalized space.
            var spritePivot = overrideSprite.pivot / spriteSize;
            var rectPivot = rectTransform.pivot;
            Rect r = GetPixelAdjustedRect();

            if (lPreserveAspect & spriteSize.sqrMagnitude > 0.0f)
            {
                PreserveSpriteAspectRatio(ref r, spriteSize);
            }

            var drawingSize = new Vector2(r.width, r.height);
            var spriteBoundSize = overrideSprite.bounds.size;

            // Calculate the drawing offset based on the difference between the two pivots.
            var drawOffset = (rectPivot - spritePivot) * drawingSize;

            var color32 = color;
            vh.Clear();

            Vector2[] vertices = overrideSprite.vertices;
            Vector2[] uvs = overrideSprite.uv;
            for (int i = 0; i < vertices.Length; ++i)
            {
                vh.AddVert(new Vector3((vertices[i].x / spriteBoundSize.x) * drawingSize.x - drawOffset.x, (vertices[i].y / spriteBoundSize.y) * drawingSize.y - drawOffset.y), color32, new Vector2(uvs[i].x, uvs[i].y));
            }

            UInt16[] triangles = overrideSprite.triangles;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                vh.AddTriangle(triangles[i + 0], triangles[i + 1], triangles[i + 2]);
            }
        }
        private void GenerateSprite(VertexHelper vh, List<Vert2D> vertlist, bool lPreserveAspect)
        {
            var spriteSize = new Vector2(overrideSprite.rect.width, overrideSprite.rect.height);

            // Covert sprite pivot into normalized space.
            var spritePivot = overrideSprite.pivot / spriteSize;
            var rectPivot = rectTransform.pivot;
            Rect r = GetPixelAdjustedRect();

            if (lPreserveAspect & spriteSize.sqrMagnitude > 0.0f)
            {
                PreserveSpriteAspectRatio(ref r, spriteSize);
            }

            var drawingSize = new Vector2(r.width, r.height);
            var spriteBoundSize = overrideSprite.bounds.size;

            // Calculate the drawing offset based on the difference between the two pivots.
            var drawOffset = (rectPivot - spritePivot) * drawingSize;

            var color32 = color;
            vh.Clear();

            Vector2[] vertices = overrideSprite.vertices;
            Vector2[] uvs = overrideSprite.uv;
            List<Color> colors = new List<Color>();
            for (int i = 0; i < uvs.Length; ++i)
            {
                //Debug.Log(uvs[i]);
                if (m_Direction == GradientDirection.Vertical)
                {
                    float tolerp = uvs[i].y;
                    //基于uv y值做插值，通过二分查找插值位置
                    int le = 0;
                    int ri = vertlist.Count;
                    int mid;
                    while (le < ri - 1)
                    {
                        mid = (le + ri) / 2;
                        float midY = vertlist[mid].uv0.y;
                        if (midY == tolerp)
                        {
                            break;
                        }
                        else if (midY < tolerp)
                        {
                            le = mid;
                        }
                        else
                        {
                            ri = mid;
                        }
                    }
                    if ((vertlist[(le + ri) / 2 + 1].uv0.y - vertlist[(le + ri) / 2].uv0.y) != 0 && (le + ri) / 2 != vertlist.Count)
                    {
                        Color lerpColor = Color.Lerp(vertlist[(le + ri) / 2].color, vertlist[(le + ri) / 2 + 1].color,
                            (tolerp - vertlist[(le + ri) / 2].uv0.y) / (vertlist[(le + ri) / 2 + 1].uv0.y - vertlist[(le + ri) / 2].uv0.y));
                        colors.Add(lerpColor);
                    }
                    else
                    {
                        Color lerpColor = vertlist[(le + ri) / 2].color;
                        colors.Add(lerpColor);
                    }
                }
                else
                {
                    //基于x做插值
                    float tolerp = uvs[i].x;
                    int le = 0;
                    int ri = vertlist.Count;
                    int mid;
                    while (le < ri - 1)
                    {
                        mid = (le + ri) / 2;
                        float midY = vertlist[mid].uv0.x;
                        if (midY == tolerp)
                        {
                            break;
                        }
                        else if (midY < tolerp)
                        {
                            le = mid;
                        }
                        else
                        {
                            ri = mid;
                        }
                    }
                    if ((vertlist[(le + ri) / 2 + 1].uv0.x - vertlist[(le + ri) / 2].uv0.x) != 0 && (le + ri) / 2 != vertlist.Count)
                    {
                        Color lerpColor = Color.Lerp(vertlist[(le + ri) / 2].color, vertlist[(le + ri) / 2 + 1].color,
                        (tolerp - vertlist[(le + ri) / 2].uv0.x) / (vertlist[(le + ri) / 2 + 1].uv0.x - vertlist[(le + ri) / 2].uv0.x));
                        colors.Add(lerpColor);
                    }
                    else
                    {
                        Color lerpColor = vertlist[(le + ri) / 2].color;
                        colors.Add(lerpColor);
                    }
                }
            }
            //
            //Debug.Log(vertices.Length);
            for (int i = 0; i < vertices.Length; ++i)
            {
                //Debug.Log(vertices[i]);
                //
                vh.AddVert(new Vector3((vertices[i].x / spriteBoundSize.x) * drawingSize.x - drawOffset.x,
                (vertices[i].y / spriteBoundSize.y) * drawingSize.y - drawOffset.y), colors[i], new Vector2(uvs[i].x, uvs[i].y));
            }

            UInt16[] triangles = overrideSprite.triangles;
            //Debug.Log(triangles.Length);
            for (int i = 0; i < triangles.Length; i += 3)
            {
                //Debug.Log(triangles[i] + " " + triangles[i + 1] + " " + triangles[i + 2]);
                vh.AddTriangle(triangles[i + 0], triangles[i + 1], triangles[i + 2]);
            }

        }

        static readonly Vector2[] s_VertScratch = new Vector2[4];
        static readonly Vector2[] s_UVScratch = new Vector2[4];
        private void GenerateSlicedSprite(VertexHelper toFill/*, List<Vert2D> vlist*/)
        {
            if (!hasBorder)
            {
                //GenerateSimpleSprite(toFill, vlist, false);
                GenerateSimpleSprite(toFill, false);
                return;
            }

            Vector4 outer, inner, padding, border;

            if (overrideSprite != null)
            {
                outer = Sprites.DataUtility.GetOuterUV(overrideSprite);
                inner = Sprites.DataUtility.GetInnerUV(overrideSprite);
                padding = Sprites.DataUtility.GetPadding(overrideSprite);
                border = overrideSprite.border;
            }
            else
            {
                outer = Vector4.zero;
                inner = Vector4.zero;
                padding = Vector4.zero;
                border = Vector4.zero;
            }

            Rect rect = GetPixelRectByFlipDirection(flipMode, flipWithCopy, flipEdge, flipFillCenter);

            border = GetAdjustedBorders(border / pixelsPerUnit, rect);
            padding = padding / pixelsPerUnit;

            s_VertScratch[0] = new Vector2(padding.x, padding.y);
            s_VertScratch[3] = new Vector2(rect.width - padding.z, rect.height - padding.w);


            s_VertScratch[1].x = border.x;
            s_VertScratch[1].y = border.y;
            s_VertScratch[2].x = rect.width - border.z;
            s_VertScratch[2].y = rect.height - border.w;

            float vertWidth = s_VertScratch[3].x - s_VertScratch[0].x;
            float vertHeight = s_VertScratch[3].y - s_VertScratch[0].y;


            for (int i = 0; i < 4; ++i)
            {
                s_VertScratch[i].x += rect.x;
                s_VertScratch[i].y += rect.y;
            }

            s_UVScratch[0] = new Vector2(outer.x, outer.y);
            s_UVScratch[1] = new Vector2(inner.x, inner.y);
            s_UVScratch[2] = new Vector2(inner.z, inner.w);
            s_UVScratch[3] = new Vector2(outer.z, outer.w);

            toFill.Clear();

            for (int x = 0; x < 3; ++x)
            {
                int x2 = x + 1;

                for (int y = 0; y < 3; ++y)
                {
                    if (!fillCenter && x == 1 && y == 1)
                        continue;

                    int y2 = y + 1;

                    Vector2 uv1Min = new Vector2((s_VertScratch[x].x - rect.x) / vertWidth, (s_VertScratch[y].y - rect.y) / vertHeight);
                    Vector2 uv1Max = new Vector2((s_VertScratch[x2].x - rect.x) / vertWidth, (s_VertScratch[y2].y - rect.y) / vertHeight);
                    AddQuad(toFill,
                            new Vector2(s_VertScratch[x].x, s_VertScratch[y].y),
                            new Vector2(s_VertScratch[x2].x, s_VertScratch[y2].y),
                            color,
                            new Vector2(s_UVScratch[x].x, s_UVScratch[y].y),
                            new Vector2(s_UVScratch[x2].x, s_UVScratch[y2].y),
                            uv1Min, uv1Max);
                }
            }
        }
        private void GenerateSlicedSprite(VertexHelper toFill, List<Vert2D> vlist)
        {
            if (!hasBorder)
            {
                GenerateSimpleSprite(toFill, vlist, false);
                //GenerateSimpleSprite(toFill, false);
                return;
            }

            Vector4 outer, inner, padding, border;

            if (overrideSprite != null)
            {
                outer = Sprites.DataUtility.GetOuterUV(overrideSprite);
                inner = Sprites.DataUtility.GetInnerUV(overrideSprite);
                padding = Sprites.DataUtility.GetPadding(overrideSprite);
                border = overrideSprite.border;
            }
            else
            {
                outer = Vector4.zero;
                inner = Vector4.zero;
                padding = Vector4.zero;
                border = Vector4.zero;
            }

            Rect rect = GetPixelRectByFlipDirection(flipMode, flipWithCopy, flipEdge, flipFillCenter);

            border = GetAdjustedBorders(border / pixelsPerUnit, rect);
            padding = padding / pixelsPerUnit;
            //Debug.Log(padding);
            //Debug.Log(border);
            //Debug.Log(inner);
            //Debug.Log(outer);
            //Debug.Log(rect.width);

            s_VertScratch[0] = new Vector2(padding.x, padding.y);
            s_VertScratch[3] = new Vector2(rect.width - padding.z, rect.height - padding.w);


            s_VertScratch[1].x = border.x;
            s_VertScratch[1].y = border.y;
            s_VertScratch[2].x = rect.width - border.z;
            s_VertScratch[2].y = rect.height - border.w;

            float vertWidth = s_VertScratch[3].x - s_VertScratch[0].x;
            float vertHeight = s_VertScratch[3].y - s_VertScratch[0].y;

            // for (int i = 0; i < 4; i++)
            // {
            //     Debug.Log(s_VertScratch[i]);
            // }

            for (int i = 0; i < 4; ++i)
            {
                s_VertScratch[i].x += rect.x;
                s_VertScratch[i].y += rect.y;
            }

            s_UVScratch[0] = new Vector2(outer.x, outer.y);
            s_UVScratch[1] = new Vector2(inner.x, inner.y);
            s_UVScratch[2] = new Vector2(inner.z, inner.w);
            s_UVScratch[3] = new Vector2(outer.z, outer.w);

            // for (int i = 0; i < 4; ++i)
            // {
            //     Debug.Log(s_UVScratch[i]);
            // }

            toFill.Clear();

            for (int x = 0; x < 3; ++x)
            {
                int x2 = x + 1;

                for (int y = 0; y < 3; ++y)
                {
                    if (!fillCenter && x == 1 && y == 1)
                        continue;

                    int y2 = y + 1;

                    Vector2 uv1Min = new Vector2((s_VertScratch[x].x - rect.x) / vertWidth, (s_VertScratch[y].y - rect.y) / vertHeight);
                    Vector2 uv1Max = new Vector2((s_VertScratch[x2].x - rect.x) / vertWidth, (s_VertScratch[y2].y - rect.y) / vertHeight);
                    AddQuad(toFill,
                            new Vector2(s_VertScratch[x].x, s_VertScratch[y].y),
                            new Vector2(s_VertScratch[x2].x, s_VertScratch[y2].y),
                            color,
                            new Vector2(s_UVScratch[x].x, s_UVScratch[y].y),
                            new Vector2(s_UVScratch[x2].x, s_UVScratch[y2].y),
                            uv1Min, uv1Max);
                }
            }
        }
        private void GenerateTiledSprite(VertexHelper toFill)
        {
            Vector4 outer, inner, border;
            Vector2 spriteSize;

            if (overrideSprite != null)
            {
                outer = Sprites.DataUtility.GetOuterUV(overrideSprite);
                inner = Sprites.DataUtility.GetInnerUV(overrideSprite);
                border = overrideSprite.border;
                spriteSize = overrideSprite.rect.size;
            }
            else
            {
                outer = Vector4.zero;
                inner = Vector4.zero;
                border = Vector4.zero;
                spriteSize = Vector2.one * 100;
            }

            Rect rect = GetPixelRectByFlipDirection(flipMode, flipWithCopy, flipEdge, flipFillCenter);

            float tileWidth = (spriteSize.x - border.x - border.z) / pixelsPerUnit;
            float tileHeight = (spriteSize.y - border.y - border.w) / pixelsPerUnit;
            border = GetAdjustedBorders(border / pixelsPerUnit, rect);


            var uvMin = new Vector2(inner.x, inner.y);
            var uvMax = new Vector2(inner.z, inner.w);

            // Min to max max range for tiled region in coordinates relative to lower left corner.
            float xMin = border.x;
            float xMax = rect.width - border.z;
            float yMin = border.y;
            float yMax = rect.height - border.w;

            toFill.Clear();
            var clipped = uvMax;

            // if either width is zero we cant tile so just assume it was the full width.
            if (tileWidth <= 0)
                tileWidth = xMax - xMin;

            if (tileHeight <= 0)
                tileHeight = yMax - yMin;

            if (overrideSprite != null && (hasBorder || overrideSprite.packed || overrideSprite.texture.wrapMode != TextureWrapMode.Repeat))
            {
                // Sprite has border, or is not in repeat mode, or cannot be repeated because of packing.
                // We cannot use texture tiling so we will generate a mesh of quads to tile the texture.

                // Evaluate how many vertices we will generate. Limit this number to something sane,
                // especially since meshes can not have more than 65000 vertices.

                int nTilesW = 0;
                int nTilesH = 0;
                if (fillCenter)
                {
                    nTilesW = (int)Math.Ceiling((xMax - xMin) / tileWidth);
                    nTilesH = (int)Math.Ceiling((yMax - yMin) / tileHeight);

                    int nVertices = 0;
                    if (hasBorder)
                    {
                        nVertices = (nTilesW + 2) * (nTilesH + 2) * 4; // 4 vertices per tile
                    }
                    else
                    {
                        nVertices = nTilesW * nTilesH * 4; // 4 vertices per tile
                    }

                    if (nVertices > 65000)
                    {
                        double maxTiles = 65000.0 / 4.0; // Max number of vertices is 65000; 4 vertices per tile.
                        double imageRatio;
                        if (hasBorder)
                        {
                            imageRatio = (nTilesW + 2.0) / (nTilesH + 2.0);
                        }
                        else
                        {
                            imageRatio = (double)nTilesW / nTilesH;
                        }

                        double targetTilesW = Math.Sqrt(maxTiles / imageRatio);
                        double targetTilesH = targetTilesW * imageRatio;
                        if (hasBorder)
                        {
                            targetTilesW -= 2;
                            targetTilesH -= 2;
                        }

                        nTilesW = (int)Math.Floor(targetTilesW);
                        nTilesH = (int)Math.Floor(targetTilesH);

                        tileWidth = (xMax - xMin) / nTilesW;
                        tileHeight = (yMax - yMin) / nTilesH;
                    }
                }
                else
                {
                    if (hasBorder)
                    {
                        // Texture on the border is repeated only in one direction.
                        nTilesW = (int)Math.Ceiling((xMax - xMin) / tileWidth);
                        nTilesH = (int)Math.Ceiling((yMax - yMin) / tileHeight);
                        int nVertices = (nTilesH + nTilesW + 2 /*corners*/) * 2 /*sides*/ * 4 /*vertices per tile*/;
                        if (nVertices > 65000)
                        {
                            double maxTiles = 65000.0 / 4.0; // Max number of vertices is 65000; 4 vertices per tile.
                            double imageRatio = (double)nTilesW / nTilesH;
                            double targetTilesW = (maxTiles - 4 /*corners*/) / (2 * (1.0 + imageRatio));
                            double targetTilesH = targetTilesW * imageRatio;

                            nTilesW = (int)Math.Floor(targetTilesW);
                            nTilesH = (int)Math.Floor(targetTilesH);
                            tileWidth = (xMax - xMin) / nTilesW;
                            tileHeight = (yMax - yMin) / nTilesH;
                        }
                    }
                    else
                    {
                        nTilesH = nTilesW = 0;
                    }
                }

                if (fillCenter)
                {
                    // TODO: we could share vertices between quads. If vertex sharing is implemented. update the computation for the number of vertices accordingly.
                    float width = nTilesW * tileWidth;
                    float height = nTilesH * tileHeight;
                    for (int j = 0; j < nTilesH; j++)
                    {
                        float y1 = yMin + j * tileHeight;
                        float y2 = yMin + (j + 1) * tileHeight;
                        if (y2 > yMax)
                        {
                            clipped.y = uvMin.y + (uvMax.y - uvMin.y) * (yMax - y1) / (y2 - y1);
                            y2 = yMax;
                        }
                        clipped.x = uvMax.x;
                        for (int i = 0; i < nTilesW; i++)
                        {
                            float x1 = xMin + i * tileWidth;
                            float x2 = xMin + (i + 1) * tileWidth;
                            if (x2 > xMax)
                            {
                                clipped.x = uvMin.x + (uvMax.x - uvMin.x) * (xMax - x1) / (x2 - x1);
                                x2 = xMax;
                            }
                            Vector2 posMin = new Vector2(x1, y1) + rect.position;
                            Vector2 posMax = new Vector2(x2, y2) + rect.position;
                            Vector2 localPosMin = new Vector2(x1, y1);
                            Vector2 localPosMax = new Vector2(x2, y2);
                            AddQuad(toFill, posMin, posMax, color, uvMin, clipped, new Vector2(localPosMin.x / width, localPosMin.y / height), new Vector2(localPosMax.x / width, localPosMax.y / height));
                        }
                    }
                }
                if (hasBorder)
                {
                    clipped = uvMax;
                    float width = nTilesW * tileWidth;
                    float height = nTilesH * tileHeight;
                    for (int j = 0; j < nTilesH; j++)
                    {
                        float y1 = yMin + j * tileHeight;
                        float y2 = yMin + (j + 1) * tileHeight;
                        if (y2 > yMax)
                        {
                            clipped.y = uvMin.y + (uvMax.y - uvMin.y) * (yMax - y1) / (y2 - y1);
                            y2 = yMax;
                        }
                        AddQuad(toFill,
                            new Vector2(0, y1) + rect.position,
                            new Vector2(xMin, y2) + rect.position,
                            color,
                            new Vector2(outer.x, uvMin.y),
                            new Vector2(uvMin.x, clipped.y),
                            new Vector2(0, y1 / height),
                            new Vector2(xMin / width, y2 / height));
                        AddQuad(toFill,
                            new Vector2(xMax, y1) + rect.position,
                            new Vector2(rect.width, y2) + rect.position,
                            color,
                            new Vector2(uvMax.x, uvMin.y),
                            new Vector2(outer.z, clipped.y),
                            new Vector2(xMax / width, y1 / height),
                            new Vector2(rect.width / width, y2 / height));

                    }

                    // Bottom and top tiled border
                    clipped = uvMax;
                    for (int i = 0; i < nTilesW; i++)
                    {
                        float x1 = xMin + i * tileWidth;
                        float x2 = xMin + (i + 1) * tileWidth;
                        if (x2 > xMax)
                        {
                            clipped.x = uvMin.x + (uvMax.x - uvMin.x) * (xMax - x1) / (x2 - x1);
                            x2 = xMax;
                        }
                        AddQuad(toFill,
                            new Vector2(x1, 0) + rect.position,
                            new Vector2(x2, yMin) + rect.position,
                            color,
                            new Vector2(uvMin.x, outer.y),
                            new Vector2(clipped.x, uvMin.y),
                            new Vector2(x1 / width, 0),
                            new Vector2(x2 / width, yMin / height));
                        AddQuad(toFill,
                            new Vector2(x1, yMax) + rect.position,
                            new Vector2(x2, rect.height) + rect.position,
                            color,
                            new Vector2(uvMin.x, uvMax.y),
                            new Vector2(clipped.x, outer.w),
                            new Vector2(x1 / width, yMax / height),
                            new Vector2(x2 / width, rect.height / height));
                    }

                    // Corners
                    AddQuad(toFill,
                        new Vector2(0, 0) + rect.position,
                        new Vector2(xMin, yMin) + rect.position,
                        color,
                        new Vector2(outer.x, outer.y),
                        new Vector2(uvMin.x, uvMin.y),
                        new Vector2(0, 0),
                        new Vector2(xMin / width, yMin / height));
                    AddQuad(toFill,
                        new Vector2(xMax, 0) + rect.position,
                        new Vector2(rect.width, yMin) + rect.position,
                        color,
                        new Vector2(uvMax.x, outer.y),
                        new Vector2(outer.z, uvMin.y),
                        new Vector2(xMax / width, 0),
                        new Vector2(rect.width / width, yMin / height));
                    AddQuad(toFill,
                        new Vector2(0, yMax) + rect.position,
                        new Vector2(xMin, rect.height) + rect.position,
                        color,
                        new Vector2(outer.x, uvMax.y),
                        new Vector2(uvMin.x, outer.w),
                        new Vector2(0, yMax / height),
                        new Vector2(xMin / width, rect.height / height));
                    AddQuad(toFill,
                        new Vector2(xMax, yMax) + rect.position,
                        new Vector2(rect.width, rect.height) + rect.position,
                        color,
                        new Vector2(uvMax.x, uvMax.y),
                        new Vector2(outer.z, outer.w),
                        new Vector2(xMax / width, yMax / height),
                        new Vector2(rect.width / width, rect.height / height));
                }
            }
            else
            {
                // Texture has no border, is in repeat mode and not packed. Use texture tiling.
                Vector2 uvScale = new Vector2((xMax - xMin) / tileWidth, (yMax - yMin) / tileHeight);

                if (fillCenter)
                {
                    AddQuad(toFill, new Vector2(xMin, yMin) + rect.position, new Vector2(xMax, yMax) + rect.position, color, Vector2.Scale(uvMin, uvScale), Vector2.Scale(uvMax, uvScale), Vector2.zero, Vector2.one);
                }
            }
        }
        private void GenerateTiledSprite(VertexHelper toFill, List<Vert2D> vertlist)
        {
            Vector4 outer, inner, border;
            Vector2 spriteSize;

            if (overrideSprite != null)
            {
                outer = Sprites.DataUtility.GetOuterUV(overrideSprite);
                inner = Sprites.DataUtility.GetInnerUV(overrideSprite);
                border = overrideSprite.border;
                spriteSize = overrideSprite.rect.size;
            }
            else
            {
                outer = Vector4.zero;
                inner = Vector4.zero;
                border = Vector4.zero;
                spriteSize = Vector2.one * 100;
            }

            Rect rect = GetPixelRectByFlipDirection(flipMode, flipWithCopy, flipEdge, flipFillCenter);

            float tileWidth = (spriteSize.x - border.x - border.z) / pixelsPerUnit;
            float tileHeight = (spriteSize.y - border.y - border.w) / pixelsPerUnit;
            border = GetAdjustedBorders(border / pixelsPerUnit, rect);


            var uvMin = new Vector2(inner.x, inner.y);
            var uvMax = new Vector2(inner.z, inner.w);

            // Min to max max range for tiled region in coordinates relative to lower left corner.
            float xMin = border.x;
            float xMax = rect.width - border.z;
            float yMin = border.y;
            float yMax = rect.height - border.w;

            toFill.Clear();
            var clipped = uvMax;

            // if either width is zero we cant tile so just assume it was the full width.
            if (tileWidth <= 0)
                tileWidth = xMax - xMin;

            if (tileHeight <= 0)
                tileHeight = yMax - yMin;

            if (overrideSprite != null && (hasBorder || overrideSprite.packed || overrideSprite.texture.wrapMode != TextureWrapMode.Repeat))
            {
                // Sprite has border, or is not in repeat mode, or cannot be repeated because of packing.
                // We cannot use texture tiling so we will generate a mesh of quads to tile the texture.

                // Evaluate how many vertices we will generate. Limit this number to something sane,
                // especially since meshes can not have more than 65000 vertices.

                int nTilesW = 0;
                int nTilesH = 0;
                if (fillCenter)
                {
                    nTilesW = (int)Math.Ceiling((xMax - xMin) / tileWidth);
                    nTilesH = (int)Math.Ceiling((yMax - yMin) / tileHeight);

                    int nVertices = 0;
                    if (hasBorder)
                    {
                        nVertices = (nTilesW + 2) * (nTilesH + 2) * 4; // 4 vertices per tile
                    }
                    else
                    {
                        nVertices = nTilesW * nTilesH * 4; // 4 vertices per tile
                    }

                    if (nVertices > 65000)
                    {
                        double maxTiles = 65000.0 / 4.0; // Max number of vertices is 65000; 4 vertices per tile.
                        double imageRatio;
                        if (hasBorder)
                        {
                            imageRatio = (nTilesW + 2.0) / (nTilesH + 2.0);
                        }
                        else
                        {
                            imageRatio = (double)nTilesW / nTilesH;
                        }

                        double targetTilesW = Math.Sqrt(maxTiles / imageRatio);
                        double targetTilesH = targetTilesW * imageRatio;
                        if (hasBorder)
                        {
                            targetTilesW -= 2;
                            targetTilesH -= 2;
                        }

                        nTilesW = (int)Math.Floor(targetTilesW);
                        nTilesH = (int)Math.Floor(targetTilesH);

                        tileWidth = (xMax - xMin) / nTilesW;
                        tileHeight = (yMax - yMin) / nTilesH;
                    }
                }
                else
                {
                    if (hasBorder)
                    {
                        // Texture on the border is repeated only in one direction.
                        nTilesW = (int)Math.Ceiling((xMax - xMin) / tileWidth);
                        nTilesH = (int)Math.Ceiling((yMax - yMin) / tileHeight);
                        int nVertices = (nTilesH + nTilesW + 2 /*corners*/) * 2 /*sides*/ * 4 /*vertices per tile*/;
                        if (nVertices > 65000)
                        {
                            double maxTiles = 65000.0 / 4.0; // Max number of vertices is 65000; 4 vertices per tile.
                            double imageRatio = (double)nTilesW / nTilesH;
                            double targetTilesW = (maxTiles - 4 /*corners*/) / (2 * (1.0 + imageRatio));
                            double targetTilesH = targetTilesW * imageRatio;

                            nTilesW = (int)Math.Floor(targetTilesW);
                            nTilesH = (int)Math.Floor(targetTilesH);
                            tileWidth = (xMax - xMin) / nTilesW;
                            tileHeight = (yMax - yMin) / nTilesH;
                        }
                    }
                    else
                    {
                        nTilesH = nTilesW = 0;
                    }
                }

                if (fillCenter)
                {
                    // TODO: we could share vertices between quads. If vertex sharing is implemented. update the computation for the number of vertices accordingly.
                    float width = nTilesW * tileWidth;
                    float height = nTilesH * tileHeight;
                    for (int j = 0; j < nTilesH; j++)
                    {
                        float y1 = yMin + j * tileHeight;
                        float y2 = yMin + (j + 1) * tileHeight;
                        if (y2 > yMax)
                        {
                            clipped.y = uvMin.y + (uvMax.y - uvMin.y) * (yMax - y1) / (y2 - y1);
                            y2 = yMax;
                        }
                        clipped.x = uvMax.x;
                        for (int i = 0; i < nTilesW; i++)
                        {
                            float x1 = xMin + i * tileWidth;
                            float x2 = xMin + (i + 1) * tileWidth;
                            if (x2 > xMax)
                            {
                                clipped.x = uvMin.x + (uvMax.x - uvMin.x) * (xMax - x1) / (x2 - x1);
                                x2 = xMax;
                            }
                            Vector2 posMin = new Vector2(x1, y1) + rect.position;
                            Vector2 posMax = new Vector2(x2, y2) + rect.position;
                            Vector2 localPosMin = new Vector2(x1, y1);
                            Vector2 localPosMax = new Vector2(x2, y2);
                            AddQuad(toFill, posMin, posMax, color, uvMin, clipped, new Vector2(localPosMin.x / width, localPosMin.y / height), new Vector2(localPosMax.x / width, localPosMax.y / height));
                        }
                    }
                }
                if (hasBorder)
                {
                    clipped = uvMax;
                    float width = nTilesW * tileWidth;
                    float height = nTilesH * tileHeight;
                    for (int j = 0; j < nTilesH; j++)
                    {
                        float y1 = yMin + j * tileHeight;
                        float y2 = yMin + (j + 1) * tileHeight;
                        if (y2 > yMax)
                        {
                            clipped.y = uvMin.y + (uvMax.y - uvMin.y) * (yMax - y1) / (y2 - y1);
                            y2 = yMax;
                        }
                        AddQuad(toFill,
                            new Vector2(0, y1) + rect.position,
                            new Vector2(xMin, y2) + rect.position,
                            color,
                            new Vector2(outer.x, uvMin.y),
                            new Vector2(uvMin.x, clipped.y),
                            new Vector2(0, y1 / height),
                            new Vector2(xMin / width, y2 / height));
                        AddQuad(toFill,
                            new Vector2(xMax, y1) + rect.position,
                            new Vector2(rect.width, y2) + rect.position,
                            color,
                            new Vector2(uvMax.x, uvMin.y),
                            new Vector2(outer.z, clipped.y),
                            new Vector2(xMax / width, y1 / height),
                            new Vector2(rect.width / width, y2 / height));

                    }

                    // Bottom and top tiled border
                    clipped = uvMax;
                    for (int i = 0; i < nTilesW; i++)
                    {
                        float x1 = xMin + i * tileWidth;
                        float x2 = xMin + (i + 1) * tileWidth;
                        if (x2 > xMax)
                        {
                            clipped.x = uvMin.x + (uvMax.x - uvMin.x) * (xMax - x1) / (x2 - x1);
                            x2 = xMax;
                        }
                        AddQuad(toFill,
                            new Vector2(x1, 0) + rect.position,
                            new Vector2(x2, yMin) + rect.position,
                            color,
                            new Vector2(uvMin.x, outer.y),
                            new Vector2(clipped.x, uvMin.y),
                            new Vector2(x1 / width, 0),
                            new Vector2(x2 / width, yMin / height));
                        AddQuad(toFill,
                            new Vector2(x1, yMax) + rect.position,
                            new Vector2(x2, rect.height) + rect.position,
                            color,
                            new Vector2(uvMin.x, uvMax.y),
                            new Vector2(clipped.x, outer.w),
                            new Vector2(x1 / width, yMax / height),
                            new Vector2(x2 / width, rect.height / height));
                    }

                    // Corners
                    AddQuad(toFill,
                        new Vector2(0, 0) + rect.position,
                        new Vector2(xMin, yMin) + rect.position,
                        color,
                        new Vector2(outer.x, outer.y),
                        new Vector2(uvMin.x, uvMin.y),
                        new Vector2(0, 0),
                        new Vector2(xMin / width, yMin / height));
                    AddQuad(toFill,
                        new Vector2(xMax, 0) + rect.position,
                        new Vector2(rect.width, yMin) + rect.position,
                        color,
                        new Vector2(uvMax.x, outer.y),
                        new Vector2(outer.z, uvMin.y),
                        new Vector2(xMax / width, 0),
                        new Vector2(rect.width / width, yMin / height));
                    AddQuad(toFill,
                        new Vector2(0, yMax) + rect.position,
                        new Vector2(xMin, rect.height) + rect.position,
                        color,
                        new Vector2(outer.x, uvMax.y),
                        new Vector2(uvMin.x, outer.w),
                        new Vector2(0, yMax / height),
                        new Vector2(xMin / width, rect.height / height));
                    AddQuad(toFill,
                        new Vector2(xMax, yMax) + rect.position,
                        new Vector2(rect.width, rect.height) + rect.position,
                        color,
                        new Vector2(uvMax.x, uvMax.y),
                        new Vector2(outer.z, outer.w),
                        new Vector2(xMax / width, yMax / height),
                        new Vector2(rect.width / width, rect.height / height));
                }
            }
            else
            {
                // Texture has no border, is in repeat mode and not packed. Use texture tiling.
                Vector2 uvScale = new Vector2((xMax - xMin) / tileWidth, (yMax - yMin) / tileHeight);

                if (fillCenter)
                {
                    AddQuad(toFill, new Vector2(xMin, yMin) + rect.position, new Vector2(xMax, yMax) + rect.position, color, Vector2.Scale(uvMin, uvScale), Vector2.Scale(uvMax, uvScale), Vector2.zero, Vector2.one);
                }
            }
        }

        static void AddQuad(VertexHelper vertexHelper, Vector3[] quadPositions, Color32 color, Vector3[] quadUVs, Vector3[] quadUV1s)
        {
            int startIndex = vertexHelper.currentVertCount;

            for (int i = 0; i < 4; ++i)
                vertexHelper.AddVert(quadPositions[i], color, quadUVs[i], quadUV1s[i], s_DefaultNormal, s_DefaultTangent);

            vertexHelper.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
            vertexHelper.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
        }

        static void AddQuad(VertexHelper vertexHelper, Vector2 posMin, Vector2 posMax, Color32 color, Vector2 uvMin, Vector2 uvMax, Vector2 uv1Min, Vector2 uv1Max)
        {
            int startIndex = vertexHelper.currentVertCount;

            vertexHelper.AddVert(new Vector3(posMin.x, posMin.y, 0), color, new Vector2(uvMin.x, uvMin.y), new Vector2(uv1Min.x, uv1Min.y), s_DefaultNormal, s_DefaultTangent);
            vertexHelper.AddVert(new Vector3(posMin.x, posMax.y, 0), color, new Vector2(uvMin.x, uvMax.y), new Vector2(uv1Min.x, uv1Max.y), s_DefaultNormal, s_DefaultTangent);
            vertexHelper.AddVert(new Vector3(posMax.x, posMax.y, 0), color, new Vector2(uvMax.x, uvMax.y), new Vector2(uv1Max.x, uv1Max.y), s_DefaultNormal, s_DefaultTangent);
            vertexHelper.AddVert(new Vector3(posMax.x, posMin.y, 0), color, new Vector2(uvMax.x, uvMin.y), new Vector2(uv1Max.x, uv1Min.y), s_DefaultNormal, s_DefaultTangent);

            vertexHelper.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
            vertexHelper.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
        }

        Vector4 GetAdjustedBorders(Vector4 border, Rect adjustedRect)
        {
            Rect originalRect = GetRectByFlipDirection(flipMode, flipWithCopy, flipEdge, flipFillCenter);

            for (int axis = 0; axis <= 1; axis++)
            {
                float borderScaleRatio;

                // The adjusted rect (adjusted for pixel correctness)
                // may be slightly larger than the original rect.
                // Adjust the border to match the adjustedRect to avoid
                // small gaps between borders (case 833201).
                if (originalRect.size[axis] != 0)
                {
                    borderScaleRatio = adjustedRect.size[axis] / originalRect.size[axis];
                    border[axis] *= borderScaleRatio;
                    border[axis + 2] *= borderScaleRatio;
                }

                // If the rect is smaller than the combined borders, then there's not room for the borders at their normal size.
                // In order to avoid artefacts with overlapping borders, we scale the borders down to fit.
                float combinedBorders = border[axis] + border[axis + 2];
                if (adjustedRect.size[axis] < combinedBorders && combinedBorders != 0)
                {
                    borderScaleRatio = adjustedRect.size[axis] / combinedBorders;
                    border[axis] *= borderScaleRatio;
                    border[axis + 2] *= borderScaleRatio;
                }
            }
            return border;
        }


        static readonly Vector3[] s_Xy = new Vector3[4];
        static readonly Vector3[] s_Uv = new Vector3[4];
        static readonly Vector3[] s_Uv1 = new Vector3[4];
        void GenerateFilledSprite(VertexHelper toFill, bool preserveAspect)
        {
            toFill.Clear();

            if (fillAmount < 0.001f)
                return;

            Vector4 v = GetDrawingDimensions(preserveAspect);
            Vector4 outer = overrideSprite != null ? Sprites.DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;
            UIVertex uiv = UIVertex.simpleVert;
            uiv.color = color;

            float tx0 = outer.x;
            float ty0 = outer.y;
            float tx1 = outer.z;
            float ty1 = outer.w;

            // Horizontal and vertical filled sprites are simple -- just end the Image prematurely
            if (fillMethod == FillMethod.Horizontal || fillMethod == FillMethod.Vertical)
            {
                if (fillMethod == FillMethod.Horizontal)
                {
                    float fill = (tx1 - tx0) * fillAmount;

                    if (fillOrigin == 1)
                    {
                        v.x = v.z - (v.z - v.x) * fillAmount;
                        tx0 = tx1 - fill;
                    }
                    else
                    {
                        v.z = v.x + (v.z - v.x) * fillAmount;
                        tx1 = tx0 + fill;
                    }
                }
                else if (fillMethod == FillMethod.Vertical)
                {
                    float fill = (ty1 - ty0) * fillAmount;

                    if (fillOrigin == 1)
                    {
                        v.y = v.w - (v.w - v.y) * fillAmount;
                        ty0 = ty1 - fill;
                    }
                    else
                    {
                        v.w = v.y + (v.w - v.y) * fillAmount;
                        ty1 = ty0 + fill;
                    }
                }
            }

            s_Xy[0] = new Vector2(v.x, v.y);
            s_Xy[1] = new Vector2(v.x, v.w);
            s_Xy[2] = new Vector2(v.z, v.w);
            s_Xy[3] = new Vector2(v.z, v.y);

            s_Uv[0] = new Vector2(tx0, ty0);
            s_Uv[1] = new Vector2(tx0, ty1);
            s_Uv[2] = new Vector2(tx1, ty1);
            s_Uv[3] = new Vector2(tx1, ty0);

            s_Uv1[0] = new Vector2(0, 0);
            s_Uv1[1] = new Vector2(0, 1);
            s_Uv1[2] = new Vector2(1, 1);
            s_Uv1[3] = new Vector2(1, 0);

            {
                if (fillAmount < 1f && fillMethod != FillMethod.Horizontal && fillMethod != FillMethod.Vertical)
                {
                    if (fillMethod == FillMethod.Radial90)
                    {
                        if (RadialCut(s_Xy, s_Uv, fillAmount, fillClockwise, fillOrigin))
                            AddQuad(toFill, s_Xy, color, s_Uv, s_Uv1);
                    }
                    else if (fillMethod == FillMethod.Radial180)
                    {
                        for (int side = 0; side < 2; ++side)
                        {
                            float fx0, fx1, fy0, fy1;
                            int even = fillOrigin > 1 ? 1 : 0;

                            if (fillOrigin == 0 || fillOrigin == 2)
                            {
                                fy0 = 0f;
                                fy1 = 1f;
                                if (side == even)
                                {
                                    fx0 = 0f;
                                    fx1 = 0.5f;
                                }
                                else
                                {
                                    fx0 = 0.5f;
                                    fx1 = 1f;
                                }
                            }
                            else
                            {
                                fx0 = 0f;
                                fx1 = 1f;
                                if (side == even)
                                {
                                    fy0 = 0.5f;
                                    fy1 = 1f;
                                }
                                else
                                {
                                    fy0 = 0f;
                                    fy1 = 0.5f;
                                }
                            }

                            s_Xy[0].x = Mathf.Lerp(v.x, v.z, fx0);
                            s_Xy[1].x = s_Xy[0].x;
                            s_Xy[2].x = Mathf.Lerp(v.x, v.z, fx1);
                            s_Xy[3].x = s_Xy[2].x;

                            s_Xy[0].y = Mathf.Lerp(v.y, v.w, fy0);
                            s_Xy[1].y = Mathf.Lerp(v.y, v.w, fy1);
                            s_Xy[2].y = s_Xy[1].y;
                            s_Xy[3].y = s_Xy[0].y;

                            s_Uv[0].x = Mathf.Lerp(tx0, tx1, fx0);
                            s_Uv[1].x = s_Uv[0].x;
                            s_Uv[2].x = Mathf.Lerp(tx0, tx1, fx1);
                            s_Uv[3].x = s_Uv[2].x;

                            s_Uv[0].y = Mathf.Lerp(ty0, ty1, fy0);
                            s_Uv[1].y = Mathf.Lerp(ty0, ty1, fy1);
                            s_Uv[2].y = s_Uv[1].y;
                            s_Uv[3].y = s_Uv[0].y;

                            float val = fillClockwise ? fillAmount * 2f - side : fillAmount * 2f - (1 - side);

                            if (RadialCut(s_Xy, s_Uv, Mathf.Clamp01(val), fillClockwise, ((side + fillOrigin + 3) % 4)))
                            {
                                AddQuad(toFill, s_Xy, color, s_Uv, s_Uv1);
                            }
                        }
                    }
                    else if (fillMethod == FillMethod.Radial360)
                    {
                        for (int corner = 0; corner < 4; ++corner)
                        {
                            float fx0, fx1, fy0, fy1;

                            if (corner < 2)
                            {
                                fx0 = 0f;
                                fx1 = 0.5f;
                            }
                            else
                            {
                                fx0 = 0.5f;
                                fx1 = 1f;
                            }

                            if (corner == 0 || corner == 3)
                            {
                                fy0 = 0f;
                                fy1 = 0.5f;
                            }
                            else
                            {
                                fy0 = 0.5f;
                                fy1 = 1f;
                            }

                            s_Xy[0].x = Mathf.Lerp(v.x, v.z, fx0);
                            s_Xy[1].x = s_Xy[0].x;
                            s_Xy[2].x = Mathf.Lerp(v.x, v.z, fx1);
                            s_Xy[3].x = s_Xy[2].x;

                            s_Xy[0].y = Mathf.Lerp(v.y, v.w, fy0);
                            s_Xy[1].y = Mathf.Lerp(v.y, v.w, fy1);
                            s_Xy[2].y = s_Xy[1].y;
                            s_Xy[3].y = s_Xy[0].y;

                            s_Uv[0].x = Mathf.Lerp(tx0, tx1, fx0);
                            s_Uv[1].x = s_Uv[0].x;
                            s_Uv[2].x = Mathf.Lerp(tx0, tx1, fx1);
                            s_Uv[3].x = s_Uv[2].x;

                            s_Uv[0].y = Mathf.Lerp(ty0, ty1, fy0);
                            s_Uv[1].y = Mathf.Lerp(ty0, ty1, fy1);
                            s_Uv[2].y = s_Uv[1].y;
                            s_Uv[3].y = s_Uv[0].y;

                            float val = fillClockwise ?
                                fillAmount * 4f - ((corner + fillOrigin) % 4) :
                                fillAmount * 4f - (3 - ((corner + fillOrigin) % 4));

                            if (RadialCut(s_Xy, s_Uv, Mathf.Clamp01(val), fillClockwise, ((corner + 2) % 4)))
                                AddQuad(toFill, s_Xy, color, s_Uv, s_Uv1);
                        }
                    }
                }
                else
                {
                    AddQuad(toFill, s_Xy, color, s_Uv, s_Uv1);
                }
            }
        }

        /// <summary>
        /// Adjust the specified quad, making it be radially filled instead.
        /// </summary>

        static bool RadialCut(Vector3[] xy, Vector3[] uv, float fill, bool invert, int corner)
        {
            // Nothing to fill
            if (fill < 0.001f) return false;

            // Even corners invert the fill direction
            if ((corner & 1) == 1) invert = !invert;

            // Nothing to adjust
            if (!invert && fill > 0.999f) return true;

            // Convert 0-1 value into 0 to 90 degrees angle in radians
            float angle = Mathf.Clamp01(fill);
            if (invert) angle = 1f - angle;
            angle *= 90f * Mathf.Deg2Rad;

            // Calculate the effective X and Y factors
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            RadialCut(xy, cos, sin, invert, corner);
            RadialCut(uv, cos, sin, invert, corner);
            return true;
        }

        /// <summary>
        /// Adjust the specified quad, making it be radially filled instead.
        /// </summary>

        static void RadialCut(Vector3[] xy, float cos, float sin, bool invert, int corner)
        {
            int i0 = corner;
            int i1 = ((corner + 1) % 4);
            int i2 = ((corner + 2) % 4);
            int i3 = ((corner + 3) % 4);

            if ((corner & 1) == 1)
            {
                if (sin > cos)
                {
                    cos /= sin;
                    sin = 1f;

                    if (invert)
                    {
                        xy[i1].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
                        xy[i2].x = xy[i1].x;
                    }
                }
                else if (cos > sin)
                {
                    sin /= cos;
                    cos = 1f;

                    if (!invert)
                    {
                        xy[i2].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
                        xy[i3].y = xy[i2].y;
                    }
                }
                else
                {
                    cos = 1f;
                    sin = 1f;
                }

                if (!invert) xy[i3].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
                else xy[i1].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
            }
            else
            {
                if (cos > sin)
                {
                    sin /= cos;
                    cos = 1f;

                    if (!invert)
                    {
                        xy[i1].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
                        xy[i2].y = xy[i1].y;
                    }
                }
                else if (sin > cos)
                {
                    cos /= sin;
                    sin = 1f;

                    if (invert)
                    {
                        xy[i2].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
                        xy[i3].x = xy[i2].x;
                    }
                }
                else
                {
                    cos = 1f;
                    sin = 1f;
                }

                if (invert) xy[i3].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
                else xy[i1].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
            }
        }





        static void AddQuad(VertexHelper vertexHelper, Vector3[] quadPositions, Color32 color, Vector3[] quadUVs)
        {
            int startIndex = vertexHelper.currentVertCount;

            for (int i = 0; i < 4; ++i)
                vertexHelper.AddVert(quadPositions[i], color, quadUVs[i]);

            vertexHelper.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
            vertexHelper.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
        }
        static void AddQuad(VertexHelper vertexHelper, Vector2 posMin, Vector2 posMax, Color32 color, Vector2 uvMin, Vector2 uvMax)
        {
            int startIndex = vertexHelper.currentVertCount;

            vertexHelper.AddVert(new Vector3(posMin.x, posMin.y, 0), color, new Vector2(uvMin.x, uvMin.y));
            vertexHelper.AddVert(new Vector3(posMin.x, posMax.y, 0), color, new Vector2(uvMin.x, uvMax.y));
            vertexHelper.AddVert(new Vector3(posMax.x, posMax.y, 0), color, new Vector2(uvMax.x, uvMax.y));
            vertexHelper.AddVert(new Vector3(posMax.x, posMin.y, 0), color, new Vector2(uvMax.x, uvMin.y));

            vertexHelper.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
            vertexHelper.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
        }
        void AddQuads(VertexHelper vertexHelper, List<Vert2D> vlist)
        {
#if UNITY_2022_2_OR_NEWER
            if (m_GradientColor.mode == GradientMode.Blend || m_GradientColor.mode == GradientMode.PerceptualBlend)
#else
            if (m_GradientColor.mode == GradientMode.Blend)
#endif
            {
                int startIndex = vertexHelper.currentVertCount;

                foreach (Vert2D v in vlist)
                {
                    vertexHelper.AddVert(v.position, v.color, v.uv0);
                }
                for (; startIndex < vertexHelper.currentVertCount - 2; startIndex++)
                {
                    vertexHelper.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
                }
            }
            else
            {
                List<Vert2D> newvlist = new List<Vert2D>();
                if (m_Direction == GradientDirection.Horizontal)
                {
                    for (int i = 0; i < vlist.Count; i++)
                    {
                        if (i == 0 || i == 1)
                        {
                            Vert2D newvert = new Vert2D(vlist[i].type);
                            newvert.SetAlpha(vlist[i + 2].color.a);
                            newvert.SetColor(vlist[i + 2].color);
                            newvert.SetUV(vlist[i].uv0.x, vlist[i].uv0.y);
                            newvert.SetPosition(vlist[i].position);
                            newvlist.Add(newvert);
                        }
                        else if (i == vlist.Count - 2 || i == vlist.Count - 1)
                        {
                            newvlist.Add(vlist[i]);

                        }
                        else
                        {
                            newvlist.Add(vlist[i]);
                            Vert2D newvert = new Vert2D(vlist[i].type);
                            newvert.SetAlpha(vlist[i + 2].color.a);
                            newvert.SetColor(vlist[i + 2].color);
                            newvert.SetUV(vlist[i].uv0.x, vlist[i].uv0.y);
                            newvert.SetPosition(vlist[i].position);
                            newvlist.Add(newvert);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < vlist.Count; i++)
                    {
                        if (i == 0 || i == 1)
                        {
                            newvlist.Add(vlist[i]);
                        }
                        else if (i == vlist.Count - 2 || i == vlist.Count - 1)
                        {
                            Vert2D newvert = new Vert2D(vlist[i].type);
                            newvert.SetAlpha(vlist[i - 2].color.a);
                            newvert.SetColor(vlist[i - 2].color);
                            newvert.SetUV(vlist[i].uv0.x, vlist[i].uv0.y);
                            newvert.SetPosition(vlist[i].position);
                            newvlist.Add(newvert);

                        }
                        else
                        {

                            Vert2D newvert = new Vert2D(vlist[i].type);
                            newvert.SetAlpha(vlist[i - 2].color.a);
                            newvert.SetColor(vlist[i - 2].color);
                            newvert.SetUV(vlist[i].uv0.x, vlist[i].uv0.y);
                            newvert.SetPosition(vlist[i].position);
                            newvlist.Add(newvert);
                            newvlist.Add(vlist[i]);
                        }
                    }
                }
                foreach (Vert2D v in newvlist)
                {
                    vertexHelper.AddVert(v.position, v.color, v.uv0);
                }
                int s = -1;
                for (; s < newvlist.Count - 1; s += 4)
                {
                    if (s == -1)
                    {
                        vertexHelper.AddTriangle(s + 1, s + 2, s + 3);
                    }
                    else
                    {
                        vertexHelper.AddTriangle(s, s + 2, s + 3);
                    }
                    if (s == newvlist.Count - 5)
                    {
                        vertexHelper.AddTriangle(s + 2, s + 3, s + 4);
                    }
                    else
                    {
                        vertexHelper.AddTriangle(s + 2, s + 3, s + 5);
                    }
                }
            }
        }
        public override Material GetModifiedMaterial(Material baseMaterial)
        {
            var toUse = baseMaterial;

            if (m_ShouldRecalculateStencil)
            {
                if (maskable)
                {
                    var rootCanvas = MaskUtilities.FindRootSortOverrideCanvas(transform);
                    m_StencilValue = MaskUtilities.GetStencilDepth(transform, rootCanvas);
                }
                else
                    m_StencilValue = 0;

                m_ShouldRecalculateStencil = false;
            }

            // if we have a enabled Mask component then it will
            // generate the mask material. This is an optimization
            // it adds some coupling between components though :(
            if (m_StencilValue > 0 && !isMaskingGraphic)
            {
                var t = GetComponentInParent<UXMask>();
                Material maskMat;
                if (m_StencilValue == 1)
                {
                    if (t != null && t.IsReverseMask)
                    {
                        maskMat = StencilMaterial.Add(toUse, (1 << m_StencilValue) - 1, StencilOp.Keep, CompareFunction.NotEqual, ColorWriteMask.All, (1 << m_StencilValue) - 1, 0);
                    }
                    else
                    {
                        maskMat = StencilMaterial.Add(toUse, (1 << m_StencilValue) - 1, StencilOp.Keep, CompareFunction.Equal, ColorWriteMask.All, (1 << m_StencilValue) - 1, 0);
                    }
                }
                else
                {
                    if (t != null && t.IsReverseMask)
                    {
                        maskMat = StencilMaterial.Add(toUse, 1, StencilOp.Keep, CompareFunction.Greater, ColorWriteMask.All, (1 << m_StencilValue) - 1, 0);
                    }
                    else
                    {
                        maskMat = StencilMaterial.Add(toUse, (1 << m_StencilValue) - 1, StencilOp.Keep, CompareFunction.Equal, ColorWriteMask.All, (1 << m_StencilValue) - 1, 0);
                    }
                }
                StencilMaterial.Remove(m_MaskMaterial);
                m_MaskMaterial = maskMat;
                toUse = m_MaskMaterial;
            }
            return toUse;
        }

        #endregion
    }

}

