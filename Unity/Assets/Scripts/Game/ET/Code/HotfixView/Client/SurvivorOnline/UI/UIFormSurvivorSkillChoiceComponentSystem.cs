using System;
using Cysharp.Threading.Tasks;
using Game;

namespace ET.Client
{
    [EntitySystemOf(typeof(UIFormSurvivorSkillChoiceComponent))]
    [ETReactiveSystem]
    public static partial class UIFormSurvivorSkillChoiceComponentSystem
    {
        [EntitySystem]
        private static void Awake(this UIFormSurvivorSkillChoiceComponent self)
        {
            self.Client = self.Root().GetComponent<SurvivorClientComponent>();
        }

        [UGFUIFormSystem]
        private static void UGFUIFormOnOpen(this UIFormSurvivorSkillChoiceComponent self)
        {
            self.Choosing = false;
            self.View.Choice1Button.SetAsync(self.ChooseFirst);
            self.View.Choice2Button.SetAsync(self.ChooseSecond);
            self.View.Choice3Button.SetAsync(self.ChooseThird);
        }

        [UGFUIFormSystem]
        private static void UGFUIFormOnUpdate(this UIFormSurvivorSkillChoiceComponent self, float elapseSeconds, float realElapseSeconds)
        {
            self.ObserveChanges();
        }

        [UGFUIFormSystem]
        private static void UGFUIFormOnClose(this UIFormSurvivorSkillChoiceComponent self, bool isShutdown)
        {
            self.ResetReactive();
            self.Choosing = false;
        }

        [ETReactiveBind(nameof(UIFormSurvivorSkillChoiceComponent.SkillChoiceRevision))]
        private static void OnSkillChoicesChanged(this UIFormSurvivorSkillChoiceComponent self, long skillChoiceRevision)
        {
            SurvivorPlayerState player = self.Client.LocalPlayer;
            if (player == null)
            {
                return;
            }

            self.View.TitleUXText.text = $"升级！选择一项技能（剩余 {player.UnspentSkillPoints}）";
            self.View.Choice1LabelText.text = FormatSkill(player.SkillChoice1, player);
            self.View.Choice2LabelText.text = FormatSkill(player.SkillChoice2, player);
            self.View.Choice3LabelText.text = FormatSkill(player.SkillChoice3, player);
            self.View.StatusUXText.text = "请选择一项技能";
            self.SetChoiceInteractable(true);
        }

        private static UniTask ChooseFirst(this UIFormSurvivorSkillChoiceComponent self)
        {
            return self.Choose(1);
        }

        private static UniTask ChooseSecond(this UIFormSurvivorSkillChoiceComponent self)
        {
            return self.Choose(2);
        }

        private static UniTask ChooseThird(this UIFormSurvivorSkillChoiceComponent self)
        {
            return self.Choose(3);
        }

        private static async UniTask Choose(this UIFormSurvivorSkillChoiceComponent self, int choiceSlot)
        {
            SurvivorPlayerState player = self.Client.LocalPlayer;
            if (self.Choosing || player == null)
            {
                return;
            }

            SurvivorSkillType skillType = OfferedSkill(player, choiceSlot);
            if (skillType == SurvivorSkillType.None)
            {
                return;
            }

            EntityRef<UIFormSurvivorSkillChoiceComponent> selfRef = self;
            self.Choosing = true;
            self.SetChoiceInteractable(false);
            SurvivorRequestResult result = await self.Client.ChooseSkill(skillType, player.SkillChoiceRevision);
            self = selfRef;
            if (self == null)
            {
                return;
            }

            self.Choosing = false;
            if (!result.Success)
            {
                self.View.StatusUXText.text = result.Message;
                self.SetChoiceInteractable(true);
                return;
            }

            self.View.StatusUXText.text = "技能已选择，等待服务器同步";
        }

        private static SurvivorSkillType OfferedSkill(SurvivorPlayerState player, int choiceSlot)
        {
            switch (choiceSlot)
            {
                case 1:
                    return player.SkillChoice1;
                case 2:
                    return player.SkillChoice2;
                default:
                    return player.SkillChoice3;
            }
        }

        private static void SetChoiceInteractable(this UIFormSurvivorSkillChoiceComponent self, bool interactable)
        {
            self.View.Choice1Button.interactable = interactable;
            self.View.Choice2Button.interactable = interactable;
            self.View.Choice3Button.interactable = interactable;
        }

        private static string FormatSkill(SurvivorSkillType skillType, SurvivorPlayerState state)
        {
            switch (skillType)
            {
                case SurvivorSkillType.AutoFire:
                    return $"<size=30><b>自动射击 Lv.{state.AutoFireLevel + 1}</b></size>\n" +
                            $"<size=22>攻击最近敌人，间隔 {NextAutoFireIntervalSeconds(state):0.0} 秒</size>";
                case SurvivorSkillType.PowerShot:
                    return $"<size=30><b>强力弹丸 Lv.{state.PowerShotLevel + 1}</b></size>\n" +
                            $"<size=22>投射物伤害提升至 {NextProjectileDamage(state)}</size>";
                case SurvivorSkillType.SwiftStep:
                    return $"<size=30><b>迅捷步伐 Lv.{state.SwiftStepLevel + 1}</b></size>\n" +
                            $"<size=22>移动速度提升至 {NextMovePerTick(state)}</size>";
                default:
                    return "无可用技能";
            }
        }

        private static float NextAutoFireIntervalSeconds(SurvivorPlayerState state)
        {
            int intervalTicks = SurvivorDefaults.AutoFireIntervalTicks - state.AutoFireLevel * SurvivorDefaults.AutoFireIntervalReductionTicks;
            return Math.Max(SurvivorDefaults.MinimumAutoFireIntervalTicks, intervalTicks) / (float)SurvivorDefaults.SimulationTicksPerSecond;
        }

        private static int NextProjectileDamage(SurvivorPlayerState state)
        {
            return SurvivorDefaults.ProjectileDamage + (state.PowerShotLevel + 1) * SurvivorDefaults.PowerShotDamagePerLevel;
        }

        private static int NextMovePerTick(SurvivorPlayerState state)
        {
            return SurvivorDefaults.PlayerMovePerTick + (state.SwiftStepLevel + 1) * SurvivorDefaults.SwiftStepMovePerTickPerLevel;
        }
    }
}
