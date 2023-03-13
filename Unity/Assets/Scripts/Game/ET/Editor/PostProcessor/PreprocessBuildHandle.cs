using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace ET.Editor
{
    public class PreprocessBuildHandle : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;
        
        public void OnPreprocessBuild(BuildReport report)
        {
#if (UNITY_IOS || UNITY_ANDROID)&&(UNITY_ET_CODEMODE_SERVER || UNITY_ET_CODEMODE_CLIENTSERVER)
            UnityEngine.Debug.LogError("iPhone and android use server not supported because MongoDB Driver!");
#endif
        }
    }
}
