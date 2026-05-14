//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityGameFramework.Runtime
{
    public sealed partial class DebuggerComponent : GameFrameworkComponent
    {
        private sealed class InputDeviceInformationWindow : ScrollableDebuggerWindowBase
        {
            protected override void OnDrawScrollableWindow()
            {
                GUILayout.Label("<b>Input Device Information</b>");

                var devices = InputSystem.devices;
                DrawItem("Device Count", devices.Count.ToString());

                for (int i = 0; i < devices.Count; i++)
                {
                    var device = devices[i];
                    GUILayout.Label(Utility.Text.Format("<b>Device [{0}]</b>", i));
                    GUILayout.BeginVertical("box");
                    {
                        DrawItem("Name", device.name);
                        DrawItem("Display Name", device.displayName);
                        DrawItem("Device Id", device.deviceId.ToString());
                        DrawItem("Layout", device.layout);
                        DrawItem("Description", device.description.ToString());
                        DrawItem("Enabled", device.enabled.ToString());
                        DrawItem("Added", device.added.ToString());
                        DrawItem("Native", device.native.ToString());
                        DrawItem("Update Before Render", device.updateBeforeRender.ToString());
                    }
                    GUILayout.EndVertical();
                }
            }
        }
    }
}
