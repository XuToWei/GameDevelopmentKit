using NUnit.Framework;

namespace ET.Tests
{
    public sealed class SurvivorWorldResetTests
    {
        [Test]
        public void ResetForLobby_PreservesRoomMembershipAndClearsBattleState()
        {
            SurvivorWorldData world = SurvivorWorldFactory.CreateWorld("ROOM42");
            SurvivorPlayerState player = SurvivorWorldFactory.CreatePlayer(1, 42, "Player42");
            player.LastInputSequence = 12;
            player.PositionX = 500;
            player.PositionY = -700;
            player.Hp = 0;
            player.Alive = false;
            player.Level = 4;
            player.Experience = 8;
            player.AutoFireLevel = 2;
            player.SwordWaveRevision = 3;
            player.UnspentSkillPoints = 1;
            world.Players.Add(player.PlayerId, player);
            world.HostPlayerId = player.PlayerId;
            world.NextStateId = 5;
            world.ServerTick = 120;
            world.Phase = SurvivorRoomPhase.Ended;
            world.Monsters.Add(2, SurvivorWorldFactory.CreateMonster(2, 100, 200));
            world.Projectiles.Add(
                3,
                SurvivorWorldFactory.CreateProjectile(
                    3,
                    player.PlayerId,
                    0,
                    0,
                    1,
                    1,
                    10));
            world.Pickups.Add(4, SurvivorWorldFactory.CreatePickup(4, 0, 0, 5));

            SurvivorWorldFactory.ResetForLobby(world);

            Assert.That(world.Phase, Is.EqualTo(SurvivorRoomPhase.Lobby));
            Assert.That(world.ServerTick, Is.Zero);
            Assert.That(world.HostPlayerId, Is.EqualTo(player.PlayerId));
            Assert.That(world.NextStateId, Is.EqualTo(5));
            Assert.That(world.Players.Count, Is.EqualTo(1));
            Assert.That(world.Monsters, Is.Empty);
            Assert.That(world.Projectiles, Is.Empty);
            Assert.That(world.Pickups, Is.Empty);

            SurvivorPlayerState reset = world.Players[player.PlayerId];
            Assert.That(reset.StateId, Is.EqualTo(player.StateId));
            Assert.That(reset.PlayerId, Is.EqualTo(player.PlayerId));
            Assert.That(reset.DisplayName, Is.EqualTo(player.DisplayName));
            Assert.That(reset.LastInputSequence, Is.Zero);
            Assert.That(reset.PositionX, Is.Zero);
            Assert.That(reset.PositionY, Is.Zero);
            Assert.That(reset.Hp, Is.EqualTo(SurvivorDefaults.PlayerMaxHp));
            Assert.That(reset.Alive, Is.True);
            Assert.That(reset.Level, Is.EqualTo(1));
            Assert.That(reset.Experience, Is.Zero);
            Assert.That(reset.AutoFireLevel, Is.Zero);
            Assert.That(reset.SwordWaveCooldown, Is.Zero);
            Assert.That(reset.SwordWaveRevision, Is.Zero);
            Assert.That(reset.UnspentSkillPoints, Is.Zero);
        }
    }
}
