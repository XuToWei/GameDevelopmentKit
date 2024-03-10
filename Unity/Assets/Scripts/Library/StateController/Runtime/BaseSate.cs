using Sirenix.OdinInspector;

namespace StateController
{
    public abstract class BaseSate : SerializedMonoBehaviour
    {
        internal abstract void Refresh();

#if UNITY_EDITOR
        internal abstract void EditorRefresh();
#endif
    }
}