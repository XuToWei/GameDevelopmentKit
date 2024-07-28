#if UNITY_EDITOR
#if UNITY_2021_2_OR_NEWER
using UnityEditor.DeviceSimulation;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System.Collections.Generic;
using System.Threading.Tasks;

public class UIDeviceSimulatorChangeController : DeviceSimulatorPlugin
{
    public override string title => "UIDeviceSimulatorChangeController";
    private static Button m_ResetCountButton = new Button(() => {
           UpdateDevice();
    });
    public static List<UIAdapter> UIAdapters = new List<UIAdapter>();
    public static List<IgnoreUIAdapter> IgnoreUIAdapters = new List<IgnoreUIAdapter>();


    public override void OnCreate(){
        m_ResetCountButton.text = "Refresh SafeArea";
        m_ResetCountButton.name = "refresh_safeArea";
        m_ResetCountButton.style.left = 0;
        m_ResetCountButton.style.fontSize = 12;
        m_ResetCountButton.style.position = Position.Relative;
        m_ResetCountButton.style.marginTop = 0;
        m_ResetCountButton.style.marginRight = 0;
        m_ResetCountButton.style.marginLeft = 0;
        m_ResetCountButton.style.marginBottom = 0;
        m_ResetCountButton.style.paddingLeft = 4;
        m_ResetCountButton.style.paddingRight = 4;
        // UIAdapters = Resources.FindObjectsOfTypeAll(typeof(UIAdapter)).ToList();
        // IgnoreUIAdapters = Resources.FindObjectsOfTypeAll(typeof(IgnoreUIAdapter)).ToList();
        UpdateDevice();
        InitializeButton();
    }

    public override void OnDestroy(){
        UpdateDevice();
    }

    public static void UpdateDevice(){
        foreach (UIAdapter uiAdapter in UIAdapters)
        {
            uiAdapter.Refresh();
        }
        foreach (IgnoreUIAdapter iguiAdapter in IgnoreUIAdapters)
        {
            iguiAdapter.Refresh();
        }
    }

    async void InitializeButton(){
        await Task.Delay(50);
        UpdateButton();
    }
    
    private void UpdateButton(){
        object[] gameViews = Resources.FindObjectsOfTypeAll(typeof(EditorWindow).Assembly.GetType("UnityEditor.PlayModeView"));
        foreach(EditorWindow gameView in gameViews)
        {
            if(!gameView.rootVisualElement.Contains(m_ResetCountButton)){
                var rc = gameView.rootVisualElement.Q<VisualElement>("rotate-ccw");
                rc.hierarchy.parent.Add(m_ResetCountButton);
            }
        }
    }
}

#endif
#endif
