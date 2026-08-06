using Cysharp.Threading.Tasks;

namespace ET.Client
{
    public static class SurvivorViewStarter
    {
        public static async UniTask OpenLobby(Scene root)
        {
            await root.GetComponent<UIComponent>()
                    .AddUIFormComponentAsync<UIFormSurvivorLobbyComponent>(UGFUIFormId.SurvivorLobby);
        }
    }
}
