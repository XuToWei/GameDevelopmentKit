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
        private const int PendingInputCompactionThreshold = 64;

        public const float InputIntervalSeconds = 1f / SurvivorDefaults.SimulationTicksPerSecond;

        private readonly List<SurvivorPendingInput> pendingInputs = new();
        private int firstPendingInputIndex;
        private float correctionX;
        private float correctionY;

        public bool IsInitialized { get; private set; }

        public int PredictedPositionX { get; private set; }

        public int PredictedPositionY { get; private set; }

        public float PresentationPositionX { get; private set; }

        public float PresentationPositionY { get; private set; }

        public float InputAccumulator { get; set; }

        public int CurrentMoveX { get; set; }

        public int CurrentMoveY { get; set; }

        public int PendingCount => this.pendingInputs.Count - this.firstPendingInputIndex;

        public long GetPendingSequence(int index)
        {
            if ((uint)index >= (uint)this.PendingCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return this.pendingInputs[this.firstPendingInputIndex + index].Sequence;
        }

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

        public void RecordInput(long sequence, int moveX, int moveY, int movePerTick)
        {
            this.pendingInputs.Add(new SurvivorPendingInput(sequence, moveX, moveY, movePerTick));
            this.ApplyPredictedInput(moveX, moveY, movePerTick);
        }

        public void Reconcile(
            int authoritativePositionX,
            int authoritativePositionY,
            long acknowledgedInputSequence,
            int currentMovePerTick)
        {
            while (this.firstPendingInputIndex < this.pendingInputs.Count &&
                   this.pendingInputs[this.firstPendingInputIndex].Sequence <= acknowledgedInputSequence)
            {
                this.firstPendingInputIndex++;
            }

            this.CompactPendingInputs();
            this.PredictedPositionX = authoritativePositionX;
            this.PredictedPositionY = authoritativePositionY;
            for (int index = this.firstPendingInputIndex; index < this.pendingInputs.Count; index++)
            {
                SurvivorPendingInput input = this.pendingInputs[index];
                this.ApplyPredictedInput(input.MoveX, input.MoveY, input.MovePerTick);
            }

            float expectedPositionX = this.PredictedPositionX / WorldCoordinateScale;
            float expectedPositionY = this.PredictedPositionY / WorldCoordinateScale;
            float partialInputRatio = this.InputAccumulator / InputIntervalSeconds;
            expectedPositionX += this.CurrentMoveX * currentMovePerTick * partialInputRatio /
                    (SurvivorDefaults.InputScale * WorldCoordinateScale);
            expectedPositionY += this.CurrentMoveY * currentMovePerTick * partialInputRatio /
                    (SurvivorDefaults.InputScale * WorldCoordinateScale);
            expectedPositionX = SurvivorDefaults.ClampPlayerPresentationPosition(expectedPositionX);
            expectedPositionY = SurvivorDefaults.ClampPlayerPresentationPosition(expectedPositionY);
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

            this.PresentationPositionX += this.CurrentMoveX * movePerTick * deltaTime *
                    SurvivorDefaults.SimulationTicksPerSecond /
                    (SurvivorDefaults.InputScale * WorldCoordinateScale);
            this.PresentationPositionY += this.CurrentMoveY * movePerTick * deltaTime *
                    SurvivorDefaults.SimulationTicksPerSecond /
                    (SurvivorDefaults.InputScale * WorldCoordinateScale);

            float correctionRatio = 1f - (float)Math.Exp(-CorrectionSharpness * deltaTime);
            float appliedCorrectionX = this.correctionX * correctionRatio;
            float appliedCorrectionY = this.correctionY * correctionRatio;
            this.PresentationPositionX += appliedCorrectionX;
            this.PresentationPositionY += appliedCorrectionY;
            this.correctionX -= appliedCorrectionX;
            this.correctionY -= appliedCorrectionY;
            this.PresentationPositionX = SurvivorDefaults.ClampPlayerPresentationPosition(this.PresentationPositionX);
            this.PresentationPositionY = SurvivorDefaults.ClampPlayerPresentationPosition(this.PresentationPositionY);
        }

        public void Reset()
        {
            this.pendingInputs.Clear();
            this.firstPendingInputIndex = 0;
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

        private void ApplyPredictedInput(int moveX, int moveY, int movePerTick)
        {
            this.PredictedPositionX += moveX * movePerTick / SurvivorDefaults.InputScale;
            this.PredictedPositionY += moveY * movePerTick / SurvivorDefaults.InputScale;
            this.PredictedPositionX = SurvivorDefaults.ClampPlayerPosition(this.PredictedPositionX);
            this.PredictedPositionY = SurvivorDefaults.ClampPlayerPosition(this.PredictedPositionY);
        }

        private void CompactPendingInputs()
        {
            if (this.firstPendingInputIndex < PendingInputCompactionThreshold ||
                this.firstPendingInputIndex * 2 < this.pendingInputs.Count)
            {
                return;
            }

            this.pendingInputs.RemoveRange(0, this.firstPendingInputIndex);
            this.firstPendingInputIndex = 0;
        }
    }

    public readonly struct SurvivorPendingInput
    {
        public SurvivorPendingInput(long sequence, int moveX, int moveY, int movePerTick)
        {
            this.Sequence = sequence;
            this.MoveX = moveX;
            this.MoveY = moveY;
            this.MovePerTick = movePerTick;
        }

        public long Sequence { get; }

        public int MoveX { get; }

        public int MoveY { get; }

        public int MovePerTick { get; }
    }
}
