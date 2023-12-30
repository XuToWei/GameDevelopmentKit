using UnityEngine;

namespace Slate
{

    ///<summary>A stub component to interface with the game MainCamera the same as the rest. We never 'set' these, we only need the getters.</summary>
    public class GameCamera : MonoBehaviour, IDirectableCamera
    {

        private Camera _cam;
        public Camera cam => _cam != null ? _cam : _cam = GetComponent<Camera>();

        public Vector3 position {
            get { return transform.position; }
            set { }
        }

        public Quaternion rotation {
            get { return transform.rotation; }
            set { }
        }

        public float fieldOfView {
            get { return cam.orthographic ? cam.orthographicSize : cam.fieldOfView; }
            set { }
        }

#if SLATE_USE_HDRP
        public float focalDistance {
            get
            {
                if ( DirectorCamera.globalPostVolume != null && DirectorCamera.globalPostVolume.sharedProfile.TryGet<UnityEngine.Rendering.HighDefinition.DepthOfField>(out var dof) ) {
                    return dof.focusDistance.value;
                }
                return DirectorCamera.DEFAULT_FOCAL_DISTANCE;
            }
            set { }
        }

        public float focalLength {
            get { return cam.focalLength; }
            set { }
        }

        public float focalAperture {
            get { return cam.GetComponent<UnityEngine.Rendering.HighDefinition.HDAdditionalCameraData>().physicalParameters.aperture; }
            set { }
        }

#elif SLATE_USE_URP

        public float focalDistance {
            get
            {
                if ( DirectorCamera.globalPostVolume != null && DirectorCamera.globalPostVolume.sharedProfile.TryGet<UnityEngine.Rendering.Universal.DepthOfField>(out var dof) ) {
                    return dof.focusDistance.value;
                }
                return DirectorCamera.DEFAULT_FOCAL_DISTANCE;
            }
            set { }
        }

        public float focalLength {
            get
            {
                if ( DirectorCamera.globalPostVolume != null && DirectorCamera.globalPostVolume.sharedProfile.TryGet<UnityEngine.Rendering.Universal.DepthOfField>(out var dof) ) {
                    return dof.focalLength.value;
                }
                return DirectorCamera.DEFAULT_FOCAL_LENGTH;
            }
            set { }
        }

        public float focalAperture {
            get
            {
                if ( DirectorCamera.globalPostVolume != null && DirectorCamera.globalPostVolume.sharedProfile.TryGet<UnityEngine.Rendering.Universal.DepthOfField>(out var dof) ) {
                    return dof.aperture.value;
                }
                return DirectorCamera.DEFAULT_FOCAL_APERTURE;
            }
            set { }
        }

#elif SLATE_USE_POSTSTACK

        public float focalDistance {
            get { return DirectorCamera.globalPostVolume != null ? DirectorCamera.globalPostVolume.sharedProfile.GetSetting<UnityEngine.Rendering.PostProcessing.DepthOfField>().focusDistance : DirectorCamera.DEFAULT_FOCAL_DISTANCE; }
            set { }
        }

        public float focalLength {
            get { return DirectorCamera.globalPostVolume != null ? DirectorCamera.globalPostVolume.sharedProfile.GetSetting<UnityEngine.Rendering.PostProcessing.DepthOfField>().focalLength : DirectorCamera.DEFAULT_FOCAL_LENGTH; }
            set { }
        }

        public float focalAperture {
            get { return DirectorCamera.globalPostVolume != null ? DirectorCamera.globalPostVolume.sharedProfile.GetSetting<UnityEngine.Rendering.PostProcessing.DepthOfField>().aperture : DirectorCamera.DEFAULT_FOCAL_APERTURE; }
            set { }
        }

#else

        public float focalDistance {
            get { return DirectorCamera.DEFAULT_FOCAL_DISTANCE; }
            set { }
        }

        public float focalLength {
            get { return DirectorCamera.DEFAULT_FOCAL_LENGTH; }
            set { }
        }

        public float focalAperture {
            get { return DirectorCamera.DEFAULT_FOCAL_APERTURE; }
            set { }
        }
#endif

    }
}