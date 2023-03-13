using UnityEngine;

namespace QFSW.QC.Extras
{
    public static class TimeCommands
    {
        [Command("time-scale", "the scale at which time is passing by.")]
        private static float TimeScale
        {
            get => Time.timeScale;
            set => Time.timeScale = value;
        }
    }
}