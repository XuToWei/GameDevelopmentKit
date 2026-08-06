using Game;
using UnityEngine;

namespace ET.Client
{
    [EntitySystemOf(typeof(SurvivorCameraComponent))]
    public static partial class SurvivorCameraComponentSystem
    {
        private const float CameraDistance = 10f;
        private const float CameraOrthographicSize = 10.75f;

        [EntitySystem]
        private static void Awake(this SurvivorCameraComponent self)
        {
            self.Camera = GameEntry.Camera.CurrentSceneCamera;
            if (self.Camera == null)
            {
                return;
            }

            self.OriginalPosition = self.Camera.transform.position;
            self.OriginalRotation = self.Camera.transform.rotation;
            self.OriginalOrthographic = self.Camera.orthographic;
            self.OriginalOrthographicSize = self.Camera.orthographicSize;
            self.OriginalClearFlags = self.Camera.clearFlags;
            self.OriginalBackgroundColor = self.Camera.backgroundColor;

            self.Camera.orthographic = true;
            self.Camera.orthographicSize = CameraOrthographicSize;
            self.Camera.clearFlags = CameraClearFlags.SolidColor;
            self.Camera.backgroundColor = new Color(0.035f, 0.07f, 0.075f, 1f);
            self.Camera.transform.rotation = Quaternion.identity;
            self.Camera.transform.position = new Vector3(0f, 0f, -CameraDistance);
        }

        [EntitySystem]
        private static void Destroy(this SurvivorCameraComponent self)
        {
            if (self.Camera == null)
            {
                return;
            }

            self.Camera.transform.SetPositionAndRotation(
                self.OriginalPosition,
                self.OriginalRotation);
            self.Camera.orthographic = self.OriginalOrthographic;
            self.Camera.orthographicSize = self.OriginalOrthographicSize;
            self.Camera.clearFlags = self.OriginalClearFlags;
            self.Camera.backgroundColor = self.OriginalBackgroundColor;
            self.Camera = null;
        }
    }
}
