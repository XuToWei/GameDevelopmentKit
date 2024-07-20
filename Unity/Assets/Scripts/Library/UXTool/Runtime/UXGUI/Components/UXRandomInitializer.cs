using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UXRandomInitializer : MonoBehaviour
{
    public float PositionRandomRate;
    public float PositionShakeFreq;
    public Vector3 RandomPositionRangeX;
    public Vector3 RandomPositionRangeY;
    public Vector3 RandomPositionRangeZ;

    public float RotationRandomRate;
    public float RotationShakeFreq;
    public bool RotationUseWorldSpace;
    public bool RotationRepeat;
    public Vector3 RandomRotationRangeX;
    public Vector3 RandomRotationRangeY;
    public Vector3 RandomRotationRangeZ;

    public float ScaleRandomRate;
    public float ScaleShakeFreq;
    public Vector3 RandomScaleRangeX = new Vector3(1, 1, 0); // 默认值为1防止用户在未设置的情况下看不到物体
    public Vector3 RandomScaleRangeY = new Vector3(1, 1, 0); // 默认值为1防止用户在未设置的情况下看不到物体
    public Vector3 RandomScaleRangeZ = new Vector3(1, 1, 0); // 默认值为1防止用户在未设置的情况下看不到物体

    public float AlphaRandomRate;
    public float AlphaShakeFreq;
    public Vector3 RandomAlphaRange = new Vector3(1, 1, 0); // 默认值为1防止用户在未设置的情况下看不到物体

    public float ColorRandomRate;
    public float ColorShakeFreq;
    public bool ColorShakeAlpha;
    public List<Color> RandomColors = new List<Color>() { Color.white }; // 默认值为白色防止用户在未设置的情况下看不到物体

    private RectTransform m_RectTransform;
    private CanvasGroup m_CanvasGroup;
    private UXImage m_UXImage;

    private float ShakeFloat(Vector2 range, int index, float rate, float time, float freq, bool useSin, bool backAndForth)
    {
        // rate为0时，均匀抖动
        float dt = 0;
        float center = 0;
        if (useSin)
        {
            // 正弦函数
            dt = Mathf.Sin(time * (2.0f * Mathf.PI * freq)) / 2.0f + 0.5f;
        }
        else
        {
            // 匀速运动
            dt = time * freq;
            dt = backAndForth ? Mathf.PingPong(dt, 1) : Mathf.Repeat(dt, 1);
        }
        center = Mathf.Lerp(range.x, range.y, dt);

        // rate为1时，随机抖动
        var nt = time * freq;
        // 使用Perlin Noise和Sin函数混合，以提高极值点出现的概率
        var perlin = Mathf.PerlinNoise(nt, index) * Mathf.PI * 4.0f;
        var noise = Mathf.Lerp(range.x, range.y, Mathf.Sin(perlin) * 0.5f + 0.5f);

        var result = center * (1 - rate) + noise * rate;
        return Mathf.Clamp(result, range.x, range.y);
    }

    private float GetRatio(Vector3 range)
    {
        var d = range.y - range.x;
        return d == 0 ? 0 : range.z / d;
    }

    void Update()
    {
        if (m_RectTransform == null)
        {
            return;
        }

        // 随机位置
        if (PositionShakeFreq != 0)
        {
            var period = 0.5f / PositionShakeFreq;
            m_RectTransform.localPosition = new Vector3(
                ShakeFloat(RandomPositionRangeX, 0, PositionRandomRate, Time.time + GetRatio(RandomPositionRangeX) * period, PositionShakeFreq, true, true),
                ShakeFloat(RandomPositionRangeY, 1, PositionRandomRate, Time.time + GetRatio(RandomPositionRangeY) * period, PositionShakeFreq, true, true),
                ShakeFloat(RandomPositionRangeZ, 2, PositionRandomRate, Time.time + GetRatio(RandomPositionRangeZ) * period, PositionShakeFreq, true, true));
        }

        // 随机缩放
        if (ScaleShakeFreq != 0)
        {
            var period = 0.5f / ScaleShakeFreq;
            m_RectTransform.localScale = new Vector3(
                ShakeFloat(RandomScaleRangeX, 0, ScaleRandomRate, Time.time + GetRatio(RandomScaleRangeX) * period, ScaleShakeFreq, true, true),
                ShakeFloat(RandomScaleRangeY, 1, ScaleRandomRate, Time.time + GetRatio(RandomScaleRangeY) * period, ScaleShakeFreq, true, true),
                ShakeFloat(RandomScaleRangeZ, 2, ScaleRandomRate, Time.time + GetRatio(RandomScaleRangeZ) * period, ScaleShakeFreq, true, true));
        }

        // 随机旋转
        if (RotationShakeFreq != 0)
        {
            var period = 0.5f / RotationShakeFreq;
            var rotation = new Vector3(
                ShakeFloat(RandomRotationRangeX, 0, RotationRandomRate, Time.time + GetRatio(RandomRotationRangeX) * period, RotationShakeFreq, false, RotationRepeat),
                ShakeFloat(RandomRotationRangeY, 1, RotationRandomRate, Time.time + GetRatio(RandomRotationRangeY) * period, RotationShakeFreq, false, RotationRepeat),
                ShakeFloat(RandomRotationRangeZ, 2, RotationRandomRate, Time.time + GetRatio(RandomRotationRangeZ) * period, RotationShakeFreq, false, RotationRepeat));
            if (RotationUseWorldSpace)
            {
                m_RectTransform.eulerAngles = rotation;
            }
            else
            {
                m_RectTransform.localEulerAngles = rotation;
            }
        }

        // 随机CanvasGroup透明度
        if (m_CanvasGroup != null && AlphaShakeFreq != 0)
        {
            var period = 0.5f / AlphaShakeFreq;
            m_CanvasGroup.alpha = ShakeFloat(RandomAlphaRange, 0, AlphaRandomRate, Time.time + GetRatio(RandomAlphaRange) * period, AlphaShakeFreq, true, true);
        }

        // 随机UXImage颜色
        if (m_UXImage != null && ColorShakeFreq != 0)
        {
            if (RandomColors.Count == 0)
            {
                return;
            }
            if (RandomColors.Count == 1)
            {
                m_UXImage.color = RandomColors[0];
                return;
            }
            var period = 0.5f / ColorShakeFreq;
            var ratio = ShakeFloat(new Vector2(0, 1), 0, ColorRandomRate, Time.time + GetRatio(Vector3.zero) * period, ColorShakeFreq, true, true);
            var index = Mathf.FloorToInt(ratio * (RandomColors.Count - 1));
            var t = ratio * (RandomColors.Count - 1) - index;
            var color = index < RandomColors.Count - 1
                ? Color.Lerp(RandomColors[index], RandomColors[index + 1], t)
                : RandomColors[index];
            color.a = ColorShakeAlpha ? color.a : m_UXImage.color.a;
            m_UXImage.color = color;
        }
    }

    void Start()
    {
        m_RectTransform = GetComponent<RectTransform>();
        m_CanvasGroup = GetComponent<CanvasGroup>();
        m_UXImage = GetComponent<UXImage>();
    }
}