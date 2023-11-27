using System.IO;

namespace ET.Analyzer
{
    public static class AnalyzeAssembly
    {
        public const string DotNet_Library = "Library";
        public const string DotNet_Model = "Model";
        public const string DotNet_Hotfix = "Hotfix";

        public const string Unity_ET_Runtime = "ET.Runtime";
        public const string Unity_ET_Code_Model = "Game.ET.Code.Model";
        public const string Unity_ET_Code_Hotfix = "Game.ET.Code.Hotfix";
        public const string Unity_ET_Code_ModelView = "Unity.ModelView";
        public const string Unity_ET_Code_HotfixView = "Unity.HotfixView";

        public static readonly string[] AllHotfix =
        {
            DotNet_Hotfix,
            Unity_ET_Code_Hotfix, Unity_ET_Code_HotfixView,
        };

        public static readonly string[] AllModel =
        {
            DotNet_Model,
            Unity_ET_Code_Model, Unity_ET_Code_ModelView,
        };

        public static readonly string[] AllModelHotfix =
        {
            DotNet_Model, DotNet_Hotfix, 
            Unity_ET_Code_Model, Unity_ET_Code_Hotfix, Unity_ET_Code_ModelView, Unity_ET_Code_HotfixView,
        };
        
        public static readonly string[] All =
        {
            DotNet_Library, DotNet_Model, DotNet_Hotfix, 
            Unity_ET_Runtime, Unity_ET_Code_Model, Unity_ET_Code_Hotfix, Unity_ET_Code_ModelView, Unity_ET_Code_HotfixView,
        };

        public static readonly string[] ServerModelHotfix =
        {
            DotNet_Model,DotNet_Hotfix,
        };
    }

    public static class UnityCodesPath
    {
        public static readonly string Unity_ET_Code_Model = @"Unity\Assets\Scripts\Game\ET\Code\Model\".Replace('\\',Path.DirectorySeparatorChar);
        public static readonly string Unity_ET_Code_ModelView = @"Unity\Assets\Scripts\Game\ET\Code\ModelView\".Replace('\\',Path.DirectorySeparatorChar);
        public static readonly string Unity_ET_Code_Hotfix = @"Unity\Assets\Scripts\Game\ET\Code\Hotfix\".Replace('\\',Path.DirectorySeparatorChar);
        public static readonly string Unity_ET_Code_HotfixView = @"Unity\Assets\Scripts\Game\ET\Code\HotfixView\".Replace('\\',Path.DirectorySeparatorChar);

        public static readonly string[] AllModelHotfix =
        {
            Unity_ET_Code_Model, Unity_ET_Code_Hotfix, Unity_ET_Code_ModelView, Unity_ET_Code_HotfixView, 
        };
        
        public static readonly string[] AllHotfix =
        {
            Unity_ET_Code_Hotfix, Unity_ET_Code_HotfixView, 
        };

        public static readonly string[] AllModel =
        {
            Unity_ET_Code_Model, Unity_ET_Code_ModelView
        };
    }
}