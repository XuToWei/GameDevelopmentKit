using UnityEngine;
using UnityEngine.UI;

namespace UGF
{
    public abstract class BaseBuiltinForm : MonoBehaviour
    {
        [SerializeField] private CanvasScaler m_CanvasScaler;

        protected virtual void Awake()
        {
            m_CanvasScaler = gameObject.GetOrAddComponent<CanvasScaler>();
            float ratio = GameEntry.Screen.SafeArea.height / GameEntry.Screen.SafeArea.width;
            m_CanvasScaler.matchWidthOrHeight = ratio > GameEntry.Screen.StandardVerticalRatio ? 0 : 1;
        }

        protected virtual void Close()
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
