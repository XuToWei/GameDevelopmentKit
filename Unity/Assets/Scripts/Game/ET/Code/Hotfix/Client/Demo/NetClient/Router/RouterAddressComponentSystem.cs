using System;
using System.Net;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [EntitySystemOf(typeof(RouterAddressComponent))]
    [FriendOf(typeof(RouterAddressComponent))]
    public static partial class RouterAddressComponentSystem
    {
        [EntitySystem]
        private static void Awake(this RouterAddressComponent self, string address)
        {
            self.Address = address;
            Uri uri = new(address);
            if (uri.HostNameType == UriHostNameType.IPv6)
            {
                self.AddressFamily = AddressFamily.InterNetworkV6;
            }
#if !UNITY_WEBGL
            else if (uri.HostNameType == UriHostNameType.Dns)
            {
                self.AddressFamily = NetworkHelper.GetHostAddress(uri.DnsSafeHost).AddressFamily;
            }
#endif
            else
            {
                self.AddressFamily = AddressFamily.InterNetwork;
            }
        }

        public static async UniTask GetAllRouter(this RouterAddressComponent self)
        {
            string url = $"{self.Address}/get_router?v={RandomGenerator.RandUInt32()}";
            Log.Debug($"start get router info: {url}");
            string routerInfo = await HttpClientHelper.Get(url);
            Log.Debug($"recv router info: {routerInfo}");
            HttpGetRouterResponse httpGetRouterResponse = MongoHelper.FromJson<HttpGetRouterResponse>(routerInfo);
            self.Info = httpGetRouterResponse;
            Log.Debug($"start get router info finish: {MongoHelper.ToJson(httpGetRouterResponse)}");
            
            // 打乱顺序
            RandomGenerator.BreakRank(self.Info.Routers);
            
            self.WaitTenMinGetAllRouter().Forget();
        }
        
        // 等10分钟再获取一次
        public static async UniTask WaitTenMinGetAllRouter(this RouterAddressComponent self)
        {
            await self.Root().GetComponent<TimerComponent>().WaitAsync(5 * 60 * 1000);
            if (self.IsDisposed)
            {
                return;
            }
            await self.GetAllRouter();
        }

        public static IPEndPoint GetAddress(this RouterAddressComponent self)
        {
            if (self.Info.Routers.Count == 0)
            {
                return null;
            }

            string address = self.Info.Routers[self.RouterIndex++ % self.Info.Routers.Count];
            Log.Info($"get router address: {self.RouterIndex - 1} {address}");
            IPEndPoint endPoint = NetworkHelper.ToIPEndPoint(address);
            if (self.AddressFamily == AddressFamily.InterNetworkV6)
            {
                endPoint.Address = endPoint.Address.MapToIPv6();
            }
            return endPoint;
        }
        
        public static IPEndPoint GetRealmAddress(this RouterAddressComponent self, string account)
        {
            int v = account.Mode(self.Info.Realms.Count);
            string address = self.Info.Realms[v];
            return NetworkHelper.ToIPEndPoint(address);
        }
    }
}
