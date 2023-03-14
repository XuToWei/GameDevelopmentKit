//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 游戏入口。
    /// </summary>
    public partial class GameEntry : MonoBehaviour
    {
        private void Start()
        {
            InitBuiltinComponents();
            InitExtensionComponents();
            InitGameComponents();
        }

        private void OnApplicationQuit()
        {
            UnityGameFramework.Runtime.GameEntry.Shutdown(UnityGameFramework.Runtime.ShutdownType.Quit);
        }
    }
}
