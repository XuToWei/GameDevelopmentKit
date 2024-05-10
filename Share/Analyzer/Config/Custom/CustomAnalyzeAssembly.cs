namespace ET.Analyzer.Custom
{
    public class CustomAnalyzeAssembly
    {
        private const string Unity_Game = "Game";
        private const string Unity_Game_Editor = "Game.Editor";
        private const string Unity_Game_ET_Editor = "Game.ET.Editor";
        private const string Unity_Game_Hot_Code = "Game.Hot.Code";
        private const string Unity_Game_Hot_Code_Editor = "Game.Hot.Code.Editor";
        private const string Unity_Game_Hot_Editor = "Game.Hot.Editor";
        private const string Unity_Game_Hot_Loader = "Game.Hot.Loader";

        public static readonly string[] All =
        [
            AnalyzeAssembly.DotNet_Core, AnalyzeAssembly.DotNet_Model, AnalyzeAssembly.DotNet_Hotfix, 
            AnalyzeAssembly.Unity_ET_Core, AnalyzeAssembly.Unity_ET_Code_Model, AnalyzeAssembly.Unity_ET_Code_Hotfix, AnalyzeAssembly.Unity_ET_Code_ModelView, AnalyzeAssembly.Unity_ET_Code_HotfixView,
            Unity_Game, Unity_Game_Editor,
            Unity_Game_ET_Editor,
            Unity_Game_Hot_Code, Unity_Game_Hot_Code_Editor, Unity_Game_Hot_Editor, Unity_Game_Hot_Loader
        ];
    }
}