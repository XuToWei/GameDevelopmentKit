using UnityEngine;

public class UIAdapterScaleScreenRate : MonoBehaviour
{
    public float rateMinWidth, rateMinHeight = 1, rateMaxWidth = 2040, rateMaxHeight = 1080;

    public float scaleVal = 0.9f;
    // Start is called before the first frame update

    private bool inited;
    private Vector3 originScale;
    void OnEnable()
    {
        if (!inited)
        {
            var transform = GetComponent<RectTransform>();
            if (transform != null)
            {
                originScale = transform.localScale;
                inited = true;
            }
        }

        Rect safeArea = UIAdapter.GetSafeArea();

        float rate = 0;
        if (safeArea.width != 0 && safeArea.height != 0)
        {
            rate = safeArea.width / (float)safeArea.height;
        }
        else
        {
            int screenWidth = Screen.width;
            int screenHeight = Screen.height;
            rate = screenWidth / (float)screenHeight;
        }

        float rateMin = rateMinWidth / rateMinHeight;
        float rateMax = rateMaxWidth / rateMaxHeight;
        if (rate >= rateMin && rate <= rateMax)
        {
            transform.localScale = originScale * scaleVal;
        }
    }
}
