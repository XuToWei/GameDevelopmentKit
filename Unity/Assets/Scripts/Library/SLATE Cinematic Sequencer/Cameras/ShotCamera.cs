using UnityEngine;
using System.Collections;
using System.Linq;

namespace Slate
{

    [AddComponentMenu("SLATE/Shot Camera")]
    [RequireComponent(typeof(Camera))]
    ///<summary>A camera for a shot within a Camera Track. We never render through this. It's only a virtual/preview camera.</summary>
    public class ShotCamera : MonoBehaviour, IDirectableCamera
    {

        public const string DEFAULT_NAME = "Shot Camera";

        [SerializeField]
        private DynamicCameraController _dynamicController = new DynamicCameraController();
        [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("_focalPoint")]
        private float _focalDistance = 0f;
        [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("_focalRange")]
        private float _focalLength = 0f;
        [SerializeField]
        private float _focalAperture = 0f;

        private Camera _cam;
        public Camera cam {
            get { return _cam != null ? _cam : _cam = GetComponent<Camera>(); }
        }

        public Vector3 position {
            get { return transform.position; }
            set { transform.position = value; }
        }

        public Quaternion rotation {
            get { return transform.rotation; }
            set { transform.rotation = value; }
        }

        public Vector3 localPosition {
            get { return transform.localPosition; }
            set { transform.localPosition = value; }
        }

        public Vector3 localEulerAngles {
            get { return transform.GetLocalEulerAngles(); }
            set { transform.SetLocalEulerAngles(value); }
        }

        public float fieldOfView {
            get { return cam.orthographic ? cam.orthographicSize : cam.fieldOfView; }
            set { cam.fieldOfView = value; cam.orthographicSize = value; }
        }

        public float focalDistance {
            get { return _focalDistance; }
            set { _focalDistance = value; }
        }

        public float focalLength {
            get { return _focalLength; }
            set { _focalLength = value; }
        }

        public float focalAperture {
            get { return _focalAperture; }
            set { _focalAperture = value; }
        }

        ///<summary>The DynamicCameraController object</summary>
        public DynamicCameraController dynamicController {
            get { return _dynamicController; }
        }

        ///<summary>Does dynamic controller controls position?</summary>
        public bool dynamicControlledPosition {
            get { return dynamicController != null && dynamicController.controlsPosition; }
        }

        ///<summary>Does dynamic controller controls rotation?</summary>
        public bool dynamicControlledRotation {
            get { return dynamicController != null && dynamicController.controlsRotation; }
        }

        ///<summary>Does dynamic controller controls FOV?</summary>
        public bool dynamicControlledFieldOfView {
            get { return dynamicController != null && dynamicController.controlsFieldOfView; }
        }

        void Awake() {
            cam.enabled = false;
            if ( cam.targetTexture != null ) {
                cam.targetTexture.Release();
                DestroyImmediate(cam.targetTexture);
            }
        }


        ///<summary>Update the dynamic camera dynamic controller without interpolation</summary>
        public void UpdateDynamicControllerHard(IDirectable directable) { dynamicController.UpdateControllerHard(this, directable); }
        ///<summary>Update the dynamic camera dynamic controller with interpolation</summary>
        public void UpdateDynamicControllerSoft(IDirectable directable) { dynamicController.UpdateControllerSoft(this, directable); }

        ///<summary>Set the target Transforms for the dynamic shot controller tranposer and composer functionality</summary>
        public void SetDynamicControllerTargets(Transform target) { SetDynamicControllerTargets(target, target); }
        public void SetDynamicControllerTargets(Transform transposerTarget, Transform composerTarget) {
            dynamicController.transposer.target = transposerTarget;
            dynamicController.composer.target = composerTarget;
        }

        ///<summary>Get a RenderTexture of this camera with specified width and height.</summary>
        public RenderTexture GetRenderTexture(int width, int height) { return GetRenderTexture(width, height, cam.cameraType); }
        ///<summary>Get a RenderTexture of this camera with specified width and height.</summary>
        public RenderTexture GetRenderTexture(int width, int height, CameraType cameraType) {
            // var wasType = cam.cameraType;
            cam.cameraType = cameraType;
            var rt = cam.targetTexture;
            if ( rt == null ) {
                rt = new RenderTexture(width, height, 16);
                rt.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;
            }
            if ( rt.width != width || rt.height != height ) {
                rt.Release();
                DestroyImmediate(rt, true);
                rt = new RenderTexture(width, height, 16);
                rt.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;
            }
            cam.targetTexture = rt;
            cam.Render();
            // cam.cameraType = wasType;
            return rt;
        }

        ///<summary>Create a new ShotCamera with optional parent transform</summary>
        public static ShotCamera Create(Transform targetParent = null) {
            var rootName = "[ CAMERA SHOTS ]";
            GameObject root = null;
            if ( targetParent == null ) {
                root = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().FirstOrDefault(x => x.name == rootName);
                if ( root == null ) {
                    root = new GameObject(rootName);
                }
            } else {
                var child = targetParent.Find(rootName);
                if ( child != null ) {
                    root = child.gameObject;
                } else {
                    root = new GameObject(rootName);
                }
            }
            root.transform.SetParent(targetParent, false);

            var shot = new GameObject(DEFAULT_NAME).AddComponent<ShotCamera>();
            shot.transform.SetParent(root.transform, false);
            shot.cam.nearClipPlane = 0.01f;
            shot.cam.farClipPlane = 1000;

#if UNITY_EDITOR
            if ( UnityEditor.SceneView.lastActiveSceneView != null ) {
                var sc = UnityEditor.SceneView.lastActiveSceneView.camera;
                var is2DMode = UnityEditor.SceneView.lastActiveSceneView.in2DMode;
                shot.position = sc.transform.position;
                shot.rotation = sc.transform.rotation;
                var mainCam = Camera.main;
                if ( mainCam != null ) {
                    shot.cam.orthographic = mainCam.orthographic;
                    shot.fieldOfView = is2DMode ? mainCam.orthographicSize : mainCam.fieldOfView;
                    shot.cam.orthographicSize = mainCam.orthographicSize;
                } else {
                    shot.cam.orthographic = is2DMode;
                    shot.fieldOfView = is2DMode ? sc.orthographicSize : sc.fieldOfView;
                    shot.cam.orthographicSize = sc.orthographicSize;
                }

            } else {
                Debug.Log("Remember that creating a ShotCamera with the Scene View open, creates it at the editor camera position");
            }
#endif

            return shot;
        }

        ///<summary>Shortcut to get a shot by it's name.</summary>
        public static ShotCamera Find(string shotName) {
            return FindObjectsOfType<ShotCamera>().FirstOrDefault(s => s.name == shotName);
        }



        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        void OnValidate() { Validate(); }
        void Reset() {
            Validate();
            cam.nearClipPlane = 0.01f;
        }

        void Validate() {
            DirectorGUI.OnGUIUpdate -= OnDirectorGUI;
            DirectorGUI.OnGUIUpdate += OnDirectorGUI;
            cam.enabled = false;
            cam.cameraType = CameraType.Preview;
#if SLATE_USE_HDRP //CameraType.Preview does not work in HDRP
            cam.cameraType = CameraType.SceneView;
#endif
            cam.backgroundColor = new Color(0.3f, 0.3f, 0.3f);
            //to hide default camera gizmos, specificaly the default frustum which is totaly anoying
            cam.hideFlags = HideFlags.HideInHierarchy | HideFlags.NotEditable;
            if ( cam.targetTexture != null ) {
                cam.targetTexture.Release();
                DestroyImmediate(cam.targetTexture, true);
            }
        }

        //subscribed to gui callback OnValidate
        void OnDirectorGUI() {
            var selectedShotClip = CutsceneUtility.selectedObject as CameraShot;
            if ( selectedShotClip != null && selectedShotClip.targetShot == this && selectedShotClip.RootTimeWithinRange() ) {
                //forward call to dynamic controller
                dynamicController.DoGUI(this, new Rect(0, 0, Screen.width, Screen.height));
            }
        }

        void OnDrawGizmos() {
            var selectedShotClip = CutsceneUtility.selectedObject as CameraShot;
            if ( ( selectedShotClip != null && selectedShotClip.targetShot == this ) || UnityEditor.Selection.activeGameObject == this.gameObject ) {
                //forward call to dynamic controller
                dynamicController.DoGizmos(this);
            }

            Gizmos.DrawIcon(position, "Camera Gizmo");
            var color = Prefs.gizmosColor;
            Gizmos.color = color;

            var hit = new RaycastHit();
            Vector3 hitPos = new Vector3(position.x, 0, position.z);
            if ( Physics.Linecast(position, position - new Vector3(0, 100, 0), out hit) ) {
                hitPos = hit.point;
            }

            var d = Vector3.Distance(hitPos, position);
            Gizmos.DrawCube(hitPos, new Vector3(0.2f, 0.05f, 0.2f));
            if ( position.y > hitPos.y ) {
                Gizmos.DrawCube(hitPos + new Vector3(0, d / 2, 0), new Vector3(0.02f, d, 0.02f));
            } else {
                Gizmos.DrawCube(hitPos - new Vector3(0, d / 2, 0), new Vector3(0.02f, d, 0.02f));
            }


            var selectedInEditor = CutsceneUtility.selectedObject is CameraShot && ( CutsceneUtility.selectedObject as CameraShot ).targetShot == this;
            color.a = selectedInEditor ? 1 : 0.3f;
            Gizmos.color = color;
            Gizmos.matrix = Matrix4x4.TRS(position, rotation, Vector3.one);
            Gizmos.DrawFrustum(new Vector3(0, 0, 0.5f), fieldOfView, 0f, 0.5f, 1);
            Gizmos.color = Color.white;
        }

        //called from editor and clip
        public void OnSceneGUI() {
            //forward call to dynamic controller
            dynamicController.DoSceneGUI(this);
        }

#endif

    }
}