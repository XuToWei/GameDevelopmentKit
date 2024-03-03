using Sirenix.OdinInspector;

namespace YIUIBind
{
    [LabelText("过度")]
    public enum UITransitionModeEnum
    {
        [LabelText("立即")]
        Instant,

        [LabelText("淡入淡出")]
        Fade,
    }
}