using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using UnityEngine;
using ThunderFireUITool;
using UnityEditor;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEditorInternal;
using UnityEngine.TestTools;

public partial class CustomUnityWindowHelper
{
    public delegate bool EnumThreadWindowsCallback(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool EnumWindows(EnumThreadWindowsCallback callback, IntPtr extraData);
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetWindowThreadProcessId(HandleRef handle, out int processId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern IntPtr GetWindow(HandleRef hWnd, int uCmd);

    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern IntPtr GetParent(HandleRef hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool IsWindowVisible(HandleRef hWnd);

    [DllImport("user32.dll")]
    private static extern bool GetWindowText(int hWnd, StringBuilder title, int maxBufSize);

    [DllImport("user32.dll", EntryPoint = "SetWindowText", CharSet = CharSet.Auto)]
    public extern static int SetWindowText(int hwnd, string lpString);

    private bool haveMainWindow = false;
    public IntPtr hwnd = IntPtr.Zero;
    private IntPtr mainWindowHandle = IntPtr.Zero;
    private int processId = 0;
    private static string originTitle = "";
    
    private static StringBuilder windowTitleBuffer = new StringBuilder(256);


    private static CustomUnityWindowHelper _instance;
    public static CustomUnityWindowHelper Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new CustomUnityWindowHelper();
                _instance.hwnd = _instance.GetMainWindowHandle(Process.GetCurrentProcess().Id);
                originTitle = GetDefaultMainWindowTitle();
            }
            return _instance;
        }
    }

    public void SetTitle(string customTitle)
    {
        //hwnd = Process.GetCurrentProcess().MainWindowHandle;
        if(hwnd == IntPtr.Zero)
        {
            hwnd = GetMainWindowHandle(Process.GetCurrentProcess().Id);
        }

        if (hwnd == IntPtr.Zero) return;

        if(string.IsNullOrEmpty(customTitle))
        {
            SetWindowText(hwnd.ToInt32(), originTitle);
        }
        else
        {
            SetWindowText(hwnd.ToInt32(), string.Format("{0} - {1}", customTitle, originTitle));
        }
        //UnityEngine.Debug.Log("Current Unity Title 2: " + customTitle);
    }

    public static string GetDefaultMainWindowTitle()
    {
        string projectName = EditorApplication.isTemporaryProject ? PlayerSettings.productName : Path.GetFileName(Path.GetDirectoryName(Application.dataPath));

#if UNITY_2021_1_OR_NEWER
        string unityVersion = (Unsupported.IsSourceBuild() || Unsupported.IsDeveloperMode()) ? InternalEditorUtility.GetUnityDisplayVersion() : Application.unityVersion;
#else
        string unityVersion = (Unsupported.IsSourceBuild() || Unsupported.IsDeveloperMode()) ? InternalEditorUtility.GetFullUnityVersion() : Application.unityVersion;
#endif
        string activeSceneName = L10n.Tr("Untitled");
        if (!string.IsNullOrEmpty(SceneManager.GetActiveScene().path))
        {
            activeSceneName = Path.GetFileNameWithoutExtension(SceneManager.GetActiveScene().path);
        }
        string targetName = BuildPipeline.GetBuildTargetName(EditorUserBuildSettings.activeBuildTarget);
        bool codeCoverageEnabled = Coverage.enabled;

            var title = Application.platform == RuntimePlatform.OSXEditor
            ? $"{activeSceneName} - {projectName}"
            : $"{projectName} - {activeSceneName}";

        // FUTURE: [CODE COVERAGE] and the build target info do not belong in the title bar. they
        // are there now because we want them to be always-visible to user, which normally would be a) buildconfig
        // bar or b) status bar, but we don't have a) and our b) needs work to support such a thing.

        if (!string.IsNullOrEmpty(targetName))
        {
            title += $" - {targetName}";
        }

        title += $" - Unity {unityVersion}";

        if (codeCoverageEnabled)
        {
            title += " " + L10n.Tr("[CODE COVERAGE]");
        }

        return title;
    }

    public IntPtr GetMainWindowHandle(int processId)
    {
        this.processId = processId;
        EnumThreadWindowsCallback callback = new EnumThreadWindowsCallback(this.EnumWindowsCallback);
        EnumWindows(callback, IntPtr.Zero);
        GC.KeepAlive(callback);
        return mainWindowHandle;
    }

    private bool EnumWindowsCallback(IntPtr handle, IntPtr extraParameter)
    {
        int num;
        GetWindowThreadProcessId(new HandleRef(this, handle), out num);
        if ((num == this.processId) && this.IsMainWindow(handle))
        {
            this.mainWindowHandle = handle;
            return false;
        }
        return true;
    }

    private static int GW_OWNER = 4;
    private bool IsMainWindow(IntPtr handle)
    {
        HandleRef handleRef = new HandleRef(this, handle);

        bool hasOwner = GetWindow(handleRef, GW_OWNER) != IntPtr.Zero;

        bool hasParent = GetParent(handleRef) != IntPtr.Zero;

        bool visible = IsWindowVisible(handleRef);
        
        //判断窗口标题是否含有"Unity 20", mainWindow是有的 messageBox目前看都没有
        GetWindowText(handle.ToInt32(), windowTitleBuffer, windowTitleBuffer.Capacity);
        
        string title = windowTitleBuffer.ToString();

        return !hasOwner && !hasParent && visible && title.Contains("Unity");
    }
}