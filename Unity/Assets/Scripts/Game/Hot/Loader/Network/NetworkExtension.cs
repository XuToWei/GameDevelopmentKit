using System.Threading;
using Cysharp.Threading.Tasks;
using UnityGameFramework.Extension;

namespace Game.Hot
{
    public static class NetworkExtension
    {
        /// <summary>
        /// 发送异步消息
        /// </summary>
        /// <param name="networkServiceComponent"></param>
        /// <param name="packet">发送消息</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <typeparam name="T1">发送消息类型</typeparam>
        /// <typeparam name="T2">返回消息类型</typeparam>
        /// <returns>返回消息</returns>
        public static UniTask<T2> SendAsync<T1, T2>(this NetworkServiceComponent networkServiceComponent, T1 packet, CancellationToken cancellationToken = default) where T1 : CSPacketBase where T2 : SCPacketBase
        {
            return networkServiceComponent.SendAsync<T1, T2>(packet);
        }
    }
}