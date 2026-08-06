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
    }
}
