using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Slate
{

    ///<summary>Tracks are contained within CutsceneGroups and contain ActionsClips within</summary>
    abstract public class CutsceneTrack : MonoBehaviour, IDirectable
    {

        [SerializeField]
        private string _name;
        [SerializeField]
        private Color _color = Color.white;
        [SerializeField, HideInInspector]
        private bool _active = true;
        [SerializeField, HideInInspector]
        private bool _isLocked = false;
        [SerializeField, HideInInspector]
        private List<ActionClip> _actionClips = new List<ActionClip>();

        ///<summary>The actor to be used in the track taken from it's parent group</summary>
        public GameObject actor {
            get { return parent != null ? parent.actor : null; }
        }

        ///<summary>The name...</summary>
        new public string name {
            get { return string.IsNullOrEmpty(_name) ? GetType().Name.SplitCamelCase() : _name; }
            set
            {
                if ( _name != value ) {
                    _name = value;
                    base.name = value;
                }
            }
        }

        ///<summary>Coloring of clips within this track</summary>
        public Color color {
            get { return _color.a > 0.1f ? _color : Color.white; }
        }

        ///<summary>All action clips of this track</summary>
        public List<ActionClip> clips {
            get { return _actionClips; }
            set { _actionClips = value; }
        }

        ///<summary>Display info</summary>
        virtual public string info {
            get { return string.Empty; }
        }

        ///<summary>Children</summary>
        IEnumerable<IDirectable> IDirectable.children {
            get { return clips.Cast<IDirectable>(); }
        }

        ///<summary>Type-based order of track</summary>
        public int layerOrder { get; private set; }

        ///<summary>Root director</summary>
        public IDirector root { get { return parent != null ? parent.root : null; } }
        ///<summary>Parent directable</summary>
        public IDirectable parent { get; private set; }

        ///<summary>Editor is collapsed?</summary>
        virtual public bool isCollapsed {
            get { return parent != null ? parent.isCollapsed : false; }
        }

        ///<summary>Is active and used?</summary>
        virtual public bool isActive {
            get { return parent != null ? parent.isActive && _active : false; }
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

        ///<summary>Editor is locked?</summary>
        virtual public bool isLocked {
            get { return parent != null ? parent.isLocked || _isLocked : false; }
            set { _isLocked = value; }
        }

        ///<summary>Start time, usually parent.startTime</summary>
        virtual public float startTime {
            get { return parent != null ? parent.startTime : 0; }
            set { }
        }

        ///<summary>Start time, usually parent.endTime</summary>
        virtual public float endTime {
            get { return parent != null ? parent.endTime : 0; }
            set { }
        }

        ///<summary>Blend in</summary>
        virtual public float blendIn {
            get { return 0f; }
            set { }
        }

        ///<summary>Blend out</summary>
        virtual public float blendOut {
            get { return 0f; }
            set { }
        }

        ///<summary>Able to cross-blend?</summary>
        public bool canCrossBlend {
            get { return false; }
        }

        //when the cutscene init (once, awake)
        bool IDirectable.Initialize() { return OnInitialize(); }
        //when the cutscene starts
        void IDirectable.Enter() { OnEnter(); }
        //when the cutscene is updated
        void IDirectable.Update(float time, float previousTime) { OnUpdate(time, previousTime); }
        //when the cutscene stops
        void IDirectable.Exit() { OnExit(); }
        //when the cutscene enters backwards
        void IDirectable.ReverseEnter() { OnReverseEnter(); }
        //when the cutscene is reversed/rewinded
        void IDirectable.Reverse() { OnReverse(); }

        //when root is enabled/started
        void IDirectable.RootEnabled() { OnRootEnabled(); }
        //when root is disabled/finished
        void IDirectable.RootDisabled() { OnRootDisabled(); }
        //when root is updated
        void IDirectable.RootUpdated(float time, float previousTime) { OnRootUpdated(time, previousTime); }
        //when root is destroyed
        void IDirectable.RootDestroyed() { OnRootDestroyed(); }

#if UNITY_EDITOR
        //Gizmos selected
        void IDirectable.DrawGizmos(bool selected) { if ( selected ) OnDrawGizmosSelected(); }
        //Scene GUI stuff
        void IDirectable.SceneGUI(bool selected) { OnSceneGUI(); }
#endif

        ///<summary>After creation</summary>
        public void PostCreate(IDirectable parent) {
            this.parent = parent;
            OnCreate();
        }

        ///<summary>Validate the track and it's clips</summary>
        public void Validate(IDirector root, IDirectable parent) {
            this.parent = parent;
            clips = GetComponents<ActionClip>().OrderBy(a => a.startTime).ToList();
            layerOrder = parent.children.Where(t => t.GetType() == this.GetType() && t.isActive).Reverse().ToList().IndexOf(this); // O.o
            OnAfterValidate();
        }

        ///----------------------------------------------------------------------------------------------
        virtual protected void OnCreate() { }
        virtual protected void OnAfterValidate() { }
        virtual protected bool OnInitialize() { return true; }
        virtual protected void OnEnter() { }
        virtual protected void OnUpdate(float time, float previousTime) { }
        virtual protected void OnExit() { }
        virtual protected void OnReverseEnter() { }
        virtual protected void OnReverse() { }
        virtual protected void OnDrawGizmosSelected() { }
        virtual protected void OnSceneGUI() { }
        virtual protected void OnRootEnabled() { }
        virtual protected void OnRootDisabled() { }
        virtual protected void OnRootUpdated(float time, float previousTime) { }
        virtual protected void OnRootDestroyed() { }
        ///----------------------------------------------------------------------------------------------

        ///----------------------------------------------------------------------------------------------
        public float GetTrackWeight() { return this.GetWeight(root.currentTime - this.startTime, this.blendIn, this.blendOut); }
        public float GetTrackWeight(float time) { return this.GetWeight(time, this.blendIn, this.blendOut); }
        public float GetTrackWeight(float time, float blendInOut) { return this.GetWeight(time, blendInOut, blendInOut); }
        public float GetTrackWeight(float time, float blendIn, float blendOut) { return this.GetWeight(time, blendIn, blendOut); }
        ///----------------------------------------------------------------------------------------------

        ///----------------------------------------------------------------------------------------------
#if !UNITY_EDITOR //runtime add/delete action
        ///<summary>Add an ActionClip to this Track</summary>
        public T AddAction<T>(float time) where T : ActionClip { return (T)AddAction(typeof(T), time); }
        public ActionClip AddAction(System.Type type, float time) {
            var newAction = gameObject.AddComponent(type) as ActionClip;
            newAction.startTime = time;
            clips.Add(newAction);
            newAction.PostCreate(this);

            var nextAction = clips.FirstOrDefault(a => a.startTime > newAction.startTime);
            if ( nextAction != null ) { newAction.endTime = Mathf.Min(newAction.endTime, nextAction.startTime); }

            root.Validate();
            return newAction;
        }

        ///<summary>Remove an ActionClip from this Track</summary>
        public void DeleteAction(ActionClip action) {
            clips.Remove(action);
            DestroyImmediate(action);
            root.Validate();
        }
#endif
        ///----------------------------------------------------------------------------------------------


        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        private const float PARAMS_TOP_MARGIN = 5f;
        private const float PARAMS_LINE_HEIGHT = 18f;
        private const float PARAMS_LINE_MARGIN = 2f;
        private const float BOX_WIDTH = 30f;

        [SerializeField, HideInInspector]
        private float _customHeight = 300f;

        private bool isResizingHeight = false;
        private float proposedHeight = 0f;
        private int inspectedParameterIndex = -1;
        private object _icon;
        private bool _showAnimationCurves;
        private ActionClip _showCurvesClip;

        [UnityEditor.InitializeOnLoadMethod]
        static void Editor_Init() {
            CutsceneUtility.onSelectionChange += OnDirectableSelectionChange;
        }

        //basically store last clip selected so that we can show multiple clip curves from different tracks at the same time
        static void OnDirectableSelectionChange(IDirectable directable) {
            if ( directable is IKeyable && directable.parent is CutsceneTrack ) {
                ( directable.parent as CutsceneTrack ).showCurvesClip = (IKeyable)directable;
            }
        }

        ///<summary>The expansion height</summary>
        private float customHeight {
            get { return _customHeight; }
            set { _customHeight = Mathf.Clamp(value, proposedHeight, 500); }
        }

        ///<summary>The final shown height</summary>
        public float finalHeight {
            get
            {
                if ( showCurves ) {
                    return inspectedParameterIndex == -1 ? Mathf.Max(proposedHeight, defaultHeight + 50) : Mathf.Max(proposedHeight, customHeight);
                }
                return defaultHeight;
            }
        }

        //are curves shown?
        virtual public bool showCurves {
            get { return _showAnimationCurves; }
            set { _showAnimationCurves = value; }
        }

        private IKeyable showCurvesClip {
            get
            {
                if ( _showCurvesClip == null || Equals(_showCurvesClip, null) ) {
                    return null;
                }
                return _showCurvesClip;
            }
            set { _showCurvesClip = (ActionClip)value; }
        }

        ///<summary>The default track height when not expanded</summary>
        virtual public float defaultHeight {
            get { return 32f; }
        }

        ///<summary>Icon shown on left if any</summary>
        virtual public Texture icon {
            get
            {
                if ( _icon == null ) {
                    var att = this.GetType().RTGetAttribute<IconAttribute>(true);
                    if ( att != null ) {
                        _icon = Resources.Load(att.iconName) as Texture;
                        if ( _icon == null && !string.IsNullOrEmpty(att.iconName) ) {
                            _icon = UnityEditor.EditorGUIUtility.FindTexture(att.iconName) as Texture;
                        }
                        if ( _icon == null && att.fromType != null ) {
                            _icon = UnityEditor.AssetPreview.GetMiniTypeThumbnail(att.fromType);
                        }
                    }

                    if ( _icon == null ) {
                        _icon = new object();
                    }
                }

                return _icon as Texture;
            }
        }

        ///----------------------------------------------------------------------------------------------

        ///<summary>Add an ActionClip to this Track</summary>
        public T AddAction<T>(float time) where T : ActionClip { return (T)AddAction(typeof(T), time); }
        public ActionClip AddAction(System.Type type, float time) {

            var catAtt = type.GetCustomAttributes(typeof(CategoryAttribute), true).FirstOrDefault() as CategoryAttribute;
            if ( catAtt != null && clips.Count == 0 ) {
                name = catAtt.category + " Track";
            }

            var newAction = UnityEditor.Undo.AddComponent(gameObject, type) as ActionClip;
            UnityEditor.Undo.RegisterCompleteObjectUndo(this, "New Action");
            newAction.startTime = time;
            clips.Add(newAction);
            newAction.PostCreate(this);

            var nextAction = clips.FirstOrDefault(a => a.startTime > newAction.startTime);
            if ( nextAction != null ) {
                newAction.endTime = Mathf.Min(newAction.endTime, nextAction.startTime);
            }

            root.Validate();
            CutsceneUtility.selectedObject = newAction;
            return newAction;
        }

        ///<summary>Remove an ActionClip from this Track</summary>
        public void DeleteAction(ActionClip action) {
            UnityEditor.Undo.RegisterCompleteObjectUndo(this, "Remove Action");
            clips.Remove(action);
            if ( ReferenceEquals(CutsceneUtility.selectedObject, action) ) {
                CutsceneUtility.selectedObject = null;
            }
            UnityEditor.Undo.DestroyObjectImmediate(action);
            root.Validate();
        }

        ///----------------------------------------------------------------------------------------------

        ///<summary>The Editor GUI in the track info on the left</summary>
        virtual public void OnTrackInfoGUI(Rect trackRect) {
            var e = Event.current;
            DoDefaultInfoGUI(e, trackRect);
            if ( showCurves ) {
                var wasEnable = GUI.enabled;
                GUI.enabled = true;
                DoParamsInfoGUI(e, trackRect, showCurvesClip, showCurvesClip is ActionClips.AnimateProperties);
                GUI.enabled = wasEnable;
            }

            GUI.color = Color.white;
            GUI.backgroundColor = Color.white;
        }


        //default track info gui 
        protected void DoDefaultInfoGUI(Event e, Rect trackRect) {

            var iconBGRect = new Rect(0, 0, BOX_WIDTH, defaultHeight);
            iconBGRect = iconBGRect.ExpandBy(-1);
            var textInfoRect = Rect.MinMaxRect(iconBGRect.xMax + 2, 0, trackRect.width - BOX_WIDTH - 2, defaultHeight);
            var curveButtonRect = new Rect(trackRect.width - BOX_WIDTH, 0, BOX_WIDTH, defaultHeight);


            GUI.color = Color.black.WithAlpha(UnityEditor.EditorGUIUtility.isProSkin ? 0.1f : 0.1f);
            GUI.DrawTexture(iconBGRect, Texture2D.whiteTexture);
            GUI.color = Color.white;

            if ( icon != null ) {
                var iconRect = new Rect(0, 0, 16, 16);
                iconRect.center = iconBGRect.center;
                GUI.color = ReferenceEquals(CutsceneUtility.selectedObject, this) ? Color.white : new Color(1, 1, 1, 0.8f);
                GUI.DrawTexture(iconRect, icon);
                GUI.color = Color.white;
            }


            var nameString = string.Format("<size=11>{0}</size>", name);
            var infoString = string.Format("<size=9><color=#707070>{0}</color></size>", info);
            GUI.color = isActive ? Color.white : Color.grey;
            GUI.Label(textInfoRect, string.Format("{0}\n{1}", nameString, infoString));
            GUI.color = Color.white;

            var wasEnable = GUI.enabled;
            GUI.enabled = true;
            var curveIconRect = new Rect(0, 0, 16, 16);
            curveIconRect.center = curveButtonRect.center - new Vector2(0, 1);
            var curveIconColor = UnityEditor.EditorGUIUtility.isProSkin ? Color.white : Color.black;
            curveIconColor.a = showCurves ? 1 : 0.3f;

            if ( GUI.Button(curveButtonRect, string.Empty, GUIStyle.none) ) {
                showCurves = !showCurves;
            }

            curveButtonRect = curveButtonRect.ExpandBy(-4);
            GUI.color = ColorUtility.Grey(UnityEditor.EditorGUIUtility.isProSkin ? 0.2f : 1f).WithAlpha(0.2f);
            GUI.Box(curveButtonRect, string.Empty, Styles.clipBoxStyle);

            GUI.color = curveIconColor;
            GUI.DrawTexture(curveIconRect, Styles.curveIcon);

            GUI.color = UnityEditor.EditorGUIUtility.isProSkin ? Color.grey : Color.grey;
            if ( !isActive ) {
                var hiddenRect = new Rect(0, 0, 16, 16);
                hiddenRect.center = curveButtonRect.center - new Vector2(curveButtonRect.width, 0);
                if ( GUI.Button(hiddenRect, Styles.hiddenIcon, GUIStyle.none) ) { isActive = !isActive; }
            }

            if ( isLocked ) {
                var lockRect = new Rect(0, 0, 16, 16);
                lockRect.center = curveButtonRect.center - new Vector2(curveButtonRect.width, 0);
                if ( !isActive ) { lockRect.center -= new Vector2(16, 0); }
                if ( GUI.Button(lockRect, Styles.lockIcon, GUIStyle.none) ) { isLocked = !isLocked; }
            }


            GUI.color = Color.white;
            GUI.enabled = wasEnable;
        }

        //show selected clip animated parameters list info
        protected void DoParamsInfoGUI(Event e, Rect trackRect, IKeyable keyable, bool showAddPropertyButton) {

            //bg graphic
            var expansionRect = Rect.MinMaxRect(5, defaultHeight, trackRect.width - 3, finalHeight - 3);
            GUI.color = UnityEditor.EditorGUIUtility.isProSkin ? new Color(0.22f, 0.22f, 0.22f) : new Color(0.7f, 0.7f, 0.7f);
            GUI.DrawTexture(expansionRect, Styles.whiteTexture);
            GUI.color = new Color(0, 0, 0, 0.05f);
            GUI.Box(expansionRect, string.Empty, Styles.shadowBorderStyle);
            GUI.color = Color.white;


            //allow resize height
            if ( inspectedParameterIndex >= 0 ) {
                var resizeRect = Rect.MinMaxRect(0, finalHeight - 4, trackRect.width, finalHeight);
                UnityEditor.EditorGUIUtility.AddCursorRect(resizeRect, UnityEditor.MouseCursor.ResizeVertical);
                GUI.color = Color.grey;
                GUI.DrawTexture(resizeRect, Styles.whiteTexture);
                GUI.color = Color.white;
                if ( e.type == EventType.MouseDown && e.button == 0 && resizeRect.Contains(e.mousePosition) ) { isResizingHeight = true; e.Use(); }
                if ( e.type == EventType.MouseDrag && isResizingHeight ) { customHeight += e.delta.y; }
                if ( e.rawType == EventType.MouseUp ) { isResizingHeight = false; }
            }

            proposedHeight = 0f;

            if ( ( ( keyable == null ) || !ReferenceEquals(keyable.parent, this) ) && !ReferenceEquals(keyable, this) ) {
                GUI.Label(expansionRect, "No Clip Selected", Styles.centerLabel);
                inspectedParameterIndex = -1;
                return;
            }

            if ( !showAddPropertyButton ) {
                if ( keyable is ActionClip && !( keyable as ActionClip ).isValid ) {
                    GUI.Label(expansionRect, "Clip Is Invalid", Styles.centerLabel);
                    return;
                }

                if ( keyable.animationData == null || !keyable.animationData.isValid ) {
                    if ( keyable is ActionClip ) {
                        GUI.Label(expansionRect, "Clip Has No Animatable Parameters", Styles.centerLabel);
                        return;
                    }
                }
            }

            proposedHeight = defaultHeight + PARAMS_TOP_MARGIN;
            if ( keyable.animationData != null && keyable.animationData.animatedParameters != null ) {
                if ( inspectedParameterIndex >= keyable.animationData.animatedParameters.Count ) {
                    inspectedParameterIndex = -1;
                }

                var paramsCount = keyable.animationData.animatedParameters.Count;
                for ( var i = 0; i < paramsCount; i++ ) {
                    var animParam = keyable.animationData.animatedParameters[i];
                    var paramRect = new Rect(expansionRect.xMin + 4, proposedHeight, expansionRect.width - 8, PARAMS_LINE_HEIGHT);
                    proposedHeight += PARAMS_LINE_HEIGHT + PARAMS_LINE_MARGIN;

                    GUI.color = inspectedParameterIndex == i ? new Color(0.5f, 0.5f, 1f, 0.4f) : new Color(0, 0.5f, 0.5f, 0.5f);
                    GUI.Box(paramRect, string.Empty, Styles.headerBoxStyle);
                    GUI.color = Color.white;

                    var paramName = string.Format(" <size=10><color=#252525>{0}</color></size>", animParam.ToString());
                    paramName = inspectedParameterIndex == i ? string.Format("<b>{0}</b>", paramName) : paramName;
                    GUI.Label(paramRect, paramName, Styles.leftLabel);

                    var gearRect = new Rect(paramRect.xMax - 16 - 4, paramRect.y, 16, 16);
                    gearRect.center = new Vector2(gearRect.center.x, paramRect.y + ( paramRect.height / 2 ) - 1);
                    GUI.enabled = true;
                    GUI.color = Color.white.WithAlpha(animParam.enabled ? 1 : 0.5f);
                    if ( GUI.Button(gearRect, Styles.gearIcon, GUIStyle.none) ) {
                        AnimatableParameterEditor.DoParamGearContextMenu(animParam, keyable);
                    }
                    GUI.enabled = animParam.enabled;
                    if ( GUI.Button(paramRect, string.Empty, GUIStyle.none) ) {
                        inspectedParameterIndex = inspectedParameterIndex == i ? -1 : i;
                        CurveEditor.FrameAllCurvesOf(animParam);
                    }
                    GUI.color = Color.white;
                    GUI.enabled = true;
                }

                proposedHeight += PARAMS_TOP_MARGIN;

                if ( inspectedParameterIndex >= 0 ) {
                    var controlRect = Rect.MinMaxRect(expansionRect.x + 6, proposedHeight + 5, expansionRect.xMax - 6, proposedHeight + 50);
                    var animParam = keyable.animationData.animatedParameters[inspectedParameterIndex];
                    GUILayout.BeginArea(controlRect);
                    AnimatableParameterEditor.ShowMiniParameterKeyControls(animParam, keyable);
                    GUILayout.EndArea();
                    proposedHeight = controlRect.yMax + 10;
                }
            }

            if ( showAddPropertyButton && inspectedParameterIndex == -1 ) {
                var buttonRect = Rect.MinMaxRect(expansionRect.x + 6, proposedHeight + 5, expansionRect.xMax - 6, proposedHeight + 25);
                var go = keyable.animatedParametersTarget as GameObject;
                GUI.enabled = go != null && root.currentTime <= 0;
                if ( GUI.Button(buttonRect, "Add Property") ) {
                    EditorTools.ShowAnimatedPropertySelectionMenu(go, keyable.TryAddParameter);
                }
                GUI.enabled = true;
                proposedHeight = buttonRect.yMax + 10;
            }

            //consume event
            if ( e.type == EventType.MouseDown && expansionRect.Contains(e.mousePosition) ) {
                e.Use();
            }
        }



        ///<summary>The Editor GUI within the timeline rectangle</summary>
        virtual public void OnTrackTimelineGUI(Rect posRect, Rect timeRect, float cursorTime, System.Func<float, float> TimeToPos) {
            var e = Event.current;

            var clipsPosRect = Rect.MinMaxRect(posRect.xMin, posRect.yMin, posRect.xMax, posRect.yMin + defaultHeight);
            DoTrackContextMenu(e, clipsPosRect, cursorTime);

            if ( showCurves ) {
                var curvesPosRect = Rect.MinMaxRect(posRect.xMin, clipsPosRect.yMax, posRect.xMax, posRect.yMax);
                DoClipCurves(e, curvesPosRect, timeRect, TimeToPos, showCurvesClip);
            }
        }


        void DoTrackContextMenu(Event e, Rect clipsPosRect, float cursorTime) {
            if ( e.type == EventType.ContextClick && clipsPosRect.Contains(e.mousePosition) ) {

                var attachableTypeInfos = new List<EditorTools.TypeMetaInfo>();

                var existing = clips.FirstOrDefault();
                var existingCatAtt = existing != null ? existing.GetType().GetCustomAttributes(typeof(CategoryAttribute), true).FirstOrDefault() as CategoryAttribute : null;
                foreach ( var info in EditorTools.GetTypeMetaDerivedFrom(typeof(ActionClip)) ) {

                    if ( !info.attachableTypes.Contains(this.GetType()) ) {
                        continue;
                    }

                    if ( existingCatAtt != null ) {
                        if ( existingCatAtt.category == info.category ) {
                            attachableTypeInfos.Add(info);
                        }
                    } else {
                        attachableTypeInfos.Add(info);
                    }
                }

                if ( attachableTypeInfos.Count > 0 ) {
                    var menu = new UnityEditor.GenericMenu();
                    foreach ( var _info in attachableTypeInfos ) {
                        var info = _info;
                        var category = string.IsNullOrEmpty(info.category) ? string.Empty : ( info.category + "/" );
                        var tName = info.name;
                        menu.AddItem(new GUIContent(category + tName), false, () => { AddAction(info.type, cursorTime); });
                    }

                    var copyType = CutsceneUtility.GetCopyType();
                    if ( copyType != null && attachableTypeInfos.Select(i => i.type).Contains(copyType) ) {
                        menu.AddSeparator("/");
                        menu.AddItem(new GUIContent(string.Format("Paste Clip ({0})", copyType.Name)), false, () => { CutsceneUtility.PasteClip(this, cursorTime); });
                    }

                    menu.ShowAsContext();
                    e.Use();
                }
            }
        }

        protected void DoClipCurves(Event e, Rect posRect, Rect timeRect, System.Func<float, float> TimeToPos, IKeyable keyable) {

            //track expanded bg
            GUI.color = Color.black.WithAlpha(0.1f);
            GUI.Box(posRect, string.Empty, Styles.timeBoxStyle);
            GUI.color = Color.white;

            if ( ( ( keyable == null ) || !ReferenceEquals(keyable.parent, this) ) && !ReferenceEquals(keyable, this) ) {
                GUI.color = Color.white.WithAlpha(0.3f);
                GUI.Label(posRect, "Select a Clip of this Track to view it's Animated Parameters here", Styles.centerLabel);
                GUI.color = Color.white;
                return;
            }

            var finalPosRect = posRect;
            var finalTimeRect = timeRect;

            //adjust rects
            if ( keyable is ActionClip ) {
                finalPosRect.xMin = Mathf.Max(posRect.xMin, TimeToPos(keyable.startTime));
                finalPosRect.xMax = Mathf.Min(posRect.xMax, TimeToPos(keyable.endTime));
                finalTimeRect.xMin = Mathf.Max(timeRect.xMin, keyable.startTime) - keyable.startTime;
                finalTimeRect.xMax = Mathf.Min(timeRect.xMax, keyable.endTime) - keyable.startTime;
            }

            //add some top/bottom margins
            finalPosRect.yMin += 1;
            finalPosRect.yMax -= 3;
            finalPosRect.width = Mathf.Max(finalPosRect.width, 5);

            //dark bg
            GUI.color = Color.black.WithAlpha(0.4f);
            GUI.DrawTexture(posRect, Styles.whiteTexture);
            GUI.color = Color.white;


            //out of view range
            if ( keyable is ActionClip ) {
                if ( keyable.startTime > timeRect.xMax || keyable.endTime < timeRect.xMin ) {
                    return;
                }
            }


            //keyable bg
            GUI.color = UnityEditor.EditorGUIUtility.isProSkin ? new Color(0.25f, 0.25f, 0.25f, 0.9f) : new Color(0.7f, 0.7f, 0.7f, 0.9f);
            GUI.Box(finalPosRect, string.Empty, Styles.clipBoxFooterStyle);
            GUI.color = Color.white;

            //if too small do nothing more
            if ( finalPosRect.width <= 5 ) {
                return;
            }

            if ( keyable is ActionClip && !( keyable as ActionClip ).isValid ) {
                GUI.Label(finalPosRect, "Clip Is Invalid", Styles.centerLabel);
                return;
            }

            if ( keyable.animationData == null || !keyable.animationData.isValid ) {
                if ( keyable is ActionClip ) {
                    GUI.Label(finalPosRect, "Clip has no Animatable Parameters", Styles.centerLabel);
                } else {
                    GUI.Label(finalPosRect, "Track has no Animated Properties. You can add some on the left side", Styles.centerLabel);
                }
                return;
            }

            if ( inspectedParameterIndex >= keyable.animationData.animatedParameters.Count ) {
                inspectedParameterIndex = -1;
            }


            //vertical guides from params to dopesheet
            if ( inspectedParameterIndex == -1 ) {
                var yPos = PARAMS_TOP_MARGIN;
                for ( var i = 0; i < keyable.animationData.animatedParameters.Count; i++ ) {
                    // var animParam = keyable.animationData.animatedParameters[i];
                    var paramRect = new Rect(0, posRect.yMin + yPos, finalPosRect.xMin - 2, PARAMS_LINE_HEIGHT);
                    yPos += PARAMS_LINE_HEIGHT + PARAMS_LINE_MARGIN;
                    paramRect.yMin += 1f;
                    paramRect.yMax -= 1f;
                    GUI.color = new Color(0, 0.5f, 0.5f, 0.1f);
                    GUI.DrawTexture(paramRect, Styles.whiteTexture);
                    GUI.color = Color.white;
                }
            }


            //begin in group and neutralize rect
            GUI.BeginGroup(finalPosRect);
            finalPosRect = new Rect(0, 0, finalPosRect.width, finalPosRect.height);

            if ( inspectedParameterIndex == -1 ) {
                var yPos = PARAMS_TOP_MARGIN;
                for ( var i = 0; i < keyable.animationData.animatedParameters.Count; i++ ) {
                    var animParam = keyable.animationData.animatedParameters[i];
                    var paramRect = new Rect(finalPosRect.xMin, finalPosRect.yMin + yPos, finalPosRect.width, PARAMS_LINE_HEIGHT);
                    yPos += PARAMS_LINE_HEIGHT + PARAMS_LINE_MARGIN;
                    paramRect.yMin += 1f;
                    paramRect.yMax -= 1f;
                    GUI.color = Color.black.WithAlpha(0.05f);
                    GUI.DrawTexture(paramRect, Texture2D.whiteTexture);
                    UnityEditor.Handles.color = Color.black.WithAlpha(0.2f);
                    UnityEditor.Handles.DrawLine(new Vector2(paramRect.xMin, paramRect.yMin), new Vector2(paramRect.xMax, paramRect.yMin));
                    UnityEditor.Handles.DrawLine(new Vector2(paramRect.xMin, paramRect.yMax), new Vector2(paramRect.xMax, paramRect.yMax));
                    UnityEditor.Handles.color = Color.white;
                    GUI.color = Color.white;

                    if ( animParam.enabled ) {
                        DopeSheetEditor.DrawDopeSheet(animParam, keyable, paramRect, finalTimeRect.x, finalTimeRect.width, true);
                    } else {
                        GUI.color = new Color(0, 0, 0, 0.2f);
                        GUI.DrawTextureWithTexCoords(paramRect, Styles.stripes, new Rect(0, 0, paramRect.width / 7, paramRect.height / 7));
                        GUI.color = Color.white;
                    }
                }
            }

            if ( inspectedParameterIndex >= 0 ) {
                var animParam = keyable.animationData.animatedParameters[inspectedParameterIndex];
                var dopeRect = finalPosRect;
                dopeRect.y += 4f;
                dopeRect.height = 16f;
                DopeSheetEditor.DrawDopeSheet(animParam, keyable, dopeRect, finalTimeRect.x, finalTimeRect.width, true);
                var curveRect = finalPosRect;
                curveRect.yMin = dopeRect.yMax + 4;
                UnityEditor.Handles.color = Color.black.WithAlpha(0.5f);
                UnityEditor.Handles.DrawLine(new Vector2(curveRect.xMin, curveRect.yMin), new Vector2(curveRect.xMax, curveRect.yMin));
                UnityEditor.Handles.color = Color.white;
                CurveEditor.DrawCurves(animParam, keyable, curveRect, finalTimeRect);
            }

            //consume event
            if ( e.type == EventType.MouseDown && finalPosRect.Contains(e.mousePosition) ) {
                e.Use();
            }

            GUI.EndGroup();

            /*
                        //darken out of clip range time
                        //will use if I make curve editing full-width
                        if (Prefs.fullWidthCurveEditing){
                            var darkBefore = Rect.MinMaxRect( posRect.xMin, posRect.yMin, Mathf.Max(posRect.xMin, TimeToPos(keyable.startTime)), posRect.yMax );
                            var darkAfter = Rect.MinMaxRect( Mathf.Min(posRect.xMax, TimeToPos(keyable.endTime)), posRect.yMin, posRect.xMax, posRect.yMax );
                            GUI.color = new Color(0.1f,0.1f,0.1f,0.6f);
                            GUI.DrawTexture(darkBefore, Styles.whiteTexture);
                            GUI.DrawTexture(darkAfter, Styles.whiteTexture);
                            GUI.color = Color.white;
                        }
            */

        }

#endif
    }
}