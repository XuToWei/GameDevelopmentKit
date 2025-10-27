using Cysharp.Threading.Tasks;

namespace ET.Client
{
    public static class LoginHelper
    {
        public static async UniTask Login(Scene root, string account, string password)
        {
            root.RemoveComponent<ClientSenderComponent>();
            ClientSenderComponent clientSenderComponent = root.AddComponent<ClientSenderComponent>();
            long playerId = await clientSenderComponent.LoginAsync(account, password);

            root.GetComponent<PlayerComponent>().MyId = playerId;
            Log.Debug("XXXXXXXX111111111");
            await EventSystem.Instance.PublishAsync(root, new LoginFinish());
        }
    }
}