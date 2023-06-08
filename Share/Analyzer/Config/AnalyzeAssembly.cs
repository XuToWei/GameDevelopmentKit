namespace ET.Analyzer
{
    public static class AnalyzeAssembly
    {
        private const string DotNet_Library = "Library";
        private const string DotNet_Model = "Model";
        private const string DotNet_Hotfix = "Hotfix";

        private const string Unity_ET_Runtime = "ET.Runtime";
        private const string Unity_ET_Loader = "Game.ET.Loader";
        private const string Unity_ET_Code_Model = "Game.ET.Code.Model";
        private const string Unity_ET_Code_Hotfix = "Game.ET.Code.Hotfix";
        private const string Unity_ET_Code_ModelView = "Game.ET.Code.ModelView";
        private const string Unity_ET_Code_HotfixView = "Game.ET.Code.HotfixView";

        public static readonly string[] AllHotfix =
        {
            DotNet_Hotfix, Unity_ET_Code_Hotfix, Unity_ET_Code_HotfixView
        };

        public static readonly string[] AllModel =
        {
            DotNet_Model, Unity_ET_Code_Model, Unity_ET_Code_ModelView
        };

        public static readonly string[] AllModelHotfix =
        {
            DotNet_Model, DotNet_Hotfix,
            Unity_ET_Code_Model, Unity_ET_Code_ModelView, Unity_ET_Code_Hotfix, Unity_ET_Code_HotfixView
        };
        
        public static readonly string[] All =
        {
            DotNet_Library, DotNet_Model, DotNet_Hotfix,
            Unity_ET_Runtime, Unity_ET_Loader,
            Unity_ET_Code_Model, Unity_ET_Code_ModelView, Unity_ET_Code_Hotfix, Unity_ET_Code_HotfixView
        };

        public static readonly string[] ServerModelHotfix =
        {
            DotNet_Model, DotNet_Hotfix
        };

    }
}