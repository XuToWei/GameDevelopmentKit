using UnityEngine;

namespace QFSW.QC.Extras
{
    public static class ApplicationCommands
    {
        [Command("quit", "Quits the player application")]
        [CommandPlatform(Platform.AllPlatforms ^ (Platform.EditorPlatforms | Platform.WebGLPlayer))]
        private static void Quit()
        {
            Application.Quit();
        }
    }
}