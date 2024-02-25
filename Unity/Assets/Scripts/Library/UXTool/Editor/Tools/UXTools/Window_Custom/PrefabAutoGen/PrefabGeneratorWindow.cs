using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ThunderFireUITool;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

/// <summary>
/// 自动识别设计图并拼接Prefab 预览版
/// </summary>
public class PrefabGeneratorWindow : EditorWindow
{
    private class ResultData
    {
        public string Image;
        public float x;
        public float y;
    }

    private static PrefabGeneratorWindow m_window;

    private static string projectPath = Application.dataPath;
    private static string pythonScriptPath = projectPath + "/Tools/ImageMatch/main.exe";
    private static string designImgPath = "/Test/Test4/_Design.png";
    private static string templateFolderPath = "/Test/Test4/";
    private static string prefabPath = "/Test/AutoGen.prefab";

    private static string result;

    private static GameObject prefabRoot;

    protected GUIStyle inputStyle;
    protected IMGUIContainer pathInput_1;
    protected IMGUIContainer pathInput_2;
    protected IMGUIContainer pathInput_3;
    protected UnityEngine.UIElements.Button pathSelectButton_1;
    protected UnityEngine.UIElements.Button pathSelectButton_2;
    protected UnityEngine.UIElements.Button pathSelectButton_3;
    private static string selectedPath1 = "";
    private static string selectedPath2 = "";
    private static string selectedPath3 = "";

    //[MenuItem(ThunderFireUIToolConfig.Menu_AutoGenPrefab, false, 153)]
    public static void OpenWindow()
    {
        int width = 400;
        int height = 300;
        m_window = GetWindow<PrefabGeneratorWindow>();
        m_window.minSize = new Vector2(width, height);
        m_window.titleContent.text = "自动生成";
        m_window.InitData();
        m_window.InitUI();
    }

    protected virtual void InitData()
    {
        selectedPath1 = ThunderFireUIToolConfig.RootPath;
        selectedPath2 = ThunderFireUIToolConfig.RootPath;
        inputStyle = new GUIStyle();
        inputStyle.normal.textColor = Color.black;
        inputStyle.fontSize = 14;
    }
    protected virtual void InitUI()
    {
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ThunderFireUIToolConfig.UIBuilderPath + "Constant/autogen.uxml");
        VisualElement labelFromUXML = visualTree.CloneTree();

        VisualElement confirmBtn = labelFromUXML.Q<VisualElement>("confirm");
        VisualElement cancelBtn = labelFromUXML.Q<VisualElement>("cancel");

        Label pathLabel = labelFromUXML.Q<Label>("pathLabel_1");
        pathInput_1 = labelFromUXML.Q<IMGUIContainer>("pathInput_1");
        pathSelectButton_1 = labelFromUXML.Q<UnityEngine.UIElements.Button>("pathSelectbutton_1");

        Label pathLabe2 = labelFromUXML.Q<Label>("pathLabel_2");
        pathInput_2 = labelFromUXML.Q<IMGUIContainer>("pathInput_2");
        pathSelectButton_2 = labelFromUXML.Q<UnityEngine.UIElements.Button>("pathSelectbutton_2");

        Label pathLabe3 = labelFromUXML.Q<Label>("pathLabel_3");
        pathInput_3 = labelFromUXML.Q<IMGUIContainer>("pathInput_3");
        pathSelectButton_3 = labelFromUXML.Q<UnityEngine.UIElements.Button>("pathSelectbutton_3");

        string confirmString = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定);
        string cancelString = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消);
        confirmBtn.Q<Label>("text").text = confirmString;
        cancelBtn.Q<Label>("text").text = cancelString;

        pathSelectButton_1.clicked +=()=> SelectPath(pathSelectButton_1);
        pathInput_1.onGUIHandler += () => OnPathInputGUI(pathSelectButton_1);

        pathSelectButton_2.clicked += () => SelectPath(pathSelectButton_2);
        pathInput_2.onGUIHandler += () => OnPathInputGUI(pathSelectButton_2);

        pathSelectButton_3.clicked += () => SelectPath(pathSelectButton_3);
        pathInput_3.onGUIHandler += () => OnPathInputGUI(pathSelectButton_3);

        confirmBtn.RegisterCallback((MouseDownEvent e) =>
        {
            PythonUtils.CallPython_MatchDesignImage(selectedPath3, OnReceiveMatchResult, selectedPath1, selectedPath2 + "/");
            GenPrefab();
            m_window.Close();
        });
        cancelBtn.RegisterCallback((MouseDownEvent e) =>
        {
            m_window.Close();
        });

        new SelectorItem(labelFromUXML.Q<VisualElement>("confirmSelector"), confirmBtn, false);
        new SelectorItem(labelFromUXML.Q<VisualElement>("cancelSelector"), cancelBtn);

        //Test
        var confirmBtn2 = new UnityEngine.UIElements.Button();
        confirmBtn.parent.parent.parent.Add(confirmBtn2);
        confirmBtn2.style.height = 30;
        confirmBtn2.style.width = 100;
        confirmBtn2.style.left = -100;
        confirmBtn2.style.bottom = 0;
        confirmBtn2.text = "旧版生成";

        confirmBtn2.clicked += () =>
        {
            ExeUtils.CallExe_MatchDesignImage(selectedPath3, OnReceiveMatchResult, selectedPath1, selectedPath2 + "/");
            GenPrefab();
            m_window.Close();
        };
        //Test End

        rootVisualElement.Add(labelFromUXML);
    }

    protected void SelectPath(UnityEngine.UIElements.Button button)
    {
        string folderPath = Application.dataPath;
        string path = "";
        if (button != pathSelectButton_2)
        {
            path = EditorUtility.OpenFilePanel(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_选择路径), folderPath, "");
        }
        else
        {
            path = EditorUtility.OpenFolderPanel(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_选择路径), folderPath, "");
        }
        if (path != "")
        {
            int index = path.IndexOf("Assets");
            if (index != -1)
            {
                PlayerPrefs.SetString("LastParticleCheckPath", path);
                path = path.Substring(index);
                if (button == pathSelectButton_1)
                {
                    selectedPath1 = path;
                }
                else if (button == pathSelectButton_2)
                {
                    selectedPath2 = path;
                }
                else
                {
                    selectedPath3 = path;
                }
            }
            else
            {
                EditorUtility.DisplayDialog("messageBox",
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_目录不在Assets下Tip),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));
            }
        }
    }

    private void OnPathInputGUI(UnityEngine.UIElements.Button button)
    {
        if(button == pathSelectButton_1)
        {
            selectedPath1 = EditorGUILayout.TextField(selectedPath1, inputStyle);
        }
        else if(button == pathSelectButton_2)
        {
            selectedPath2 = EditorGUILayout.TextField(selectedPath2, inputStyle);
        }
        else
        {
            selectedPath3 = EditorGUILayout.TextField(selectedPath3, inputStyle);
        }
    }


    /// <summary>
    /// 处理Python运行结果,创建Prefab
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public static void OnReceiveMatchResult(object sender, DataReceivedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Data) == false)
        {
            UnityEngine.Debug.Log(e.Data);
            result = e.Data;
        }
    }

    private static void GenPrefab()
    {
        if (string.IsNullOrEmpty(result)) return;

        GenPrefabRoot();

        List<string> matchResult = result.Split(';').ToList();
        foreach (var resultStr in matchResult)
        {
            ResultData result = JsonUtility.FromJson<ResultData>(resultStr);

            GameObject imageNode = new GameObject(Path.GetFileNameWithoutExtension(result.Image));
            RectTransform rectTransform = imageNode.AddComponent<RectTransform>();
            imageNode.transform.parent = prefabRoot.transform;

            imageNode.AddComponent<CanvasRenderer>();

            UXImage image = imageNode.AddComponent<UXImage>();
            Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(selectedPath2 + "/" + result.Image);
            if(sp == null)
            {
                UnityEngine.Debug.Log("Can't Load Sprite: " + selectedPath2 + "/" + result.Image);
                continue;
            }
            
            image.sprite = sp;

            rectTransform.sizeDelta = new Vector2(sp.rect.width, sp.rect.height);
            rectTransform.localPosition = new Vector3(result.x, result.y, 0);
        }

        //GameObject prefabAsset = ToolUtils.CreatePrefab(go, prefabPath);
    }

    private static void GenPrefabRoot()
    {
        GameObject canvasGo = Transform.FindObjectOfType<Canvas>()?.gameObject;
        if (canvasGo == null)
        {
            canvasGo = new GameObject("Canvas");
            canvasGo.AddComponent<RectTransform>();
            canvasGo.AddComponent<Canvas>();
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();
        }

        prefabRoot = new GameObject("AutoGen");
        RectTransform prefabRootTrans = prefabRoot.AddComponent<RectTransform>();
        
        prefabRootTrans.parent = canvasGo.transform;
        Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(selectedPath1);
        prefabRootTrans.anchoredPosition = Vector2.zero;
        prefabRootTrans.sizeDelta = new Vector2(sp.rect.width, sp.rect.height);


        prefabRoot.AddComponent<CanvasRenderer>();

        UXImage image = prefabRoot.AddComponent<UXImage>();
        Color color = Color.white;
        color.a = 0;
        image.color = color;
    }
}
