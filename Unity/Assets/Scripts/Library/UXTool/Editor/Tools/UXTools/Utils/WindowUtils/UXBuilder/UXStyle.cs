using UnityEngine;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class UXStyle : IStyle
    {
        public StyleLength width { get; set; }
        public StyleLength height { get; set; }
        public StyleLength maxWidth { get; set; }
        public StyleLength maxHeight { get; set; }
        public StyleLength minWidth { get; set; }
        public StyleLength minHeight { get; set; }
        public StyleLength flexBasis { get; set; }
        public StyleFloat flexGrow { get; set; }
        public StyleFloat flexShrink { get; set; }
        public StyleEnum<FlexDirection> flexDirection { get; set; }
        public StyleEnum<Wrap> flexWrap { get; set; }
        public StyleEnum<Overflow> overflow { get; set; }
        public StyleEnum<OverflowClipBox> unityOverflowClipBox { get; set; }
        public StyleLength left { get; set; }
        public StyleLength top { get; set; }
        public StyleLength right { get; set; }
        public StyleLength bottom { get; set; }
        public StyleLength marginLeft { get; set; }
        public StyleLength marginTop { get; set; }
        public StyleLength marginRight { get; set; }
        public StyleLength marginBottom { get; set; }
        public StyleLength paddingLeft { get; set; }
        public StyleLength paddingTop { get; set; }
        public StyleLength paddingRight { get; set; }
        public StyleLength paddingBottom { get; set; }
        public StyleEnum<Position> position { get; set; }
        public StyleEnum<Align> alignSelf { get; set; }
        public StyleEnum<TextAnchor> unityTextAlign { get; set; }
        public StyleEnum<FontStyle> unityFontStyleAndWeight { get; set; }
        public StyleFont unityFont { get; set; }
        public StyleLength fontSize { get; set; }
        public StyleEnum<WhiteSpace> whiteSpace { get; set; }
        public StyleColor color { get; set; }
        public StyleColor backgroundColor { get; set; }
        public StyleColor borderColor { get; set; }
        public StyleBackground backgroundImage { get; set; }
        public StyleEnum<ScaleMode> unityBackgroundScaleMode { get; set; }
        public StyleColor unityBackgroundImageTintColor { get; set; }
        public StyleEnum<Align> alignItems { get; set; }
        public StyleEnum<Align> alignContent { get; set; }
        public StyleEnum<Justify> justifyContent { get; set; }
        public StyleColor borderLeftColor { get; set; }
        public StyleColor borderTopColor { get; set; }
        public StyleColor borderRightColor { get; set; }
        public StyleColor borderBottomColor { get; set; }
        public StyleFloat borderLeftWidth { get; set; }
        public StyleFloat borderTopWidth { get; set; }
        public StyleFloat borderRightWidth { get; set; }
        public StyleFloat borderBottomWidth { get; set; }
        public StyleLength borderTopLeftRadius { get; set; }
        public StyleLength borderTopRightRadius { get; set; }
        public StyleLength borderBottomRightRadius { get; set; }
        public StyleLength borderBottomLeftRadius { get; set; }
        public StyleInt unitySliceLeft { get; set; }
        public StyleInt unitySliceTop { get; set; }
        public StyleInt unitySliceRight { get; set; }
        public StyleInt unitySliceBottom { get; set; }
        public StyleFloat opacity { get; set; }
        public StyleEnum<Visibility> visibility { get; set; }
        public StyleCursor cursor { get; set; }
        public StyleEnum<DisplayStyle> display { get; set; }
        public StyleLength wordSpacing { get; set; }
        public StyleLength letterSpacing { get; set; }
        public StyleLength unityParagraphSpacing { get; set; }
        public StyleColor unityTextOutlineColor { get; set; }
        public StyleFloat unityTextOutlineWidth { get; set; }
#if UNITY_2020_3_OR_NEWER
        public StyleEnum<TextOverflow> textOverflow { get; set; }
        public StyleEnum<TextOverflowPosition> unityTextOverflowPosition { get; set; }
#endif
#if UNITY_2021_1_OR_NEWER
        public StyleRotate rotate { get; set; }
        public StyleScale scale { get; set; }
        public StyleTextShadow textShadow { get; set; }
        public StyleTransformOrigin transformOrigin { get; set; }
        public StyleList<TimeValue> transitionDelay { get; set; }
        public StyleList<TimeValue> transitionDuration { get; set; }
        public StyleList<StylePropertyName> transitionProperty { get; set; }
        public StyleList<EasingFunction> transitionTimingFunction { get; set; }
        public StyleTranslate translate { get; set; }
        public StyleFontDefinition unityFontDefinition { get; set; }
#endif
#if UNITY_2022_2_OR_NEWER
        public StyleBackgroundRepeat backgroundRepeat { get; set; }
        public StyleBackgroundPosition backgroundPositionY { get; set; }
        public StyleBackgroundPosition backgroundPositionX { get; set; }
        public StyleBackgroundSize backgroundSize { get; set; }
        public StyleFloat unitySliceScale { get; set; }
#endif
    }
}