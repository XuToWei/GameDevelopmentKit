using ET.Client;
using NUnit.Framework;
using UnityEngine;

namespace ET.Tests
{
    public sealed class SurvivorPresentationPositionTests
    {
        [Test]
        public void TenHertzSnapshots_AreSpreadAcrossRenderFrames()
        {
            SurvivorPresentationPosition presentation = new SurvivorPresentationPosition();
            presentation.SetTarget(Vector3.zero);

            const int renderFrames = 60;
            const int framesPerSnapshot = 6;
            const float snapshotMovement = 0.36f;
            const float renderDeltaTime = 1f / 60f;
            Vector3 previous = presentation.CurrentPosition;
            int stationaryFrames = 0;
            float maximumFrameMovement = 0f;

            for (int frame = 1; frame <= renderFrames; frame++)
            {
                if (frame % framesPerSnapshot == 0)
                {
                    presentation.SetTarget(
                        new Vector3(frame / framesPerSnapshot * snapshotMovement, 0f, 0f));
                }

                Vector3 current = presentation.Advance(renderDeltaTime);
                float frameMovement = Vector3.Distance(previous, current);
                if (frameMovement < 0.0001f)
                {
                    stationaryFrames++;
                }

                maximumFrameMovement = Mathf.Max(maximumFrameMovement, frameMovement);
                previous = current;
            }

            Assert.That(stationaryFrames, Is.LessThanOrEqualTo(framesPerSnapshot - 1));
            Assert.That(maximumFrameMovement, Is.LessThan(0.07f));
        }

        [Test]
        public void NewSnapshot_KeepsCurrentPositionAndReachesTargetInOneInterval()
        {
            SurvivorPresentationPosition presentation = new SurvivorPresentationPosition();
            presentation.SetTarget(Vector3.zero);
            Vector3 beforeRetarget = presentation.SetTarget(Vector3.right);

            Assert.That(beforeRetarget, Is.EqualTo(Vector3.zero));
            Assert.That(
                presentation.Advance(SurvivorPresentationPosition.SnapshotIntervalSeconds),
                Is.EqualTo(Vector3.right));
        }

        [Test]
        public void ProjectilePrediction_AdvancesAtAuthoritativeVelocity()
        {
            SurvivorProjectilePrediction prediction = new SurvivorProjectilePrediction();
            prediction.Initialize(
                0,
                0,
                SurvivorDefaults.ProjectileMovePerTick,
                SurvivorDefaults.ProjectileMovePerTick / 2);

            Vector3 position = prediction.Advance(1f / SurvivorDefaults.SimulationTicksPerSecond);

            Assert.That(
                position.x,
                Is.EqualTo(SurvivorDefaults.ProjectileMovePerTick / 1000f).Within(0.0001f));
            Assert.That(
                position.y,
                Is.EqualTo(SurvivorDefaults.ProjectileMovePerTick / 2000f).Within(0.0001f));
        }

        [Test]
        public void ProjectilePrediction_ReconcileSmoothsSmallError()
        {
            SurvivorProjectilePrediction prediction = new SurvivorProjectilePrediction();
            prediction.Initialize(0, 0, 0, 0);

            Vector3 reconciled = prediction.Reconcile(1000, 0, 0, 0);
            Vector3 corrected = prediction.Advance(0.05f);

            Assert.That(reconciled, Is.EqualTo(Vector3.zero));
            Assert.That(corrected.x, Is.GreaterThan(0f));
            Assert.That(corrected.x, Is.LessThan(1f));
        }

        [Test]
        public void ProjectilePrediction_ReconcileSnapsLargeError()
        {
            SurvivorProjectilePrediction prediction = new SurvivorProjectilePrediction();
            prediction.Initialize(0, 0, 0, 0);

            Vector3 reconciled = prediction.Reconcile(2000, 0, 0, 0);

            Assert.That(reconciled.x, Is.EqualTo(2f).Within(0.0001f));
            Assert.That(reconciled.y, Is.Zero);
        }

        [Test]
        public void ProjectilePrediction_ResetClearsState()
        {
            SurvivorProjectilePrediction prediction = new SurvivorProjectilePrediction();
            prediction.Initialize(1000, 2000, SurvivorDefaults.ProjectileMovePerTick, 0);
            prediction.Advance(0.1f);

            prediction.Reset();

            Assert.That(prediction.IsInitialized, Is.False);
            Assert.That(prediction.CurrentPosition, Is.EqualTo(Vector3.zero));
        }
    }
}
