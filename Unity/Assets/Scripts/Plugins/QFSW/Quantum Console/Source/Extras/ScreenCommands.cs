using System.Collections.Generic;
using UnityEngine;

namespace QFSW.QC.Extras
{
    public static class ScreenCommands
    {
        [Command("fullscreen", "fullscreen state of the application.")]
        private static bool Fullscreen
        {
            get => Screen.fullScreen;
            set => Screen.fullScreen = value;
        }

        [Command("screen-dpi", "dpi of the current device's screen.")]
        private static float DPI => Screen.dpi;

        [Command("screen-orientation", "the orientation of the screen.")]
        [CommandPlatform(Platform.MobilePlatforms)]
        private static ScreenOrientation Orientation
        {
            get => Screen.orientation;
            set => Screen.orientation = value;
        }

        [Command("current-resolution", "current resolution of the application or window.")]
        private static Resolution GetCurrentResolution()
        {
            Resolution resolution = new Resolution
            {
                width = Screen.width,
                height = Screen.height,
                refreshRate = Screen.currentResolution.refreshRate
            };

            return resolution;
        }

        [Command("supported-resolutions", "all resolutions supported by this device in fullscreen mode.")]
        [CommandPlatform(Platform.AllPlatforms ^ Platform.WebGLPlayer)]
        private static IEnumerable<Resolution> GetSupportedResolutions()
        {
            foreach (Resolution resolution in Screen.resolutions)
            {
                yield return resolution;
            }
        }

        [Command("set-resolution")]
        private static void SetResolution(int x, int y)
        {
            SetResolution(x, y, Screen.fullScreen);
        }

        [Command("set-resolution", "sets the resolution of the current application, optionally setting the fullscreen state too.")]
        private static void SetResolution(int x, int y, bool fullscreen)
        {
            Screen.SetResolution(x, y, fullscreen);
        }

        [Command("capture-screenshot")]
        [CommandDescription("Captures a screenshot and saves it to the supplied file path as a PNG.\n" +
                            "If superSize is supplied the screenshot will be captured at a higher than native resolution.")]
        private static void CaptureScreenshot(
            [CommandParameterDescription("The name of the file to save the screenshot in")] string filename,
            [CommandParameterDescription("Factor by which to increase resolution")] int superSize = 1
        )
        {
            ScreenCapture.CaptureScreenshot(filename, superSize);
        }
    }
}
