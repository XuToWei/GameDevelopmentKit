namespace UnityEngine.UI
{
    public class UXScrollRect : ScrollRect
    {
        [SerializeField]
        private Object m_ItemCell;
        public enum LayoutType
        {
            GridLayout,
            HorizontalLayout,
            VerticalLayout
        }
        [SerializeField]
        private LayoutType m_layoutType;
    }
}