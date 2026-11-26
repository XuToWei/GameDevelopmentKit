namespace ET
{
    public static partial class ConstValue
    {
        public const string RouterHttpUrl = "http://127.0.0.1:30300";
        // 本地调试可使用 ws。发布微信小程序或小游戏时，需改用已配置为 socket 合法域名的 wss 地址，例如 wss://game.example.com/ws/。
        public const string RouterWebSocketUrl = "ws://127.0.0.1:30301/ws/";
        public const int SessionTimeoutTime = 30 * 1000;
    }
}
