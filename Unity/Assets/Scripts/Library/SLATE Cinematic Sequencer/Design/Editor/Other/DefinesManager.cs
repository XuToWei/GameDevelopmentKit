#if UNITY_EDITOR

using System.Linq;
using UnityEditor;

namespace Slate
{

    ///<summary>Utility for handling player setting defines</summary>
	public static class DefinesManager
    {

        ///<summary>Is define..defined in player settings for current target?</summary>
        public static bool HasDefineForCurrentTargetGroup(string define) {
            var currentTarget = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(currentTarget).Split(';');
            return defines.Contains(define);
        }

        ///<summary>Set define for current target</summary>
        public static void SetDefineActiveForCurrentTargetGroup(string define, bool enable) {
            var currentTarget = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            SetDefineActiveForTargetGroup(currentTarget, define, enable);
        }

        ///<summary>Set define for target</summary>
        public static void SetDefineActiveForTargetGroup(BuildTargetGroup target, string define, bool enable) {
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(target).Split(';').ToList();
            if ( enable == true && !defines.Contains(define) ) {
                defines.Add(define);
            }
            if ( enable == false ) {
                defines.Remove(define);
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(target, string.Join(";", defines));
        }

        ///<summary>Toggle define in player settings for all targets</summary>
        public static void SetDefineActiveForAllTargetGroups(string define, bool enable) {
            foreach ( BuildTargetGroup target in System.Enum.GetValues(typeof(BuildTargetGroup)) ) {
                if ( target == BuildTargetGroup.Unknown ) {
                    continue;
                }

                if ( typeof(BuildTargetGroup).GetField(target.ToString()).IsDefined(typeof(System.ObsoleteAttribute), true) ) {
                    continue;
                }

                SetDefineActiveForTargetGroup(target, define, enable);
            }
        }
    }
}

#endif