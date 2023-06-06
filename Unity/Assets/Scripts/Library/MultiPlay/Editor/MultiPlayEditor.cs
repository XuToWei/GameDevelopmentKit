using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using static MultiPlay.Utils;
using System.Runtime.CompilerServices;
namespace MultiPlay
{
    internal sealed class MultiPlayEditor : EditorWindow
    {
        #region privateMembers

        private string sourcePath;

        //Format
        private Texture2D headerTexture;
        private static float windowMinWidth = 180;
        private static float windowMinHeight = 180;
        private static float windowMaxWidthExpanded = 420;

        private Rect fullRect;
        private Rect headerRect;
        private Rect bodyRect;

        private static float headerTexScale = 0.20f;
        private GUISkin skin;
        private float pad = 15;

        private bool isCreatingReferences;
        private static bool hasChanged;
        private static bool isClone;
        private static string sceneFilePath;
        private static DateTime lastSyncTime;
        private static DateTime lastWriteTime;
        private static bool autoSync;

        private int cloneIndex;
        private object cloneName;
        private string headerText;
        private GUIStyle headerStyle;
        private GUIStyle linkStyle;
        private GUIStyle nonLinkStyle;

        private Color defaultFontColor;
        private string cloneHeaderText;

        private static string myPubID = "46749";
        private Vector2 scrollPos;
        private bool showSettings;
        public static MultiPlayEditor window;


        private static float ppp;
        private static float buttonHeight = 28;
        private static SynchronizationContext _mainThreadContext;


        #region License Setup

        private const string licenseMenuCaption =
            Settings.productLicence == Settings.Licence.Full ? "MultiPlay" : "DualPlay";

        #endregion

        #endregion

        private void Awake()
        {
            InitializeTextures();
        }

        #region menus

        [MenuItem("Tools/" + licenseMenuCaption + "/clone Manager &C", false, 10)]
        public static void OpenWindow()
        {
            try
            {
                string windowTitle = (Settings.productLicence == Settings.Licence.Full) ? "MultiPlay" : "DualPlay";
                if (window == null)
                {
                    window = GetWindow<MultiPlayEditor>(windowTitle, typeof(SceneView));
                }

                window.titleContent = new GUIContent(windowTitle,
                    EditorGUIUtility.ObjectContent(CreateInstance<MultiPlayEditor>(), typeof(MultiPlayEditor)).image);
                if (isClone)
                {
                    window.minSize = new Vector2(windowMinWidth, windowMinHeight);
                }
                else
                {
                    window.minSize = new Vector2(windowMinWidth, windowMinHeight);
                    window.maxSize = new Vector2(windowMaxWidthExpanded, windowMaxWidthExpanded * 1.5f);
                }

                RescaleUI();
                window.Show();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }


        [MenuItem("Tools/" + licenseMenuCaption + "/Clean Up", false, 11)]
        static void Menu_Cleanup()
        {
            CleanUpMenuItem.CleanUpClones();
        }

        [MenuItem("Tools/" + licenseMenuCaption + "/Clean Up", true, 11)]
        static bool Validate_Menu_Cleanup()
        {
            int cnt = Application.dataPath.Split('/').Length;
            string appFolderName = Application.dataPath.Split('/')[cnt - 2];

            return !Application.isPlaying && !isClone;
        }

        [MenuItem("Tools/" + licenseMenuCaption + "/Rate Please :)", false, 30)]
        public static void MenuRate() =>
            Application.OpenURL(
                $"https://assetstore.unity.com/packages/tools/utilities/multiplay-multiplayer-testing-without-builds-170pad9?aid=1011lds77&utm_source=aff#reviews");

        [MenuItem("Tools/" + licenseMenuCaption + "/Help", false, 30)]
        public static void MenuHelp()
        {
            Application.OpenURL("https://panettonegames.com/");

            string helpFilePath = Application.dataPath + @"/Plugins/PanettoneGames/MultiPlay/MultiPlay Read Me.pdf";
            Debug.Log($"Help file is in: {helpFilePath}");
            Application.OpenURL(helpFilePath);
            Application.OpenURL($"https://assetstore.unity.com/publishers/" + myPubID);
        }

        #endregion


        private void OnEnable()
        {
            InitializeTextures();
            _mainThreadContext = SynchronizationContext.Current;
            try
            {
                SceneChangeDetector.SceneChanged += OnSceneChanged;
                EditorApplication.playModeStateChanged += HandleOnPlayModeChanged;

                sceneFilePath = Application.dataPath + "/../" + SceneManager.GetActiveScene().path;
                sceneFilePath = sceneFilePath.Replace(@"/", @"\");

                isCreatingReferences = false;

                //headerColor = new Color(0, 0, 0);

                sourcePath = $"{Application.dataPath}/..";
                sourcePath = sourcePath.Replace(@"/", @"\");


                headerText = (Settings.productLicence == Settings.Licence.Full) ? "MultiPlay" : "DualPlay";
                headerStyle = (Settings.productLicence == Settings.Licence.Full)
                    ? skin.GetStyle("PanHeaderFull")
                    : skin.GetStyle("PanHeaderDefault");
                linkStyle = skin.GetStyle("SymLink");
                nonLinkStyle = skin.GetStyle("NoLink");

                defaultFontColor = GUI.contentColor;

                //RescaleUI();

                isClone = Utils.IsClone();


                //reset status
                hasChanged = false;
                lastSyncTime = DateTime.Now;

                InitializeTextures();
                CleanUpMenuItem.RemoveFromHub();
                if (isClone)
                {
                    Debug.Log($"lastWrite: {lastWriteTime}, lastSync: {lastSyncTime}");
                }


            Settings.SettingsAsset =
                    Resources.Load<MultiPlaySettings>(
                        "settings/MultiPlaySettings"); //there's already one scriptable object asset provided and you don't actually need to create another one, just find it and change its variables
                Settings.LoadSettings(this);
                if (isClone) ClearConsole();

                if (Settings.productLicence == Settings.Licence.Full)
                {
                    cloneIndex = GetCurrentCloneIndex();
                    cloneName = cloneIndex == 0 ? "Main" : $"clone[{cloneIndex}]";
                }

                string libraryText = Utils.IsLibraryLinked() ? " - Ω" : String.Empty;
                cloneHeaderText = (Settings.productLicence == Settings.Licence.Full)
                    ? $"Clone [{cloneIndex}]{libraryText}"
                    : $"Clone";
            }
            catch (Exception ex)
            {
                Debug.LogError($"{ex.Message}");
            }
        }

        private static bool isRescaled;
        private static void RescaleUI()
        {
            if (isRescaled) return;
            isRescaled = true;
            ppp = EditorGUIUtility.pixelsPerPoint;
            buttonHeight /= ppp;
            headerTexScale /= ppp;
            windowMinWidth /= ppp;
            windowMinHeight /= ppp;
            windowMaxWidthExpanded /= ppp;
        }

        private void OnDestroy()
        {
            Settings.SaveSettings();
            SceneChangeDetector.SceneChanged -= OnSceneChanged;
            EditorApplication.playModeStateChanged -= HandleOnPlayModeChanged;
        }


        private void HandleOnPlayModeChanged(PlayModeStateChange obj)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode && isClone && !autoSync)
            {
                //CheckIfSceneChanged();
                //if (hasChanged)
                {
                    ReloadScene(SceneManager.GetActiveScene().path);
                    hasChanged = false;
                }
            }
        }


        private static void OnSceneChanged(string sceneName)
        {
            if (!isClone) return;
            try
            {
                lastWriteTime = File.GetLastWriteTime(sceneFilePath);
                if(!autoSync) return;
                ReloadScene(sceneName);
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.Message}");
            }
        }

        private void InitializeTextures()
        {
            try
            {
                headerTexture = (Settings.productLicence == Settings.Licence.Full)
                    ? Resources.Load<Texture2D>("icons/MP_EditorHeader")
                    : Resources.Load<Texture2D>("icons/DP_EditorHeader");
                skin = Resources.Load<GUISkin>("guiStyles/Default");
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.Message}");
            }
        }

        private void OnGUI()
        {
            DrawLayout();
        }

        private static long GetDirSize(string searchDirectory)
        {
            // var files = Directory.EnumerateFiles(searchDirectory);
            // var directories = Directory.EnumerateDirectories(searchDirectory);
            // var subDirSize = (from directory in directories select GetDirSize(directory)).Sum();
            // return subDirSize;

            DirectoryInfo dirInfo = new DirectoryInfo(@searchDirectory);
            long dirSize = dirInfo.EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);
            return dirSize;
        }


        private void DrawLayout()
        {
            ppp = EditorGUIUtility.pixelsPerPoint;

            fullRect = new Rect(pad, pad, Screen.width - pad * 2, Screen.height - pad * 2);
            DrawHeader();
            DrawBody();
        }

        private void DrawHeader()
        {
            if (headerTexture == null || skin == null)
                InitializeTextures();

            //Header
            headerRect = new Rect(((Screen.width - headerTexture.width * headerTexScale) / ppp) - (20), pad,
                headerTexture.width * headerTexScale, headerTexture.height * headerTexScale);
            GUI.DrawTexture(headerRect, headerTexture);
            pad /= ppp;

            GUILayout.BeginArea(fullRect);

            if (isClone)
            {
                headerStyle = IsLibraryLinked() ? linkStyle : nonLinkStyle;
            }

            GUILayout.Label(isClone ? cloneHeaderText : headerText, headerStyle);
            GUILayout.EndArea();
        }

        private void DrawBody()
        {
            //Body
            bodyRect = new Rect(pad, headerRect.height + pad, Screen.width - pad * 2,
                Screen.height - headerRect.height - pad * 2);


            #region EditorPlaying

            if (EditorApplication.isPlaying)
            {
                GUILayout.BeginArea(bodyRect);
                EditorGUILayout.HelpBox($"{cloneName}: Control panel is disabled while playing.", MessageType.Info);
                ShowNotification(new GUIContent($"Running on {cloneName}..."), 1);
                if (GUILayout.Button("More cool tools...", skin.GetStyle("PanStoreLink")))
                {
                    Application.OpenURL($"https://assetstore.unity.com/publishers/" + myPubID);
                    Application.OpenURL("https://panettonegames.com/");
                }

                GUILayout.EndArea();
                return;
            }

            #endregion

            if (isClone) //clone
            {
                GUILayout.BeginArea(bodyRect);
                if (GUILayout.Button("Sync"))
                {
                    hasChanged = false;
                    lastSyncTime = DateTime.Now;
                    ShowNotification(new GUIContent("Syncing..."));
                    ReloadScene(SceneManager.GetActiveScene().path);
                }

                string autoSyncCaption =
                    !IsLibraryLinked() ? "Auto Sync" : "Auto Sync unavailable in Link Library Mode";
                GUI.enabled = !Utils.IsLibraryLinked();
                autoSync = GUILayout.Toggle(!IsLibraryLinked() && autoSync, autoSyncCaption);
                GUI.enabled = true;

                if (hasChanged)
                    EditorGUILayout.HelpBox(
                        "Changes from original build were detected. Make sure to Sync before running",
                        MessageType.Warning);
                else
                    EditorGUILayout.HelpBox(
                        $"You're Good to Go!\nLast Changed:\t{lastWriteTime}\nLast Synced:\t{lastSyncTime}",
                        MessageType.Info);

                GUILayout.EndArea();
            }

            else //Original Copy
            {
                GUILayout.BeginArea(bodyRect); ////////////////////1
                GUILayout.BeginVertical(GUILayout.Height((Screen.height - pad) / ppp),
                    GUILayout.Width((Screen.width - pad * 2) / ppp)); //////////////2

                if (isCreatingReferences)
                {
                    isCreatingReferences = false;
                    ShowNotification(new GUIContent("Creating clone..."));
                }
                else //Create References Or Launch
                {
                    scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true),
                        GUILayout.Height(105 / ppp), GUILayout.Width((Screen.width - pad * 2) / ppp));

                    for (int i = 1; i < Settings.MaxClones + 1; i++)
                    {
                        string destinationPath =
                            $"{Settings.ClonesPath}/{Application.productName}_[{i}]_Clone".Replace(@"/", @"\");
                        var createLinkCaption = Settings.LinkLibrary ? "- Ω" : string.Empty;
                        var libPath = Path.Combine(destinationPath, "Library");
                        var linkExists = Directory.Exists(libPath);
                        var openLinkCaption = string.Empty;
                        if (linkExists)
                            openLinkCaption = IsSymbolic(libPath) ? "- Ω" : String.Empty;

                        string btnCaption = Directory.Exists(destinationPath)
                            ? $"Launch clone [{i}] {openLinkCaption}"
                            : $"Create clone [{i}] {createLinkCaption}";
                        GUI.enabled = !Directory.Exists(destinationPath + "\\Temp");

                        GUILayout.BeginHorizontal();
                        if (Directory.Exists(destinationPath)) GUI.contentColor = Color.cyan;
                        if (Directory.Exists(destinationPath) && IsSymbolic(libPath)) GUI.contentColor = Color.yellow;
                        if (GUILayout.Button(btnCaption, GUILayout.Height(buttonHeight)))
                        {
                            if (!Directory.Exists(destinationPath))
                            {
                                if (!Settings.LinkLibrary)
                                {
                                    var libSize = GetDirSize($"{Application.dataPath}/../Library");
                                    string sizeInMB = libSize.ToSize(ByteExtensions.SizeUnits.MB);
                                    var msg =
                                        $"WARNING!! You're about to create a clone with {sizeInMB}. Are you sure you want to proceed?";
                                    var result = EditorUtility.DisplayDialog("Cloning with a library copy", msg,
                                        "Proceed", "Cancel");
                                    if (!result)
                                    {
                                        Debug.Log($"Operation canceled by user.");
                                        GUILayout.EndHorizontal();
                                        EditorGUILayout.EndScrollView();
                                        GUILayout.EndVertical();
                                        GUILayout.EndArea();
                                        return;
                                    }
                                }

                                Debug.Log($"creating clone {i} in {destinationPath.Replace("\\\\", "\\")}");

                                Settings.SaveSettings();
                                Settings.LoadSettings(this);

                                EditorSceneManager.SaveScene(SceneManager.GetActiveScene());

                                isCreatingReferences = false;

                                CreateLink(destinationPath, "Assets");
                                CreateLink(destinationPath, "ProjectSettings");
                                CreateLink(destinationPath, "Packages");

                                if (Settings.LinkLibrary)
                                {
                                    CreateLink(destinationPath, "Library"); //kills auto sync.
                                }
                            }

                            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                            hasChanged = false;
                            LaunchClone(destinationPath);
                            CleanUpMenuItem.RemoveFromHub();
                            //Close();

                            GUI.enabled = true;
                        }

                        if (Directory.Exists(destinationPath))
                        {
                            GUI.contentColor = Color.red;
                            if (GUILayout.Button("x", GUILayout.Height(buttonHeight), GUILayout.Width(35 / ppp)))
                            {
                                Debug.Log($"Deleting [{new DirectoryInfo(destinationPath).Name}]");
                                CleanUpMenuItem.ClearClone(destinationPath);
                            }
                        }

                        GUILayout.EndHorizontal();
                        GUI.contentColor = defaultFontColor;
                    }

                    EditorGUILayout.EndScrollView();
                    GUI.enabled = true;
                }

                if (Settings.productLicence == Settings.Licence.Full)
                {
                    showSettings =
                        EditorGUILayout.BeginFoldoutHeaderGroup(showSettings,
                            "Settings"); //, skin.GetStyle("PanHeaderDefault"));
                    if (showSettings)
                    {
                        try
                        {
                            EditorGUILayout.Space(5 / ppp);
                            GUILayout.BeginVertical(GUILayout.Height(Screen.height - pad * 2),
                                GUILayout.Width(Screen.width - pad * 2));
                            Settings.LinkLibrary = GUILayout.Toggle(Settings.LinkLibrary, "Link Library");

                            Settings.MaxClones = EditorGUILayout.IntField(
                                new GUIContent("Max clones:",
                                    $"Maximum number of allowed clones is {Settings.MaxClonesLimit}"),
                                Mathf.Clamp(Settings.MaxClones, 1, Settings.MaxClonesLimit));
                            Settings.MaxClones = Mathf.Clamp(Settings.MaxClones, 1, Settings.MaxClonesLimit);

                            Settings.ClonesPath = EditorGUILayout.TextField(
                                new GUIContent("Clones Path:", "Default Path of project clones"), Settings.ClonesPath);
                            if (GUILayout.Button("Browse", GUILayout.Height(buttonHeight),
                                    GUILayout.Width((Screen.width - pad * 2) / ppp)))
                            {
                                string path = EditorUtility.OpenFolderPanel("Select Clones Folder", Settings.ClonesPath,
                                    "");
                                if (path.Length != 0)
                                {
                                    Settings.ClonesPath = path.Replace('/', '\\');
                                    Settings.SaveSettings();
                                    Repaint();
                                }
                            }

                            //GUI.Label(new Rect(10, pad0, 100, 40), GUI.tooltip); //another way to display the tool tip
                            string libraryTip = (Settings.LinkLibrary)
                                ? $"including Library link. i.e. faster but may break some 3rd party packages (recommended for most small projects)"
                                : "excluding Library link. i.e. project configuration and packages will be stored separately at an extra disk cost. This option is safer for larger projects";
                            var msgType = (Settings.LinkLibrary) ? MessageType.Warning : MessageType.Info;

                            GUILayout.BeginHorizontal(GUILayout.Width((Screen.width - pad) / ppp));
                            EditorGUILayout.HelpBox(
                                $"New clones will be created in [{new DirectoryInfo(Settings.ClonesPath).Name}] {libraryTip}.",
                                msgType);
                            GUILayout.EndHorizontal();

                            //GUILayout.Space(10);
                            GUILayout.EndVertical();
                        }
                        catch (Exception)
                        {
                            Debug.LogError("Settings Error");
                        }
                    }
                }

                GUILayout.EndVertical();
                GUILayout.EndArea();
            }
        }

        private bool IsSymbolic(string path)
        {
            FileInfo pathInfo = new FileInfo(path);
            return pathInfo.Attributes.HasFlag(FileAttributes.ReparsePoint);
        }

        public static int DoLinksExist()
        {
            int cnt = 0;

            for (int i = 1; i < Settings.MaxClones + 1; i++)
            {
                string destinationPath =
                    $"{Settings.ClonesPath}/{Application.productName}_[{i}]_Clone".Replace(@"/", @"\");
                //if (i == 1) result = Directory.Exists(destinationPath);
                //result = result || Directory.Exists(destinationPath);

                if (Directory.Exists(destinationPath)) cnt++;
            }

            return cnt;
        }

        public static bool DoLinksLive()
        {
            bool result = false;
            for (int i = 1; i < Settings.MaxClones + 1; i++)
            {
                string destinationPath =
                    $"{Settings.ClonesPath}/{Application.productName}_[{i}]_Clone".Replace(@"/", @"\");
                if (i == 1) result = Directory.Exists(destinationPath + "\\Temp");

                result = result || Directory.Exists(destinationPath + "\\Temp");
            }

            return result;
        }

        private static void ReloadScene(string scenePath)
        {
            try
            { 
                _mainThreadContext.Post(state =>
                {
                    // Scene scene = SceneManager.GetSceneByPath(scenePath);
                    // if (scene.IsValid())
                    // {
                    //     EditorSceneManager.LoadScene(scene.name, LoadSceneMode.Additive);
                    // }
                    EditorSceneManager.OpenScene(scenePath);
                    Canvas.ForceUpdateCanvases();

                }, null);

            }
            catch (Exception e)
            {
                    Debug.LogError($"Error reloading Scene. {e.Message}");
            }
        }

        private void CreateLink(string destPath, string subDirectory)
        {
            if (!Directory.Exists(destPath))
                Directory.CreateDirectory(destPath);

            string cmd, args;
            try
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.WindowsEditor:
                        cmd = "cmd";
                        args = $"/c mklink /j \"{destPath}\\{subDirectory}\" \"{sourcePath}\\{subDirectory}\"";
                        break;

                    case RuntimePlatform.OSXEditor:
                    case RuntimePlatform.LinuxEditor:
                        cmd = "/bin/bash";
                        args = $"ln -s \"{sourcePath}/{subDirectory}\" \"{destPath}/{subDirectory}\"";
                        break;

                    default:
                        throw new NotImplementedException("Platform not supported!");
                }

                var process = new System.Diagnostics.Process();
                process.StartInfo.FileName = cmd;
                process.StartInfo.Arguments = args;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    Debug.LogWarning($"Could not link {subDirectory}, trying again...\n{output}");
                    // Handle the error case here
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Links failed. Please contact your system administrator\n{e.Message}");
            }
        }


        private void LaunchClone(string destPath)
        {
            try
            {
                string editorPath = GetAppPath(Application.platform);

                string editorArgs =
                    $"-DisableDirectoryMonitor ‑ignorecompilererrors -disable-assembly-updater -silent-crashes";
                string projectPath = $" -projectPath \"{destPath}\"";

                var thread = new Thread(delegate() { ExcuteCmd($"\"{editorPath}\"", editorArgs + projectPath); });
                //Debug.Log();

                thread.Start();
                //RemoveFromHub();
                if (isClone) ClearConsole();
            }
            catch (Exception e)
            {
                Debug.LogError(
                    $"Unable to read temporary files due to insufficient User Privileges. Please contact your system administrator. \nDetails: {e.Message}");
            }
        }

        private string GetAppPath(RuntimePlatform currentPlatform)
        {
            switch (currentPlatform)
            {
                case RuntimePlatform.WindowsEditor:
                    return EditorApplication.applicationPath;
                case RuntimePlatform.OSXEditor:
                    return EditorApplication.applicationPath + "/Contents/MacOS/Unity";
                case RuntimePlatform.LinuxEditor:
                    return EditorApplication.applicationPath;
                default:
                    throw new NotImplementedException("Platform not supported!");
            }
        }


        private static void ClearConsole()
        {
            var logEntries = Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
            if (logEntries != null)
            {
                var clearMethod = logEntries.GetMethod("Clear",
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                clearMethod?.Invoke(null, null);
            }
        }

        public static void ExcuteCmd(string prog, string args)
        {
            if (prog == null) return;
            try
            {
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                bool isCleaningUp = args.StartsWith("/c rd");
                startInfo.WindowStyle = isCleaningUp ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Maximized;

                startInfo.FileName = prog;
                startInfo.Arguments = args;

                process.StartInfo = startInfo;
                process.Start();

                process.WaitForExit();

                process.Close();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
            finally
            {
                CleanUpMenuItem.RemoveFromHub();
            }
        }
        
        private static void CreateCopy(string tgtDir, string srcDirName)
        {
            CopyDirectory($"{Application.dataPath}/../{srcDirName}", $"{tgtDir}/{srcDirName}");
            
            void CopyDirectory(string srcDir, string tgtDir)
            {
                DirectoryInfo source = new DirectoryInfo(srcDir);
                DirectoryInfo target = new DirectoryInfo(tgtDir);

                if (target.FullName.StartsWith(source.FullName, StringComparison.CurrentCultureIgnoreCase))
                {
                    throw new Exception("父目录不能拷贝到子目录！");
                }

                if (!source.Exists)
                {
                    return;
                }

                if (!target.Exists)
                {
                    target.Create();
                }

                FileInfo[] files = source.GetFiles();

                for (int i = 0; i < files.Length; i++)
                {
                    File.Copy(files[i].FullName, Path.Combine(target.FullName, files[i].Name), true);
                }

                DirectoryInfo[] dirs = source.GetDirectories();

                for (int j = 0; j < dirs.Length; j++)
                {
                    CopyDirectory(dirs[j].FullName, Path.Combine(target.FullName, dirs[j].Name));
                }
            }
        }
    }
}