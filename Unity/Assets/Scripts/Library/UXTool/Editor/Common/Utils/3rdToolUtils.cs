using System.Diagnostics;

/// <summary>
/// 简单的Unity Python 交互
/// </summary>
public static class PythonUtils
{
    public static void CallPython_Script(string pyScriptPath)
    {
        DataReceivedEventHandler handler = new DataReceivedEventHandler(ReceivePython_Script);
        CallPythonBase(pyScriptPath, handler);
    }

    public static void ReceivePython_Script(object sender, DataReceivedEventArgs e)
    {
        // 结果不为空才打印
        if (string.IsNullOrEmpty(e.Data) == false)
        {
            UnityEngine.Debug.Log(e.Data);
        }
    }

    /// <summary>
    /// Unity 调用 Python
    /// </summary>
    /// <param name="pyScriptPath">python 脚本路径</param>
    /// <param name="handler">结果处理委托</param>
    /// <param name="argvs">python 函数参数</param>
    public static void CallPythonBase(string pyScriptPath, DataReceivedEventHandler handler, params string[] argvs)
    {
        Process process = new Process();

        // python 的解释器位置 python.exe
        process.StartInfo.FileName = @"C:\DevelopTools\Python\Python38\python.exe";

        pyScriptPath = @"C:\Project_Compony\UXTools\UXTools2022\UXTools\Assets\Tools\ImageMatch\main.py";
        if (argvs != null)
        {
            foreach (string item in argvs)
            {
                pyScriptPath += " " + item;
            }
        }
        UnityEngine.Debug.Log(pyScriptPath);

        process.StartInfo.UseShellExecute = false;
        process.StartInfo.Arguments = pyScriptPath;     // 路径+参数
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.CreateNoWindow = true;        // 不显示执行窗口

        // 开始执行，获取执行输出，添加结果输出委托
        process.Start();
        process.BeginOutputReadLine();
        process.OutputDataReceived += handler;
        process.WaitForExit();
    }
    public static void CallPython_MatchDesignImage(string pyScriptPath, DataReceivedEventHandler receivedFunc ,string designImgPath, string templateFolderPath)
    {
        DataReceivedEventHandler handler = new DataReceivedEventHandler(receivedFunc);
        CallPythonBase(pyScriptPath, handler, designImgPath, templateFolderPath);
    }
}

/// <summary>
/// 简单的Unity Exe应用程序 交互
/// </summary>
public static class ExeUtils
{
    public static void CallExe(string pyScriptPath)
    {
        DataReceivedEventHandler handler = new DataReceivedEventHandler(ReceiveExeResponse);
        CallExeBase(pyScriptPath, handler);
    }

    public static void ReceiveExeResponse(object sender, DataReceivedEventArgs e)
    {
        // 结果不为空才打印
        if (string.IsNullOrEmpty(e.Data) == false)
        {
            UnityEngine.Debug.Log(e.Data);
        }
    }

    /// <summary>
    /// Unity 调用 Exe应用程序
    /// </summary>
    /// <param name="exePath">exe 路径</param>
    /// <param name="handler">结果处理委托</param>
    /// <param name="argvs">参数</param>
    public static void CallExeBase(string exePath, DataReceivedEventHandler handler, params string[] argvs)
    {
        Process process = new Process();

        exePath = @"C:\Project_Compony\UXTools\UXTools2022\UXTools\Assets\Tools\ImageMatch\main.exe";
        process.StartInfo.FileName = exePath;

        string argumentsStr = "";
        if (argvs != null)
        {
            foreach (string item in argvs)
            {
                argumentsStr += " " + item;
            }
        }
        UnityEngine.Debug.Log(argumentsStr);

        process.StartInfo.UseShellExecute = false;
        process.StartInfo.Arguments = argumentsStr;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.CreateNoWindow = true;    // 不显示执行窗口

        // 开始执行，获取执行输出，添加结果输出委托
        process.Start();
        process.BeginOutputReadLine();
        process.OutputDataReceived += handler;
        process.WaitForExit();
    }
    public static void CallExe_MatchDesignImage(string exePath, DataReceivedEventHandler receivedFunc, string designImgPath, string templateFolderPath)
    {
        DataReceivedEventHandler handler = new DataReceivedEventHandler(receivedFunc);
        CallExeBase(exePath, handler, designImgPath, templateFolderPath);
    }
}