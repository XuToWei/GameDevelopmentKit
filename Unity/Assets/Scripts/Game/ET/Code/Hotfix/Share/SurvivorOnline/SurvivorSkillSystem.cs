namespace ET
{
    public static class SurvivorSkillSystem
    {
        public static void RefreshSkillChoices(this SurvivorWorldComponent self, SurvivorPlayerState state)
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
                !self.Data.Players.TryGetValue(playerId, out SurvivorPlayerState player))
            {
                return false;
            }

            if (player.UnspentSkillPoints <= 0 ||
                player.SkillChoiceRevision != choiceRevision ||
                !player.IsSkillOffered(skillType))
            {
                return false;
            }

            switch (skillType)
            {
                case SurvivorSkillType.AutoFire:
                    player.AutoFireLevel++;
                    player.AutoFireCooldown = 0;
                    break;
                case SurvivorSkillType.PowerShot:
                    player.PowerShotLevel++;
                    break;
                case SurvivorSkillType.SwiftStep:
                    player.SwiftStepLevel++;
                    break;
                default:
                    return false;
            }

            player.UnspentSkillPoints--;
            if (player.UnspentSkillPoints > 0)
            {
                self.RefreshSkillChoices(player);
            }
            else
            {
                player.SkillChoice1 = SurvivorSkillType.None;
                player.SkillChoice2 = SurvivorSkillType.None;
                player.SkillChoice3 = SurvivorSkillType.None;
                player.SkillChoiceRevision++;
            }

            return true;
        }

        public static bool IsSkillOffered(this SurvivorPlayerState self, SurvivorSkillType skillType)
        {
            return skillType != SurvivorSkillType.None &&
                    (self.SkillChoice1 == skillType ||
                     self.SkillChoice2 == skillType ||
                     self.SkillChoice3 == skillType);
        }

        public static int AutoFireIntervalTicks(this SurvivorPlayerState self)
        {
            int upgradedLevels = self.AutoFireLevel > 1 ? self.AutoFireLevel - 1 : 0;
            int interval = SurvivorDefaults.AutoFireIntervalTicks -
                    upgradedLevels * SurvivorDefaults.AutoFireIntervalReductionTicks;
            return interval < SurvivorDefaults.MinimumAutoFireIntervalTicks
                    ? SurvivorDefaults.MinimumAutoFireIntervalTicks
                    : interval;
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
