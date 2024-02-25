using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class UXBuilder
    {
        public static UXBuilderButton Button(VisualElement v, UXBuilderButtonStruct s)
        {
            return UXBuilderButton.Create(v, s);
        }

        public static UXBuilderText Text(VisualElement v, UXBuilderTextStruct s)
        {
            return UXBuilderText.Create(v, s);
        }

        public static UXBuilderInput Input(VisualElement v, UXBuilderInputStruct s)
        {
            return UXBuilderInput.Create(v, s);
        }

        public static UXBuilderCheckBox CheckBox(VisualElement v, UXBuilderCheckBoxStruct s)
        {
            return UXBuilderCheckBox.Create(v, s);
        }
        
        public static UXBuilderSelect Select(VisualElement v, UXBuilderSelectStruct s)
        {
            return UXBuilderSelect.Create(v, s);
        }
#if UNITY_2021_1_OR_NEWER
        public static UXBuilderRadio Radio(VisualElement v, UXBuilderRadioStruct s)
        {
            return UXBuilderRadio.Create(v, s);
        }

        public static UXBuilderRadioGroup RadioGroup(VisualElement v, UXBuilderRadioGroupStruct s)
        {
            return UXBuilderRadioGroup.Create(v, s);
        }
#endif
        public static UXBuilderSlider Slider(VisualElement v, UXBuilderSliderStruct s)
        {
            return UXBuilderSlider.Create(v, s);
        }

        public static UXBuilderPathUpload Upload(VisualElement v, UXBuilderPathUploadStruct s)
        {
            return UXBuilderPathUpload.Create(v, s);
        }

        public static UXBuilderRow Row(VisualElement v, UXBuilderRowStruct s)
        {
            return UXBuilderRow.Create(v, s);
        }

        public static UXBuilderCol Col(VisualElement v, UXBuilderColStruct s)
        {
            return UXBuilderCol.Create(v, s);
        }

        public static UXBuilderDiv Div(VisualElement v, UXBuilderDivStruct s)
        {
            return UXBuilderDiv.Create(v, s);
        }

        public static UXBuilderScrollView ScrollView(VisualElement v, UXBuilderScrollViewStruct s)
        {
            return UXBuilderScrollView.Create(v, s);
        }

        public static UXBuilderTabs Tabs(VisualElement v, UXBuilderTabsStruct s)
        {
            return UXBuilderTabs.Create(v, s);
        }
        
        public static UXBuilderDivider Divider(VisualElement v, UXBuilderDividerStruct s)
        {
            return UXBuilderDivider.Create(v, s);
        }

    }
}