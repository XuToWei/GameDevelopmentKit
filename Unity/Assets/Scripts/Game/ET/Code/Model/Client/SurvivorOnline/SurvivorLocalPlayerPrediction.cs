using System;
using System.Collections.Generic;

namespace ET.Client
{
    [EnableClass]
    public sealed class SurvivorLocalPlayerPrediction
    {
        private const float WorldCoordinateScale = 1000f;
        private const float HardSnapDistance = 1.5f;
        private const float CorrectionSharpness = 20f;

        public const float InputIntervalSeconds =
                1f / SurvivorDefaults.SimulationTicksPerSecond;

        public List<long> PendingSequences { get; } = new();

        public List<int> PendingMoveXs { get; } = new();

        public List<int> PendingMoveYs { get; } = new();

        public List<int> PendingMovePerTicks { get; } = new();

        public bool IsInitialized { get; private set; }

        public int PredictedPositionX { get; private set; }

        public int PredictedPositionY { get; private set; }

        public float PresentationPositionX { get; private set; }

        public float PresentationPositionY { get; private set; }

        public float InputAccumulator { get; set; }

        public int CurrentMoveX { get; set; }

        public int CurrentMoveY { get; set; }

        public int PendingCount => this.PendingSequences.Count;

        private float correctionX;
        private float correctionY;

        public void Initialize(int authoritativePositionX, int authoritativePositionY)
        {
            this.IsInitialized = true;
            this.PredictedPositionX = authoritativePositionX;
            this.PredictedPositionY = authoritativePositionY;
            this.PresentationPositionX = authoritativePositionX / WorldCoordinateScale;
            this.PresentationPositionY = authoritativePositionY / WorldCoordinateScale;
            this.InputAccumulator = 0f;
            this.correctionX = 0f;
            this.correctionY = 0f;
        }

        public void RecordInput(
            long sequence,
            int moveX,
            int moveY,
            int movePerTick)
        {
            this.PendingSequences.Add(sequence);
            this.PendingMoveXs.Add(moveX);
            this.PendingMoveYs.Add(moveY);
            this.PendingMovePerTicks.Add(movePerTick);
            this.PredictedPositionX +=
                    moveX * movePerTick / SurvivorDefaults.InputScale;
            this.PredictedPositionY +=
                    moveY * movePerTick / SurvivorDefaults.InputScale;
            this.PredictedPositionX = SurvivorDefaults.ClampPlayerPosition(
                this.PredictedPositionX);
            this.PredictedPositionY = SurvivorDefaults.ClampPlayerPosition(
                this.PredictedPositionY);
        }

        public void Reconcile(
            int authoritativePositionX,
            int authoritativePositionY,
            long acknowledgedInputSequence,
            int currentMovePerTick)
        {
            while (this.PendingSequences.Count > 0 &&
                   this.PendingSequences[0] <= acknowledgedInputSequence)
            {
                this.PendingSequences.RemoveAt(0);
                this.PendingMoveXs.RemoveAt(0);
                this.PendingMoveYs.RemoveAt(0);
                this.PendingMovePerTicks.RemoveAt(0);
            }

            this.PredictedPositionX = authoritativePositionX;
            this.PredictedPositionY = authoritativePositionY;
            for (int index = 0; index < this.PendingSequences.Count; index++)
            {
                this.PredictedPositionX +=
                        this.PendingMoveXs[index] * this.PendingMovePerTicks[index] /
                        SurvivorDefaults.InputScale;
                this.PredictedPositionY +=
                        this.PendingMoveYs[index] * this.PendingMovePerTicks[index] /
                        SurvivorDefaults.InputScale;
                this.PredictedPositionX = SurvivorDefaults.ClampPlayerPosition(
                    this.PredictedPositionX);
                this.PredictedPositionY = SurvivorDefaults.ClampPlayerPosition(
                    this.PredictedPositionY);
            }

            float expectedPositionX = this.PredictedPositionX / WorldCoordinateScale;
            float expectedPositionY = this.PredictedPositionY / WorldCoordinateScale;
            float partialInputRatio = this.InputAccumulator / InputIntervalSeconds;
            expectedPositionX +=
                    this.CurrentMoveX * currentMovePerTick * partialInputRatio /
                    (SurvivorDefaults.InputScale * WorldCoordinateScale);
            expectedPositionY +=
                    this.CurrentMoveY * currentMovePerTick * partialInputRatio /
                    (SurvivorDefaults.InputScale * WorldCoordinateScale);
            expectedPositionX = SurvivorDefaults.ClampPlayerPresentationPosition(
                expectedPositionX);
            expectedPositionY = SurvivorDefaults.ClampPlayerPresentationPosition(
                expectedPositionY);
            float errorX = expectedPositionX - this.PresentationPositionX;
            float errorY = expectedPositionY - this.PresentationPositionY;
            if (errorX * errorX + errorY * errorY > HardSnapDistance * HardSnapDistance)
            {
                this.PresentationPositionX = expectedPositionX;
                this.PresentationPositionY = expectedPositionY;
                this.correctionX = 0f;
                this.correctionY = 0f;
                return;
            }

            this.correctionX = errorX;
            this.correctionY = errorY;
        }

        public void AdvancePresentation(float deltaTime, int movePerTick)
        {
            if (!this.IsInitialized || deltaTime <= 0f)
            {
                return;
            }

            this.PresentationPositionX +=
                    this.CurrentMoveX * movePerTick * deltaTime *
                    SurvivorDefaults.SimulationTicksPerSecond /
                    (SurvivorDefaults.InputScale * WorldCoordinateScale);
            this.PresentationPositionY +=
                    this.CurrentMoveY * movePerTick * deltaTime *
                    SurvivorDefaults.SimulationTicksPerSecond /
                    (SurvivorDefaults.InputScale * WorldCoordinateScale);

            float correctionRatio = 1f - (float)Math.Exp(-CorrectionSharpness * deltaTime);
            float appliedCorrectionX = this.correctionX * correctionRatio;
            float appliedCorrectionY = this.correctionY * correctionRatio;
            this.PresentationPositionX += appliedCorrectionX;
            this.PresentationPositionY += appliedCorrectionY;
            this.correctionX -= appliedCorrectionX;
            this.correctionY -= appliedCorrectionY;
            this.PresentationPositionX = SurvivorDefaults.ClampPlayerPresentationPosition(
                this.PresentationPositionX);
            this.PresentationPositionY = SurvivorDefaults.ClampPlayerPresentationPosition(
                this.PresentationPositionY);
        }

        public void Reset()
        {
            this.PendingSequences.Clear();
            this.PendingMoveXs.Clear();
            this.PendingMoveYs.Clear();
            this.PendingMovePerTicks.Clear();
            this.IsInitialized = false;
            this.PredictedPositionX = 0;
            this.PredictedPositionY = 0;
            this.PresentationPositionX = 0f;
            this.PresentationPositionY = 0f;
            this.InputAccumulator = 0f;
            this.CurrentMoveX = 0;
            this.CurrentMoveY = 0;
            this.correctionX = 0f;
            this.correctionY = 0f;
        }
    }
}
