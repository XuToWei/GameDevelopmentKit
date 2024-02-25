using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class UXBuilderPathUploadStruct
    {
        public string className = "UXPathUpload";
        public UXStyle style = new UXStyle();
        public UXStyle buttonStyle = new UXStyle() { width = 30 };
        public UXStyle inputStyle = new UXStyle() { width = Length.Percent(60), marginRight = 8 };

        public bool disabled = false;
        public string openPath = PlayerPrefs.GetString("LastParticleCheckPath");

        public Action<string> onChange = s => { };
        public UXUploadType type = UXUploadType.Path;
    }

    public class UXBuilderPathUpload : VisualElement
    {
        private static UXBuilderPathUpload _mUXPathUpload;
        private static UXStyle _mStyle = new UXStyle();

        public static UXBuilderButton _mUXButton;
        public static UXBuilderInput _mUXInput;

        public static string pathName = "";

        private static UXBuilderPathUploadStruct _mComponent = new UXBuilderPathUploadStruct();

        public UXBuilderPathUploadStruct GetComponents()
        {
            return _mComponent;
        }

        public UXBuilderPathUpload SetComponents(UXBuilderPathUploadStruct component)
        {
            VisualElement parent = this.hierarchy.parent;
            if (parent == null) return this;
            _mComponent = component;
            var a = Create(parent, _mComponent);
            a.PlaceBehind(this);
            parent.Remove(this);
            return a;
        }

        public static UXBuilderPathUpload Create(VisualElement root, UXBuilderPathUploadStruct component)
        {
            _mComponent = component;

            _mUXPathUpload = new UXBuilderPathUpload();

            _mUXInput = UXBuilder.Input(_mUXPathUpload, new UXBuilderInputStruct()
            {
                disabled = component.disabled,
                readOnly = true,
                style = component.inputStyle,
                onChange = component.onChange,
            });
            _mUXInput.value = component.openPath;

            _mUXButton = UXBuilder.Button(_mUXPathUpload, new UXBuilderButtonStruct()
            {
                disabled = component.disabled,
                text = "...",
                style = component.buttonStyle,
                OnClick = () =>
                {
                    pathName = _mUXInput.value;
                    if (component.type == UXUploadType.File)
                    {
                        var file = SelectFile(component.openPath);
                        pathName = !string.IsNullOrEmpty(file)
                            ? file
                            : pathName;
                    }
                    else if (component.type == UXUploadType.Path)
                    {
                        var path = SelectPath(component.openPath);
                        pathName = !string.IsNullOrEmpty(path)
                            ? path
                            : pathName;
                    }
                    _mUXInput.value = pathName;
                }
            });

            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(ThunderFireUIToolConfig.UIBuilderPath +
                                                          "USS/UXElements/UXPathUpload.uss");
            _mUXPathUpload.styleSheets.Add(styleSheet);


            _mUXPathUpload.AddToClassList("ux-path-upload");

            _mUXPathUpload.name = component.className;
            StyleCopy.IStyleToUXStyle(_mStyle, _mUXPathUpload.style);
            InitComponent(component);
            root.Add(_mUXPathUpload);

            return _mUXPathUpload;
        }

        private static void InitComponent(UXBuilderPathUploadStruct component)
        {
            InitStyle(component.style);
        }

        private static void InitStyle(UXStyle style)
        {
            if (style == _mStyle) return;
            UXStyle compStyle = new UXStyle();
            Type type = typeof(UXStyle);
            PropertyInfo[] properties = type.GetProperties();
            foreach (var property in properties)
            {
                var val = property.GetValue(style);
                if (!val.Equals(property.GetValue(compStyle)))
                {
                    property.SetValue(_mStyle, property.GetValue(style));
                }
            }

            StyleCopy.UXStyleToIStyle(_mUXPathUpload.style, _mStyle);
        }

        private static string SelectPath(string openPath)
        {
            string folderPath = openPath;
            string path = EditorUtility.OpenFolderPanel(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_选择路径), folderPath, "");
            if (path != "")
            {
                int index = path.IndexOf("Assets");
                if (index != -1)
                {
                    PlayerPrefs.SetString("LastParticleCheckPath", path);
                    path = path.Substring(index);
                    return path + "/";
                }
                else
                {
                    EditorUtility.DisplayDialog("messageBox",
                        EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_目录不在Assets下Tip),
                        EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                        EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));
                }
            }
            return null;
        }

        private static string SelectFile(string openPath)
        {
            string filePath = openPath;
            string path =
                EditorUtility.OpenFilePanel(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_选择文件),
                    filePath, "");
            if (path != "")
            {
                int index = path.IndexOf("Assets");
                if (index != -1)
                {
                    PlayerPrefs.SetString("LastParticleCheckPath", path);
                    path = path.Substring(index);
                    return path;
                }
                else
                {
                    EditorUtility.DisplayDialog("messageBox",
                        EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_目录不在Assets下Tip),
                        EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                        EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));
                }
            }
            return null;
        }
    }
}