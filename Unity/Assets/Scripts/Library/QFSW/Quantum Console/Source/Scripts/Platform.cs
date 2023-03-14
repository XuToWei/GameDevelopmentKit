using System;
using UnityEngine;

namespace QFSW.QC
{
    /// <summary>
    /// Bitwise flag enum for the runtime platform. Setting a platform bit to 0 includes it as a supported platform.
    /// </summary>
    [Flags]
    public enum Platform : long
    {
#pragma warning disable 612,618
        OSXEditor = 1L << RuntimePlatform.OSXEditor,
        OSXPlayer = 1L << RuntimePlatform.OSXPlayer,
        WindowsPlayer = 1L << RuntimePlatform.WindowsPlayer,
        OSXWebPlayer = 1L << 3,
        OSXDashboardPlayer = 1L << 4,
        WindowsWebPlayer = 1L << 5,
        WindowsEditor = 1L << RuntimePlatform.WindowsEditor,
        IPhonePlayer = 1L << RuntimePlatform.IPhonePlayer,
        PS3 = 1L << RuntimePlatform.PS3,
        XBOX360 = 1L << RuntimePlatform.XBOX360,
        Android = 1L << RuntimePlatform.Android,
        NaCl = 1L << RuntimePlatform.NaCl,
        LinuxPlayer = 1L << RuntimePlatform.LinuxPlayer,
        FlashPlayer = 1L << RuntimePlatform.FlashPlayer,
        LinuxEditor = 1L << RuntimePlatform.LinuxEditor,
        WebGLPlayer = 1L << RuntimePlatform.WebGLPlayer,
        MetroPlayerX86 = 1L << RuntimePlatform.MetroPlayerX86,
        WSAPlayerX86 = 1L << RuntimePlatform.WSAPlayerX86,
        MetroPlayerX64 = 1L << RuntimePlatform.MetroPlayerX64,
        WSAPlayerX64 = 1L << RuntimePlatform.WSAPlayerX64,
        MetroPlayerARM = 1L << RuntimePlatform.MetroPlayerARM,
        WSAPlayerARM = 1L << RuntimePlatform.WSAPlayerARM,
        WP8Player = 1L << RuntimePlatform.WP8Player,
        BlackBerryPlayer = 1L << RuntimePlatform.BlackBerryPlayer,
        TizenPlayer = 1L << RuntimePlatform.TizenPlayer,
        PSP2 = 1L << RuntimePlatform.PSP2,
        PS4 = 1L << RuntimePlatform.PS4,
        PSM = 1L << RuntimePlatform.PSM,
        XboxOne = 1L << RuntimePlatform.XboxOne,
        SamsungTVPlayer = 1L << RuntimePlatform.SamsungTVPlayer,
        WiiU = 1L << RuntimePlatform.WiiU,
        tvOS = 1L << RuntimePlatform.tvOS,
        Switch = 1L << RuntimePlatform.Switch,
        Lumin = 1L << RuntimePlatform.Lumin,
#if UNITY_2019_3_OR_NEWER
        Stadia = 1L << RuntimePlatform.Stadia,
#endif
#pragma warning restore 612, 618

        None = 0,
        AllPlatforms = ~0,
        EditorPlatforms = LinuxEditor | OSXEditor | WindowsEditor,
        BuildPlatforms = AllPlatforms ^ EditorPlatforms,
        MobilePlatforms = IPhonePlayer | Android | WP8Player
    }

    public static class PlatformExtensions
    {
        /// <summary>Converts Unity's RuntimePlatform to QC's bitwise Platform.</summary>
        public static Platform ToPlatform(this RuntimePlatform pl)
        {
            int val = (int)pl;
            long bitwise = 1L << val;
            return (Platform)bitwise;
        }
    }
}
