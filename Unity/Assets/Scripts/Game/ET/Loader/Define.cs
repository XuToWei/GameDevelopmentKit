namespace ET
{
    public static class Define
    {
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
        public static bool EnableIL2CPP => true;
#else
        public static bool EnableIL2CPP => false;
#endif

#if UNITY_ET_CODEMODE_CLIENT
        public static CodeMode CodeMode => CodeMode.Client;
#elif UNITY_ET_CODEMODE_SERVER
        public static CodeMode CodeMode => CodeMode.Server;
#elif UNITY_ET_CODEMODE_CLIENTSERVER
        public static CodeMode CodeMode => CodeMode.ClientServer;
#else
        public static CodeMode CodeMode => CodeMode.Client;
#endif
    }
}