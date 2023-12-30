using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace Slate
{

    ///<summary>The topmost IDirectable of a Cutscene, containing CutsceneTracks and targeting a specific GameObject Actor</summary>
    abstract public class CutsceneGroup : MonoBehaviour, IDirectable
    {

        public enum ActorReferenceMode
        {
            UseOriginal = 0,
            UseInstanceHideOriginal = 1
        }

        public enum ActorInitialTransformation
        {
            UseOriginal = 0,
            UseLocal = 1
        }

        ///<summary>Raised when a section has been reached</summary>
        public event System.Action<Section> OnSectionReached;

        [SerializeField, HideInInspector]
        private List<CutsceneTrack> _tracks = new List<CutsceneTrack>();
        [SerializeField, HideInInspector]
        private List<Section> _sections = new List<Section>();
        [SerializeField, HideInInspector]
        private bool _isCollapsed = false;
        [SerializeField, HideInInspector]
        private bool _active = true;
        [SerializeField, HideInInspector]
        private bool _isLocked = false;

        private TransformSnapshot transformSnapshot;
        private ObjectSnapshot objectSnapshot;
        private GameObject refDuplicateActor;

        ///<summary>The name of the group</summary>
        new abstract public string name { get; set; }
        ///<summary>The actor gameobject that is attached to this group</summary>
        abstract public GameObject actor { get; set; }
        ///<summary>The mode of reference for target actor</summary>
        abstract public ActorReferenceMode referenceMode { get; set; }
        ///<summary>The mode of initial transformation for target actor</summary>
        abstract public ActorInitialTransformation initialTransformation { get; set; }
        ///<summary>The local position of the actor in Cutscene Space if set to UseLocal</summary>
        abstract public Vector3 initialLocalPosition { get; set; }
        ///<summary>The local rotation of the actor in Cutscene Space if set to UseLocal</summary>
        abstract public Vector3 initialLocalRotation { get; set; }
        ///<summary>And editor option to display or not the mesh gizmo</summary>
        abstract public bool displayVirtualMeshGizmo { get; set; }

        ///<summary>The child tracks</summary>
        public List<CutsceneTrack> tracks {
            get { return _tracks; }
            set { _tracks = value; }
        }

        ///<summary>The sections defined for this group</summary>
        public List<Section> sections {
            get { return _sections; }
            set { _sections = value; }
        }

        IEnumerable<IDirectable> IDirectable.children { get { return tracks.Cast<IDirectable>(); } }
        float IDirectable.startTime { get { return 0; } }
        float IDirectable.endTime { get { return root.length; } }
        float IDirectable.blendIn { get { return 0f; } }
        float IDirectable.blendOut { get { return 0f; } }
        bool IDirectable.canCrossBlend { get { return false; } }
        IDirectable IDirectable.parent { get { return null; } }
        public IDirector root { get; private set; }

        public bool isActive {
            get { return _active; }
            set
            {
                if ( _active != value ) {
                    _active = value;
                    if ( root != null ) {
                        root.Validate();
                    }
                }
            }
        }

        public bool isCollapsed {
            get { return _isCollapsed; }
            set { _isCollapsed = value; }
        }

        public bool isLocked {
            get { return _isLocked; }
            set { _isLocked = value; }
        }

        public override string ToString() {
            return name;
        }

        //Validate the group and it's tracks
        public void Validate(IDirector root, IDirectable parent) {
            this.root = root;
            var foundTracks = GetComponentsInChildren<CutsceneTrack>(true);
            for ( var i = 0; i < foundTracks.Length; i++ ) {
                if ( !tracks.Contains(foundTracks[i]) ) {
                    tracks.Add(foundTracks[i]);
                }
            }
            if ( tracks.Any(t => t == null) ) { tracks = foundTracks.ToList(); }
            sections = sections.OrderBy(s => s.time).ToList();
        }

        //Get a Section by it's name
        public Section GetSectionByName(string name) {
            if ( name.ToUpper() == "INTRO" ) return new Section("Intro", 0);
            return sections.Find(s => s.name.ToUpper() == name.ToUpper());
        }

        //Get a Section by it's UID
        public Section GetSectionByUID(string UID) {
            return sections.Find(s => s.UID == UID);
        }

        ///<summary>Get a Section whos time is great specified time</summary>
        public Section GetSectionAfter(float time) {
            return sections.FirstOrDefault(s => s.time > time);
        }

        ///<summary>Get a Section whos time is less specified time</summary>
        public Section GetSectionBefore(float time) {
            return sections.LastOrDefault(s => s.time < time);
        }

        //...
        bool IDirectable.Initialize() {

            if ( actor == null ) {
                return false;
            }

#if UNITY_EDITOR //do a fail safe checkup at least in editor
            if ( UnityEditor.EditorUtility.IsPersistent(actor) ) {
                if ( referenceMode == ActorReferenceMode.UseOriginal ) {
                    Debug.LogWarning("A prefab is referenced in an Actor Group, but the Reference Mode is set to Use Original. This is not allowed to avoid prefab corruption. Please select the Actor Group and set Reference Mode to 'Use Instance'");
                    return false;
                }
            }
#endif

            //reset sections looping state
            for ( var i = 0; i < sections.Count; i++ ) {
                sections[i].ResetLoops();
            }

            //instantiate in init
            if ( !root.isReSampleFrame ) {
                switch ( referenceMode ) {
                    case ActorReferenceMode.UseInstanceHideOriginal:
                        TryInstantiateLocalActor();
                        break;
                }
            }

            return true;
        }

        ///<summary>Store undo snapshot</summary>
        void IDirectable.Enter() {

            if ( root.isReSampleFrame ) {
                return;
            }

            switch ( referenceMode ) {
                case ActorReferenceMode.UseOriginal:
                    StoreActorState();
                    break;

                case ActorReferenceMode.UseInstanceHideOriginal:
                    TryInstantiateLocalActor();
                    break;
            }

            switch ( initialTransformation ) {
                case ActorInitialTransformation.UseOriginal:
                    //...
                    break;

                case ActorInitialTransformation.UseLocal:
                    SetActorLocalCoords();
                    break;
            }
        }

        ///<summary>Restore undo snapshot</summary>
        void IDirectable.Reverse() {

            if ( root.isReSampleFrame ) {
                return;
            }

            switch ( referenceMode ) {
                case ActorReferenceMode.UseOriginal:
                    RestoreActorState();
                    break;

                case ActorReferenceMode.UseInstanceHideOriginal:
                    ReleaseLocalActorInstance();
                    break;
            }

            switch ( initialTransformation ) {
                case ActorInitialTransformation.UseOriginal:
                    //...
                    break;

                case ActorInitialTransformation.UseLocal:
                    //...
                    break;
            }
        }

        ///<summary>...</summary>
        void IDirectable.Update(float time, float previousTime) {

            if ( root.isReSampleFrame ) {
                return;
            }

            if ( OnSectionReached != null ) {
                for ( var i = 0; i < sections.Count; i++ ) {
                    var section = sections[i];
                    if ( time >= section.time && previousTime < section.time ) {
                        //raise only if within small delta
                        if ( this.WithinBufferTriggerRange(time, previousTime) ) {
                            OnSectionReached(section);
                        }
                    }

                    //when passed completely, reset looping state of previous section
                    if ( time > section.time && previousTime > section.time ) {
                        if ( i > 0 ) { sections[i - 1].ResetLoops(); }
                    }
                }
            }
        }

        ///<summary>...</summary>
        void IDirectable.Exit() {

            if ( root.isReSampleFrame ) {
                return;
            }

            if ( Application.isPlaying ) {
                switch ( referenceMode ) {
                    case ActorReferenceMode.UseOriginal:
                        //...
                        break;

                    case ActorReferenceMode.UseInstanceHideOriginal:
                        ReleaseLocalActorInstance();
                        break;
                }

                switch ( initialTransformation ) {
                    case ActorInitialTransformation.UseOriginal:
                        //...
                        break;

                    case ActorInitialTransformation.UseLocal:
                        //...
                        break;
                }
            }
        }

        ///<summary>...</summary>
        void IDirectable.ReverseEnter() {

            if ( root.isReSampleFrame ) {
                return;
            }

            if ( Application.isPlaying ) {
                switch ( referenceMode ) {
                    case ActorReferenceMode.UseOriginal:
                        //...
                        break;

                    case ActorReferenceMode.UseInstanceHideOriginal:
                        TryInstantiateLocalActor();
                        break;
                }

                switch ( initialTransformation ) {
                    case ActorInitialTransformation.UseOriginal:
                        //...
                        break;

                    case ActorInitialTransformation.UseLocal:
                        //...
                        break;
                }
            }
        }

        void IDirectable.RootEnabled() { }
        void IDirectable.RootDisabled() { }
        void IDirectable.RootUpdated(float time, float previousTime) { }
        void IDirectable.RootDestroyed() { }


#if UNITY_EDITOR
        ///<summary>Draw the gizmos of virtual actor references</summary>
        void IDirectable.DrawGizmos(bool selected) {

            if ( initialTransformation == ActorInitialTransformation.UseOriginal ) {
                return;
            }

            if ( !selected && !displayVirtualMeshGizmo ) {
                return;
            }

            if ( actor != null && isActive && root.currentTime == 0 ) {
                var t = root.context.transform;
                foreach ( var renderer in actor.GetComponentsInChildren<Renderer>() ) {
                    Mesh mesh = null;
                    var pos = this.TransformPosition(initialLocalPosition, TransformSpace.CutsceneSpace);
                    var rot = this.TransformRotation(initialLocalRotation, TransformSpace.CutsceneSpace);
                    if ( renderer is SkinnedMeshRenderer ) { mesh = ( (SkinnedMeshRenderer)renderer ).sharedMesh; } else {
                        var filter = renderer.GetComponent<MeshFilter>();
                        if ( filter != null ) { mesh = filter.sharedMesh; }
                    }
                    Gizmos.DrawMesh(mesh, pos, rot, renderer.transform.localScale);
                }
                Gizmos.DrawLine(t.position, t.TransformPoint(initialLocalPosition));
            }
        }

        ///<summary>Just the tools to handle the initial virtual actor reference pos and rot</summary>
        void IDirectable.SceneGUI(bool selected) {

            if ( !selected || !isActive ) {
                return;
            }

            if ( initialTransformation == ActorInitialTransformation.UseOriginal ) {
                return;
            }

            if ( actor != null && root.currentTime == 0 ) {
                var _initPos = initialLocalPosition;
                var _initRot = initialLocalRotation;
                SceneGUIUtility.DoVectorPositionHandle(this, TransformSpace.CutsceneSpace, _initRot, ref _initPos);
                SceneGUIUtility.DoVectorRotationHandle(this, TransformSpace.CutsceneSpace, _initPos, ref _initRot);
                initialLocalPosition = _initPos;
                initialLocalRotation = _initRot;
            }
        }
#endif


        //Store snapshots
        void StoreActorState() {
            if ( objectSnapshot == null && transformSnapshot == null ) {
                objectSnapshot = new ObjectSnapshot(actor);
                transformSnapshot = new TransformSnapshot(actor, TransformSnapshot.StoreMode.All);
            }
        }

        //Restore snapshots
        void RestoreActorState() {
            if ( objectSnapshot != null ) {
                objectSnapshot.Restore();
                objectSnapshot = null;
            }
            if ( transformSnapshot != null ) {
                transformSnapshot.Restore();
                transformSnapshot = null;
            }
        }

        //Initialize actor reference mode
        void TryInstantiateLocalActor() {
            if ( refDuplicateActor == null ) {
                var original = actor;
                refDuplicateActor = (GameObject)Instantiate(original);
                SceneManager.MoveGameObjectToScene(refDuplicateActor, root.context.scene);
                refDuplicateActor.transform.SetParent(original.transform.parent, false);
                original.SetActive(false);
                refDuplicateActor.SetActive(true);
            }
        }

        //Release actor reference mode
        void ReleaseLocalActorInstance() {
            if ( refDuplicateActor != null ) {
                DestroyImmediate(refDuplicateActor);
                refDuplicateActor = null;
            }
            actor.SetActive(true);
        }

        void SetActorLocalCoords() {
            actor.transform.position = this.TransformPosition(initialLocalPosition, TransformSpace.CutsceneSpace);
            actor.transform.rotation = this.TransformRotation(initialLocalRotation, TransformSpace.CutsceneSpace);
        }

        ///<summary>Resolve final actor used. Returns ref clone if exists, or original.</summary>
        protected GameObject ResolveActor(GameObject original) {
            return refDuplicateActor != null ? refDuplicateActor : original;
        }

        ///<summary>Can track be added in this group?</summary>
        public bool CanAddTrack(CutsceneTrack track) {
            return track != null ? CanAddTrackOfType(track.GetType()) : false;
        }

        ///<summary>Can track type be added in this group?</summary>
        public bool CanAddTrackOfType(System.Type type) {
            if ( type == null || !type.IsSubclassOf(typeof(CutsceneTrack)) || type.IsAbstract ) {
                return false;
            }
            if ( type.IsDefined(typeof(UniqueElementAttribute), true) && tracks.FirstOrDefault(t => t.GetType() == type) != null ) {
                return false;
            }

            var attachAtt = type.RTGetAttribute<AttachableAttribute>(true);
            if ( attachAtt == null || attachAtt.types == null || !attachAtt.types.Any(t => t == this.GetType()) ) {
                return false;
            }
            return true;
        }

        ///----------------------------------------------------------------------------------------------
#if !UNITY_EDITOR //runtime add/delete track
        ///<summary>Add a new track to this group</summary>
        public T AddTrack<T>(string name = null) where T : CutsceneTrack { return (T)AddTrack(typeof(T), name); }
        public CutsceneTrack AddTrack(System.Type type, string name = null) {
            if ( !CanAddTrackOfType(type) ) { return null; }
            var go = new GameObject(type.Name);
            var newTrack = go.AddComponent(type) as CutsceneTrack;
            newTrack.transform.SetParent(this.transform);
            newTrack.transform.localPosition = Vector3.zero;
            if ( name != null ) { newTrack.name = name; }

            var index = tracks.FirstOrDefault() is CameraTrack ? 1 : 0;
            tracks.Insert(index, newTrack);
            newTrack.PostCreate(this);
            root.Validate();
            return newTrack;
        }

        ///<summary>Delete a track of this group</summary>
        public void DeleteTrack(CutsceneTrack track) {
            tracks.Remove(track);
            DestroyImmediate(track.gameObject);
            root.Validate();
        }
#endif
        ///----------------------------------------------------------------------------------------------


        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        ///<summary>Add a new track to this group</summary>
        public T AddTrack<T>(string name = null) where T : CutsceneTrack { return (T)AddTrack(typeof(T), name); }
        public CutsceneTrack AddTrack(System.Type type, string name = null) {

            if ( !CanAddTrackOfType(type) ) {
                return null;
            }

            var go = new GameObject(type.Name.SplitCamelCase());
            UnityEditor.Undo.RegisterCreatedObjectUndo(go, "New Track");
            var newTrack = UnityEditor.Undo.AddComponent(go, type) as CutsceneTrack;
            UnityEditor.Undo.SetTransformParent(newTrack.transform, this.transform, "New Track");
            UnityEditor.Undo.RegisterCompleteObjectUndo(this, "New Track");
            newTrack.transform.localPosition = Vector3.zero;
            if ( name != null ) { newTrack.name = name; }

            var index = 0;
            //well thats a bit of special case. I really want CameraTrack to stay on top :)
            if ( tracks.FirstOrDefault() is CameraTrack ) { index = 1; }
            tracks.Insert(index, newTrack);

            newTrack.PostCreate(this);
            root.Validate();
            CutsceneUtility.selectedObject = newTrack;
            return newTrack;
        }

        ///<summary>Delete a track of this group</summary>
        public void DeleteTrack(CutsceneTrack track) {
            if ( !track.gameObject.IsSafePrefabDelete() ) {
                UnityEditor.EditorUtility.DisplayDialog("Delete Track", "This track is part of the prefab asset and can not be deleted from within the prefab instance. If you want to delete the track, please open the prefab asset for editing.", "OK");
                return;
            }

            UnityEditor.Undo.RegisterCompleteObjectUndo(this, "Delete Track");
            tracks.Remove(track);
            if ( ReferenceEquals(CutsceneUtility.selectedObject, track) ) {
                CutsceneUtility.selectedObject = null;
            }
            UnityEditor.Undo.DestroyObjectImmediate(track.gameObject);
            root.Validate();
        }

        ///<summary>Duplicate the track in this group</summary>
        public CutsceneTrack DuplicateTrack(CutsceneTrack track) {

            if ( !CanAddTrack(track) ) {
                return null;
            }

            var newTrack = (CutsceneTrack)Instantiate(track);
            UnityEditor.Undo.RegisterCreatedObjectUndo(newTrack.gameObject, "Duplicate Track");
            UnityEditor.Undo.SetTransformParent(newTrack.transform, this.transform, "Duplicate Track");
            UnityEditor.Undo.RegisterCompleteObjectUndo(this, "Duplicate Track");
            newTrack.transform.localPosition = Vector3.zero;
            tracks.Add(newTrack);
            root.Validate();
            CutsceneUtility.selectedObject = newTrack;
            return newTrack;
        }

#endif
    }
}