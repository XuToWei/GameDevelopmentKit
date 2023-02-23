namespace ET
{
    public static class Define
    {
        public const string BuildOutputDir = "./Temp/Bin/Debug";

#if UNITY_EDITOR
        public static bool IsEditor => true;
#else
        public static bool IsEditor => false;
#endif

#if UNITY_HOTFIX
        public static bool EnableHotfix => true;
#else
        public static bool EnableHotfix => false;
#endif

#if UNITY_ET_VIEW && UNITY_EDITOR
        public static bool EnableEditorView => true;
#else
        public static bool EnableEditorView => false;
#endif
        
        public static CodeMode CodeMode
        {
            get;
            private set;
        }

        public static void SetCodeMode(CodeMode codeMode)
        {
            CodeMode = codeMode;
        }

        public static string GetLubanAssetPath(string fileName, bool isJson)
        {
            if (CodeMode == CodeMode.Client)
            {
                if (isJson)
                {
                    return $"Assets/Res/Runtime/ET/Client/Luban/{fileName}.json";
                }

                return $"Assets/Res/Runtime/ET/Client/Luban/{fileName}.byte";
            }
            
            if (isJson)
            {
                return $"Assets/Res/Runtime/ET/ClientServer/Luban/{fileName}.json";
            }

            return $"Assets/Res/Runtime/ET/ClientServer/Luban/{fileName}.byte";
        }
    }
}