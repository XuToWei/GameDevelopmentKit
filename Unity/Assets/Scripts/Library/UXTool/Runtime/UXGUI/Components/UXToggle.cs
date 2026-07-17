namespace UnityEngine.UI
{
    [AddComponentMenu("UI/UXToggle", 47)]
    [RequireComponent(typeof(RectTransform))]
    public class UXToggle : Toggle
    {
        [SerializeField]
        private Animator m_ToggleAnimator;

        protected override void Awake()
        {
            base.Awake();
            if (m_ToggleAnimator != null)
            {
                onValueChanged.RemoveListener(PlayToggleAnimation);
                onValueChanged.AddListener(PlayToggleAnimation);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayToggleAnimation(isOn);
        }

        private void PlayToggleAnimation(bool toggleIsOn)
        {
            if (m_ToggleAnimator == null || !m_ToggleAnimator.isActiveAndEnabled)
                return;
            m_ToggleAnimator.Play(toggleIsOn ? "anim_toggle_up" : "anim_toggle_down");
        }
    }
}
