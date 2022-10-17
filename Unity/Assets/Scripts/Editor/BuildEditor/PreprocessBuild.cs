using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Compilation;
using UnityEngine;

namespace ET
{
    public class PreprocessBuild: IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Scripts/Codes/ModelView/Unity.ModelView.Codes.asmdef");
            
            
        }
    }
}
