using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace Game
{
    public sealed class SceneCameraDisableEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(SceneCameraDisableEventArgs).GetHashCode();

        public override int Id => EventId;

        public Camera SceneCamera { get; private set; }

        public override void Clear()
        {
            SceneCamera = null;
        }

        public static SceneCameraDisableEventArgs Create(Camera sceneCamera)
        {
            SceneCameraDisableEventArgs eventArgs = ReferencePool.Acquire<SceneCameraDisableEventArgs>();
            eventArgs.SceneCamera = sceneCamera;
            return eventArgs;
        }
    }
}