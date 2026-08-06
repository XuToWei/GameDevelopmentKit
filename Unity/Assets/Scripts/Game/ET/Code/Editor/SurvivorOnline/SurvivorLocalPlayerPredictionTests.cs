using ET.Client;
using NUnit.Framework;

namespace ET.Tests
{
    public sealed class SurvivorLocalPlayerPredictionTests
    {
        [Test]
        public void Reconcile_ReplaysOnlyUnacknowledgedLocalInputs()
        {
            SurvivorLocalPlayerPrediction prediction = new SurvivorLocalPlayerPrediction();
            prediction.Initialize(0, 0);
            prediction.RecordInput(1, SurvivorDefaults.InputScale, 0, SurvivorDefaults.PlayerMovePerTick);
            prediction.RecordInput(2, SurvivorDefaults.InputScale, 0, SurvivorDefaults.PlayerMovePerTick);
            prediction.RecordInput(3, 0, SurvivorDefaults.InputScale, SurvivorDefaults.PlayerMovePerTick);

            prediction.Reconcile(
                SurvivorDefaults.PlayerMovePerTick,
                0,
                1,
                SurvivorDefaults.PlayerMovePerTick);

            Assert.That(prediction.PendingCount, Is.EqualTo(2));
            Assert.That(prediction.PendingSequences[0], Is.EqualTo(2));
            Assert.That(
                prediction.PredictedPositionX,
                Is.EqualTo(SurvivorDefaults.PlayerMovePerTick * 2));
            Assert.That(
                prediction.PredictedPositionY,
                Is.EqualTo(SurvivorDefaults.PlayerMovePerTick));
        }

        [Test]
        public void Presentation_RespondsImmediatelyAtPredictedMovementSpeed()
        {
            SurvivorLocalPlayerPrediction prediction = new SurvivorLocalPlayerPrediction();
            prediction.Initialize(0, 0);
            prediction.CurrentMoveX = SurvivorDefaults.InputScale;

            prediction.AdvancePresentation(1f / 60f, SurvivorDefaults.PlayerMovePerTick);

            Assert.That(prediction.PresentationPositionX, Is.EqualTo(0.06f).Within(0.0001f));
            Assert.That(prediction.PresentationPositionY, Is.Zero);
        }

        [Test]
        public void Reset_DiscardsOnlyLocalPredictionHistory()
        {
            SurvivorLocalPlayerPrediction prediction = new SurvivorLocalPlayerPrediction();
            prediction.Initialize(100, 200);
            prediction.RecordInput(1, SurvivorDefaults.InputScale, 0, SurvivorDefaults.PlayerMovePerTick);

            prediction.Reset();

            Assert.That(prediction.IsInitialized, Is.False);
            Assert.That(prediction.PendingCount, Is.Zero);
            Assert.That(prediction.PredictedPositionX, Is.Zero);
            Assert.That(prediction.PredictedPositionY, Is.Zero);
        }

        [Test]
        public void Prediction_StopsAtArenaBoundary()
        {
            int limit = SurvivorDefaults.ArenaHalfExtent - SurvivorDefaults.PlayerCollisionRadius;
            SurvivorLocalPlayerPrediction prediction = new SurvivorLocalPlayerPrediction();
            prediction.Initialize(limit - 10, 0);
            prediction.CurrentMoveX = SurvivorDefaults.InputScale;

            prediction.RecordInput(
                1,
                SurvivorDefaults.InputScale,
                0,
                SurvivorDefaults.PlayerMovePerTick);
            prediction.AdvancePresentation(1f, SurvivorDefaults.PlayerMovePerTick);

            Assert.That(prediction.PredictedPositionX, Is.EqualTo(limit));
            Assert.That(
                prediction.PresentationPositionX,
                Is.EqualTo(limit / 1000f).Within(0.0001f));
        }
    }
}
