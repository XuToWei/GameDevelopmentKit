using Cysharp.Threading.Tasks;

namespace ET.Client
{
    public static class SurvivorViewStarter
    {
        public static async UniTask OpenLobby(Scene root)
        {
            if (root.GetComponent<SurvivorClientComponent>() == null)
            {
                root.AddComponent<SurvivorClientComponent>();
            }

            await root.GetComponent<UIComponent>()
                    .AddUIFormComponentAsync<UIFormSurvivorLobbyComponent>(UGFUIFormId.SurvivorLobby);
        }
    }
}
