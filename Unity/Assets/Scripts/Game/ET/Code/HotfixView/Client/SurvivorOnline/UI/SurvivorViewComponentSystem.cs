using Cysharp.Threading.Tasks;
using Game;

namespace ET.Client
{
    [EntitySystemOf(typeof(SurvivorViewComponent))]
    [ETReactiveSystem]
    public static partial class SurvivorViewComponentSystem
    {
        private const string BattleSceneName = "SurvivorBattle";

        [EntitySystem]
        private static void Awake(this SurvivorViewComponent self)
        {
            self.Client = self.GetParent<SurvivorClientComponent>();
        }

        [EntitySystem]
        private static void Update(this SurvivorViewComponent self)
        {
            self.ObserveChanges();
        }

        [EntitySystem]
        private static void Destroy(this SurvivorViewComponent self)
        {
            self.ClearReactive();
            self.Client = null;
            self.Switching = false;
        }

        [ETReactiveBind(nameof(SurvivorViewComponent.Phase))]
        private static void OnPhaseChanged(this SurvivorViewComponent self, SurvivorRoomPhase phase)
        {
            self.RequestViewState();
        }

        [ETReactiveBind(nameof(SurvivorViewComponent.SkillChoiceAvailable))]
        private static void OnSkillChoiceAvailableChanged(this SurvivorViewComponent self, bool skillChoiceAvailable)
        {
            self.RequestViewState();
        }

        private static void RequestViewState(this SurvivorViewComponent self)
        {
            if (self.Switching)
            {
                return;
            }

            ApplyViewStateAsync(self).Forget();
        }

        /// <summary>
        /// 收敛式切换：每轮按观察到的值应用一次，await 结束后重新读取期望状态，
        /// 不一致就再来一轮。因此不需要事件载荷回传 Revision 做二次校验。
        /// </summary>
        private static async UniTaskVoid ApplyViewStateAsync(SurvivorViewComponent self)
        {
            EntityRef<SurvivorViewComponent> selfRef = self;
            self.Switching = true;
            try
            {
                while (true)
                {
                    SurvivorRoomPhase phase = self.Phase;
                    bool skillChoiceAvailable = self.SkillChoiceAvailable;
                    await self.ApplyPhaseView(phase);
                    self = selfRef;
                    if (self == null)
                    {
                        return;
                    }

                    await self.ApplySkillChoiceView(skillChoiceAvailable);
                    self = selfRef;
                    if (self == null)
                    {
                        return;
                    }

                    if (phase == self.Phase && skillChoiceAvailable == self.SkillChoiceAvailable)
                    {
                        return;
                    }
                }
            }
            finally
            {
                SurvivorViewComponent current = selfRef;
                if (current != null)
                {
                    current.Switching = false;
                }
            }
        }

        private static async UniTask ApplyPhaseView(this SurvivorViewComponent self, SurvivorRoomPhase phase)
        {
            if (phase == SurvivorRoomPhase.Running)
            {
                await self.ApplyRunningView();
                return;
            }

            if (phase == SurvivorRoomPhase.Ended)
            {
                await self.ApplyEndedView();
                return;
            }

            await self.ApplyLobbyView();
        }

        private static async UniTask ApplyRunningView(this SurvivorViewComponent self)
        {
            EntityRef<SurvivorViewComponent> selfRef = self;
            Scene root = self.Root();
            string battleSceneAsset = AssetUtility.GetSceneAsset(BattleSceneName);
            UGFComponent ugfComponent = root.GetComponent<UGFComponent>();
            if (!ugfComponent.SceneIsLoaded(battleSceneAsset) && !ugfComponent.SceneIsLoading(battleSceneAsset))
            {
                await ugfComponent.LoadSceneAsync(battleSceneAsset);
                self = selfRef;
                if (self == null)
                {
                    return;
                }
            }

            if (self.Client.GetComponent<SurvivorCameraComponent>() == null)
            {
                self.Client.AddComponent<SurvivorCameraComponent>();
            }

            UIComponent uiComponent = root.GetComponent<UIComponent>();
            RemoveUIForm<UIFormSurvivorGameOverComponent>(uiComponent);
            if (uiComponent.GetComponent<UIFormSurvivorHudComponent>() == null)
            {
                await uiComponent.AddUIFormComponentAsync<UIFormSurvivorHudComponent>(UGFUIFormId.SurvivorHud);
                self = selfRef;
                if (self == null)
                {
                    return;
                }
            }

            RemoveUIForm<UIFormSurvivorLobbyComponent>(uiComponent);
        }

        private static async UniTask ApplyEndedView(this SurvivorViewComponent self)
        {
            UIComponent uiComponent = self.Root().GetComponent<UIComponent>();
            RemoveUIForm<UIFormSurvivorSkillChoiceComponent>(uiComponent);
            RemoveUIForm<UIFormSurvivorHudComponent>(uiComponent);
            if (uiComponent.GetComponent<UIFormSurvivorGameOverComponent>() == null)
            {
                await uiComponent.AddUIFormComponentAsync<UIFormSurvivorGameOverComponent>(UGFUIFormId.SurvivorGameOver);
            }
        }

        private static async UniTask ApplyLobbyView(this SurvivorViewComponent self)
        {
            UIComponent uiComponent = self.Root().GetComponent<UIComponent>();
            RemoveUIForm<UIFormSurvivorSkillChoiceComponent>(uiComponent);
            RemoveUIForm<UIFormSurvivorHudComponent>(uiComponent);
            RemoveUIForm<UIFormSurvivorGameOverComponent>(uiComponent);
            if (uiComponent.GetComponent<UIFormSurvivorLobbyComponent>() == null)
            {
                await uiComponent.AddUIFormComponentAsync<UIFormSurvivorLobbyComponent>(UGFUIFormId.SurvivorLobby);
            }
        }

        private static async UniTask ApplySkillChoiceView(this SurvivorViewComponent self, bool skillChoiceAvailable)
        {
            UIComponent uiComponent = self.Root().GetComponent<UIComponent>();
            if (!skillChoiceAvailable)
            {
                RemoveUIForm<UIFormSurvivorSkillChoiceComponent>(uiComponent);
                return;
            }

            if (uiComponent.GetComponent<UIFormSurvivorSkillChoiceComponent>() == null)
            {
                await uiComponent.AddUIFormComponentAsync<UIFormSurvivorSkillChoiceComponent>(UGFUIFormId.SurvivorSkillChoice);
            }
        }

        private static void RemoveUIForm<T>(UIComponent uiComponent) where T : Entity
        {
            if (uiComponent.GetComponent<T>() != null)
            {
                uiComponent.RemoveComponent<T>();
            }
        }
    }
}
