using System;
using System.Collections.Generic;

namespace ET.Editor
{
    public static class DockDefine
    {
        public static readonly Type[] Types =
        {
            typeof (CodeCreatorEditor),
            typeof (BuildToolEditor),
            typeof (ServerCommandLineEditor),
        };
    }
}