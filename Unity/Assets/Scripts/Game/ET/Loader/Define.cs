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

#if ENABLE_IL2CPP
        public static bool EnableIL2CPP = true;
#else
        public static bool EnableIL2CPP = false;
#endif
        public static CodeMode CodeMode { get; internal set; }
    }
}