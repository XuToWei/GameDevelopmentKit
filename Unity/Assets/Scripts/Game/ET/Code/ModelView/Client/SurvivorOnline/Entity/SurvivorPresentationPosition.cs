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
}
