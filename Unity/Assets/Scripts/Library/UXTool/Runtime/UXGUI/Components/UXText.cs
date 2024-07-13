using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ThunderFireUnityEx;

namespace UnityEngine.UI
{
    public class UXText : Text, ILocalizationText
    {
        [SerializeField]
        private LocalizationHelper.TextLocalizationType m_localizationType = LocalizationHelper.TextLocalizationType.RuntimeUse;
        public LocalizationHelper.TextLocalizationType localizationType { get { return m_localizationType; } set { m_localizationType = value; } }
        // 记录一下未提取的RuntimeUse类型的原文,方便切换语言时显示前缀
        private string originText;
        [SerializeField]
        private bool m_ignoreLocalization = true;
        public bool ignoreLocalization { get { return m_ignoreLocalization; } set { m_ignoreLocalization = value; } }
        private UXTextLocaleProcesser localeProcesser;


        public bool UseTashkeel = false;
        public bool UseHinduNumber = false;
        public bool EnableArabicFix = false;
        public bool useParagraphShrinkFit = true;
#if !UNITY_EDITOR
        private bool textBeModified = false;
#endif

        /// <summary>
        /// 需要被渲染的文字,已经经过Localization,还没经过替换省略号
        /// </summary>
        private string RenderedText { get; set; } = null;

        public float limitPreferredWidth = 0;
        public float limitPreferredHeight = 0;

        public enum TextEllipsisType
        {
            [InspectorName("文字超框显示省略号在左侧")]
            EllipsisOnLeft = 0,
            [InspectorName("文字超框显示省略号在右侧")]
            EllipsisOnRight = 1,
            [InspectorName("文字超框不显示省略号")]
            NoEllipsis = 2,
        }
        public TextEllipsisType ellipsType = TextEllipsisType.EllipsisOnRight;
        private const string ELLIPSIS = "…";
        /// <summary>
        /// 是否有超框文字
        /// </summary>
        private bool HasEllipsis { get; set; } = false;
        /// <summary>
        /// 本地化用到的ID
        /// </summary>
        [SerializeField]
        private string m_localizationID = "";
        public string localizationID
        {
            get { return m_localizationID; }
            set { m_localizationID = value; }
        }
        [SerializeField]
        private string m_previewID = "";
        public string previewID
        {
            get { return m_previewID; }
            set
            {
                if (m_previewID != value)
                {
                    m_previewID = value;
                    ChangeLanguage(LocalizationHelper.GetLanguage());
                }
            }
        }
        private static readonly string need_replace = "UNFILLED TEXT";

        public override float preferredWidth
        {
            get
            {
                var settings = GetGenerationSettings(Vector2.zero);
                var width = cachedTextGeneratorForLayout.GetPreferredWidth(m_Text, settings) / pixelsPerUnit;

                return limitPreferredWidth > 0 ? Mathf.Min(width, limitPreferredWidth) : width;
            }
        }

        public override float preferredHeight
        {
            get
            {
                var settings = GetGenerationSettings(new Vector2(GetPixelAdjustedRect().size.x, 0.0f));
                var height = cachedTextGeneratorForLayout.GetPreferredHeight(m_Text, settings) / pixelsPerUnit;
                return limitPreferredHeight > 0 ? Mathf.Min(height, limitPreferredHeight) : height;
            }
        }

        public System.Action<UXText> OnTextWidthChanged;
        public override string text
        {
            get
            {
                return base.text;
            }
            set
            {
#if !UNITY_EDITOR
                textBeModified = true;
#endif
                UpdateQuadImage(value);
                base.text = value;
            }
        }

        readonly UIVertex[] m_TempVerts = new UIVertex[4];

        private UXRolling rolling;

        [NonSerialized]
        public TextAnchor originAlignment;
        protected override void Awake()
        {
            // clear preview text if not editer
#if !UNITY_EDITOR
            if (localizationType == LocalizationHelper.TextLocalizationType.Preview && !textBeModified)
            {
                base.text = "";
            }
#endif
            base.Awake();
            originAlignment = alignment;
            rolling = GetComponentInParent<UXRolling>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            CreateLocaleProcesser();
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (font == null)
                return;

            // We don't care if we the font Texture changes while we are doing our Update.
            // The end result of cachedTextGenerator will be valid for this instance.
            // Otherwise we can get issues like Case 619238.
            m_DisableFontTextureRebuiltCallback = true;

            #region UX Custom
            // Localization/BestFit排版/超框处理 
            CustomRefreshRenderedText();
            cachedTextGenerator.PopulateWithErrors(RenderedText, customGenerationSettings, gameObject);
            localeProcesser.FixVerts(cachedTextGenerator.verts);
            #endregion

            #region Text
            // Text.OnPopulateMesh()的原实现，有变动需要同步过来
            // Apply the offset to the vertices
            IList<UIVertex> verts = cachedTextGenerator.verts;
            float unitsPerPixel = 1 / pixelsPerUnit;
            int vertCount = verts.Count;

            // We have no verts to process just return (case 1037923)
            if (vertCount <= 0)
            {
                toFill.Clear();
                return;
            }

            Vector2 roundingOffset = new Vector2(verts[0].position.x, verts[0].position.y) * unitsPerPixel;
            roundingOffset = PixelAdjustPoint(roundingOffset) - roundingOffset;
            toFill.Clear();

            if (roundingOffset != Vector2.zero)
            {
                for (int i = 0; i < vertCount; ++i)
                {
                    int tempVertsIndex = i & 3;
                    m_TempVerts[tempVertsIndex] = verts[i];
                    m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                    m_TempVerts[tempVertsIndex].position.x += roundingOffset.x;
                    m_TempVerts[tempVertsIndex].position.y += roundingOffset.y;
                    if (tempVertsIndex == 3)
                        toFill.AddUIVertexQuad(m_TempVerts);
                }
            }
            else
            {
                for (int i = 0; i < vertCount; ++i)
                {
                    int tempVertsIndex = i & 3;
                    m_TempVerts[tempVertsIndex] = verts[i];
                    m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                    if (tempVertsIndex == 3)
                        toFill.AddUIVertexQuad(m_TempVerts);
                }
            }

            m_DisableFontTextureRebuiltCallback = false;
            #endregion

            #region TextWithSprite
            for (var i = 0; i < m_ImagesVertexIndex.Count; i++)
            {
                var endIndex = m_ImagesVertexIndex[i];
                var rt = m_ImagesPool[i].rectTransform;
                var size = rt.sizeDelta;
                if (endIndex < toFill.currentVertCount)
                {
                    var v1 = new UIVertex();
                    var v2 = new UIVertex();
                    var v3 = new UIVertex();
                    var v4 = new UIVertex();

                    toFill.PopulateUIVertex(ref v1, endIndex - 3);
                    toFill.PopulateUIVertex(ref v2, endIndex - 2);
                    toFill.PopulateUIVertex(ref v3, endIndex - 1);
                    toFill.PopulateUIVertex(ref v4, endIndex);
                    var mid = 0.5f * (v1.position + v3.position);
                    v1.position = new Vector3(mid.x - size.x / 2, mid.y + size.y / 2, 0);
                    v2.position = new Vector3(mid.x + size.x / 2, mid.y + size.y / 2, 0);
                    v3.position = new Vector3(mid.x + size.x / 2, mid.y - size.y / 2, 0);
                    v4.position = new Vector3(mid.x - size.x / 2, mid.y - size.y / 2, 0);

                    toFill.SetUIVertex(v1, endIndex - 3);
                    toFill.SetUIVertex(v2, endIndex - 2);
                    toFill.SetUIVertex(v3, endIndex - 1);
                    toFill.SetUIVertex(v4, endIndex);


                    UIVertex vert = new UIVertex();
                    toFill.PopulateUIVertex(ref vert, endIndex);
                    rt.anchoredPosition = new Vector2(vert.position.x + size.x / 2, vert.position.y + size.y / 2);
                    for (var k = endIndex - 3; k <= endIndex; k++)
                    {
                        UIVertex vertK = new UIVertex();
                        toFill.PopulateUIVertex(ref vertK, endIndex);
                        vertK.color = new Color32(0, 0, 0, 0);
                        toFill.SetUIVertex(vertK, k);
                    }
                }
            }
            #endregion

        }

        public TextGenerationSettings customGenerationSettings;
        public TextGenerationSettings CustomGenerationSettings { get => customGenerationSettings; }


        public void CustomModifyLocalizationSettings()
        {

        }
        public void CustomRefreshRenderedText()
        {
            UnityEngine.Profiling.Profiler.BeginSample("UXText.CustomRefreshRenderedText");

            // 所有PopulateWithErrors都使用如下调用来完成，已确保之前的修改都应用上了
            // cachedTextGenerator.PopulateWithErrors(RenderedText, customGenerationSettings, this.gameObject);

            PrepareCustomTextRender();

            DoShrinkFit();
            // 如果文本超框 末尾显示省略号
            ReplaceOutOfAreaTextWithEllipsis();
            UnityEngine.Profiling.Profiler.EndSample();
        }


        private void CreateLocaleProcesser()
        {
            // 初始化localeProcesser 各个语种实现各自的processer
            if (localeProcesser == null || localeProcesser.LocalizationType != (LocalizationHelper.LanguageType)LocalizationHelper.GetLanguage())
            {
                localeProcesser = LocaleProcesserFactory.Create(this);
            }
            if (Application.isPlaying)
            {
                //runtime修改Localization后Text参数
                localeProcesser.ModifyLocaleTextSettings();
            }
        }

        private void PrepareCustomTextRender()
        {
            UnityEngine.Profiling.Profiler.BeginSample("UXText.PrepareCustomTextRender");
            // 初始化customGenerationSettings
            customGenerationSettings = GetGenerationSettings(rectTransform.rect.size);

            // 初始化RenderedText
            string needRenderderTxt = null;
            if (Application.isPlaying)
            {
                //现在运行时本地化切换语言后，会更新this.text，所以不需要再读一遍本地化文本了
                needRenderderTxt = localeProcesser.GenLocaleRenderedString(text);
            }
            else
            {
#if UNITY_EDITOR
                needRenderderTxt = localeProcesser.GenLocaleRenderedString(text);
#endif
            }
            this.RenderedText = needRenderderTxt;

            DisplayTextPreferredWidth = GetDisplayTextWidth(RenderedText);
            UnityEngine.Profiling.Profiler.EndSample();
        }

        /// <summary>
        /// process for best fit
        /// </summary>
        private void DoShrinkFit()
        {
            UnityEngine.Profiling.Profiler.BeginSample("UXText.DoShrinkFit");
            if (this.UseParagraphShrinkFit())
            {
                customGenerationSettings.resizeTextForBestFit = false;

                int minSize = resizeTextMinSize;
                int txtLen = this.RenderedText.Length;
                for (int i = resizeTextMaxSize; i >= minSize; --i)
                {
                    customGenerationSettings.fontSize = i;
                    cachedTextGenerator.PopulateWithErrors(this.RenderedText, customGenerationSettings, this.gameObject);
                    if (cachedTextGenerator.characterCountVisible >= txtLen) break;
                }
            }
            else
            {
                cachedTextGenerator.PopulateWithErrors(this.RenderedText, customGenerationSettings, this.gameObject);
            }
            UnityEngine.Profiling.Profiler.EndSample();
        }
        public bool UseParagraphShrinkFit()
        {
            return this.resizeTextForBestFit
                && this.useParagraphShrinkFit
                && this.horizontalOverflow == HorizontalWrapMode.Wrap;
        }

        /// <summary>
        /// process for ...
        /// </summary>
        private void ReplaceOutOfAreaTextWithEllipsis()
        {
            if (ellipsType == TextEllipsisType.NoEllipsis) return;
            var needRenderderTxt = this.RenderedText;
            var characterCountVisible = cachedTextGenerator.characterCountVisible;

            UnityEngine.Profiling.Profiler.BeginSample("UXText.ReplaceOutOfAreaTextWithEllipsis");
            //滚动文字就不用省略号
            HasEllipsis = rolling == null && needRenderderTxt.Length > characterCountVisible && characterCountVisible > 0;
            if (HasEllipsis)
            {
                if (Application.isPlaying)
                {
                    Debug.Log($"有UXText组件文字超框,path:{this.transform.PathFromRoot()}, content:{needRenderderTxt.Substring(0, Mathf.Min(needRenderderTxt.Length, 10))}");
                }
                if (this.ellipsType == TextEllipsisType.EllipsisOnRight)
                {
                    RenderedText = needRenderderTxt.Substring(0, characterCountVisible - 1) + ELLIPSIS;
                }
                else if (this.ellipsType == TextEllipsisType.EllipsisOnLeft)
                {
                    RenderedText = ELLIPSIS + needRenderderTxt.Substring(needRenderderTxt.Length - characterCountVisible - 1);
                }
                else
                {
                    RenderedText = needRenderderTxt.Substring(0, characterCountVisible);
                }
                cachedTextGenerator.PopulateWithErrors(RenderedText, customGenerationSettings, this.gameObject);
                characterCountVisible = cachedTextGenerator.characterCountVisible;
                while (RenderedText.Length > 1 && RenderedText.Length > characterCountVisible)
                {
                    var needLen = RenderedText.Length - 2;
                    if (this.ellipsType == TextEllipsisType.EllipsisOnRight)
                        RenderedText = needRenderderTxt.Substring(0, needLen) + ELLIPSIS;
                    else if (this.ellipsType == TextEllipsisType.EllipsisOnLeft)
                        RenderedText = ELLIPSIS + needRenderderTxt.Substring(needRenderderTxt.Length - needLen);
                    cachedTextGenerator.PopulateWithErrors(RenderedText, customGenerationSettings, this.gameObject);
                    characterCountVisible = cachedTextGenerator.characterCountVisible;
                }
            }
            UnityEngine.Profiling.Profiler.EndSample();
        }

        float displayTextPreferredWidth = -1;
        public float DisplayTextPreferredWidth
        {
            get
            {
                if (displayTextPreferredWidth < 0) return preferredWidth;
                return displayTextPreferredWidth;
            }
            private set
            {
                displayTextPreferredWidth = value;
                if (OnTextWidthChanged != null)
                    OnTextWidthChanged(this);
            }
        }
        float GetDisplayTextWidth(string text)
        {
            return cachedTextGeneratorForLayout.GetPreferredWidth(text, this.customGenerationSettings) / pixelsPerUnit;
        }

        private static bool loaded = false;
        private int origin_len;

        protected override void Start()
        {
            base.Start();
            if (!Application.isPlaying) return;
            origin_len = text.Length;
            if (!loaded)
            {
                loaded = true;
            }
            ChangeLanguage(LocalizationHelper.GetLanguage());
        }

        protected override void OnDestroy()
        {
            if (m_LoadedSprites.Count > 0)
            {
                foreach (Sprite sprite in m_LoadedSprites)
                {
                    ResourceManager.Unload(sprite);
                }
                m_LoadedSprites.Clear();
            }
            base.OnDestroy();
        }

        public void ChangeLanguage(LocalizationHelper.LanguageType language)
        {
            if (language == LocalizationHelper.LanguageType.NoWord && !ignoreLocalization)
            {
                text = "";
                for (int i = 0; i < origin_len; i++)
                {
                    text += '□';
                }
                return;
            }
            string id = localizationType == LocalizationHelper.TextLocalizationType.RuntimeUse ? localizationID : m_previewID;
            if (language == LocalizationHelper.LanguageType.ShowKey && !ignoreLocalization)
            {
                text = id;
                return;
            }
            if (language >= 0 && id != "" && !ignoreLocalization)
            {
                text = LocalizationHelper.GetString(language, id, need_replace);
            }
        }

        public static GameObject GetRootObject(GameObject gameObject)
        {
            GameObject rootObject = gameObject;

            while (rootObject.transform.parent != null)
            {
                rootObject = rootObject.transform.parent.gameObject;
            }

            return rootObject;
        }

        private readonly List<Sprite> m_LoadedSprites = new List<Sprite>();
        private readonly List<int> m_ImagesVertexIndex = new List<int>();
        private readonly List<Image> m_ImagesPool = new List<Image>();
        private static readonly Regex s_Regex =
            new Regex(@"<quad name=(.+?) width=(\d*\.?\d+%?) height=(\d*\.?\d+%?) />", RegexOptions.Singleline);

        public void UpdateQuadImage(string updatedText)
        {
            m_ImagesVertexIndex.Clear();
            if (string.IsNullOrEmpty(updatedText))
            {
                return;
            }
            var matches = s_Regex.Matches(updatedText);
            var picLength = 0;
            var picNum = 0;
            foreach (Match match in matches)
            {
                var str = updatedText.Substring(0, match.Index);
                var replace = str.Replace(" ", "");
                var length = str.Length - replace.Length;
                var picIndex = match.Index - picLength - length + picNum;
                picNum += 1;
                picLength += match.Length - 4;
                var endIndex = picIndex * 4 + 3;
                m_ImagesVertexIndex.Add(endIndex);

                m_ImagesPool.RemoveAll(image => image == null);
                if (m_ImagesPool.Count == 0)
                {
                    GetComponentsInChildren<Image>(true, m_ImagesPool);
                }
                if (m_ImagesVertexIndex.Count > m_ImagesPool.Count)
                {
                    GameObject go = new GameObject("UXImage");
                    Image image = go.AddComponent<UXImage>();
                    go.layer = gameObject.layer;
                    var rt = go.transform as RectTransform;
                    if (rt)
                    {
                        rt.SetParent(rectTransform);
                        rt.localPosition = Vector3.zero;
                        rt.localRotation = Quaternion.identity;
                        rt.localScale = Vector3.one;
                        var transform1 = transform;
                        rt.anchorMax = ((RectTransform)transform1).anchorMax;
                        rt.anchorMin = ((RectTransform)transform1).anchorMin;
                    }
                    m_ImagesPool.Add(image);
                }

                var spritePath = match.Groups[1].Value;
                var width = float.Parse(match.Groups[2].Value);
                var height = float.Parse(match.Groups[3].Value);
                var img = m_ImagesPool[m_ImagesVertexIndex.Count - 1];
                if (img.sprite == null || img.sprite.name != spritePath)
                {
                    ResourceManager.AsyncLoad<Sprite>(spritePath, sprite =>
                    {
                        if (img != null)
                        {
                            m_LoadedSprites.Add(sprite);
                            img.sprite = sprite;
                            img.rectTransform.sizeDelta = new Vector2(width, height);
                            img.gameObject.SetActive(true);
                        }
                        else
                        {
                            ResourceManager.Unload(sprite);
                        }
                    });
                    // img.enabled = true;
                }
            }

            for (var i = m_ImagesVertexIndex.Count; i < m_ImagesPool.Count; i++)
            {
                if (m_ImagesPool[i])
                {
                    // m_ImagesPool[i].enabled =  false;
                    m_ImagesPool[i].gameObject.SetActive(false);
                }
            }
            // SetVerticesDirty();
        }
    }
}
