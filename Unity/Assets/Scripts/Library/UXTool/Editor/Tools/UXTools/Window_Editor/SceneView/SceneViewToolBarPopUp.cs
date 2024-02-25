#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System.Collections.Generic;
using System;

namespace ThunderFireUITool
{
    public class ToolbarPopupOption
    {
        public string text;
        public string iconPath;
        public Action action;
    }

    public class ToolbarPopup
    {
        private VisualElement root;

        public ToolbarPopup()
        {
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView == null) return;

            VisualTreeAsset toolbarPopUpTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ThunderFireUIToolConfig.UIBuilderPath + "toolbar_secondPopup.uxml");
            root = toolbarPopUpTreeAsset.CloneTree().Children().First();
            sceneView.rootVisualElement.Add(root);

            var modelWindowMask = root.Q<VisualElement>("modelWindowMask");

            modelWindowMask.RegisterCallback((MouseDownEvent e) =>
            {
                sceneView.rootVisualElement.Remove(root);
            });
        }

        public void Init(Vector2 position, List<ToolbarPopupOption> options, float width = 140)
        {
            VisualTreeAsset optionTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ThunderFireUIToolConfig.UIBuilderPath + "toolbar_secondOption.uxml");
            root.style.height = options.Count * 40 + 20;
            root.style.width = width;

            for (int i = 0; i < options.Count; i++)
            {
                ToolbarPopupOption option = options[i];

                var optionRoot = optionTreeAsset.CloneTree().Children().First();
                root.Add(optionRoot);
                optionRoot.Q<VisualElement>("icon").style.backgroundImage = (StyleBackground)ToolUtils.GetIcon(option.iconPath);
                optionRoot.Q<Label>("label").text = option.text;
                optionRoot.RegisterCallback((MouseDownEvent e) =>
                {
                    option.action?.Invoke();

                    SceneView sceneView = SceneView.lastActiveSceneView;
                    if (sceneView == null) return;
                    sceneView.rootVisualElement.Remove(root);
                });
                RegisterOptionHoverState(optionRoot);

                optionRoot.style.top = 5 + i * 40;
            }

            root.style.position = Position.Absolute;
            root.style.left = position.x - root.style.width.value.value;
            root.style.top = position.y - root.style.height.value.value;
        }

        private void RegisterOptionHoverState(VisualElement optionRoot)
        {
            optionRoot.RegisterCallback<MouseEnterEvent>((MouseEnterEvent e) => { SetOptionHoverState(optionRoot, true); });
            optionRoot.RegisterCallback<MouseLeaveEvent>((MouseLeaveEvent e) => { SetOptionHoverState(optionRoot, false); });
        }

        private void SetOptionHoverState(VisualElement optionRoot, bool isHover)
        {
            if (isHover)
            {
                optionRoot.style.backgroundColor = ThunderFireUIToolConfig.hoverColor;
            }
            else
            {
                optionRoot.style.backgroundColor = ThunderFireUIToolConfig.normalColor;
            }
        }
    }
}
#endif