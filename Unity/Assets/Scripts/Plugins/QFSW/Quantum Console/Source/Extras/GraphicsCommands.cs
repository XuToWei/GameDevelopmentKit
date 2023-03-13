using UnityEngine;

namespace QFSW.QC.Extras
{
    public static class GraphicsCommands
    {
        [Command("max-fps", "the maximum FPS imposed on the application. Set to -1 for unlimited.")]
        private static int MaxFPS
        {
            get => Application.targetFrameRate;
            set => Application.targetFrameRate = value;
        }

        [Command("vsync", "enables or disables vsync for the application.")]
        private static bool VSync
        {
            get => QualitySettings.vSyncCount > 0;
            set => QualitySettings.vSyncCount = value ? 1 : 0;
        }

        [Command("msaa", "Gets or sets the number of msaa samples in use. Valid values are 0, 2, 4 and 8.")]
        private static int MSAA
        {
            get => QualitySettings.antiAliasing;
            set => QualitySettings.antiAliasing = value;
        }
    }
}
