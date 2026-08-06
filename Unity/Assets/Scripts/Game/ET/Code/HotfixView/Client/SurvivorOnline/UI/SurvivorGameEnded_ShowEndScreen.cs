using Cysharp.Threading.Tasks;
using Game;

namespace ET.Client
{
    [Event(SceneType.SurvivorView)]
    public sealed class SurvivorGameEnded_ShowEndScreen: AEvent<Scene, SurvivorGameEnded>
    {
        protected override async UniTask Run(Scene scene, SurvivorGameEnded args)
        {
            UIComponent uiComponent = scene.GetComponent<UIComponent>();
            if (uiComponent.GetComponent<UIFormSurvivorSkillChoiceComponent>() != null)
            {
                uiComponent.RemoveComponent<UIFormSurvivorSkillChoiceComponent>();
            }

            if (uiComponent.GetComponent<UIFormSurvivorHudComponent>() != null)
            {
                uiComponent.RemoveComponent<UIFormSurvivorHudComponent>();
            }

            if (uiComponent.GetComponent<UIFormSurvivorGameOverComponent>() == null)
            {
                await uiComponent.AddUIFormComponentAsync<UIFormSurvivorGameOverComponent>(
                    UGFUIFormId.SurvivorGameOver);
            }
        }
    }
}
