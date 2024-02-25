using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using System.Text.RegularExpressions;

namespace UnityEngine.UI
{
    /// <summary>
    /// Simple toggle -- something that has an 'on' and 'off' states: checkbox, toggle button, radio button, etc.
    /// </summary>
    [AddComponentMenu("UI/UXToggle", 47)]
    [RequireComponent(typeof(RectTransform))]
    public class UXToggle : Selectable, IPointerClickHandler, ISubmitHandler, ICanvasElement
    {
        //static string ANIM_STATENAME_ON = "anim_toggle_up";
        //static string ANIM_STATENAME_OFF = "anim_toggle_down";
        static Regex ANIM_STATENAME_OPEN_REGEX = new Regex("anim_[\\s\\w_]+?_up");
        static Regex ANIM_STATENAME_CLOSE_REGEX = new Regex("anim_[\\s\\w_]+?_down");
        private string ANIM_STATENAME_OPEN;
        private string ANIM_STATENAME_CLOSE;

        //public string audioEventName = string.Empty;
        public bool HideTargetGraphic = false;

        [RangeAttribute(0, 5)]
        public float minClickInterval = 0.2f;

        [RangeAttribute(0, 5)]
        public float fadeTime = 0.1f;

        [SerializeField]
        //public string m_BindInputAction = string.Empty;

        //public bool CursorHoverControl = true;



        //zrh 这个不要序列化进prefab
        [System.NonSerialized]
        float lastClickTime = 0;

        public enum ToggleTransition
        {
            None,
            Fade
        }

        public enum ToggleColorBlockMode
        {
            Normal, //default Off
            NonInteractiveSimulation, // disabled
            ToggleOn, // On
        }
        [Serializable]
        public class ToggleClickedEvent : UnityEvent
        {

        }
        [Serializable]
        public class ToggleEvent : UnityEvent<bool>
        { }

        //zrh 这个不要序列化进prefab
        [System.NonSerialized]
        public int id;
        /// <summary>
        /// Transition type.
        /// </summary>
        public ToggleTransition toggleTransition = ToggleTransition.Fade;

        /// <summary>
        /// Graphic the toggle should be working with.
        /// </summary>
        public Graphic graphic;

        [System.NonSerialized]
        public ColorBlock nonInteractiveSimulationColors = new ColorBlock();
        [System.NonSerialized]
        public ColorBlock originalColors = new ColorBlock();

        //不能把随便把toggle on定义为 选中, toggle on/off 应该有独立的一套配色
        public ColorBlock toggleOnColors = new ColorBlock();

        private ToggleColorBlockMode colorBlockMode;

        // group that this toggle can belong to
        [SerializeField]
        private UXToggleGroup m_Group;

        public Animation anim_toggle;

        // 只响应点击不进行toggle
        [System.NonSerialized]
        public bool interceptToggle = false;
        [System.NonSerialized]
        public UnityEvent onInterceptToggleClick = new UnityEvent();

        private ToggleClickedEvent m_OnClickRightButton = new ToggleClickedEvent();

        //isOn=True的时候再次点击执行时依然sendCallback
        [System.NonSerialized]
        public bool sendCallbackIfAlreadyOn = false;

#if UNITY_EDITOR
        //[XLua.BlackList]
        public void InitalizeToggleOnColorFromDefaultColor()
        {
            toggleOnColors = colors;
        }
#endif
        protected override void Awake()
        {
            base.Awake();
            id = Random.Range(int.MinValue, int.MaxValue);
            if (anim_toggle != null && anim_toggle.GetClipCount() > 0)
            {
                foreach (AnimationState state in anim_toggle)
                {
                    if (ANIM_STATENAME_OPEN_REGEX.IsMatch(state.name))
                    {
                        ANIM_STATENAME_OPEN = state.name;
                    }
                    else if (ANIM_STATENAME_CLOSE_REGEX.IsMatch(state.name))
                    {
                        ANIM_STATENAME_CLOSE = state.name;
                    }

                }
            }

            nonInteractiveSimulationColors.colorMultiplier = this.colors.colorMultiplier;
            nonInteractiveSimulationColors.fadeDuration = this.colors.fadeDuration;
            nonInteractiveSimulationColors.normalColor = this.colors.disabledColor;
            nonInteractiveSimulationColors.pressedColor = this.colors.disabledColor;
            nonInteractiveSimulationColors.disabledColor = this.colors.disabledColor;
            nonInteractiveSimulationColors.highlightedColor = this.colors.disabledColor;
            nonInteractiveSimulationColors.selectedColor = this.colors.selectedColor;

            originalColors.colorMultiplier = this.colors.colorMultiplier;
            originalColors.fadeDuration = this.colors.fadeDuration;
            originalColors.normalColor = this.colors.normalColor;
            originalColors.pressedColor = this.colors.pressedColor;
            originalColors.disabledColor = this.colors.disabledColor;
            originalColors.highlightedColor = this.colors.highlightedColor;
            originalColors.selectedColor = this.colors.selectedColor;

            this.ChangeColorBlock(ToggleColorBlockMode.Normal);
        }

        public UXToggleGroup group
        {
            get { return m_Group; }
            set
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
#endif
                {
                    SetToggleGroup(value, true);
                    PlayEffect(true);
                }
                m_Group = value;
            }
        }

        /// <summary>
        /// Allow for delegate-based subscriptions for faster events than 'eventReceiver', and allowing for multiple receivers.
        /// </summary>
        public ToggleEvent onValueChanged = new ToggleEvent();

        // Whether the toggle is on
        [FormerlySerializedAs("m_IsActive")]
        [Tooltip("Is the toggle currently on or off?")]
        [SerializeField]
        private bool m_IsOn;

        protected UXToggle()
        {

        }
        public ToggleClickedEvent onClickRightButton
        {
            get { return m_OnClickRightButton; }
            set { m_OnClickRightButton = value; }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            var prefabType = UnityEditor.PrefabUtility.GetPrefabAssetType(this);
            if (prefabType == UnityEditor.PrefabAssetType.NotAPrefab && !Application.isPlaying)
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        }

#endif

        public virtual void Rebuild(CanvasUpdate executing)
        {
#if UNITY_EDITOR
            if (executing == CanvasUpdate.Prelayout)
                onValueChanged.Invoke(m_IsOn);
#endif
        }

        public virtual void LayoutComplete()
        {
        }

        public virtual void GraphicUpdateComplete()
        {
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (!interactable)
                ChangeColorBlock(ToggleColorBlockMode.NonInteractiveSimulation);
            else
            {
                ChangeColorBlock(m_IsOn
                    ? ToggleColorBlockMode.ToggleOn
                    : ToggleColorBlockMode.Normal);
            }
            SetToggleGroup(m_Group, false);
            PlayEffect(true);
        }

        protected override void OnDisable()
        {
            // 关掉Toggle所在Panel时要将Toggle Group的选中状态清空
            if (m_Group != null && m_Group.selectId == id)
            {
                m_Group.selectId = -1;
            }

            SetToggleGroup(null, false);
            m_IsOn = false;
            if (!interactable)
                ChangeColorBlock(ToggleColorBlockMode.NonInteractiveSimulation);
            else
            {
                ChangeColorBlock(m_IsOn ? ToggleColorBlockMode.ToggleOn : ToggleColorBlockMode.Normal);
            }
            base.OnDisable();
            //暂时去掉鼠标的操作表现
            //TODO:统一加上鼠标的操作表现 区分平台类型
            /*
            if (CursorHoverControl)
            {
                var cursorController = CursorController.Instance;
                if (cursorController)
                {
                    cursorController.SetHoverObj(this.gameObject, true);
                }
            }
            */
        }

        protected override void OnDidApplyAnimationProperties()
        {
            // Check if isOn has been changed by the animation.
            // Unfortunately there is no way to check if we don�t have a graphic.
            if (graphic != null)
            {
                bool oldValue = !Mathf.Approximately(graphic.canvasRenderer.GetColor().a, 0);
                if (m_IsOn != oldValue)
                {
                    m_IsOn = oldValue;
                    Set(!oldValue);
                }
            }

            base.OnDidApplyAnimationProperties();
        }

        private void SetToggleGroup(UXToggleGroup newGroup, bool setMemberValue)
        {
            UXToggleGroup oldGroup = m_Group;

            // Sometimes IsActive returns false in OnDisable so don't check for it.
            // Rather remove the toggle too often than too little.
            if (m_Group != null)
                m_Group.UnregisterToggle(this);

            // At runtime the group variable should be set but not when calling this method from OnEnable or OnDisable.
            // That's why we use the setMemberValue parameter.
            if (setMemberValue)
                m_Group = newGroup;

            // Only register to the new group if this Toggle is active.
            if (newGroup != null && IsActive())
                newGroup.RegisterToggle(this);

            // If we are in a new group, and this toggle is on, notify group.
            // Note: Don't refer to m_Group here as it's not guaranteed to have been set.
            if (newGroup != null && newGroup != oldGroup && isOn && IsActive())
                newGroup.NotifyToggleOn(id);
        }

        /// <summary>
        /// Whether the toggle is currently active.
        /// </summary>
        public bool isOn
        {
            get { return m_IsOn; }
            set
            {
                Set(value);
            }
        }

        void Set(bool value)
        {
            Set(value, true);
        }

        void Set(bool value, bool sendCallback)
        {
            if (m_IsOn == value)
            {
                return;
            }

            //在组中就不能拦了，会出问题
            if (m_Group != null)
            {
                realSet(value, sendCallback);
                if (Time.time - lastClickTime <= minClickInterval)
                {
                    return;
                }
                lastClickTime = Time.time;
            }
            else
            {
                if (Time.time - lastClickTime <= minClickInterval)
                {
                    return;
                }
                lastClickTime = Time.time;
                realSet(value, sendCallback);
            }
        }

        public void ChangeColorBlock(ToggleColorBlockMode mode)
        {
            colorBlockMode = mode;
            switch (mode)
            {
                case ToggleColorBlockMode.Normal:
                    this.colors = this.originalColors;
                    break;
                case ToggleColorBlockMode.NonInteractiveSimulation:
                    this.colors = this.nonInteractiveSimulationColors;
                    break;
                case ToggleColorBlockMode.ToggleOn:
                    this.colors = this.toggleOnColors;
                    break;
                default:
                    this.colors = this.originalColors;
                    break;
            }
        }

        public ToggleColorBlockMode GetColorBlockMode()
        {
            return this.colorBlockMode;
        }

        // 忽略间隔点击时间 不播放音效 不调用回调
        // 用于在不满足toggle状态变化条件时置回原有状态
        public void LXForceSet(bool value)
        {
            if (m_IsOn == value)
            {
                return;
            }

            //在组中就不能拦了，会出问题
            if (m_Group != null)
            {
                realSet(value, false);
                lastClickTime = Time.time;
            }
            else
            {
                lastClickTime = Time.time;
                realSet(value, false);
            }
        }

        void realSet(bool value, bool sendCallback)
        {
            //Debug.Log("LxToggle  " + name + "  Stoped");
            //return;
            // if we are in a group and set to true, do group logic
            m_IsOn = value;
            if (m_Group != null && IsActive())
            {
                //|| (!m_Group.AnyTogglesOn() && !m_Group.allowSwitchOff)
                if (m_IsOn)
                {
                    m_IsOn = true;
                    m_Group.NotifyToggleOn(id);
                }
            }

            // Always send event when toggle is clicked, even if value didn't change
            // due to already active toggle in a toggle group being clicked.
            // Controls like Dropdown rely on this.
            // It's up to the user to ignore a selection being set to the same value it already was, if desired.
            PlayEffect(toggleTransition == ToggleTransition.None);

            if (sendCallback)
            {
                onValueChanged.Invoke(m_IsOn);
            }

            //on了就选中了
            if (value)
            {
                ChangeColorBlock(ToggleColorBlockMode.ToggleOn);
            }
            else
            {
                if (colorBlockMode == ToggleColorBlockMode.ToggleOn)
                    ChangeColorBlock(ToggleColorBlockMode.Normal);
                else
                {
                    ChangeColorBlock(colorBlockMode);
                }
            }
            DoStateTransition(currentSelectionState, true);
        }

        /// <summary>
        /// Play the appropriate effect.
        /// </summary>
        private void PlayEffect(bool instant)
        {
            if (graphic == null)
                return;
            if (anim_toggle != null)
            {
                if (m_IsOn)
                {
                    anim_toggle.Play(ANIM_STATENAME_OPEN);
                }
                else
                {
                    anim_toggle.Play(ANIM_STATENAME_CLOSE);
                }
            }
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                SetCanvasRenderAlpha(graphic.transform, instant, m_IsOn, true);
                if (targetGraphic != null && HideTargetGraphic)
                {
                    SetCanvasRenderAlpha(targetGraphic.transform, instant, !m_IsOn, true);
                }
            }
            else
            {
#endif
                SetCanvasRenderAlpha(graphic.transform, instant, m_IsOn, false);
                if (targetGraphic != null && HideTargetGraphic)
                {
                    SetCanvasRenderAlpha(targetGraphic.transform, instant, !m_IsOn, false);
                }
#if UNITY_EDITOR
            }
#endif

        }

        public void SetCanvasRenderAlpha(Transform transform, bool instant, bool isOn, bool isEditor)
        {
            Graphic[] graphics = transform.GetComponentsInChildren<Graphic>();
            foreach (Graphic graphic in graphics)
            {
                if (isEditor)
                {
                    graphic.canvasRenderer.SetAlpha(isOn ? 1f : 0f);
                }
                else
                {
                    graphic.CrossFadeAlpha(isOn ? 1f : 0f, instant ? 0 : fadeTime, true);
                }
            }
        }

        /// <summary>
        /// Assume the correct visual state.
        /// </summary>
        protected override void Start()
        {
            PlayEffect(true);
        }

        private void InternalToggle()
        {
            if (!IsActive() || !IsInteractable())
                return;

            // 拦截toggle
            if (interceptToggle)
            {
                onInterceptToggleClick.Invoke();
                return;
            }

            if (isOn && m_Group != null)
            {
                if (sendCallbackIfAlreadyOn)
                {
                    onValueChanged.Invoke(isOn);
                }
                return;
            }

            isOn = !isOn;
        }
        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);
        }

        /// <summary>
        /// React to clicks.
        /// </summary>
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                m_OnClickRightButton.Invoke();
            }

            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            InternalToggle();
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
        }
        public virtual void OnSubmit(BaseEventData eventData)
        {

            InternalToggle();
        }

        public class PointerEnterEvent : UnityEvent { }

        // Event delegates triggered on pointer enter.
        [SerializeField]
        private PointerEnterEvent m_OnPointerEnter = new PointerEnterEvent();
        public PointerEnterEvent OnPointerEnterEvent => m_OnPointerEnter;

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            OnPointerEnterEvent.Invoke();
            //暂时去掉鼠标的操作表现
            //TODO:统一加上鼠标的操作表现 区分平台类型
            /*
            if (CursorHoverControl)
            {
                var cursorController = CursorController.Instance;
                if (cursorController)
                {
                    cursorController.SetHoverObj(this.gameObject, false);
                }
            }
            */
        }

        public class PointerExitEvent : UnityEvent { }
        // Event delegates triggered on pointer enter.
        [SerializeField]
        private PointerExitEvent m_OnPointerExit = new PointerExitEvent();
        public PointerExitEvent OnPointerExitEvent => m_OnPointerExit;
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            OnPointerExitEvent.Invoke();
            //暂时去掉鼠标的操作表现
            //TODO:统一加上鼠标的操作表现 区分平台类型
            /*    
            //按住状态挪出去取消选中, 如果这个选中状态是点击之后的一种独立的状态, 我建议用toggle, 而不是button, 那个是有on/off, 选中就是on
            var currentMouse = Mouse.current;
            if (!isOn && currentMouse != null //有时候可能没有鼠标
                && currentMouse.leftButton.IsPressed() &&
                eventData.button == PointerEventData.InputButton.Left)
            {
                //Selectable 里 OnPointerDown会在鼠标按下时选中, 导致按着鼠标移开按钮之后还是处于选中状态, 这里清空一下
                //或者对于按钮来说可能可以覆盖 OnPointerDown, 在OnPointerUp中选中, 现在还不确定选中行为发生在什么时候比较合适
                if (UIManager.inst.CurrentSelected() == gameObject)
                {
                    UIManager.inst.Select(null); //clear select state
                }

                DoStateTransition(SelectionState.Normal, false);
            }
             
            
            if (CursorHoverControl)
            {
                var cursorController = CursorController.Instance;
                if (cursorController)
                {
                    cursorController.SetHoverObj(this.gameObject, true);
                }
            }
            */
        }
        public int GetSize()
        {
            return 1;
        }
        //public string GetActionName(int i)
        //{
        //    return m_BindInputAction;
        //}

        //暂时去掉UIStyles
        //TODO: 完成UIStyle的整体开发工作
        //style 属于这个按钮的表现类
        //public LXToggleStyles Style = LXToggleStyles.None;

    }
}
