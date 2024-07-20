using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using ThunderFireUITool;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

/// <summary>
/// 自动识别设计图并拼接Prefab
/// </summary>
public class PrefabAutoStitchingWindow : EditorWindow
{
    private class ResultData
    {
        public string Image;
        public float x;
        public float y;
    }

    private static PrefabAutoStitchingWindow m_window;


    private GUIStyle inputStyle;
    private IMGUIContainer assembleToolPathInput;
    private Button assembleToolPathSelectButton;

    private Label assembleToolInfoLabel;

    private VisualElement designImage;
    private ListView separateImagesPathsListView;
    private List<ObjectField> separateImageListViewItems;

    private string assembleToolPath = "";
    private static string designImgPath = "";
    private List<Object> separateImageListObjects;

    //缓存加载出来的Design图片尺寸,用于校验
    private Vector2 designTextureSize;
    //存储拼接程序返回的其他拼接信息
    private static List<string> assembleInfos;
    //存储拼接程序返回的拼接结果
    private static string assembleResult;

    private static GameObject prefabRoot;


    [MenuItem(ThunderFireUIToolConfig.Menu_AutoAssemblePrefab, false, 153)]
    public static void OpenWindow()
    {
        int width = 450;
        int height = 510;
        m_window = GetWindow<PrefabAutoStitchingWindow>();
        m_window.minSize = new Vector2(width, height);
        m_window.maxSize = new Vector2(width, height);
        m_window.titleContent.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_自动拼接工具);
        m_window.InitWindowData();
        m_window.InitWindowUI();

        if (!File.Exists(ThunderFireUIToolConfig.AutoAssembleToolPath))
        {
            ExtractZipFile();
        }
    }
    public static void ExtractZipFile()
    {
#if UNITY_2021_1_OR_NEWER
        using (ZipArchive archive = ZipFile.OpenRead(ThunderFireUIToolConfig.AutoAssembleToolZipPath))
        {
            try
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    entry.ExtractToFile(ThunderFireUIToolConfig.AutoAssembleToolPath, true);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e.Message);
            }
        }
#endif
    }

    protected void InitWindowData()
    {
        inputStyle = new GUIStyle();
        inputStyle.normal.textColor = Color.black;
        inputStyle.fontSize = 14;

        assembleToolPath = ThunderFireUIToolConfig.AutoAssembleToolPath;
        designImgPath = ThunderFireUIToolConfig.RootPath;
        separateImageListObjects = new List<Object> { null };

        separateImageListViewItems = new List<ObjectField>();
    }
    protected void InitWindowUI()
    {
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ThunderFireUIToolConfig.UIBuilderPath + "PrefabAutoStitchingWindow.uxml");
        VisualElement rootFromUXML = visualTree.CloneTree();

        Label assembleToolPathLabel = rootFromUXML.Q<Label>("AssembleToolPathLabel");
        assembleToolPathInput = rootFromUXML.Q<IMGUIContainer>("AssembleToolPathInput");
        assembleToolPathSelectButton = rootFromUXML.Q<Button>("AssembleToolPathSelectButton");

        assembleToolPathLabel.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_适配器);
        assembleToolPathSelectButton.clicked += () => SelectAssembleToolPath();
        assembleToolPathSelectButton.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_选择路径);
        assembleToolPathInput.onGUIHandler += () => OnPathInputGUI();

        assembleToolInfoLabel = rootFromUXML.Q<Label>("assembleToolInfoLabel");
        assembleToolInfoLabel.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_适配器解压说明);

        Label designLabel = rootFromUXML.Q<Label>("DesignLabel");
        Button selectDesignBtn = rootFromUXML.Q<Button>("SelectDesignButton");
        designImage = rootFromUXML.Q<VisualElement>("DesignImage");

        designLabel.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_设计图);
        selectDesignBtn.clicked += SelectDesignImgPath;
        selectDesignBtn.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_选择路径);

        Label separateImageLabel = rootFromUXML.Q<Label>("SeparateImageLabel");
        separateImagesPathsListView = rootFromUXML.Q<ListView>("IconListView");
        Button addBtn = rootFromUXML.Q<Button>("AddPathButton");
        Button removeBtn = rootFromUXML.Q<Button>("RemovePathButton");

        separateImageLabel.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_切图);
        addBtn.clicked += AddSeparateImagesPath;
        removeBtn.clicked += RemoveSeparateImagesPath;

        Func<VisualElement> makeItem = () =>
        {
            var objectField = new ObjectField("");
            objectField.objectType = typeof(Object);

            objectField.RegisterCallback<ChangeEvent<Object>>((evt) =>
            {
                objectField.value = evt.newValue;
                try
                {
                    int index = int.Parse(objectField.name);
                    if (separateImageListObjects.Count > index)
                    {
                        separateImageListObjects[index] = evt.newValue;
                    }
                }
                catch
                {

                }
            });
            separateImageListViewItems.Add(objectField);
            return objectField;
        };
        Action<VisualElement, int> bindItem = (e, i) =>
        {
            (e as ObjectField).value = separateImageListObjects[i];
            (e as ObjectField).name = i.ToString();
        };

        separateImageListViewItems.Clear();
        separateImagesPathsListView.makeItem = makeItem;
        separateImagesPathsListView.bindItem = bindItem;
        separateImagesPathsListView.itemsSource = separateImageListObjects;
        separateImagesPathsListView.selectionType = SelectionType.Multiple;
        //separateImagesPathsListView.showAddRemoveFooter = true;

        Button submitButton = rootFromUXML.Q<Button>("SubmitButton");
        Button clearButton = rootFromUXML.Q<Button>("ClearButton");

        submitButton.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定);
        clearButton.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消);

        submitButton.clicked += AutoAssemble;
        clearButton.clicked += () => { m_window.Close(); };

        rootVisualElement.Add(rootFromUXML);
    }

    private void SelectAssembleToolPath()
    {
        string folderPath = PlayerPrefs.GetString("LastAssembleToolPath");
        if (string.IsNullOrEmpty(folderPath))
        {
            folderPath = Path.GetDirectoryName(assembleToolPath);
        }

        string path = EditorUtility.OpenFilePanel(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_选择路径), folderPath, "");

        if (!string.IsNullOrEmpty(path))
        {
            int index = path.IndexOf("Assets");
            if (index != -1)
            {
                PlayerPrefs.SetString("LastAssembleToolPath", path);
                path = path.Substring(index);
                assembleToolPath = path;
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

    private void OnPathInputGUI()
    {
        assembleToolPath = EditorGUILayout.TextField(assembleToolPath, inputStyle);
    }

    protected void SelectDesignImgPath()
    {
        string folderPath = Application.dataPath;
        string path = EditorUtility.OpenFilePanel(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_选择路径), folderPath, "png");
        if (path != "")
        {
            designImgPath = path;
            int index = path.IndexOf("Assets");
            if (index != -1)
            {
                string relativePath = FileUtil.GetProjectRelativePath(designImgPath);
                var designTexture = AssetDatabase.LoadAssetAtPath<Texture>(relativePath);
                designTextureSize = new Vector2(designTexture.width, designTexture.height);
                designImage.style.backgroundImage = (StyleBackground)designTexture;
            }
            else
            {
                byte[] imageData = File.ReadAllBytes(path);
                var texture = new Texture2D(2, 2);
                // 加载图片二进制数据到 Texture2D 对象
                texture.LoadImage(imageData);
                designTextureSize = new Vector2(texture.width, texture.height);
                designImage.style.backgroundImage = (StyleBackground)texture;
            }
        }
    }

    private void AddSeparateImagesPath()
    {
        separateImageListObjects.Add(null);
        separateImageListViewItems.Clear();
#if UNITY_2021_3_OR_NEWER
        separateImagesPathsListView.Rebuild();
#else
        separateImagesPathsListView.Refresh();
#endif
    }

    private void RemoveSeparateImagesPath()
    {
#if UNITY_2020_3_OR_NEWER
        var selectedIndices = separateImagesPathsListView.selectedIndices.ToList();
        selectedIndices.Sort();
        for (int i = selectedIndices.Count - 1; i >= 0; i--)
        {
            separateImageListObjects.RemoveAt(selectedIndices[i]);
        }
#else
        if (separateImagesPathsListView.selectedIndex > 0)
        {
            separateImageListObjects.RemoveAt(separateImagesPathsListView.selectedIndex);
        }
#endif

        separateImageListViewItems.Clear();

#if UNITY_2021_3_OR_NEWER
        separateImagesPathsListView.Rebuild();
#else
        separateImagesPathsListView.Refresh();
#endif
    }

    private string GenSeparateImageArgv()
    {
        List<string> allSeparateImagesPaths = CollectAllSeparateImages();
        return string.Join(";", allSeparateImagesPaths);
    }

    private List<string> CollectAllSeparateImages()
    {
        List<string> allSeparateImagesPaths = new List<string>();
        for (int i = 0; i < separateImageListViewItems.Count; i++)
        {
            if (separateImageListViewItems[i].value != null)
            {
                string path = AssetDatabase.GetAssetPath(separateImageListViewItems[i].value);
                path = Path.GetFullPath(path);
                allSeparateImagesPaths.Add(path);
            }
        }
        return allSeparateImagesPaths;
    }

    private bool PreCheckDesignAndSeparateImages()
    {
        bool checkSuccess = true;
        StringBuilder sb = new StringBuilder();

        //路径不能为空
        if (string.IsNullOrEmpty(designImgPath))
        {
            checkSuccess = false;
            sb.Append("Design is IsNullOrEmpty\n");
        }
        //路径中不能包含空格
        if (designImgPath.Contains(" "))
        {
            checkSuccess = false;
            sb.Append("Design Path Contains Space\n");
        }

        List<string> allSeparateImagesPaths = CollectAllSeparateImages();
        foreach (string path in allSeparateImagesPaths)
        {
            //路径中不能包含空格
            if (path.Contains(" "))
            {
                checkSuccess = false;
                sb.Append(path + " Contains Space\n");
            }

            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path, "*.png", SearchOption.AllDirectories);

                for (int i = 0; i < files.Length; i++)
                {
                    var t = files[i].Replace("\\", "/");
                    string sppath = FileUtil.GetProjectRelativePath(t);
                    Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(sppath);
                    if (sp.texture.width > designTextureSize.x)
                    {
                        checkSuccess = false;
                        sb.Append(sppath + " weight > Design's weight\n");
                    }
                    if (sp.texture.height > designTextureSize.y)
                    {
                        checkSuccess = false;
                        sb.Append(sppath + " height > Design's height\n");
                    }
                }
            }
            else if (File.Exists(path))
            {
                var t = path.Replace("\\", "/");
                string sppath = FileUtil.GetProjectRelativePath(t);
                Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(sppath);
                if (sp.texture.width > designTextureSize.x)
                {
                    checkSuccess = false;
                    sb.Append(sppath + " weight > Design's weight\n");
                }
                if (sp.texture.height > designTextureSize.y)
                {
                    checkSuccess = false;
                    sb.Append(sppath + " height > Design's height\n");
                }
            }
        }
        if (!checkSuccess)
        {
            EditorUtility.DisplayDialog("messageBox",
                    sb.ToString(),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));
        }
        return checkSuccess;
    }

    private void AutoAssemble()
    {
        if (PreCheckDesignAndSeparateImages())
        {
            string separateImagesPath = GenSeparateImageArgv();

            //byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(designImgPath);
            //string utf8designImgPath = System.Text.Encoding.UTF8.GetString(utf8Bytes);
            //string base64designImgPath = Convert.ToBase64String(utf8Bytes);
            ExeUtils.CallExe_MatchDesignImage(assembleToolPath, OnReceiveMatchResult, designImgPath, separateImagesPath);
            GenPrefab();
            //m_window.Close();
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
            Debug.Log(e.Data);
            if (e.Data.Contains("UXMatchResult:"))
            {
                assembleResult = e.Data.Replace("UXMatchResult:", "");
            }
        }
    }

    private static void GenPrefab()
    {
        if (string.IsNullOrEmpty(assembleResult)) return;

        GenPrefabRoot();

        List<string> matchResult = assembleResult.Split(new Char[] { ';' }).ToList();
        foreach (var resultStr in matchResult)
        {
            var t = resultStr.Replace("\\", "/");
            try
            {
                ResultData result = JsonUtility.FromJson<ResultData>(t);

                GameObject imageNode = new GameObject(Path.GetFileNameWithoutExtension(result.Image));
                RectTransform rectTransform = imageNode.AddComponent<RectTransform>();
                imageNode.transform.parent = prefabRoot.transform;

                imageNode.AddComponent<CanvasRenderer>();

                UXImage image = imageNode.AddComponent<UXImage>();
                string relativePath = FileUtil.GetProjectRelativePath(result.Image);
                Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(relativePath);
                if (sp == null)
                {
                    Debug.Log("Can't Load Sprite: " + relativePath);
                    continue;
                }

                image.sprite = sp;

                rectTransform.sizeDelta = new Vector2(sp.rect.width, sp.rect.height);
                rectTransform.localPosition = new Vector3(result.x, result.y, 0);
            }
            catch
            {
                Debug.Log("Can't Parse Result: " + resultStr);
            }

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
        prefabRootTrans.anchoredPosition = Vector2.zero;

        byte[] imageData = File.ReadAllBytes(designImgPath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageData);
        prefabRootTrans.sizeDelta = new Vector2(texture.width, texture.height);

        prefabRoot.AddComponent<CanvasRenderer>();

        UXImage image = prefabRoot.AddComponent<UXImage>();
        Color color = Color.white;
        color.a = 0;
        image.color = color;

        Selection.activeObject = prefabRoot;
    }
}
