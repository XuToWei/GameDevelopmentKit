using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.Win32;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace MultiPlay
{
    internal static class CleanUpMenuItem
    {
        public static void CleanUpClones()
        {
            int clonesFound = MultiPlayEditor.DoLinksExist();

            if (clonesFound == 0)
            {
                if (!EditorUtility.DisplayDialog("Clearing References",
                        $"No clones were found in {Settings.ClonesPath}, Try clear references anyway?",
                        "Proceed", "Cancel"))
                    return;
            }
            else if (MultiPlayEditor.DoLinksLive())
            {
                Debug.LogWarning(
                    "WARNING: Live clones were detected! You Should close them before clearing references; Otherwise, Unity may crash!");
                if (!EditorUtility.DisplayDialog("Clearing References",
                        "WARNING!! Make sure ALL clones are CLOSED before proceeding!!", "Proceed", "Cancel"))
                    return;
            }
            else if (!EditorUtility.DisplayDialog("Clearing References",
                         $"Clearing cached references to {clonesFound} clones, are you sure you want to proceed?",
                         "Proceed", "Cancel"))
            {
                return;
            }

            try
            {
                Debug.Log("Cleaning cache...");
                EditorUtility.DisplayProgressBar("Processing..", "Shows a progress", 0.9f);
                PurgeAllClones();
                EditorUtility.ClearProgressBar();
                Debug.Log("MultiPlay: References cleared successfully");
                RemoveFromHub();
                EditorUtility.DisplayDialog("Success", "All Clear!", "OK");
                if (MultiPlayEditor.window != null)
                {
                    MultiPlayEditor.window.Repaint();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        public static void RemoveFromHub()
        {
            try
            {
                string keyName = @"Software\Unity Technologies\Unity Editor 5.x";
                using RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName, true);
                if (key == null) return;
                string[] values = key.GetValueNames();
                foreach (string k in values)
                {
                    if (k.Contains("RecentlyUsedProjectPaths-") && key.GetValueKind(k) == RegistryValueKind.Binary)
                    {
                        var value = (byte[])key.GetValue(k);
                        var valueAsString = Encoding.ASCII.GetString(value);

                        if (valueAsString.EndsWith("clone"))
                        {
                            key.DeleteValue(k);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(
                    $"Unable to clear system cache due to insufficient User Privileges. Please contact your system administrator. \nDetails: {e.Message}");
            }
        }

        public static void ClearClone(string destPath)
        {
            if (!Directory.Exists(destPath)) return;

            try
            {
                string args = $"/c rd /s /q \"{destPath}\"";
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd",
                        Arguments = args,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    }
                };

                process.Start();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error resetting clone: {e.Message}");
            }
        }

        private static void PurgeAllClones()
        {
            try
            {
                var tmpPath = new DirectoryInfo($"{Settings.ClonesPath}");

                foreach (var dir in tmpPath.EnumerateDirectories("*clone*"))
                {
                    ClearClone(dir.FullName);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error resetting clones: {e.Message}");
            }
        }
    }
}