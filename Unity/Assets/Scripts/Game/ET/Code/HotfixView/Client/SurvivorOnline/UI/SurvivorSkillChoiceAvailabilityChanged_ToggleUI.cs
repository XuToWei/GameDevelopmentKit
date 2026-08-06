using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [Event(SceneType.SurvivorView)]
    public sealed class SurvivorSkillChoiceAvailabilityChanged_ToggleUI:
            AEvent<Scene, SurvivorSkillChoiceAvailabilityChanged>
    {
        protected override async UniTask Run(
            Scene scene,
            SurvivorSkillChoiceAvailabilityChanged args)
        {
            SurvivorPlayerState state = scene.GetComponent<SurvivorClientComponent>()
                    ?.LocalPlayerState();
            if ((state?.SkillChoiceRevision ?? 0) != args.Revision)
            {
                return;
            }

            UIComponent uiComponent = scene.GetComponent<UIComponent>();
            UIFormSurvivorSkillChoiceComponent skillChoice =
                    uiComponent.GetComponent<UIFormSurvivorSkillChoiceComponent>();
            if (!args.Show)
            {
                if (skillChoice != null)
                {
                    uiComponent.RemoveComponent<UIFormSurvivorSkillChoiceComponent>();
                }

                return;
            }

            if (skillChoice == null)
            {
                skillChoice = await uiComponent
                        .AddUIFormComponentAsync<UIFormSurvivorSkillChoiceComponent>(
                            UGFUIFormId.SurvivorSkillChoice);
            }

        }
    }
}
