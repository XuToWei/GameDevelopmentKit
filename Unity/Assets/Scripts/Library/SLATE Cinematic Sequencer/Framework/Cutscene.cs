using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Slate
{

    [DisallowMultipleComponent]
    public class Cutscene : MonoBehaviour, IDirector
    {

        public const float VERSION_NUMBER = 2.21f;

        ///<summary>How the cutscene wraps</summary>
        public enum WrapMode
        {
            Once,
            Loop,
            PingPong
        }

        ///<summary>What happens when cutscene stops</summary>
        public enum StopMode
        {
            Skip,
            Rewind,
            Hold,
            SkipRewindNoUndo
        }

        ///<summary>Update modes for cutscene</summary>
        public enum UpdateMode
        {
            Normal,
            AnimatePhysics,
            UnscaledTime,
            Manual
        }

        ///<summary>The direction the cutscene can play. An enum for clarity.</summary>
        public enum PlayingDirection
        {
            Forwards,
            Backwards
        }

        ///<summary>Raised when any cutscene starts playing.</summary>
        public static event System.Action<Cutscene> OnCutsceneStarted;
        ///<summary>Raised when any cutscene stops playing.</summary>
        public static event System.Action<Cutscene> OnCutsceneStopped;

        ///<summary>Raised when a cutscene section has been reached.</summary>
        public event System.Action<Section> OnSectionReached;
        ///<summary>Raised when a global message has been send by this cutscene.</summary>
        public event System.Action<string, object> OnGlobalMessageSend;

        ///<summary>Raised when the cutscene is stopped. Important: Subscribers are cleared once the event is raised.</summary>
        public event System.Action OnStop;

        [SerializeField]
        [Tooltip("When is the cutscene updated.")]
        private UpdateMode _updateMode;
        [SerializeField]
        [Tooltip("How the cutscene wraps (relevant to runtime only)")]
        private WrapMode _defaultWrapMode;
        [SerializeField]
        [Tooltip("What will happen when the cutscene is stopped.")]
        private StopMode _defaultStopMode;
        [SerializeField]
        [Tooltip("The speed at which the cutscene is playing. Can be both positive or negative.")]
        private float _playbackSpeed = 1f;
        [SerializeField]
        [Tooltip("If enabled, the cutscene will start playing when enter play.")]
        private bool _playOnStart = false;
        [SerializeField]
        [Tooltip("If enabled, you can set only some layers to be active for the duration of this cutscene.")]
        private bool _explicitActiveLayers;
        [SerializeField]
        [Tooltip("The layers to enable, all other layers will be disabled. Only affects gameobjects in the scene root.")]
        private LayerMask _activeLayers = -1;

        [SerializeField, HideInInspector]
        public List<CutsceneGroup> groups = new List<CutsceneGroup>();

        [SerializeField, HideInInspector]
        private float _length = 20f;
        [SerializeField, HideInInspector]
        private float _viewTimeMin = 0f;
        [SerializeField, HideInInspector]
        private float _viewTimeMax = 21f;

        [System.NonSerialized]
        private float _currentTime;
        [System.NonSerialized]
        private float _playTimeMin;
        [System.NonSerialized]
        private float _playTimeMax;
        [System.NonSerialized]
        private Transform _groupsRoot;
        [System.NonSerialized]
        private List<IDirectableTimePointer> timePointers;
        [System.NonSerialized]
        private List<IDirectableTimePointer> unsortedStartTimePointers;
        [System.NonSerialized]
        private Dictionary<GameObject, bool> affectedLayerGOStates;
        [System.NonSerialized]
        private static Dictionary<string, Cutscene> allSceneCutscenes = new Dictionary<string, Cutscene>();
        [System.NonSerialized]
        private bool preInitialized;
        [System.NonSerialized]
        private bool _isReSampleFrame;

        ///----------------------------------------------------------------------------------------------

        ///<summary>The root on which groups are added for organization</summary>
        public Transform groupsRoot {
            get
            {
                if ( _groupsRoot == null ) {
                    _groupsRoot = transform.Find("__GroupsRoot__");
                    if ( _groupsRoot == null ) {
                        _groupsRoot = new GameObject("__GroupsRoot__").transform;
                        _groupsRoot.SetParent(this.transform);
                    }

#if UNITY_EDITOR
                    _groupsRoot.gameObject.hideFlags = Prefs.showTransforms ? HideFlags.None : HideFlags.HideInHierarchy;
#endif
                    _groupsRoot.gameObject.SetActive(false); //we dont need it or it's children active at all
                }

                return _groupsRoot;
            }
        }

        ///<summary>When is the cutscene updated</summary>
        public UpdateMode updateMode {
            get { return _updateMode; }
            set { _updateMode = value; }
        }

        ///<summary>How the cutscene wraps when playing by default</summary>
        public WrapMode defaultWrapMode {
            get { return _defaultWrapMode; }
            set { _defaultWrapMode = value; }
        }

        ///<summary>What will happen when the cutscene is stopped by default</summary>
        public StopMode defaultStopMode {
            get { return _defaultStopMode; }
            set { _defaultStopMode = value; }
        }

        ///<summary>Will the cutscene start playing automatically?</summary>
        public bool playOnStart {
            get { return _playOnStart; }
            set { _playOnStart = value; }
        }

        ///<summary>Will active layers option be used?</summary>
        public bool explicitActiveLayers {
            get { return _explicitActiveLayers; }
            set { _explicitActiveLayers = value; }
        }

        ///<summary>The layers that will be active when the cutscene is active. Everything else is disabled for the duration of the cutscene</summary>
        public LayerMask activeLayers {
            get { return _activeLayers; }
            set { _activeLayers = value; }
        }

        ///<summary>The single Director Group of the cutscene</summary>
        public DirectorGroup directorGroup {
            get
            {
                //DirectorGroup should always be in index 0.
                if ( groups.Count > 0 && groups[0] is DirectorGroup ) {
                    return (DirectorGroup)groups[0];
                }
                //but if it's not for whatever reason, find it.
                return groups.Find(g => g is DirectorGroup) as DirectorGroup;
            }
        }

        ///<summary>The single Camera Track of the cutscene</summary>
        public CameraTrack cameraTrack {
            get { return directorGroup.tracks.Find(t => t is CameraTrack) as CameraTrack; }
        }

        ///<summary>The current sample time</summary>
        public float currentTime {
            get { return _currentTime; }
            set { _currentTime = Mathf.Clamp(value, 0, length); }
        }

        ///<summary>Total length</summary>
        public float length {
            get { return _length; }
            set { _length = Mathf.Max(value, 0.1f); }
        }

        ///<summary>Min view time</summary>
        public float viewTimeMin {
            get { return _viewTimeMin; }
            set { if ( viewTimeMax > 0 ) _viewTimeMin = Mathf.Min(value, viewTimeMax - 0.25f); }
        }

        ///<summary>Max view time</summary>
        public float viewTimeMax {
            get { return _viewTimeMax; }
            set { _viewTimeMax = Mathf.Max(value, viewTimeMin + 0.25f, 0); }
        }

        ///<summary>The time the WrapMode is taking effect in runtime. Usually equal to 0.</summary>
        public float playTimeMin {
            get { return _playTimeMin; }
            set { _playTimeMin = Mathf.Clamp(value, 0, playTimeMax); }
        }

        ///<summary>The time the WrapMode is taking effect in runtime. Usually equal to length.</summary>
        public float playTimeMax {
            get { return _playTimeMax; }
            set { _playTimeMax = Mathf.Clamp(value, playTimeMin, length); }
        }

        ///<summary>The speed at which the cutscene is played back. Can be positive or negative. Not applicaple when Sampled manually without calling Play().</summary>
        public float playbackSpeed {
            get { return _playbackSpeed; }
            set { _playbackSpeed = value; }
        }

        ///<summary>The direction the cutscene is playing if at all. Can be changed while cutscene is playing.</summary>
        public PlayingDirection playingDirection { get; set; }
        ///<summary>The WrapMode the cutscene is currently using. Can be changed while cutscene is playing.</summary>
        public WrapMode playingWrapMode { get; set; }
        ///<summary>All directable elements within the cutscene</summary>
        public List<IDirectable> directables { get; private set; }
        ///<summary>Is cutscene playing? (Note: it can be paused and isActive still be true)</summary>
        public bool isActive { get; private set; }
        ///<summary>Is cutscene paused?</summary>
        public bool isPaused { get; private set; }
        ///<summary>The last sampled time</summary>
        public float previousTime { get; private set; }
        ///<summary>internal use. will be true when Sampling due to ReSample call </summary>
        bool IDirector.isReSampleFrame => _isReSampleFrame;
        ///<summary>internal use. check this null for UnityObject comparer</summary>
        GameObject IDirector.context => this != null ? this.gameObject : null;
        ///<summary>The groups</summary>
        IEnumerable<IDirectable> IDirector.children => groups.Cast<IDirectable>();
        ///<summary>The remaining playing time.</summary>
        public float remainingTime {
            get
            {
                if ( playingDirection == PlayingDirection.Forwards ) {
                    return playTimeMax - currentTime;
                }
                if ( playingDirection == PlayingDirection.Backwards ) {
                    return currentTime - playTimeMin;
                }
                return 0;
            }
        }

        ///----------------------------------------------------------------------------------------------

        //UNITY CALLBACK
        protected void Awake() {
            Validate();
            allSceneCutscenes[this.name] = this;
            if ( directorGroup != null ) {
                directorGroup.OnSectionReached += DirectorSectionReached;
            }
        }

        //UNITY CALLBACK
        protected void Start() {
            if ( playOnStart ) { Play(); }
        }

        //UNITY CALLBACK
        protected void OnDestroy() {
            if ( isActive ) { Stop(StopMode.Rewind); }
            StopAllCoroutines();
            isActive = false;
            allSceneCutscenes.Remove(this.name);
            for ( var i = 0; i < directables.Count; i++ ) {
                directables[i].RootDestroyed();
            }
        }

        //UNITY CALLBACK
        protected void LateUpdate() {
            if ( isActive && ( updateMode == UpdateMode.Normal || updateMode == UpdateMode.UnscaledTime ) ) {
                if ( isPaused ) {
                    Sample();
                    return;
                }
                var dt = updateMode == UpdateMode.Normal ? Time.deltaTime : Time.unscaledDeltaTime;
                UpdateCutscene(dt);
            }
        }

        //UNITY CALLBACK
        protected void FixedUpdate() {
            if ( isActive && updateMode == UpdateMode.AnimatePhysics ) {
                if ( isPaused ) {
                    Sample();
                    return;
                }
                UpdateCutscene(Time.fixedDeltaTime);
            }
        }

        ///----------------------------------------------------------------------------------------------

        ///<summary>Called when a Director Section has been reached. Subscribed in Awake</summary>
        void DirectorSectionReached(Section section) {

            if ( !isActive ) { return; }

            var previousSection = directorGroup.GetSectionBefore(section.time);
            if ( previousSection != null ) {
                if ( previousSection.exitMode == Section.ExitMode.Loop ) {
                    if ( previousSection.TryUpdateLoop() ) {
                        Sample(previousSection.time);
                        return;
                    }
                }
            }

            SendGlobalMessage("OnSectionReached", section);
            if ( OnSectionReached != null ) {
                OnSectionReached(section);
            }
        }

        ///<summary>Get all affected actors within the groups of the cutscene</summary>
        public IEnumerable<GameObject> GetAffectedActors() {
            return groups.OfType<ActorGroup>().Select(g => g.actor);
        }

        ///<summary>Get the start/end times of all directables, optionally excluding a specific directable</summary>
        public float[] GetPointerTimes() {
            if ( timePointers == null ) {
                InitializeTimePointers();
            }
            return timePointers.Select(t => t.time).ToArray();
        }

        ///<summary>Start or resume playing the cutscene at optional start time and optional provided callback for when it stops</summary>
        public void Play() { Play(0); }
        public void Play(System.Action callback) { Play(0, callback); }
        public void Play(float startTime) { Play(startTime, length, defaultWrapMode); }
        public void Play(float startTime, System.Action callback) { Play(startTime, length, defaultWrapMode, callback); }
        public void Play(
            float startTime,
            float endTime,
            WrapMode wrapMode = WrapMode.Once,
            System.Action callback = null,
            PlayingDirection playDirection = PlayingDirection.Forwards
            ) {

            if ( startTime > endTime && playDirection != PlayingDirection.Backwards ) {
                Debug.LogError("End Time must be greater than Start Time.", gameObject);
                return;
            }

            if ( isPaused ) { //if it's paused resume.
                Debug.LogWarning("Play called on a Paused cutscene. Cutscene will now resume instead.", gameObject);
                playingDirection = playDirection;
                Resume();
                return;
            }

            if ( isActive ) {
                Debug.LogWarning("Cutscene is already Running.", gameObject);
                return;
            }

            playTimeMin = 0; //for mathf.clamp setter

            playTimeMax = endTime;
            playTimeMin = startTime;
            currentTime = startTime;
            playingWrapMode = wrapMode;
            playingDirection = playDirection;

            if ( playDirection == PlayingDirection.Forwards ) {
                if ( currentTime >= playTimeMax ) {
                    currentTime = playTimeMin;
                }
            }

            if ( playDirection == PlayingDirection.Backwards ) {
                if ( currentTime <= playTimeMin ) {
                    currentTime = playTimeMax;
                }
            }

            isActive = true;
            isPaused = false;
            OnStop = callback != null ? callback : OnStop;

            UpdateCutscene(Time.deltaTime);//immediately update once now instead of waiting LaterUpdate, FixedUpdate etc.

            SendGlobalMessage("OnCutsceneStarted", this);
            if ( OnCutsceneStarted != null ) {
                OnCutsceneStarted(this);
            }
        }


        ///<summary>Stops the cutscene completely.</summary>
        public void Stop() { Stop(defaultStopMode); }
        public void Stop(StopMode stopMode) {

            if ( !isActive ) {
                return;
            }

            isActive = false;
            isPaused = false;

            if ( stopMode == StopMode.Skip ) {
                Sample(playTimeMax);
            }

            if ( stopMode == StopMode.Rewind ) {
                Sample(playTimeMin);
            }

            if ( stopMode == StopMode.Hold ) {
                Sample();
            }

            if ( stopMode == StopMode.SkipRewindNoUndo ) {
                Sample(playTimeMax);
                RewindNoUndo();
            }

            SendGlobalMessage("OnCutsceneStopped", this);
            if ( OnCutsceneStopped != null ) {
                OnCutsceneStopped(this);
            }

            if ( OnStop != null ) {
                OnStop();
                OnStop = null;
            }
        }


        ///<summary>Start or resume playing the cutscene at reverse, at optional new start time and optional provided callback for when it stops</summary>
        public void PlayReverse() { PlayReverse(0, length); }
        public void PlayReverse(float startTime, float endTime) { Play(startTime, endTime, WrapMode.Once, null, PlayingDirection.Backwards); }
        ///<summary>Pause the cutscene</summary>
        public void Pause() { isPaused = true; }
        ///<summary>Resume if cutscene was active</summary>
        public void Resume() { isPaused = false; }
        ///<summary>Skip the cutscene to the end</summary>
        public void SkipAll() { if ( isActive ) Stop(StopMode.Skip); else Sample(length); }
        ///<summary>Rewind the cutscene to it's initial 0 time state</summary>
        public void Rewind() { if ( isActive ) Stop(StopMode.Rewind); else Sample(0); }

        ///<summary>Rewinds the cutscene to it's initial 0 time state without undoing anything, thus keeping current state as finalized.</summary>
        public void RewindNoUndo() {
            if ( isActive ) {
                Stop(StopMode.Hold);
            }
            currentTime = 0;
            previousTime = currentTime; //this is why no undo is happening
            Sample();
        }

        ///<summary>Breaks out of the cutscene Loop (or PingPong) in case the Cutscene was looping in the first place</summary>
        public void BreakCutsceneLoop() {
            playingWrapMode = WrapMode.Once;
        }

        ///<summary>Breaks out of the current Section Loop, in case the current Section is looping in the first place</summary>
        public void BreakSectionLoop(bool alsoSkip = false) {
            var currentSection = directorGroup.GetSectionBefore(currentTime);
            if ( currentSection != null ) {
                currentSection.BreakLoop();
                if ( alsoSkip ) { SkipCurrentSection(); }
            }
        }

        [System.Obsolete("Use 'SkipCurrentSection' instead")]
        public void Skip() { SkipCurrentSection(); }
        ///<summary>Skip the cutscene time to the next Section or end time if none.</summary>
        public void SkipCurrentSection() {
            var forward = playingDirection == PlayingDirection.Forwards;
            var section = forward ? directorGroup.GetSectionAfter(currentTime) : directorGroup.GetSectionBefore(currentTime);
            currentTime = section != null ? section.time : ( forward ? length : 0 );
        }


        ///<summary>Set the cutscene time to a specific section by name</summary>
        public bool JumpToSection(string name) { return JumpToSection(GetSectionByName(name)); }
        public bool JumpToSection(Section section) {
            if ( section == null ) {
                Debug.LogError("Null Section Provided", gameObject);
                return false;
            }
            currentTime = section.time;
            return true;
        }


        ///<summary>Start playing from a specific Section</summary>
        public bool PlayFromSection(string name) { return PlayFromSection(name, defaultWrapMode); }
        public bool PlayFromSection(string name, WrapMode wrap, System.Action callback = null) { return PlayFromSection(GetSectionByName(name), wrap, callback); }

        public bool PlayFromSection(Section section) { return PlayFromSection(section, defaultWrapMode); }
        public bool PlayFromSection(Section section, WrapMode wrap, System.Action callback = null) {
            if ( section == null ) {
                Debug.LogError("Null Section Provided", gameObject);
                return false;
            }
            Play(section.time, length, wrap, callback);
            return true;
        }


        ///<summary>Play a specific Section only</summary>
        public bool PlaySection(string name) { return PlaySection(GetSectionByName(name), defaultWrapMode); }
        public bool PlaySection(string name, WrapMode wrap, System.Action callback = null) { return PlaySection(GetSectionByName(name), wrap, callback); }

        public bool PlaySection(Section section) { return PlaySection(section, defaultWrapMode); }
        public bool PlaySection(Section section, WrapMode wrap, System.Action callback = null) {
            if ( section == null ) {
                Debug.LogError("Null Section Provided", gameObject);
                return false;
            }
            var nextSection = directorGroup.GetSectionAfter(section.time);
            var endTime = nextSection != null ? nextSection.time : length;
            Play(section.time, endTime, wrap, callback);
            return true;
        }

        ///----------------------------------------------------------------------------------------------

        ///<summary>Sample cutscene state at time specified (currentTime by default). You can call this however and whenever you like without any pre-requirements</summary>
        public void Sample() { Sample(currentTime); }
        public void Sample(float time) {

            currentTime = time;

            //ignore same minmax times
            if ( ( currentTime == 0 || currentTime == length ) && previousTime == currentTime ) {
                return;
            }

            //Initialize time pointers if required.
            if ( !preInitialized && currentTime > 0 && previousTime == 0 ) {
                InitializeTimePointers();
            }

            //Sample started
            if ( currentTime > 0 && currentTime < length && ( previousTime == 0 || previousTime == length ) ) {
                OnSampleStarted();
            }

            //Sample pointers
            if ( timePointers != null ) {
                Internal_SamplePointers(currentTime, previousTime);
            }

            //Sample ended
            if ( ( currentTime == 0 || currentTime == length ) && previousTime > 0 && previousTime < length ) {
                OnSampleEnded();
            }

            previousTime = currentTime;
        }

        /*
                // *Personal Reminder.*
                void Internal_SuperSamplePointers(float time, float previousTime, int framerate){
                    var sampleRate = (1f/framerate);
                    if (time == previousTime || Mathf.Abs(time - previousTime) < sampleRate){
                        Internal_SamplePointers(time, previousTime);
                        return;
                    }
                    if (time > previousTime){
                        for (var t = previousTime + sampleRate; t <= time + sampleRate; t += sampleRate ){
                            var current = Mathf.Min( t, time );
                            var previous = Mathf.Max( t - sampleRate, previousTime );
                            Internal_SamplePointers(current, previous);
                        }
                    }
                    if (time < previousTime){
                        for (var t = previousTime - sampleRate; t >= time - sampleRate; t -= sampleRate ){
                            var current = Mathf.Max( t, time );
                            var previous = Mathf.Min( t + sampleRate, previousTime );
                            Internal_SamplePointers(current, previous);
                        }
                    }
                }
        */

        //Samples the initialized pointers.
        void Internal_SamplePointers(float currentTime, float previousTime) {
            //Update timePointers triggering forwards
            if ( !Application.isPlaying || currentTime > previousTime ) {
                for ( var i = 0; i < timePointers.Count; i++ ) {
                    try { timePointers[i].TriggerForward(currentTime, previousTime); }
                    catch ( System.Exception e ) { Debug.LogException(e); }
                }
            }

            //Update timePointers triggering backwards
            if ( !Application.isPlaying || currentTime < previousTime ) {
                for ( var i = timePointers.Count - 1; i >= 0; i-- ) {
                    try { timePointers[i].TriggerBackward(currentTime, previousTime); }
                    catch ( System.Exception e ) { Debug.LogException(e); }
                }
            }

            //Update timePointers
            if ( unsortedStartTimePointers != null ) {
                for ( var i = 0; i < unsortedStartTimePointers.Count; i++ ) {
                    try { unsortedStartTimePointers[i].Update(currentTime, previousTime); }
                    catch ( System.Exception e ) { Debug.LogException(e); }
                }
            }
        }

        ///<summary>Resamples cutscene. Useful when action settings have been changed</summary>
        public void ReSample() {

            if ( Application.isPlaying ) {
                return;
            }

            if ( currentTime > 0 && currentTime < length && timePointers != null ) {
                _isReSampleFrame = true;

#if UNITY_EDITOR
                List<CutsceneUtility.ChangedParameterCallbacks> cache = null;
                if ( !Prefs.autoKey ) {
                    cache = CutsceneUtility.changedParameterCallbacks.Values.ToList();
                }
#endif

                Internal_SamplePointers(0, currentTime);
                Internal_SamplePointers(currentTime, 0);

#if UNITY_EDITOR
                if ( !Prefs.autoKey && cache != null ) {
                    foreach ( var param in cache ) { param.Restore(); }
                }
#endif

                _isReSampleFrame = false;
            }
        }

        ///<summary>Gather and validate directables. This is done in Awake as well as in editor when a directable is added or removed and OnValidate.</summary>
        public void Validate() {

            if ( groupsRoot.transform.parent != this.transform ) { groupsRoot.transform.parent = this.transform; }

            directables = new List<IDirectable>();
            foreach ( IDirectable group in groups.AsEnumerable().Reverse() ) {
                directables.Add(group);
                try { group.Validate(this, null); }
                catch ( System.Exception e ) { Debug.LogException(e); }
                foreach ( IDirectable track in group.children.Reverse() ) {
                    directables.Add(track);
                    try { track.Validate(this, group); }
                    catch ( System.Exception e ) { Debug.LogException(e); }
                    foreach ( IDirectable clip in track.children ) {
                        directables.Add(clip);
                        try { clip.Validate(this, track); }
                        catch ( System.Exception e ) { Debug.LogException(e); }
                    }
                }
            }
        }

        //Initialize the time pointers (in/out). Bottom to top.
        //Time pointers dectate all directables execution order. All pointers are collapsed into a list and ordered by their time.
        //Reverse() is used for in case pointers have same time. This is mostly true for groups and tracks.
        //(Group Enter -> Track Enter -> Clip Enter | Clip Exit -> Track Exit -> Group Exit)
        void InitializeTimePointers() {

            timePointers = new List<IDirectableTimePointer>();
            unsortedStartTimePointers = new List<IDirectableTimePointer>();

            foreach ( IDirectable group in groups.AsEnumerable().Reverse() ) {
                if ( group.isActive && group.Initialize() ) {
                    var p1 = new StartTimePointer(group);
                    timePointers.Add(p1);

                    foreach ( IDirectable track in group.children.Reverse() ) {
                        if ( track.isActive && track.Initialize() ) {
                            var p2 = new StartTimePointer(track);
                            timePointers.Add(p2);

                            foreach ( IDirectable clip in track.children ) {
                                if ( clip.isActive && clip.Initialize() ) {
                                    var p3 = new StartTimePointer(clip);
                                    timePointers.Add(p3);

                                    unsortedStartTimePointers.Add(p3);
                                    timePointers.Add(new EndTimePointer(clip));
                                }
                            }

                            unsortedStartTimePointers.Add(p2);
                            timePointers.Add(new EndTimePointer(track));
                        }
                    }

                    unsortedStartTimePointers.Add(p1);
                    timePointers.Add(new EndTimePointer(group));
                }
            }

            timePointers = timePointers.OrderBy(p => p.time).ToList();
        }

        //When Sample begins
        void OnSampleStarted() {
            SetLayersActive();
            if ( DirectorCamera.current == null ) { Debug.LogWarning("Director Camera is null. Have you disabled the AutoCreateDirectorCamera in Preferences?"); }
            if ( DirectorGUI.current ) { DirectorGUI.current.enabled = true; }
            for ( var i = 0; i < directables.Count; i++ ) {
                try { directables[i].RootEnabled(); }
                catch ( System.Exception e ) { Debug.LogException(e, gameObject); }
            }
#if UNITY_EDITOR
            transform.hideFlags = HideFlags.NotEditable;
#endif
        }

        //When Sample ends
        void OnSampleEnded() {
            RestoreLayersActive();
            if ( DirectorGUI.current ) { DirectorGUI.current.enabled = false; }
            for ( var i = 0; i < directables.Count; i++ ) {
                try { directables[i].RootDisabled(); }
                catch ( System.Exception e ) { Debug.LogException(e, gameObject); }
            }
#if UNITY_EDITOR
            transform.hideFlags = HideFlags.None;
#endif
        }

        ///----------------------------------------------------------------------------------------------

        //use of active layers to toggle root object on or off during cutscene
        void SetLayersActive() {
            if ( explicitActiveLayers ) {
                var rootObjects = this.gameObject.scene.GetRootGameObjects();
                affectedLayerGOStates = new Dictionary<GameObject, bool>();
                for ( var i = 0; i < rootObjects.Length; i++ ) {
                    var o = rootObjects[i];
                    affectedLayerGOStates[o] = o.activeInHierarchy;
                    o.SetActive(( activeLayers.value & ( 1 << o.layer ) ) > 0);
                }
            }
        }

        //restore layer object states.
        void RestoreLayersActive() {
            if ( affectedLayerGOStates != null ) {
                foreach ( var pair in affectedLayerGOStates ) {
                    if ( pair.Key != null ) {
                        pair.Key.SetActive(pair.Value);
                    }
                }
            }
        }

        ///<summary>Update the cutscene by delta</summary>
        void UpdateCutscene(float delta) {

            //update time
            delta *= playbackSpeed;
            currentTime += playingDirection == PlayingDirection.Forwards ? delta : -delta;

            switch ( playingWrapMode ) {

                //handle Once
                case WrapMode.Once:
                    if ( currentTime >= playTimeMax && playingDirection == PlayingDirection.Forwards ) {
                        Stop();
                        return;
                    }
                    if ( currentTime <= playTimeMin && playingDirection == PlayingDirection.Backwards ) {
                        Stop();
                        return;
                    }
                    break;

                //handle Loop
                case WrapMode.Loop:
                    if ( currentTime >= playTimeMax ) {
                        Sample(playTimeMin);
                        currentTime = playTimeMin + delta;
                    }
                    if ( currentTime <= playTimeMin ) {
                        Sample(playTimeMax);
                        currentTime = playTimeMax - delta;
                    }
                    break;

                //hanlde PingPong
                case WrapMode.PingPong:
                    if ( currentTime >= playTimeMax ) {
                        Sample(playTimeMax);
                        currentTime = playTimeMax - delta;
                        playingDirection = playbackSpeed >= 0 ? PlayingDirection.Backwards : PlayingDirection.Forwards;
                    }
                    if ( currentTime <= playTimeMin ) {
                        Sample(playTimeMin);
                        currentTime = playTimeMin + delta;
                        playingDirection = playbackSpeed >= 0 ? PlayingDirection.Forwards : PlayingDirection.Backwards;
                    }
                    break;
            }

            //finally sample
            Sample();
        }

        ///<summary>Play a cutscene of specified name that exists either in the Resources folder or in the scene. In that order.</summary>
        public static Cutscene Play(string name) { return Play(name, null); }
        public static Cutscene Play(string name, System.Action callback) {
            var cutscene = FindFromResources(name);
            if ( cutscene != null ) {
                var instance = (Cutscene)Instantiate(cutscene);
                Debug.Log("Instantiating cutscene from Resources");
                instance.Play(() =>
                {
                    Destroy(instance.gameObject);
                    Debug.Log("Instantiated Cutscene Destroyed");
                    if ( callback != null ) {
                        callback();
                    }
                });
                return cutscene;
            }

            cutscene = Find(name);
            if ( cutscene != null ) {
                cutscene.Play(callback);
                return cutscene;
            }

            return null;
        }

        ///<summary>Find a cutscene from Resources folder</summary>
        public static Cutscene FindFromResources(string name) {
            var go = Resources.Load(name, typeof(GameObject)) as GameObject;
            if ( go != null ) {
                var cut = go.GetComponent<Cutscene>();
                if ( cut != null ) {
                    return cut;
                }
            }
            Debug.LogWarning(string.Format("Cutscene of name '{0}' does not exists in the Resources folder", name));
            return null;
        }

        ///<summary>Find a cutscene of specified name that exists in the scene</summary>
        public static Cutscene Find(string name) {
            Cutscene cutscene = null;
            if ( allSceneCutscenes.TryGetValue(name, out cutscene) ) {
                return cutscene;
            }
            Debug.LogError(string.Format("Cutscene of name '{0}' does not exists in the scene", name));
            return null;
        }

        ///<summary>Stop all running cutscenes found in the scene</summary>
        public static void StopAllCutscenes() {
            foreach ( var cutscene in FindObjectsOfType<Cutscene>() ) {
                if ( cutscene.isActive ) {
                    cutscene.Stop();
                }
            }
        }

        ///<summary>Sends a message to all affected gameObject actors (including Director Camera), as well as the cutscene gameObject itself.</summary>
        public void SendGlobalMessage(string message, object value = null) {
            this.gameObject.SendMessage(message, value, SendMessageOptions.DontRequireReceiver);
            foreach ( var actor in GetAffectedActors() ) {
                if ( actor != null ) {
                    actor.SendMessage(message, value, SendMessageOptions.DontRequireReceiver);
                }
            }

            if ( OnGlobalMessageSend != null ) {
                OnGlobalMessageSend(message, value);
            }

#if UNITY_EDITOR
            Debug.Log(string.Format("<b>({0}) Global Message Send:</b> '{1}' ({2})", name, message, value), gameObject);
#endif
        }

        ///<summary>Set the target actor of an Actor Group by the group's name.</summary>
        public void SetGroupActorOfName(string groupName, GameObject newActor) {

            if ( currentTime > 0 ) {
                Debug.LogError("Setting a Group Actor is only allowed when the Cutscene is not active and is rewinded", gameObject);
                return;
            }

            var group = groups.OfType<ActorGroup>().FirstOrDefault(g => g.name.ToLower() == groupName.ToLower());
            if ( group == null ) {
                Debug.LogError(string.Format("Actor Group with name '{0}' doesn't exist in cutscene", groupName), gameObject);
                return;
            }

            group.actor = newActor;
        }

        ///<summary>Providing a path to the element in the order of Group->Track->Clip, like for example ("MyGroup/MyTrack/MyClip"), returns that element.</summary>
        public T FindElement<T>(string path) where T : IDirectable { return (T)FindElement(path); }
        ///<summary>Providing a path to the element in the order of Group->Track->Clip, like for example ("MyGroup/MyTrack/MyClip"), returns that element.</summary>
        public IDirectable FindElement(string path) {
            var split = path.Split('/');
            var result = groups.FirstOrDefault(g => g.name.ToLower() == split[0].ToLower()) as IDirectable;
            if ( result != null ) {
                for ( var i = 1; i < split.Length; i++ ) {
                    result = result.FindChild(split[i]);
                    if ( result == null ) {
                        break;
                    }
                }
            }

            if ( result == null ) {
                Debug.LogWarning(string.Format("Cutscene element path to '{0}', was not found", path));
            }

            return result;
        }

        //...
        public override string ToString() {
            return string.Format("'{0}' Cutscene", name);
        }

        ///<summary>Get a section by name</summary>
        public Section GetSectionByName(string name) {
            return directorGroup.GetSectionByName(name);
        }

        ///<summary>Get a section by UID</summary>
        public Section GetSectionByUID(string UID) {
            return directorGroup.GetSectionByUID(UID);
        }

        ///<summary>All section names of the DirectorGroup</summary>
        public Section[] GetSections() {
            return directorGroup.sections.ToArray();
        }

        ///<summary>Returns a named section's length</summary>
        public float GetSectionLength(string name) {
            var section = directorGroup.GetSectionByName(name);
            if ( section != null ) {
                var nextSection = directorGroup.GetSectionAfter(section.time);
                return nextSection != null ? nextSection.time - section.time : length - section.time;
            }
            return -1;
        }

        ///<summary>All section names of the DirectorGroup</summary>
        public string[] GetSectionNames() {
            return directorGroup.sections.Select(s => s.name).ToArray();
        }

        ///<summary>Get all names of SendGlobalMessage ActionClips</summary>
        public string[] GetDefinedEventNames() {
            return directables.OfType<IEvent>().Select(d => d.name).ToArray();
        }

        ///<summary>By default cutscene is initialized when it starts playing. You can pre-initialize it if you want so for performance in case there is any lag when cutscene is started.</summary>
        public void PreInitialize() {
            InitializeTimePointers();
            preInitialized = true;
        }


        ///<summary>Render the cutscene to an image sequence in runtime and get a Texture2D[] of the rendered frames. This operation will take several frames to complete. Use the callback parameter to get the result when rendering is done.</summary>
        public void RenderCutscene(int width, int height, int frameRate, System.Action<Texture2D[]> callback) {

            if ( !Application.isPlaying ) {
                Debug.LogError("Rendering Cutscene with RenderCutscene function is only meant for runtime", this);
                return;
            }

            if ( isActive ) {
                Debug.LogWarning("You called RenderCutscene to an actively playing Cutscene. The cutscene will now Stop.", this);
                Stop();
            }

            StartCoroutine(Internal_RenderCutscene(width, height, frameRate, callback));
        }

        //runtime rendering to Texture2D[]
        IEnumerator Internal_RenderCutscene(int width, int height, int frameRate, System.Action<Texture2D[]> callback) {
            var renderSequence = new List<Texture2D>();
            var sampleRate = 1f / frameRate;
            for ( var i = sampleRate; i <= length; i += sampleRate ) {
                Sample(i);
                yield return new WaitForEndOfFrame();
                var texture = new Texture2D(width, height, TextureFormat.RGB24, false);
                texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                texture.Apply();
                renderSequence.Add(texture);
            }
            callback(renderSequence.ToArray());
        }

        ///----------------------------------------------------------------------------------------------
#if !UNITY_EDITOR //runtime add/delete group
        ///<summary>Add a group to the cutscene.</summary>
        public T AddGroup<T>(GameObject targetActor = null) where T : CutsceneGroup { return (T)AddGroup(typeof(T), targetActor); }
        public CutsceneGroup AddGroup(System.Type type, GameObject targetActor = null) {
            if ( !type.IsSubclassOf(typeof(CutsceneGroup)) || type.IsAbstract ) { return null; }
            if ( type == typeof(DirectorGroup) && directorGroup != null ) { return directorGroup; }
            var newGroup = new GameObject(type.Name).AddComponent(type) as CutsceneGroup;
            newGroup.transform.SetParent(groupsRoot);
            newGroup.transform.localPosition = Vector3.zero;
            newGroup.actor = targetActor;
            groups.Add(newGroup);
            Validate();
            return newGroup;
        }

        ///<summary>Delete a group from the cutscene.</summary>
        public void DeleteGroup(CutsceneGroup group) {
            if ( group is DirectorGroup ) { return; }
            groups.Remove(group);
            DestroyImmediate(group.gameObject);
            Validate();
        }
#endif
        ///----------------------------------------------------------------------------------------------


        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        [ContextMenu("Reset")] //override
        void Reset() { TryReset(); }
        [ContextMenu("Copy Component")] //override
        void CopyComponent() { }
        [ContextMenu("Remove Component")] //override
        void RemoveComponent() { Debug.LogWarning("Removing the Cutscene Component is not possible. Please delete the GameObject instead"); }
        [ContextMenu("Show Transforms")]
        void ShowTransforms() { Prefs.showTransforms = true; groupsRoot.hideFlags = HideFlags.None; }
        [ContextMenu("Hide Transforms")]
        void HideTransforms() { Prefs.showTransforms = false; groupsRoot.hideFlags = HideFlags.HideInHierarchy; }

        //UNITY CALLBACK
        protected void OnValidate() {
            if ( !UnityEditor.EditorUtility.IsPersistent(this) && !Application.isPlaying && !UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode ) {
                Validate();
            }
            // UnityEditor.SceneManagement.EditorSceneManager.preventCrossSceneReferences = false;
        }

        //UNITY CALLBACK
        protected void OnApplicationQuit() {
            if ( currentTime > 0 ) {
                Stop(StopMode.Rewind);
            }
        }

        //UNITY CALLBACK
        protected void OnDrawGizmos() {
            var l = Prefs.gizmosLightness;
            Gizmos.color = new Color(l, l, l);
            Gizmos.DrawSphere(transform.position, 0.025f);
            Gizmos.color = Color.white;

            if ( CutsceneUtility.cutsceneInEditor == this ) {
                for ( var i = 0; i < directables.Count; i++ ) {
                    var directable = directables[i];
                    directable.DrawGizmos(CutsceneUtility.selectedObject == directable);
                }
            }
        }

        public static Cutscene Create(Transform parent = null) {
            var cutscene = new GameObject("Cutscene").AddComponent<Cutscene>();
            if ( parent != null ) {
                cutscene.transform.SetParent(parent, false);
            }
            cutscene.transform.localPosition = Vector3.zero;
            cutscene.transform.localRotation = Quaternion.identity;
            return cutscene;
        }

        ///<summary>Add a group to the cutscene.</summary>
        public T AddGroup<T>(GameObject targetActor = null) where T : CutsceneGroup { return (T)AddGroup(typeof(T), targetActor); }
        public CutsceneGroup AddGroup(System.Type type, GameObject targetActor = null) {

            if ( !type.IsSubclassOf(typeof(CutsceneGroup)) || type.IsAbstract ) {
                return null;
            }

            if ( type == typeof(DirectorGroup) && directorGroup != null ) {
                Debug.LogWarning("A Cutscene can only contain one Director Group", this);
                return directorGroup;
            }

            var newGroup = new GameObject(type.Name).AddComponent(type) as CutsceneGroup;
            UnityEditor.Undo.RegisterCreatedObjectUndo(newGroup.gameObject, "Add Group");
            UnityEditor.Undo.SetTransformParent(newGroup.transform, groupsRoot, "Add Group");
            UnityEditor.Undo.RegisterCompleteObjectUndo(this, "Add Group");
            newGroup.transform.localPosition = Vector3.zero;
            newGroup.actor = targetActor;
            groups.Add(newGroup);
            Validate();

            if ( type != typeof(DirectorGroup) && targetActor != null ) {
                if ( targetActor.GetComponent<Animator>() != null ) {
                    newGroup.AddTrack<AnimatorTrack>();
                }
                if ( targetActor.GetComponent<Animation>() != null ) {
                    newGroup.AddTrack<AnimationTrack>();
                }
            }

            CutsceneUtility.selectedObject = newGroup;
            return newGroup;
        }

        ///<summary>Delete a group from the cutscene.</summary>
        public void DeleteGroup(CutsceneGroup group) {

            if ( !group.gameObject.IsSafePrefabDelete() ) {
                UnityEditor.EditorUtility.DisplayDialog("Delete Group", "This group is part of the prefab asset and can not be deleted from within the prefab instance. If you want to delete the group, please open the prefab asset for editing.", "OK");
                return;
            }

            if ( group is DirectorGroup ) {
                Debug.LogWarning("The Director Group can't be removed from the Cutscene", gameObject);
                return;
            }

            UnityEditor.Undo.RegisterCompleteObjectUndo(this, "Delete Group");
            groups.Remove(group);
            UnityEditor.Undo.DestroyObjectImmediate(group.gameObject);
            UnityEditor.EditorUtility.SetDirty(this);
            Validate();
        }

        ///<summary>Duplicate a group in the cutscene.</summary>
        public CutsceneGroup DuplicateGroup(CutsceneGroup group, GameObject targetActor = null) {

            if ( group == null || ( group is DirectorGroup ) ) {
                return null;
            }

            var newGroup = (CutsceneGroup)Instantiate(group);
            UnityEditor.Undo.RegisterCreatedObjectUndo(newGroup.gameObject, "Duplicate Group");
            UnityEditor.Undo.SetTransformParent(newGroup.transform, groupsRoot, "Duplicate Group");
            UnityEditor.Undo.RegisterCompleteObjectUndo(this, "Duplicate Group");
            newGroup.actor = targetActor;
            newGroup.transform.localPosition = Vector3.zero;
            groups.Add(newGroup);
            Validate();
            CutsceneUtility.selectedObject = newGroup;
            return newGroup;
        }

        void TryReset() {
            if ( directorGroup == null ) {
                var newDirectorGroup = AddGroup<DirectorGroup>();
                newDirectorGroup.AddTrack<DirectorActionTrack>();
                newDirectorGroup.AddTrack<DirectorAudioTrack>();
                newDirectorGroup.AddTrack<CameraTrack>();
                CutsceneUtility.selectedObject = null;
                length = 20;
                viewTimeMin = -1;
                viewTimeMax = 21;
            }
        }

#endif
    }
}