//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System.Net.Sockets;

namespace GameFramework.Network
{
    internal sealed partial class NetworkManager : GameFrameworkModule, INetworkManager
    {
        /// <summary>
        /// Socket 网络频道基类。
        /// </summary>
        private abstract class SocketNetworkChannelBase : NetworkChannelBase
        {
            protected Socket m_Socket;

            /// <summary>
            /// 初始化 Socket 网络频道基类的新实例。
            /// </summary>
            /// <param name="name">网络频道名称。</param>
            /// <param name="networkChannelHelper">网络频道辅助器。</param>
            public SocketNetworkChannelBase(string name, INetworkChannelHelper networkChannelHelper)
                : base(name, networkChannelHelper)
            {
                m_Socket = null;
            }

            /// <summary>
            /// 获取网络频道所使用的连接对象。
            /// </summary>
            public override object Handle
            {
                get
                {
                    return m_Socket;
                }
            }

            /// <summary>
            /// 获取网络频道所使用的 Socket。
            /// </summary>
            public Socket Socket
            {
                get
                {
                    return m_Socket;
                }
            }

            /// <summary>
            /// 获取网络频道是否有效。
            /// </summary>
            protected override bool Valid
            {
                get
                {
                    return m_Socket != null;
                }
            }

            /// <summary>
            /// 获取是否已连接。
            /// </summary>
            public override bool Connected
            {
                get
                {
                    if (m_Socket != null)
                    {
                        return m_Socket.Connected;
                    }

                    return false;
                }
            }

            /// <summary>
            /// 内部关闭连接。
            /// </summary>
            protected override void InternalClose()
            {
                try
                {
                    m_Socket.Shutdown(SocketShutdown.Both);
                }
                catch
                {
                }
                finally
                {
                    m_Socket.Close();
                    m_Socket = null;
                }
            }
        }
    }
}
