using UnityEngine;

namespace ET.Client
{
    [EnableClass]
    public sealed class SurvivorPresentationPosition
    {
        public const float SnapshotIntervalSeconds =
                SurvivorDefaults.SnapshotTicks / (float)SurvivorDefaults.SimulationTicksPerSecond;

        private Vector3 startPosition;
        private Vector3 targetPosition;
        private float elapsedSeconds;

        public bool IsInitialized { get; private set; }

        public Vector3 CurrentPosition { get; private set; }

        public Vector3 SetTarget(Vector3 position)
        {
            if (!this.IsInitialized)
            {
                this.IsInitialized = true;
                this.startPosition = position;
                this.targetPosition = position;
                this.CurrentPosition = position;
                this.elapsedSeconds = SnapshotIntervalSeconds;
                return this.CurrentPosition;
            }

            if (position == this.targetPosition)
            {
                return this.CurrentPosition;
            }

            this.startPosition = this.CurrentPosition;
            this.targetPosition = position;
            this.elapsedSeconds = 0f;
            return this.CurrentPosition;
        }

        public Vector3 Advance(float deltaTime)
        {
            if (!this.IsInitialized)
            {
                return this.CurrentPosition;
            }

            this.elapsedSeconds = Mathf.Min(
                this.elapsedSeconds + Mathf.Max(0f, deltaTime),
                SnapshotIntervalSeconds);
            this.CurrentPosition = Vector3.Lerp(
                this.startPosition,
                this.targetPosition,
                this.elapsedSeconds / SnapshotIntervalSeconds);
            return this.CurrentPosition;
        }

        public void Reset()
        {
            this.IsInitialized = false;
            this.startPosition = Vector3.zero;
            this.targetPosition = Vector3.zero;
            this.CurrentPosition = Vector3.zero;
            this.elapsedSeconds = 0f;
        }
    }

    /// <summary>
    /// 在权威快照之间按服务端下发的速度外推投射物位置。
    /// 新快照只负责纠偏；投射物生成、命中、伤害和销毁仍由服务端权威状态决定。
    /// </summary>
    [EnableClass]
    public sealed class SurvivorProjectilePrediction
    {
        private const float WorldCoordinateScale = 1000f;
        private const float HardSnapDistance = 1.5f;
        private const float CorrectionSharpness = 20f;
        private const float MaxPredictionDeltaSeconds = 5f / SurvivorDefaults.SimulationTicksPerSecond;

        private Vector3 velocityPerSecond;
        private Vector3 correction;

        public bool IsInitialized { get; private set; }

        public Vector3 CurrentPosition { get; private set; }

        public Vector3 Initialize(int positionX, int positionY, int velocityX, int velocityY)
        {
            this.IsInitialized = true;
            this.CurrentPosition = new Vector3(
                positionX / WorldCoordinateScale,
                positionY / WorldCoordinateScale,
                0f);
            this.velocityPerSecond = new Vector3(
                velocityX * SurvivorDefaults.SimulationTicksPerSecond / WorldCoordinateScale,
                velocityY * SurvivorDefaults.SimulationTicksPerSecond / WorldCoordinateScale,
                0f);
            this.correction = Vector3.zero;
            return this.CurrentPosition;
        }

        public Vector3 Reconcile(int positionX, int positionY, int velocityX, int velocityY)
        {
            if (!this.IsInitialized)
            {
                return this.Initialize(positionX, positionY, velocityX, velocityY);
            }

            this.velocityPerSecond = new Vector3(
                velocityX * SurvivorDefaults.SimulationTicksPerSecond / WorldCoordinateScale,
                velocityY * SurvivorDefaults.SimulationTicksPerSecond / WorldCoordinateScale,
                0f);
            Vector3 authoritativePosition = new Vector3(
                positionX / WorldCoordinateScale,
                positionY / WorldCoordinateScale,
                0f);
            Vector3 error = authoritativePosition - this.CurrentPosition;
            if (error.sqrMagnitude > HardSnapDistance * HardSnapDistance)
            {
                this.CurrentPosition = authoritativePosition;
                this.correction = Vector3.zero;
                return this.CurrentPosition;
            }

            this.correction = error;
            return this.CurrentPosition;
        }

        public Vector3 Advance(float deltaTime)
        {
            if (!this.IsInitialized || deltaTime <= 0f)
            {
                return this.CurrentPosition;
            }

            deltaTime = Mathf.Min(deltaTime, MaxPredictionDeltaSeconds);
            this.CurrentPosition += this.velocityPerSecond * deltaTime;
            float correctionRatio = 1f - Mathf.Exp(-CorrectionSharpness * deltaTime);
            Vector3 appliedCorrection = this.correction * correctionRatio;
            this.CurrentPosition += appliedCorrection;
            this.correction -= appliedCorrection;
            return this.CurrentPosition;
        }

        public void Reset()
        {
            this.IsInitialized = false;
            this.CurrentPosition = Vector3.zero;
            this.velocityPerSecond = Vector3.zero;
            this.correction = Vector3.zero;
        }
    }
}
