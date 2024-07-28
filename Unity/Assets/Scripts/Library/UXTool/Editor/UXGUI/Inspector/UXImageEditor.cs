using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.AnimatedValues;
using UnityEditor.UI;

using ThunderFireUITool;

namespace UnityEngine.UI
{
    [CustomEditor(typeof(UXImage), true)]
    [CanEditMultipleObjects]
    public class UXImageEditor : GraphicEditor
    {
        private GameObject go;
        private UXImage image;
        private RectTransform rect;

        private SerializedProperty m_Sprite;
        private SerializedProperty m_Type;
        private SerializedProperty m_IgnoreLocalization;
        private SerializedProperty m_sourceImage;
        private SerializedProperty m_ColorType;
        private SerializedProperty m_Direction;

        SerializedProperty m_UseSpriteMesh;
        SerializedProperty m_PreserveAspect;
        SerializedProperty m_FillCenter;
        SerializedProperty m_PixelsPerUnitMultiplier;
        SerializedProperty m_FillMethod;
        SerializedProperty m_FillOrigin;
        SerializedProperty m_FillAmount;
        SerializedProperty m_FillClockwise;

        SerializedProperty m_FlipMode;
        SerializedProperty m_FlipWithCopy;
        SerializedProperty m_FlipEdgeHorizontal;
        SerializedProperty m_FlipEdgeVertical;
        SerializedProperty m_FlipFillCenter;
        SerializedProperty m_GradientColor;
        GUIContent m_ClockwiseContent;
        GUIContent m_SpriteFlipContent;
        GUIContent m_FlipModeContent;
        GUIContent m_FlipEdgeContent;
        GUIContent m_FlipFillContent;

        private SerializedProperty m_LinearToGamma;
        private SerializedProperty m_GammaToLinear;

        private SerializedProperty m_EnableGrey;
        private SerializedProperty m_Contrast;
        private SerializedProperty m_Saturation;

        private SerializedProperty m_CornerRadius;
        private SerializedProperty m_EnableRadiusCorner;

        private SerializedProperty m_EnableDistortion;
        private SerializedProperty m_BottomLeftDistortion;
        private SerializedProperty m_BottomRightDistortion;
        private SerializedProperty m_TopLeftDistortion;
        private SerializedProperty m_TopRightDistortion;

        private ReorderableList m_CustomList;
        private List<Sprite> m_SpriteList;
        private bool[] m_ToggleValues;
        private int m_LastToggleIndex;
        private UXImage m_targetObject;
        private GameObject m_CloneObj;
        private string m_Origin_name;
        private Sprite m_Need_replace;
        private bool m_InitialHide;
#if UNITY_2021_1_OR_NEWER
        private bool m_bIsDriven;
#endif
        private GUIContent m_SpriteContent_self;
        private GUIContent m_PaddingContent_self;
        private GUIContent m_LeftContent_self;
        private GUIContent m_RightContent_self;
        private GUIContent m_TopContent_self;
        private GUIContent m_BottomContent_self;
        private GUIContent m_CorrectButtonContent_self;
        GUIContent m_SpriteTypeContent;

        AnimBool m_ShowType;
        AnimBool m_ShowSlicedOrTiled;
        AnimBool m_ShowSliced;
        AnimBool m_ShowTiled;
        AnimBool m_ShowFilled;

        static private bool m_ShowPadding = false;

        private static readonly int EnableGrey = Shader.PropertyToID("_EnableGrey");
        private static readonly int Contrast = Shader.PropertyToID("_Contrast");
        private static readonly int Saturation = Shader.PropertyToID("_Saturation");
        private static readonly int LinearToGamma = Shader.PropertyToID("_LinearToGamma");
        private static readonly int GammaToLinear = Shader.PropertyToID("_GammaToLinear");

        private static readonly int RadiusCorner = Shader.PropertyToID("_RadiusCorner");
        private static readonly int prop_halfSize = Shader.PropertyToID("_halfSize");
        private static readonly int prop_radiuses = Shader.PropertyToID("_r");
        private static readonly int prop_rect2props = Shader.PropertyToID("_rect2props");

        private class Styles
        {
            public static GUIContent text = EditorGUIUtility.TrTextContent("Fill Origin");
            public static GUIContent[] OriginHorizontalStyle =
            {
                EditorGUIUtility.TrTextContent("Left"),
                EditorGUIUtility.TrTextContent("Right")
            };

            public static GUIContent[] OriginVerticalStyle =
            {
                EditorGUIUtility.TrTextContent("Bottom"),
                EditorGUIUtility.TrTextContent("Top")
            };

            public static GUIContent[] Origin90Style =
            {
                EditorGUIUtility.TrTextContent("BottomLeft"),
                EditorGUIUtility.TrTextContent("TopLeft"),
                EditorGUIUtility.TrTextContent("TopRight"),
                EditorGUIUtility.TrTextContent("BottomRight")
            };

            public static GUIContent[] Origin180Style =
            {
                EditorGUIUtility.TrTextContent("Bottom"),
                EditorGUIUtility.TrTextContent("Left"),
                EditorGUIUtility.TrTextContent("Top"),
                EditorGUIUtility.TrTextContent("Right")
            };

            public static GUIContent[] Origin360Style =
            {
                EditorGUIUtility.TrTextContent("Bottom"),
                EditorGUIUtility.TrTextContent("Right"),
                EditorGUIUtility.TrTextContent("Top"),
                EditorGUIUtility.TrTextContent("Left")
            };
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            image = target as UXImage;
            rect = image.GetComponent<RectTransform>();
            go = image.gameObject;

            UIEffectWrapDrawer.InitEffectDrawer(UIEffectWrapDrawer.EffectDrawerTargetType.UXImage, go);

            m_Color = serializedObject.FindProperty("m_Color");
            m_Sprite = serializedObject.FindProperty("m_Sprite");
            m_Type = serializedObject.FindProperty("m_Type");
            m_Material = serializedObject.FindProperty("m_Material");
            m_IgnoreLocalization = serializedObject.FindProperty("m_ignoreLocalization");
            m_RaycastTarget = serializedObject.FindProperty("m_RaycastTarget");
#if UNITY_2021_1_OR_NEWER
            m_RaycastPadding = serializedObject.FindProperty("m_RaycastPadding");
#endif
            m_sourceImage = serializedObject.FindProperty("m_Sprite");
            m_Maskable = serializedObject.FindProperty("m_Maskable");
            //spriteContent = new GUIContent("Source Image");
            m_targetObject = serializedObject.targetObject as UXImage;
            m_Origin_name = m_Sprite.objectReferenceValue?.name;
            m_ToggleValues = new bool[EditorLocalizationTool.ReadyLanguageTypes.Length];
            m_LastToggleIndex = -1;
            // m_Need_replace = ResourceManager.Load<Sprite>("need_replace");
            // ResourceManager.AsyncLoad<Sprite>(UXGUIConfig.UXGUINeedReplaceSpritePath, sprite => m_Need_replace = sprite);
            m_Need_replace = AssetDatabase.LoadAssetAtPath<Sprite>(UXGUIConfig.UXGUINeedReplaceSpritePathReplace);
            m_SpriteList = new List<Sprite>();


            m_ColorType = serializedObject.FindProperty("m_ColorType");
            m_GradientColor = serializedObject.FindProperty("m_GradientColor");
            m_Direction = serializedObject.FindProperty("m_Direction");

            m_UseSpriteMesh = serializedObject.FindProperty("m_UseSpriteMesh");
            m_PreserveAspect = serializedObject.FindProperty("m_PreserveAspect");
            m_FillCenter = serializedObject.FindProperty("m_FillCenter");
            m_PixelsPerUnitMultiplier = serializedObject.FindProperty("m_PixelsPerUnitMultiplier");
            m_FillMethod = serializedObject.FindProperty("m_FillMethod");
            m_FillOrigin = serializedObject.FindProperty("m_FillOrigin");
            m_FillClockwise = serializedObject.FindProperty("m_FillClockwise");
            m_FillAmount = serializedObject.FindProperty("m_FillAmount");

            m_FlipModeContent = new GUIContent(EditorLocalization.GetLocalization("UXImage", "FlipMode"));
            m_FlipEdgeContent = new GUIContent(EditorLocalization.GetLocalization("UXImage", "FlipEdge"));
            m_FlipFillContent = new GUIContent(EditorLocalization.GetLocalization("UXImage", "FlipFill"));

            m_FlipMode = serializedObject.FindProperty("m_FlipMode");
            m_FlipWithCopy = serializedObject.FindProperty("m_FlipWithCopy");
            m_FlipEdgeHorizontal = serializedObject.FindProperty("m_FlipEdgeHorizontal");
            m_FlipEdgeVertical = serializedObject.FindProperty("m_FlipEdgeVertical");
            m_FlipFillCenter = serializedObject.FindProperty("m_FlipFillCenter");


            m_Contrast = serializedObject.FindProperty("m_Contrast");
            m_Saturation = serializedObject.FindProperty("m_Saturation");
            m_EnableGrey = serializedObject.FindProperty("m_EnableGrey");

            m_GammaToLinear = serializedObject.FindProperty("m_GammaToLinear");
            m_LinearToGamma = serializedObject.FindProperty("m_LinearToGamma");

            m_EnableRadiusCorner = serializedObject.FindProperty("m_EnableRadiusCorner");
            m_CornerRadius = serializedObject.FindProperty("m_CornerRadius");

            m_EnableDistortion = serializedObject.FindProperty("m_EnableDistortion");
            m_BottomLeftDistortion = serializedObject.FindProperty("m_BottomLeftDistortion");
            m_BottomRightDistortion = serializedObject.FindProperty("m_BottomRightDistortion");
            m_TopLeftDistortion = serializedObject.FindProperty("m_TopLeftDistortion");
            m_TopRightDistortion = serializedObject.FindProperty("m_TopRightDistortion");

#if UNITY_2021_1_OR_NEWER
            m_bIsDriven = false;
#endif
            m_PaddingContent_self = EditorGUIUtility.TrTextContent("Raycast Padding");
            m_LeftContent_self = EditorGUIUtility.TrTextContent("Left");
            m_RightContent_self = EditorGUIUtility.TrTextContent("Right");
            m_TopContent_self = EditorGUIUtility.TrTextContent("Top");
            m_BottomContent_self = EditorGUIUtility.TrTextContent("Bottom");
            m_CorrectButtonContent_self = EditorGUIUtility.TrTextContent("Set Native Size", "Sets the size to match the content.");
            m_SpriteTypeContent = EditorGUIUtility.TrTextContent("Image Type");
            m_ClockwiseContent = EditorGUIUtility.TrTextContent("Clockwise");
            m_SpriteContent_self = EditorGUIUtility.TrTextContent("Source Image");

            m_ShowType = new AnimBool(m_Sprite.objectReferenceValue != null);
            m_ShowType.valueChanged.AddListener(Repaint);
            m_ShowNativeSize = new AnimBool(false);
            m_ShowNativeSize.valueChanged.AddListener(Repaint);

            var typeEnum = (Image.Type)m_Type.enumValueIndex;
            m_ShowSlicedOrTiled = new AnimBool(!m_Type.hasMultipleDifferentValues && typeEnum == Image.Type.Sliced);
            m_ShowSliced = new AnimBool(!m_Type.hasMultipleDifferentValues && typeEnum == Image.Type.Sliced);
            m_ShowTiled = new AnimBool(!m_Type.hasMultipleDifferentValues && typeEnum == Image.Type.Tiled);
            m_ShowFilled = new AnimBool(!m_Type.hasMultipleDifferentValues && typeEnum == Image.Type.Filled);
            m_ShowSlicedOrTiled.valueChanged.AddListener(Repaint);
            m_ShowSliced.valueChanged.AddListener(Repaint);
            m_ShowTiled.valueChanged.AddListener(Repaint);
            m_ShowFilled.valueChanged.AddListener(Repaint);

            ChangeAvailables();

            m_CustomList = new ReorderableList(m_SpriteList, typeof(Sprite), false, true, false, false);
            m_CustomList.elementHeight = 80;
            m_CustomList.drawElementCallback = (Rect rect, int index, bool selected, bool focused) =>
            {
                EditorGUI.BeginChangeCheck();
                m_ToggleValues[index] = EditorGUI.Toggle(rect, m_ToggleValues[index]);
                if (EditorGUI.EndChangeCheck())
                {
                    if (m_ToggleValues[index])
                    {
                        if (m_LastToggleIndex == -1)
                        {
                            m_InitialHide = SceneVisibilityManager.instance.IsHidden(m_targetObject.gameObject);
                        }
                        else
                        {
                            m_ToggleValues[m_LastToggleIndex] = false;
                        }
                        m_LastToggleIndex = index;
                        if (m_CloneObj != null)
                        {
                            DestroyImmediate(m_CloneObj);
                        }
                        SceneVisibilityManager.instance.Hide(m_targetObject.gameObject, true);
                        m_targetObject.ignoreLocalization = true;
                        m_CloneObj = Instantiate(m_targetObject.gameObject, m_targetObject.transform.position, m_targetObject.transform.rotation, m_targetObject.transform);
                        m_CloneObj.transform.localScale = new Vector3(1, 1, 1);
                        RectTransform cloneRect = m_CloneObj.GetComponent<RectTransform>();
                        cloneRect.anchorMax = new Vector2(1, 1);
                        cloneRect.anchorMin = new Vector2(0, 0);
                        cloneRect.offsetMax = new Vector2(0, 0);
                        cloneRect.offsetMin = new Vector2(0, 0);
                        m_targetObject.ignoreLocalization = false;
                        m_CloneObj.GetComponent<Image>().sprite = m_SpriteList[index];
                        m_CloneObj.hideFlags = HideFlags.HideAndDontSave;
                    }
                    else
                    {
                        if (m_CloneObj != null)
                        {
                            DestroyImmediate(m_CloneObj);
                        }
                        if (!m_InitialHide)
                        {
                            SceneVisibilityManager.instance.Show(m_targetObject.gameObject, true);
                        }
                        m_LastToggleIndex = -1;
                    }
                }
                GUI.enabled = false;
                GUI.Label(new Rect(rect) { x = rect.x + 20 }, LocalizationLanguage.GetLanguage((int)EditorLocalizationTool.ReadyLanguageTypes[index]));
                EditorGUI.ObjectField(new Rect(rect) { x = rect.x + rect.width - 80, width = 80 }, m_SpriteList[index], typeof(Texture), false);
                GUI.enabled = true;
            };
            m_CustomList.drawHeaderCallback = (Rect rect) =>
            {
                int validNums = 0;
                foreach (var item in m_SpriteList)
                {
                    if (item != m_Need_replace)
                    {
                        validNums = validNums + 1;
                    }
                }
                GUI.Label(rect, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_本地化图素数目));
                if (m_Sprite.objectReferenceValue == null)
                {
                    GUI.Label(new Rect(rect) { x = rect.x + rect.width - 28 }, "0/0");
                    return;
                }
                if (validNums != EditorLocalizationTool.ReadyLanguageTypes.Length)
                {
                    GUI.contentColor = new Color(0xda / 255f, 0x5b / 255, 0x5b / 255);
                    GUI.Label(GUILayoutUtility.GetLastRect(), EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_请检查文件夹中图片的命名规范));
                }
                GUI.Label(new Rect(rect) { x = rect.x + rect.width - 28 - validNums / 10 * 6 }, validNums + "");
                GUI.contentColor = Color.white;
                GUI.Label(new Rect(rect) { x = rect.x + rect.width - 20 }, "/" + EditorLocalizationTool.ReadyLanguageTypes.Length);
            };
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (m_CloneObj != null)
            {
                DestroyImmediate(m_CloneObj);
            }
            if (m_LastToggleIndex != -1 && !m_InitialHide && m_targetObject != null)
            {
                SceneVisibilityManager.instance.Show(m_targetObject.gameObject, true);
            }
            m_ShowType.valueChanged.RemoveListener(Repaint);
            m_ShowNativeSize.valueChanged.RemoveListener(Repaint);
            m_ShowSlicedOrTiled.valueChanged.RemoveListener(Repaint);
            m_ShowSliced.valueChanged.RemoveListener(Repaint);
            m_ShowTiled.valueChanged.RemoveListener(Repaint);
            m_ShowFilled.valueChanged.RemoveListener(Repaint);
        }

        private void ChangeAvailables()
        {
            m_SpriteList.Clear();
            if (m_Sprite.objectReferenceValue == null || m_IgnoreLocalization.boolValue) return;
            foreach (LocalizationHelper.LanguageType languageType in EditorLocalizationTool.ReadyLanguageTypes)
            {
                string actualName = Application.isPlaying ? m_targetObject.origin_name : m_Sprite.objectReferenceValue.name;
                Sprite spr = AssetDatabase.LoadAssetAtPath<Sprite>($"{UXGUIConfig.LocalizationFolder}/{languageType}/{actualName}.png");
                m_SpriteList.Add(spr ?? m_Need_replace);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            m_IgnoreLocalization.boolValue = !EditorGUILayout.Toggle(EditorLocalization.GetLocalization("UXImage", "Enable localization"), !m_IgnoreLocalization.boolValue);
            if (EditorGUI.EndChangeCheck())
            {
                if (!m_IgnoreLocalization.boolValue)
                {
                    ChangeAvailables();
                }
            }
            if (!m_IgnoreLocalization.boolValue)
            {
                if (!Application.isPlaying && m_Sprite.objectReferenceValue == null && m_Origin_name != null)
                {
                    m_Origin_name = null;
                    m_SpriteList.Clear();
                }
                else if (!Application.isPlaying && m_Sprite.objectReferenceValue != null && m_Sprite.objectReferenceValue.name != m_Origin_name)
                {
                    m_Origin_name = m_Sprite.objectReferenceValue.name;
                    ChangeAvailables();
                }
                if (m_Sprite.objectReferenceValue != null && GUILayout.Button(new GUIContent(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_导入所有语种图片),
                EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_可同时导入多张图片))))
                {
                    string path = Utils.SelectFolder(false);
                    if (path != null && m_Sprite.objectReferenceValue != null)
                    {
                        string[] files = Directory.GetFiles(path, "*.png", SearchOption.AllDirectories);
                        foreach (LocalizationHelper.LanguageType languageType in EditorLocalizationTool.ReadyLanguageTypes)
                        {
                            Directory.CreateDirectory($"{UXGUIConfig.LocalizationFolder}/{languageType}");
                        }
                        foreach (string file in files)
                        {
                            string dirName = Path.GetDirectoryName(file);
                            if(!Enum.TryParse(dirName, out LocalizationHelper.LanguageType languageType))
                                continue;
                            if(!EditorLocalizationTool.ReadyLanguageTypes.Contains(languageType))
                                continue;
                            string fileName = file.Split('\\', '/').Last();
                            string dest = $"{UXGUIConfig.LocalizationFolder}/{languageType}/{fileName}";
                            File.Copy(file, dest, true);
                            AssetDatabase.ImportAsset(dest);
                        }
                        ChangeAvailables();
                    }
                }
                m_CustomList.DoLayoutList();
            }
            serializedObject.ApplyModifiedProperties();
            //base.OnInspectorGUI();
            //CustomEditorGUILayout.PropertyField(sourceImage);
            //CustomEditorGUILayout.PropertyField(color);
            //CustomEditorGUILayout.PropertyField(material);
            //CustomEditorGUILayout.PropertyField(raycastTarget);
            //CustomEditorGUILayout.PropertyField(maskAble);
            serializedObject.Update();

            
#if UNITY_2021_1_OR_NEWER
            m_bIsDriven = (rect.drivenByObject as Slider)?.fillRect == rect;
#endif
            SpriteGUI();
            AppearanceControlsGUI();
            RaycastControlsGUI();
            MaskableControlsGUI();

            ImageTypeGUI();

            SetShowNativeSize(false);
            if (EditorGUILayout.BeginFadeGroup(m_ShowNativeSize.faded))
            {
                EditorGUI.indentLevel++;

                if ((Image.Type)m_Type.enumValueIndex == Image.Type.Simple)
                    EditorGUILayout.PropertyField(m_UseSpriteMesh);

                EditorGUILayout.PropertyField(m_PreserveAspect);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();
            NativeSizeButtonGUI();

            FlipGUI();
            UXImage uxImage = (UXImage)target;
            if((uxImage.rectTransform.rect.width == 0 || uxImage.rectTransform.rect.height == 0) && uxImage.m_FlipMode != UXImage.FlipMode.None)
            {
                EditorGUILayout.HelpBox(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_镜像模式警告), MessageType.Warning);
            }
            
            if (image.material != null && image.material.name.Contains("UXImage"))
            {
                MaterialEffectGUI();
            }

            Rect effectWarpRect = EditorGUILayout.GetControlRect();
            UIEffectWrapDrawer.Draw(effectWarpRect);

            serializedObject.ApplyModifiedProperties();
        }

        protected void SpriteGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_Sprite, m_SpriteContent_self);
            if (EditorGUI.EndChangeCheck())
            {
                var newSprite = m_Sprite.objectReferenceValue as Sprite;
                if (newSprite)
                {
                    UXImage.Type oldType = (UXImage.Type)m_Type.enumValueIndex;
                    if (newSprite.border.SqrMagnitude() > 0)
                    {
                        m_Type.enumValueIndex = (int)UXImage.Type.Sliced;
                    }
                    else if (oldType == UXImage.Type.Sliced)
                    {
                        m_Type.enumValueIndex = (int)UXImage.Type.Simple;
                    }
                }
            }
        }

        protected new void AppearanceControlsGUI()
        {
            EditorGUI.BeginChangeCheck();
            string[] labels = { EditorLocalization.GetLocalization("UXImage", "Solid_Color"), EditorLocalization.GetLocalization("UXImage", "Gradient_Color") };
            var type = (UXImage.ColorType)Utils.EnumPopupLayoutEx(EditorLocalization.GetLocalization("UXImage", "m_ColorType"), typeof(UXImage.ColorType), m_ColorType.intValue, labels);
            // EditorGUILayout.EnumPopup(EditorLocalization.GetLocalization("UXImage", "m_ColorType"), (UXImage.ColorType)m_ColorType.intValue);
            if (EditorGUI.EndChangeCheck())
            {
                m_ColorType.intValue = (int)type;
                if ((int)type == 1 && m_Type.intValue != 0)
                {
                    m_Type.intValue = 0;
                }
                if ((int)type == 1 && m_FlipMode.intValue != 0)
                {
                    m_FlipMode.intValue = 0;
                }
            }
            GUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUIUtility.labelWidth);
            if (GUILayout.Button(EditorLocalization.GetLocalization("UXImage", "UseUIColorConfig"), EditorStyles.miniButton))
            {
                //弹窗
                if (type == UXImage.ColorType.Solid_Color)
                {
                    ColorChooseWindow.OpenWindow();
                    ColorChooseWindow.r_window.target = (UXImage)target;
                }
                if (type == UXImage.ColorType.Gradient_Color)
                {
                    GradientChooseWindow.OpenWindow();
                    GradientChooseWindow.r_window.target = (UXImage)target;
                }
                //m_Color.colorValue = UIColorChooseWindow.r_window.usecolor;
                //m_GradientColor.
            }

            GUILayout.EndHorizontal();
            if (type == UXImage.ColorType.Solid_Color)
            {
                EditorGUILayout.PropertyField(m_Color);
            }
            else if (type == UXImage.ColorType.Gradient_Color)
            {
                EditorGUI.BeginChangeCheck();
                string[] labels2 = { EditorLocalization.GetLocalization("UXImage", "Vertical"), EditorLocalization.GetLocalization("UXImage", "Horizontal") };
                var direction = (UXImage.GradientDirection)Utils.EnumPopupLayoutEx(EditorLocalization.GetLocalization("UXImage", "m_Direction"), typeof(UXImage.GradientDirection), m_Direction.intValue, labels2);
                if (EditorGUI.EndChangeCheck())
                {
                    m_Direction.intValue = (int)direction;
                }
                //EditorGUILayout.ColorField(m_Color);
                EditorGUILayout.PropertyField(m_GradientColor, new GUIContent(EditorLocalization.GetLocalization("UXImage", "Gradient_Color")));
            }

            EditorGUILayout.PropertyField(m_Material);
        }
        protected new void RaycastControlsGUI()
        {
            EditorGUILayout.PropertyField(m_RaycastTarget);
#if UNITY_2021_1_OR_NEWER
            float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            if (m_ShowPadding)
                height += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 4;

            var rect = EditorGUILayout.GetControlRect(true, height);
            if (m_RaycastPadding != null)
            {
                EditorGUI.BeginProperty(rect, m_PaddingContent_self, m_RaycastPadding);
                rect.height = EditorGUIUtility.singleLineHeight;

                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    m_ShowPadding = EditorGUI.Foldout(rect, m_ShowPadding, m_PaddingContent_self, true);
                    if (check.changed)
                    {
                        SceneView.RepaintAll();
                    }
                }

                if (m_ShowPadding)
                {
                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        EditorGUI.indentLevel++;
                        Vector4 newPadding = m_RaycastPadding.vector4Value;

                        rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        newPadding.x = EditorGUI.FloatField(rect, m_LeftContent_self, newPadding.x);

                        rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        newPadding.y = EditorGUI.FloatField(rect, m_BottomContent_self, newPadding.y);

                        rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        newPadding.z = EditorGUI.FloatField(rect, m_RightContent_self, newPadding.z);

                        rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        newPadding.w = EditorGUI.FloatField(rect, m_TopContent_self, newPadding.w);

                        if (check.changed)
                        {
                            m_RaycastPadding.vector4Value = newPadding;
                        }
                        EditorGUI.indentLevel--;
                    }
                }

                EditorGUI.EndProperty();
            }
#endif
        }
        protected void ImageTypeGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_Type, m_SpriteTypeContent);
            if (EditorGUI.EndChangeCheck())
            {
                if (m_Type.intValue != 0 && m_ColorType.intValue == 1)
                {
                    m_ColorType.intValue = 0;
                }
            }

            ++EditorGUI.indentLevel;
            {
                Image.Type typeEnum = (Image.Type)m_Type.enumValueIndex;

                bool showSlicedOrTiled = (!m_Type.hasMultipleDifferentValues && (typeEnum == Image.Type.Sliced || typeEnum == Image.Type.Tiled));
                if (showSlicedOrTiled && targets.Length > 1)
                    showSlicedOrTiled = targets.Select(obj => obj as Image).All(img => img.hasBorder);

                m_ShowSlicedOrTiled.target = showSlicedOrTiled;
                m_ShowSliced.target = (showSlicedOrTiled && !m_Type.hasMultipleDifferentValues && typeEnum == Image.Type.Sliced);
                m_ShowTiled.target = (showSlicedOrTiled && !m_Type.hasMultipleDifferentValues && typeEnum == Image.Type.Tiled);
                m_ShowFilled.target = (!m_Type.hasMultipleDifferentValues && typeEnum == Image.Type.Filled);

                Image image = target as Image;
                if (EditorGUILayout.BeginFadeGroup(m_ShowSlicedOrTiled.faded))
                {
                    if (image.hasBorder)
                        EditorGUILayout.PropertyField(m_FillCenter);
                    EditorGUILayout.PropertyField(m_PixelsPerUnitMultiplier);
                }
                EditorGUILayout.EndFadeGroup();

                if (EditorGUILayout.BeginFadeGroup(m_ShowSliced.faded))
                {
                    if (image.sprite != null && !image.hasBorder)
                        EditorGUILayout.HelpBox("This Image doesn't have a border.", MessageType.Warning);
                }
                EditorGUILayout.EndFadeGroup();

                if (EditorGUILayout.BeginFadeGroup(m_ShowTiled.faded))
                {
                    if (image.sprite != null && !image.hasBorder && (image.sprite.texture.wrapMode != TextureWrapMode.Repeat || image.sprite.packed))
                        EditorGUILayout.HelpBox("It looks like you want to tile a sprite with no border. It would be more efficient to modify the Sprite properties, clear the Packing tag and set the Wrap mode to Repeat.", MessageType.Warning);
                }
                EditorGUILayout.EndFadeGroup();

                if (EditorGUILayout.BeginFadeGroup(m_ShowFilled.faded))
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(m_FillMethod);
                    if (EditorGUI.EndChangeCheck())
                    {
                        m_FillOrigin.intValue = 0;
                    }
                    var shapeRect = EditorGUILayout.GetControlRect(true);
                    switch ((Image.FillMethod)m_FillMethod.enumValueIndex)
                    {
                        case Image.FillMethod.Horizontal:
                            m_FillOrigin.intValue = EditorGUI.Popup(shapeRect, Styles.text, m_FillOrigin.intValue, Styles.OriginHorizontalStyle);
                            break;
                        case Image.FillMethod.Vertical:
                            m_FillOrigin.intValue = EditorGUI.Popup(shapeRect, Styles.text, m_FillOrigin.intValue, Styles.OriginVerticalStyle);
                            break;
                        case Image.FillMethod.Radial90:
                            m_FillOrigin.intValue = EditorGUI.Popup(shapeRect, Styles.text, m_FillOrigin.intValue, Styles.Origin90Style);
                            break;
                        case Image.FillMethod.Radial180:
                            m_FillOrigin.intValue = EditorGUI.Popup(shapeRect, Styles.text, m_FillOrigin.intValue, Styles.Origin180Style);
                            break;
                        case Image.FillMethod.Radial360:
                            m_FillOrigin.intValue = EditorGUI.Popup(shapeRect, Styles.text, m_FillOrigin.intValue, Styles.Origin360Style);
                            break;
                    }
#if UNITY_2021_1_OR_NEWER
                    if (m_bIsDriven)
                        EditorGUILayout.HelpBox("The Fill amount property is driven by Slider.", MessageType.None);
                    using (new EditorGUI.DisabledScope(m_bIsDriven))
                    {
                        EditorGUILayout.PropertyField(m_FillAmount);
                    }
#else
                    EditorGUILayout.PropertyField(m_FillAmount);
#endif

                    if ((Image.FillMethod)m_FillMethod.enumValueIndex > Image.FillMethod.Vertical)
                    {
                        EditorGUILayout.PropertyField(m_FillClockwise, m_ClockwiseContent);
                    }
                }
                EditorGUILayout.EndFadeGroup();
            }
            --EditorGUI.indentLevel;
        }
        private void FlipGUI()
        {
            EditorGUI.BeginChangeCheck();
            //EditorGUILayout.PropertyField(m_gradientMode, m_SpriteGradientContent);

            UXImage.FlipMode flipmodeEnumOld = (UXImage.FlipMode)m_FlipMode.enumValueIndex;
            bool flipWithCopyOld = m_FlipWithCopy.boolValue;
#if UNITY_2021_2_OR_NEWER    
            UXImage.FlipEdgeVertical edgeVerticalOld = (UXImage.FlipEdgeVertical)(m_FlipEdgeVertical.enumValueFlag);
#else
            UXImage.FlipEdgeVertical edgeVerticalOld = (UXImage.FlipEdgeVertical)(m_FlipEdgeVertical.enumValueIndex + 3);
#endif
            UXImage.FlipEdgeHorizontal edgeHorizontalOld = (UXImage.FlipEdgeHorizontal)m_FlipEdgeHorizontal.enumValueIndex;

            string[] labels = {EditorLocalization.GetLocalization("UXImage", "None"), EditorLocalization.GetLocalization("UXImage", "Horizontal"),
            EditorLocalization.GetLocalization("UXImage", "Vertical"), EditorLocalization.GetLocalization("UXImage", "FourCorner")};
            EditorGUI.BeginChangeCheck();
            m_FlipMode.intValue = Utils.EnumPopupLayoutEx(m_FlipModeContent.text, typeof(UXImage.FlipMode), m_FlipMode.intValue, labels);
            if (EditorGUI.EndChangeCheck())
            {
                if (m_FlipMode.intValue != 0 && m_ColorType.intValue == 1)
                {
                    m_ColorType.intValue = 0;
                }
            }
            //EditorGUILayout.PropertyField(m_FlipMode, m_FlipModeContent);
            UXImage.FlipMode flipmodeEnum = (UXImage.FlipMode)m_FlipMode.enumValueIndex;
            switch (flipmodeEnum)
            {
                case UXImage.FlipMode.Horziontal:
                    ++EditorGUI.indentLevel;
                    bool iscopy1 = m_FlipWithCopy.boolValue;
                    m_FlipWithCopy.boolValue = EditorGUILayout.Toggle(EditorLocalization.GetLocalization("UXImage", "Copy"), m_FlipWithCopy.boolValue);
                    if (iscopy1 == true && m_FlipWithCopy.boolValue == false)
                    {
                        if (m_FlipEdgeHorizontal.intValue == 0)
                        {
                            go.transform.Translate(Vector3.right * go.GetComponent<RectTransform>().rect.width / 4);
                        }
                        else if (m_FlipEdgeHorizontal.intValue == 2)
                        {
                            go.transform.Translate(Vector3.left * go.GetComponent<RectTransform>().rect.width / 4);
                        }
                    }
                    if (m_FlipWithCopy.boolValue)
                    {
                        int last = m_FlipEdgeHorizontal.intValue;
                        if (iscopy1 == false)
                        {
                            if (last == 0)
                            {
                                go.transform.Translate(Vector3.left * go.GetComponent<RectTransform>().rect.width / 2);
                            }
                            else if (last == 2)
                            {
                                go.transform.Translate(Vector3.right * go.GetComponent<RectTransform>().rect.width / 2);
                            }
                        }
                        string[] labels2 = {EditorLocalization.GetLocalization("UXImage", "Left"), EditorLocalization.GetLocalization("UXImage", "Middle"),
                        EditorLocalization.GetLocalization("UXImage", "Right")};
                        m_FlipEdgeHorizontal.intValue = Utils.EnumPopupLayoutEx(m_FlipEdgeContent.text, typeof(UXImage.FlipEdgeHorizontal), m_FlipEdgeHorizontal.intValue, labels2);
                        //EditorGUILayout.PropertyField(m_FlipEdgeHorizontal, m_FlipEdgeContent);
                        int now = m_FlipEdgeHorizontal.intValue;

                        if (last != now)
                        {
                            if ((last == 0 && now == 1))
                            {
                                go.transform.Translate(Vector3.right * go.GetComponent<RectTransform>().rect.width / 4);
                            }
                            else if ((last == 2 && now == 1))
                            {
                                go.transform.Translate(Vector3.left * go.GetComponent<RectTransform>().rect.width / 4);
                            }
                            else if (last == 0 && now == 2 || (last == 1 && now == 2))
                            {
                                go.transform.Translate(Vector3.right * go.GetComponent<RectTransform>().rect.width / 2);
                            }
                            else if ((last == 1 && now == 0) || (last == 2 && now == 0))
                            {
                                go.transform.Translate(Vector3.left * go.GetComponent<RectTransform>().rect.width / 2);
                            }
                        }
                    }
                    --EditorGUI.indentLevel;
                    break;
                case UXImage.FlipMode.Vertical:
                    ++EditorGUI.indentLevel;
                    bool iscopy = m_FlipWithCopy.boolValue;
                    m_FlipWithCopy.boolValue = EditorGUILayout.Toggle(EditorLocalization.GetLocalization("UXImage", "Copy"), m_FlipWithCopy.boolValue);
                    if (iscopy == true && m_FlipWithCopy.boolValue == false)
                    {
                        if (m_FlipEdgeVertical.intValue == 3)
                        {
                            go.transform.Translate(Vector3.down * go.GetComponent<RectTransform>().rect.height / 4);
                        }
                        else if (m_FlipEdgeVertical.intValue == 5)
                        {
                            go.transform.Translate(Vector3.up * go.GetComponent<RectTransform>().rect.height / 4);
                        }
                    }
                    if (m_FlipWithCopy.boolValue)
                    {
                        int last = m_FlipEdgeVertical.intValue;
                        if (iscopy == false)
                        {
                            if (last == 3)
                            {
                                go.transform.Translate(Vector3.up * go.GetComponent<RectTransform>().rect.height / 2);
                            }
                            else if (last == 5)
                            {
                                go.transform.Translate(Vector3.down * go.GetComponent<RectTransform>().rect.height / 2);
                            }
                        }
                        string[] labels2 = {EditorLocalization.GetLocalization("UXImage", "Up"), EditorLocalization.GetLocalization("UXImage", "Middle"),
                        EditorLocalization.GetLocalization("UXImage", "Down")};
                        m_FlipEdgeVertical.intValue = Utils.EnumPopupLayoutEx(m_FlipEdgeContent.text, typeof(UXImage.FlipEdgeVertical), m_FlipEdgeVertical.intValue, labels2);
                        //EditorGUILayout.PropertyField(m_FlipEdgeVertical, m_FlipEdgeContent);
                        int now = m_FlipEdgeVertical.intValue;
                        if (last != now)
                        {
                            if ((last == 3 && now == 4))
                            {
                                go.transform.Translate(Vector3.down * go.GetComponent<RectTransform>().rect.height / 4);
                            }
                            else if ((last == 5 && now == 4))
                            {
                                go.transform.Translate(Vector3.up * go.GetComponent<RectTransform>().rect.height / 4);
                            }
                            else if (last == 3 && now == 5 || (last == 4 && now == 5))
                            {
                                go.transform.Translate(Vector3.down * go.GetComponent<RectTransform>().rect.height / 2);
                            }
                            else if ((last == 4 && now == 3) || last == 5 && now == 3)
                            {
                                go.transform.Translate(Vector3.up * go.GetComponent<RectTransform>().rect.height / 2);
                            }
                        }
                    }
                    --EditorGUI.indentLevel;
                    break;
                case UXImage.FlipMode.FourCorner:
                    ++EditorGUI.indentLevel;
                    int lastC = m_FlipFillCenter.intValue;
                    string[] labels3 = {EditorLocalization.GetLocalization("UXImage", "LeftTop"), EditorLocalization.GetLocalization("UXImage", "RightTop"),
                        EditorLocalization.GetLocalization("UXImage", "RightBottom"), EditorLocalization.GetLocalization("UXImage", "LeftBottom")};
                    m_FlipFillCenter.intValue = Utils.EnumPopupLayoutEx(m_FlipFillContent.text, typeof(UXImage.FlipFillCenter), m_FlipFillCenter.intValue, labels3);
                    //EditorGUILayout.PropertyField(m_FlipFillCenter, m_FlipFillContent);
                    int nowC = m_FlipFillCenter.intValue;
                    // if (lastC != nowC)
                    // {
                    //     if(lastC==0&&nowC==1||(lastC==3&&nowC==2)){
                    //         go.transform.Translate(Vector3.right * go.GetComponent<RectTransform>().rect.width / 2);
                    //     }
                    //     else if(lastC==0&& nowC==2){
                    //         go.transform.Translate(Vector3.right * go.GetComponent<RectTransform>().rect.width / 2);
                    //         go.transform.Translate(Vector3.down * go.GetComponent<RectTransform>().rect.height / 2);
                    //     }
                    //     else if(lastC==0&& nowC==3 || (lastC==1&&nowC==2)){
                    //         go.transform.Translate(Vector3.down * go.GetComponent<RectTransform>().rect.height / 2);
                    //     }
                    //     else if(lastC==1&&nowC==0||(lastC==2&&nowC==3)){
                    //         go.transform.Translate(Vector3.left * go.GetComponent<RectTransform>().rect.width / 2);
                    //     }
                    //     else if(lastC==1&&nowC==3){
                    //         go.transform.Translate(Vector3.left * go.GetComponent<RectTransform>().rect.width / 2);
                    //         go.transform.Translate(Vector3.down * go.GetComponent<RectTransform>().rect.height / 2);
                    //     }
                    //     else if(lastC==2&&nowC==1||(lastC==3&&nowC==0)){
                    //         go.transform.Translate(Vector3.up * go.GetComponent<RectTransform>().rect.height / 2);
                    //     }
                    //     else if(lastC==2&&nowC==0){
                    //         go.transform.Translate(Vector3.left * go.GetComponent<RectTransform>().rect.width / 2);
                    //         go.transform.Translate(Vector3.up * go.GetComponent<RectTransform>().rect.height / 2);
                    //     }
                    //     else if(lastC==3&&nowC==1){
                    //         go.transform.Translate(Vector3.right * go.GetComponent<RectTransform>().rect.width / 2);
                    //         go.transform.Translate(Vector3.up * go.GetComponent<RectTransform>().rect.height / 2);
                    //     }
                    // }
                    --EditorGUI.indentLevel;
                    break;
                default:
                    break;
            }

            if (EditorGUI.EndChangeCheck())
            {
                int OldWidthScaler = 1;
                int OldHeightScaler = 1;
                int NewWidthScaler = 1;
                int NewHeightScaler = 1;


                if (flipmodeEnumOld != flipmodeEnum)
                {
                    m_FlipWithCopy.boolValue = false;
                }

                if (flipmodeEnumOld == UXImage.FlipMode.Horziontal)
                {
                    GetSizeScaler(flipmodeEnumOld, flipWithCopyOld, (UXImage.FlipEdge)(int)edgeHorizontalOld, ref OldWidthScaler, ref OldHeightScaler);
                }
                if (flipmodeEnumOld == UXImage.FlipMode.Vertical)
                {
                    GetSizeScaler(flipmodeEnumOld, flipWithCopyOld, (UXImage.FlipEdge)(int)edgeVerticalOld, ref OldWidthScaler, ref OldHeightScaler);
                }
                if (flipmodeEnumOld == UXImage.FlipMode.FourCorner)
                {
                    GetSizeScaler(flipmodeEnumOld, flipWithCopyOld, UXImage.FlipEdge.None, ref OldWidthScaler, ref OldHeightScaler);
                }

                if (flipmodeEnum == UXImage.FlipMode.Horziontal)
                {
#if UNITY_2021_2_OR_NEWER
                    GetSizeScaler(flipmodeEnum, m_FlipWithCopy.boolValue, (UXImage.FlipEdge)(int)m_FlipEdgeHorizontal.enumValueFlag, ref NewWidthScaler, ref NewHeightScaler);
#else
                    GetSizeScaler(flipmodeEnum, m_FlipWithCopy.boolValue, (UXImage.FlipEdge)(int)m_FlipEdgeHorizontal.enumValueIndex, ref NewWidthScaler, ref NewHeightScaler);
#endif
                }
                if (flipmodeEnum == UXImage.FlipMode.Vertical)
                {
#if UNITY_2021_2_OR_NEWER
                    GetSizeScaler(flipmodeEnum, m_FlipWithCopy.boolValue, (UXImage.FlipEdge)(int)m_FlipEdgeVertical.enumValueFlag, ref NewWidthScaler, ref NewHeightScaler);
#else
                    GetSizeScaler(flipmodeEnum, m_FlipWithCopy.boolValue, (UXImage.FlipEdge)(int)(m_FlipEdgeVertical.enumValueIndex + 3), ref NewWidthScaler, ref NewHeightScaler);
#endif
                }
                if (flipmodeEnum == UXImage.FlipMode.FourCorner)
                {
                    GetSizeScaler(flipmodeEnum, m_FlipWithCopy.boolValue, UXImage.FlipEdge.None, ref NewWidthScaler, ref NewHeightScaler);
                }

                UXImage image = target as UXImage;
                float width = image.rectTransform.rect.width * ((float)NewWidthScaler / OldWidthScaler);
                float height = image.rectTransform.rect.height * ((float)NewHeightScaler / OldHeightScaler);
                image.rectTransform.sizeDelta = new Vector2(width, height);
                serializedObject.ApplyModifiedProperties();
            }
        }
        private void MaterialEffectGUI()
        {
            //EditorGUILayout.LabelField("颜色空间");
            EditorGUILayout.LabelField(EditorLocalization.GetLocalization("UXImage", "ColorSpace"));
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();
            m_GammaToLinear.boolValue = EditorGUILayout.Toggle(EditorLocalization.GetLocalization("UXImage", "GammaToLinear"), m_GammaToLinear.boolValue);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                if (m_GammaToLinear.boolValue)
                {
                    m_LinearToGamma.boolValue = false;
                    image.material.SetFloat(GammaToLinear, 1);
                    image.material.SetFloat(LinearToGamma, 0);

                    image.material.EnableKeyword("GAMMA_TO_LINEAR");
                    image.material.DisableKeyword("LINEAR_TO_GAMMA");

                }
                else
                {
                    image.material.SetFloat(GammaToLinear, 0);
                    image.material.DisableKeyword("GAMMA_TO_LINEAR");
                }
            }

            EditorGUI.BeginChangeCheck();
            m_LinearToGamma.boolValue = EditorGUILayout.Toggle(EditorLocalization.GetLocalization("UXImage", "LinearToGamma"), m_LinearToGamma.boolValue);
            //EditorGUILayout.PropertyField(m_LinearToGamma);
            if (EditorGUI.EndChangeCheck())
            {
                if (m_LinearToGamma.boolValue)
                {
                    m_GammaToLinear.boolValue = false;

                    image.material.SetFloat(LinearToGamma, 1);
                    image.material.SetFloat(GammaToLinear, 0);

                    image.material.EnableKeyword("LINEAR_TO_GAMMA");
                    image.material.DisableKeyword("GAMMA_TO_LINEAR");
                }
                else
                {
                    image.material.SetFloat(LinearToGamma, 0);
                    image.material.DisableKeyword("LINEAR_TO_GAMMA");
                }
            }
            EditorGUI.indentLevel--;
            serializedObject.ApplyModifiedProperties();

            EditorGUI.BeginChangeCheck();
            //m_EnableGrey.boolValue = EditorGUILayout.Toggle("置灰", m_EnableGrey.boolValue);
            m_EnableGrey.boolValue = EditorGUILayout.Toggle(EditorLocalization.GetLocalization("UXImage", "GreyEffect"), m_EnableGrey.boolValue);
            if (m_EnableGrey.boolValue)
            {
                EditorGUI.indentLevel++;
                m_Contrast.floatValue = EditorGUILayout.Slider(EditorLocalization.GetLocalization("UXImage", "Contrast"), m_Contrast.floatValue, 0f, 2.0f);
                m_Saturation.floatValue = EditorGUILayout.Slider(EditorLocalization.GetLocalization("UXImage", "Saturation"), m_Saturation.floatValue, 0f, 2.0f);
                EditorGUI.indentLevel--;
            }
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                if (m_EnableGrey.boolValue)
                {
                    image.material.SetFloat(EnableGrey, 1);

                    image.material.EnableKeyword("ENABLE_GREY");
                    image.material.SetFloat(Contrast, m_Contrast.floatValue);
                    image.material.SetFloat(Saturation, m_Saturation.floatValue);
                }
                else
                {
                    image.material.SetFloat(EnableGrey, 0);
                    image.material.DisableKeyword("ENABLE_GREY");
                }
            }

            EditorGUI.BeginChangeCheck();
            //m_EnableRadiusCorner.boolValue = EditorGUILayout.Toggle("圆角", m_EnableRadiusCorner.boolValue);
            m_EnableRadiusCorner.boolValue = EditorGUILayout.Toggle(EditorLocalization.GetLocalization("UXImage", "Radius"), m_EnableRadiusCorner.boolValue);
            if (m_EnableRadiusCorner.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_CornerRadius);
                EditorGUI.indentLevel--;
            }
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                if (m_EnableRadiusCorner.boolValue)
                {
                    image.material.SetFloat(RadiusCorner, 1);

                    image.material.EnableKeyword("RADIUS_CORNER");
                    image.RefreshRadius();
                }
                else
                {
                    image.material.SetFloat(RadiusCorner, 0);
                    image.material.DisableKeyword("RADIUS_CORNER");
                }
            }

            m_EnableDistortion.boolValue = EditorGUILayout.Toggle("扭曲", m_EnableDistortion.boolValue);
            if (m_EnableDistortion.boolValue)
            {
                EditorGUI.indentLevel++;
                m_BottomLeftDistortion.vector2Value = EditorGUILayout.Vector2Field("左下角", m_BottomLeftDistortion.vector2Value);
                m_BottomRightDistortion.vector2Value = EditorGUILayout.Vector2Field("右下角", m_BottomRightDistortion.vector2Value);
                m_TopLeftDistortion.vector2Value = EditorGUILayout.Vector2Field("左上角", m_TopLeftDistortion.vector2Value);
                m_TopRightDistortion.vector2Value = EditorGUILayout.Vector2Field("右上角", m_TopRightDistortion.vector2Value);
                EditorGUI.indentLevel--;
            }
        }

        void GetSizeScaler(UXImage.FlipMode flipMode, bool flipWithCopy, UXImage.FlipEdge flipEdge, ref int widthScaler, ref int heightScaler)
        {
            if (flipMode == UXImage.FlipMode.FourCorner)
            {
                widthScaler = 2;
                heightScaler = 2;
            }
            if (flipMode == UXImage.FlipMode.Vertical)
            {
                if (flipWithCopy && flipEdge != UXImage.FlipEdge.VertMiddle)
                {
                    widthScaler = 1;
                    heightScaler = 2;
                }
            }
            if (flipMode == UXImage.FlipMode.Horziontal)
            {
                if (flipWithCopy && flipEdge != UXImage.FlipEdge.HorzMiddle)
                {
                    widthScaler = 2;
                    heightScaler = 1;
                }
            }
        }

        void SetShowNativeSize(bool instant)
        {
            Image.Type type = (Image.Type)m_Type.enumValueIndex;
            bool showNativeSize = (type == Image.Type.Simple || type == Image.Type.Filled) && m_Sprite.objectReferenceValue != null;
            if (instant)
                m_ShowNativeSize.value = showNativeSize;
            else
                m_ShowNativeSize.target = showNativeSize;
        }
    }
}
