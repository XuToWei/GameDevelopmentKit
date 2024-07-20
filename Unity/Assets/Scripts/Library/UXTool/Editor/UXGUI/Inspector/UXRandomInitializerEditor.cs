using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(UXRandomInitializer))]
public class UXRandomInitializerEditor : Editor
{
    SerializedProperty m_PositionRandomRate;
    SerializedProperty m_PositionShakeFreq;
    SerializedProperty m_RandomPositionRangeX;
    SerializedProperty m_RandomPositionRangeY;
    SerializedProperty m_RandomPositionRangeZ;
    
    SerializedProperty m_RotationRandomRate;
    SerializedProperty m_RotationShakeFreq;
    SerializedProperty m_RotationUseWorldSpace;
    SerializedProperty m_RotationRepeat;
    SerializedProperty m_RandomRotationRangeX;
    SerializedProperty m_RandomRotationRangeY;
    SerializedProperty m_RandomRotationRangeZ;

    SerializedProperty m_ScaleRandomRate;
    SerializedProperty m_ScaleShakeFreq;
    SerializedProperty m_RandomScaleRangeX;
    SerializedProperty m_RandomScaleRangeY;
    SerializedProperty m_RandomScaleRangeZ;
    
    SerializedProperty m_AlphaRandomRate;
    SerializedProperty m_AlphaShakeFreq;
    SerializedProperty m_RandomAlphaRange;
    
    SerializedProperty m_ColorRandomRate;
    SerializedProperty m_ColorShakeFreq;
    SerializedProperty m_ColorShakeAlpha;
    SerializedProperty m_RandomColors;
    
    private CanvasGroup m_CanvasGroup;
    private UXImage m_UXImage;
    
    private bool m_ShowPosition;
    private bool m_ShowRotation;
    private bool m_ShowScale;
    private bool m_ShowAlpha;
    private bool m_ShowColor;

    private void OnEnable()
    {
        m_PositionRandomRate = serializedObject.FindProperty("PositionRandomRate");
        m_PositionShakeFreq = serializedObject.FindProperty("PositionShakeFreq");
        m_RandomPositionRangeX = serializedObject.FindProperty("RandomPositionRangeX");
        m_RandomPositionRangeY = serializedObject.FindProperty("RandomPositionRangeY");
        m_RandomPositionRangeZ = serializedObject.FindProperty("RandomPositionRangeZ");
        
        m_RotationRandomRate = serializedObject.FindProperty("RotationRandomRate");
        m_RotationShakeFreq = serializedObject.FindProperty("RotationShakeFreq");
        m_RotationUseWorldSpace = serializedObject.FindProperty("RotationUseWorldSpace");
        m_RotationRepeat = serializedObject.FindProperty("RotationRepeat");
        m_RandomRotationRangeX = serializedObject.FindProperty("RandomRotationRangeX");
        m_RandomRotationRangeY = serializedObject.FindProperty("RandomRotationRangeY");
        m_RandomRotationRangeZ = serializedObject.FindProperty("RandomRotationRangeZ");
        
        m_ScaleRandomRate = serializedObject.FindProperty("ScaleRandomRate");
        m_ScaleShakeFreq = serializedObject.FindProperty("ScaleShakeFreq");
        m_RandomScaleRangeX = serializedObject.FindProperty("RandomScaleRangeX");
        m_RandomScaleRangeY = serializedObject.FindProperty("RandomScaleRangeY");
        m_RandomScaleRangeZ = serializedObject.FindProperty("RandomScaleRangeZ");
        
        m_AlphaRandomRate = serializedObject.FindProperty("AlphaRandomRate");
        m_AlphaShakeFreq = serializedObject.FindProperty("AlphaShakeFreq");
        m_RandomAlphaRange = serializedObject.FindProperty("RandomAlphaRange");
        
        m_ColorRandomRate = serializedObject.FindProperty("ColorRandomRate");
        m_ColorShakeFreq = serializedObject.FindProperty("ColorShakeFreq");
        m_ColorShakeAlpha = serializedObject.FindProperty("ColorShakeAlpha");
        m_RandomColors = serializedObject.FindProperty("RandomColors");

        UXRandomInitializer  randomInitializer = target as UXRandomInitializer;
        m_CanvasGroup = randomInitializer.GetComponent<CanvasGroup>();
        m_UXImage = randomInitializer.GetComponent<UXImage>();
    }
    
    private Vector3 ClampRange(Vector3 range)
    {
        range.y = range.y > range.x ? range.y : range.x;
        range.z = Mathf.Clamp(range.z, 0, (range.y - range.x) * 2.0f);
        return range;
    }
    
    private Vector3 ClampAlphaRange(Vector3 range)
    {
        range.x = Mathf.Clamp(range.x, 0, 1);
        range.y = Mathf.Clamp(range.y, 0, 1);
        range.y = range.y > range.x ? range.y : range.x;
        range.z = Mathf.Clamp(range.z, 0, (range.y - range.x) * 2.0f);
        return range;
    }
    
    private Vector3 Vector3RangeField(string fieldName, Vector3 value)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(fieldName);
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("最小值");
        value.x = EditorGUILayout.FloatField(value.x);
        GUILayout.Label("最大值");
        value.y = EditorGUILayout.FloatField(value.y);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("偏移量");
        value.z = EditorGUILayout.Slider(value.z, 0, (value.y - value.x) * 2.0f);
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        return value;
    }

    private void ColorListPropertyField(string fieldName, SerializedProperty value)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(fieldName);
        EditorGUILayout.PropertyField(value, true);
        GUILayout.EndHorizontal();
    }
    
    private void BoolPropertyField(string fieldName, SerializedProperty property)
    {
        property.boolValue = EditorGUILayout.Toggle(fieldName, property.boolValue);
    }
    
    private void FloatPropertyField(string fieldName, SerializedProperty property, float leftValue, float rightValue)
    {
        property.floatValue = EditorGUILayout.Slider(fieldName, property.floatValue, leftValue, rightValue);
    }
    
    private void RangePropertyField(string fieldName, SerializedProperty property)
    {
        property.vector3Value = ClampRange(Vector3RangeField(fieldName, property.vector3Value));
    }
    
    private void AlphaRangePropertyField(string fieldName, SerializedProperty property)
    {
        property.vector3Value = ClampAlphaRange(Vector3RangeField(fieldName, property.vector3Value));
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        // 随机位置
        m_ShowPosition = EditorGUILayout.Foldout(m_ShowPosition, "随机位置");
        if (m_ShowPosition)
        {
            FloatPropertyField("随机率", m_PositionRandomRate, 0.0f, 1.0f);
            FloatPropertyField("频率", m_PositionShakeFreq, 0.0f, 10.0f);
            RangePropertyField("随机范围 X", m_RandomPositionRangeX);
            RangePropertyField("随机范围 Y", m_RandomPositionRangeY);
            RangePropertyField("随机范围 Z", m_RandomPositionRangeZ);
        }
        
        // 随机旋转
        GUILayout.Space(10);
        m_ShowRotation = EditorGUILayout.Foldout(m_ShowRotation, "随机旋转");
        if (m_ShowRotation)
        {
            FloatPropertyField("随机率", m_RotationRandomRate, 0.0f, 1.0f);
            FloatPropertyField("频率", m_RotationShakeFreq, 0.0f, 10.0f);
            BoolPropertyField("使用世界坐标", m_RotationUseWorldSpace);
            BoolPropertyField("来回往复", m_RotationRepeat);
            RangePropertyField("随机范围 X", m_RandomRotationRangeX);
            RangePropertyField("随机范围 Y", m_RandomRotationRangeY);
            RangePropertyField("随机范围 Z", m_RandomRotationRangeZ);
        }
        
        // 随机缩放
        GUILayout.Space(10);
        m_ShowScale = EditorGUILayout.Foldout(m_ShowScale, "随机缩放");
        if (m_ShowScale)
        {
            FloatPropertyField("随机率", m_ScaleRandomRate, 0.0f, 1.0f);
            FloatPropertyField("频率", m_ScaleShakeFreq, 0.0f, 10.0f);
            RangePropertyField("随机范围 X", m_RandomScaleRangeX);
            RangePropertyField("随机范围 Y", m_RandomScaleRangeY);
            RangePropertyField("随机范围 Z", m_RandomScaleRangeZ);
        }
        
        // 随机CanvasGroup透明度
        if (m_CanvasGroup != null)
        {
            GUILayout.Space(10);
            m_ShowAlpha = EditorGUILayout.Foldout(m_ShowAlpha, "随机CanvasGroup透明度");
            if (m_ShowAlpha)
            {
                FloatPropertyField("随机率", m_AlphaRandomRate, 0.0f, 1.0f);
                FloatPropertyField("频率", m_AlphaShakeFreq, 0.0f, 10.0f);
                AlphaRangePropertyField("随机范围", m_RandomAlphaRange);
            }
        }

        // 随机UXImage颜色
        if (m_UXImage != null)
        {
            GUILayout.Space(10);
            m_ShowColor = EditorGUILayout.Foldout(m_ShowColor, "随机UXImage颜色");
            if (m_ShowColor)
            {
                FloatPropertyField("随机率", m_ColorRandomRate, 0.0f, 1.0f);
                FloatPropertyField("频率", m_ColorShakeFreq, 0.0f, 10.0f);
                BoolPropertyField("使用透明度", m_ColorShakeAlpha);
                ColorListPropertyField("颜色范围", m_RandomColors);
            }
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}