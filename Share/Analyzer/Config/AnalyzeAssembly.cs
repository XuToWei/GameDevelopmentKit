namespace ET.Analyzer
{
    public static class AnalyzeAssembly
    {
        public const string DotNet_Core = "Core";
        public const string DotNet_Model = "Model";
        public const string DotNet_Hotfix = "Hotfix";

        public const string Unity_ET_Core = "ET.Core";
        public const string Unity_ET_Code_Model = "Game.ET.Code.Model";
        public const string Unity_ET_Code_Hotfix = "Game.ET.Code.Hotfix";
        public const string Unity_ET_Code_ModelView = "Game.ET.Code.ModelView";
        public const string Unity_ET_Code_HotfixView = "Game.ET.Code.HotfixView";

        public static readonly string[] AllHotfix =
        [
            DotNet_Hotfix,
            Unity_ET_Code_Hotfix, Unity_ET_Code_HotfixView
        ];

        public static readonly string[] AllModel =
        [
            DotNet_Model,
            Unity_ET_Code_Model, Unity_ET_Code_ModelView
        ];

        public static readonly string[] AllModelHotfix =
        [
            DotNet_Model, DotNet_Hotfix, 
            Unity_ET_Code_Model, Unity_ET_Code_Hotfix, Unity_ET_Code_ModelView, Unity_ET_Code_HotfixView
        ];
        
        public static readonly string[] All =
        [
            DotNet_Core, DotNet_Model, DotNet_Hotfix, 
            Unity_ET_Core, Unity_ET_Code_Model, Unity_ET_Code_Hotfix, Unity_ET_Code_ModelView, Unity_ET_Code_HotfixView
        ];

        public static readonly string[] ServerModelHotfix =
        [
            DotNet_Model,DotNet_Hotfix
        ];

        public static readonly string[] AllLogicModel =
        [
            DotNet_Model, Unity_ET_Code_Model
        ];
    }
}