using UnityEngine;

namespace ET.Client
{
    [ComponentOf(typeof(SurvivorClientComponent))]
    public sealed class SurvivorCameraComponent: Entity, IAwake, IDestroy
    {
        public Camera Camera { get; set; }

        public Vector3 OriginalPosition { get; set; }

        public Quaternion OriginalRotation { get; set; }

        public bool OriginalOrthographic { get; set; }

        public float OriginalOrthographicSize { get; set; }

        public CameraClearFlags OriginalClearFlags { get; set; }

        public Color OriginalBackgroundColor { get; set; }
    }
}
