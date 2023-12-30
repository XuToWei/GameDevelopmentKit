#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;


namespace Slate
{

    public static class Commands
    {

        ///----------------------------------------------------------------------------------------------

        [UnityEditor.InitializeOnLoadMethod]
        public static void SetSlateDefine() {
            DefinesManager.SetDefineActiveForCurrentTargetGroup("SLATE", true);
        }

        ///----------------------------------------------------------------------------------------------

        [MenuItem("Tools/SLATE/", false, 500)]
        [MenuItem("Tools/SLATE/Open SLATE", false, 500)]
        public static void OpenDirectorWindow() {
            CutsceneEditor.ShowWindow(null);
        }

        [MenuItem("Tools/SLATE/Website...", false, 500)]
        public static void VisitWebsite() {
            Help.BrowseURL("https://slate.paradoxnotion.com");
        }

        ///----------------------------------------------------------------------------------------------

        [MenuItem("Tools/SLATE/Create/New Cutscene", false, 500)]
        public static Cutscene CreateCutscene() {
            var cutscene = Cutscene.Create();
            CutsceneEditor.ShowWindow(cutscene);
            Selection.activeObject = cutscene;
            return cutscene;
        }

        [MenuItem("Tools/SLATE/Create/Shot Camera", false, 500)]
        public static ShotCamera CreateShot() {
            var shot = ShotCamera.Create();
            Selection.activeObject = shot;
            return shot;
        }

        [MenuItem("Tools/SLATE/Create/Bezier Path", false, 500)]
        public static Path CreateBezierPath() {
            var path = BezierPath.Create();
            Selection.activeObject = path;
            return path;
        }

        [MenuItem("Tools/SLATE/Create/", false, 500)]
        [MenuItem("Tools/SLATE/Create/Cutscene Starter", false, 500)]
        public static GameObject CreateCutsceneStartPlayer() {
            var go = PlayCutsceneOnStart.Create();
            Selection.activeObject = go;
            return go.gameObject;
        }

        [MenuItem("Tools/SLATE/Create/Cutscene Zone Trigger", false, 500)]
        public static GameObject CreateCutsceneTriggerPlayer() {
            var go = PlayCutsceneOnTrigger.Create();
            Selection.activeObject = go;
            return go.gameObject;
        }

        [MenuItem("Tools/SLATE/Create/Cutscene Click Trigger", false, 500)]
        public static GameObject CreateCutsceneClickPlayer() {
            var go = PlayCutsceneOnClick.Create();
            Selection.activeObject = go;
            return go.gameObject;
        }

        [MenuItem("Tools/SLATE/Create/Cutscenes Sequence Player", false, 500)]
        public static GameObject CreateCutscenesSequencePlayer() {
            var go = CutsceneSequencePlayer.Create();
            Selection.activeObject = go;
            return go.gameObject;
        }

        /*
#if !NO_UTJ
                [MenuItem("Tools/SLATE/Create/Import Alembic File", false, 500)]
                public static void ImportAlembicDialog(){
                    UTJ.Alembic.AlembicManualImporterEditor.ShowWindow();
                }
#endif
        */

    }
}

#endif