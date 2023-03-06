using System;

namespace Baracuda.PreprocessorDefinitionFiles.Scripts.Utilities
{
    [Flags]
    internal enum FlagsBuildTargetGroup
    {
        Unknown = 0,
        Standalone = 1,
        IOS = 2,
        Android = 4,
        WebGL = 8,
        PS4 = 16, 
        XboxOne = 32, 
        Tvos = 64,
#if ! UNITY_2019_3_OR_NEWER
        Facebook = 128,
#endif
        Switch = 256, 
        Lumin = 512, 
        Stadia = 1024,
        CloudRendering = 2048, 
        GameCoreXboxSeries = 4096, 
        GameCoreXboxOne = 8192, 
        PS5 = 16384,
    }
}