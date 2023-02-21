namespace ET.Analyzer
{
    public static class AnalyzeAssembly
    {
        private const string DotNet_Library = "DotNet.Library";
        private const string DotNet_Model = "DotNet.Model";
        private const string DotNet_Hotfix = "DotNet.Hotfix";

        private const string Unity_ET_Runtime = "ET.Runtime";
        private const string Unity_ET_Loader = "Code.ET.Loader";
        private const string Unity_ET_Code_Model = "Code.ET.Code.Model";
        private const string Unity_ET_Code_Hotfix = "Code.ET.Code.Hotfix";
        private const string Unity_ET_Code_ModelView = "Code.ET.Code.ModelView";
        private const string Unity_ET_Code_HotfixView = "Code.ET.Code.HotfixView";

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
    }
}