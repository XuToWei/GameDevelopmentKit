using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class UXBuilderDisplayWindow : EditorWindow
    {
        private static UXBuilderDisplayWindow _mWindow;
        
        // [MenuItem("ThunderFireUXTool/UXBuilder展示(UXBuilder Display)", false, 56)]
        public static void OpenWindow()
        {
            _mWindow = GetWindow<UXBuilderDisplayWindow>();
            int width = 800;
            int height = 600;
            _mWindow = GetWindow<UXBuilderDisplayWindow>();
            _mWindow.minSize = new Vector2(width, height);
            _mWindow.titleContent.text = "UXBuilder Display";
            _mWindow.position = new Rect((Screen.currentResolution.width - width) / 2,
                (Screen.currentResolution.height - height) / 2, width, height);
        }
        
        private void OnEnable()
        {
            VisualElement root = rootVisualElement;
            UXBuilder.Divider(root, new UXBuilderDividerStruct());
            UXBuilder.Divider(root, new UXBuilderDividerStruct()
            {
                type = UXDividerType.Vertical,
            });
        }
    }
}