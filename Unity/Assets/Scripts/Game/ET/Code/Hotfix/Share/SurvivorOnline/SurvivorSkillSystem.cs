namespace ET
{
    public static class SurvivorSkillSystem
    {
        public static void RefreshSkillChoices(
            this SurvivorWorldComponent self,
            SurvivorPlayerState state)
        {
            state.SkillChoice1 = SurvivorSkillType.AutoFire;
            state.SkillChoice2 = SurvivorSkillType.PowerShot;
            state.SkillChoice3 = SurvivorSkillType.SwiftStep;
            state.SkillChoiceRevision++;
        }

        public static bool TryChooseSkill(
            this SurvivorWorldComponent self,
            long playerId,
            long choiceRevision,
            SurvivorSkillType skillType)
        {
            if (self.Data.Phase != SurvivorRoomPhase.Running ||
                !self.Data.Players.ContainsKey(playerId))
            {
                return false;
            }

            self.Runtime.Player = self.Data.Players[playerId];
            if (self.Runtime.Player.UnspentSkillPoints <= 0 ||
                self.Runtime.Player.SkillChoiceRevision != choiceRevision ||
                !self.Runtime.Player.IsSkillOffered(skillType))
            {
                self.Runtime.Player = null;
                return false;
            }

            switch (skillType)
            {
                case SurvivorSkillType.AutoFire:
                    self.Runtime.Player.AutoFireLevel++;
                    self.Runtime.Player.AutoFireCooldown = 0;
                    break;
                case SurvivorSkillType.PowerShot:
                    self.Runtime.Player.PowerShotLevel++;
                    break;
                case SurvivorSkillType.SwiftStep:
                    self.Runtime.Player.SwiftStepLevel++;
                    break;
                default:
                    self.Runtime.Player = null;
                    return false;
            }

            self.Runtime.Player.UnspentSkillPoints--;
            if (self.Runtime.Player.UnspentSkillPoints > 0)
            {
                self.RefreshSkillChoices(self.Runtime.Player);
            }
            else
            {
                self.Runtime.Player.SkillChoice1 = SurvivorSkillType.None;
                self.Runtime.Player.SkillChoice2 = SurvivorSkillType.None;
                self.Runtime.Player.SkillChoice3 = SurvivorSkillType.None;
                self.Runtime.Player.SkillChoiceRevision++;
            }

            self.Runtime.Player = null;
            return true;
        }

        public static bool IsSkillOffered(
            this SurvivorPlayerState self,
            SurvivorSkillType skillType)
        {
            return skillType != SurvivorSkillType.None &&
                    (self.SkillChoice1 == skillType ||
                     self.SkillChoice2 == skillType ||
                     self.SkillChoice3 == skillType);
        }

        public static int AutoFireIntervalTicks(this SurvivorPlayerState self)
        {
            int upgradedLevels = self.AutoFireLevel > 1 ? self.AutoFireLevel - 1 : 0;
            int intervalReduction = upgradedLevels *
                    SurvivorDefaults.AutoFireIntervalReductionTicks;
            return SurvivorDefaults.AutoFireIntervalTicks -
                    intervalReduction <
                    SurvivorDefaults.MinimumAutoFireIntervalTicks
                    ? SurvivorDefaults.MinimumAutoFireIntervalTicks
                    : SurvivorDefaults.AutoFireIntervalTicks -
                      intervalReduction;
        }

        public static int ProjectileDamage(this SurvivorPlayerState self)
        {
            return SurvivorDefaults.ProjectileDamage +
                    self.PowerShotLevel * SurvivorDefaults.PowerShotDamagePerLevel;
        }

        public static int MovePerTick(this SurvivorPlayerState self)
        {
            return SurvivorDefaults.PlayerMovePerTick +
                    self.SwiftStepLevel * SurvivorDefaults.SwiftStepMovePerTickPerLevel;
        }
    }
}
