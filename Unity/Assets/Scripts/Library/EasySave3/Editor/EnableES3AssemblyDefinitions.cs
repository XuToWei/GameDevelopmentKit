using UnityEditor;
using System.IO;

public class EnableES3AssemblyDefinitions : Editor
{
    [MenuItem("Tools/Easy Save 3/Enable Assembly Definition Files", false, 150)]
    public static void EnableAsmDef()
    {
        var pathToEasySaveFolder = ES3Settings.PathToEasySaveFolder();
        File.Delete(pathToEasySaveFolder + "Editor/EasySave3.asmdef.disabled.meta");
        File.Delete(pathToEasySaveFolder + "Editor/EasySave3Editor.asmdef.disabled.meta");
        File.Move(pathToEasySaveFolder + "Editor/EasySave3Editor.asmdef.disabled", pathToEasySaveFolder + "Editor/EasySave3Editor.asmdef");
        File.Move(pathToEasySaveFolder + "Editor/EasySave3.asmdef.disabled", pathToEasySaveFolder + "EasySave3.asmdef");
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        EditorUtility.DisplayDialog("Assembly definition files installed", "Assembly definition files for Easy Save 3 installed.\n\nYou may need to go to 'Assets > Reimport' to apply the changes.", "Done");
    }

    [MenuItem("Tools/Easy Save 3/Enable Assembly Definition Files", true, 150)]
    public static bool CanEnableAsmDef()
    {
        return !File.Exists(ES3Settings.PathToEasySaveFolder() + "EasySave3.asmdef");
    }
}
