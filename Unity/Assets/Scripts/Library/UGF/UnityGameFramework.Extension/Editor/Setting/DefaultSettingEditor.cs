using System.IO;
using GameFramework;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension.Editor
{
    internal sealed class DefaultSettingEditor : EditorWindow
    {
        private const string SettingFileName = "GameFrameworkSetting.dat";
        
        private string m_FilePath = null;
        private DefaultSetting m_Settings = null;
        private DefaultSettingSerializer m_Serializer = null;
        private string[] m_SettingsNames;
        private string m_NewKey;
        private string m_NewValue;
        private Vector2 m_ScrollPosition;

        [MenuItem("Game Framework/Default Setting Editor", false, 0)]
        private static void Open()
        {
            var window = GetWindow<DefaultSettingEditor>("Default Setting Editor", true);
            window.minSize = new Vector2(500f, 600f);
        }

        // [MenuItem("Game Framework/Default Setting Editor", true)]
        // private static bool OpenValidateFunction()
        // {
        //     return !EditorApplication.isPlayingOrWillChangePlaymode;
        // }

        private void OnEnable()
        {
            m_FilePath = Utility.Path.GetRegularPath(Path.Combine(Application.persistentDataPath, SettingFileName));
            m_Settings = new DefaultSetting();
            m_Serializer = new DefaultSettingSerializer();
            m_Serializer.RegisterSerializeCallback(0, SerializeDefaultSettingCallback);
            m_Serializer.RegisterDeserializeCallback(0, DeserializeDefaultSettingCallback);
            Load();
            m_SettingsNames = m_Settings.GetAllSettingNames();
            m_ScrollPosition = Vector2.zero;
        }

        private void OnGUI()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorGUILayout.HelpBox("Can't modify in playmode.", MessageType.Warning);
            }
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                EditorGUILayout.LabelField("Value", GUILayout.Width(200f));
                EditorGUILayout.LabelField("Add Setting (Key - Value)", EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("+", GUILayout.Width(30f)))
                {
                    if (!string.IsNullOrEmpty(m_NewKey))
                    {
                        m_Settings.SetString(m_NewKey, m_NewValue);
                        m_SettingsNames = m_Settings.GetAllSettingNames();
                        Save();
                        m_NewKey = string.Empty;
                        m_NewValue = string.Empty;
                        GUI.FocusControl("");
                    }
                }
                m_NewKey = EditorGUILayout.TextField(m_NewKey, GUILayout.MaxWidth(500f));
                m_NewValue = EditorGUILayout.TextField(m_NewValue, GUILayout.MinWidth(300f));
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("All Settings (Key - Value)", EditorStyles.boldLabel);
                if (GUILayout.Button("Remove All Settings", GUILayout.Width(150f)))
                {
                    m_Settings.RemoveAllSettings();
                    m_SettingsNames = m_Settings.GetAllSettingNames();
                    Save();
                }
                EditorGUILayout.EndHorizontal();
                if (m_Settings.Count > 0)
                {
                    m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
                    foreach (string settingName in m_SettingsNames)
                    {
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("-", GUILayout.Width(30f)))
                        {
                            m_Settings.RemoveSetting(settingName);
                            m_SettingsNames = m_Settings.GetAllSettingNames();
                            Save();
                        }
                        GUILayout.Box(settingName, GUILayout.MaxWidth(500f));
                        string value = m_Settings.GetString(settingName);
                        string newValue = EditorGUILayout.TextField(value, GUILayout.MinWidth(300f));
                        if (!string.Equals(newValue, value))
                        {
                            m_Settings.SetString(settingName, newValue);
                            Save();
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndScrollView();
                }
            }
            EditorGUI.EndDisabledGroup();
        }

        private bool SerializeDefaultSettingCallback(Stream stream, DefaultSetting defaultSetting)
        {
            m_Settings.Serialize(stream);
            return true;
        }

        private DefaultSetting DeserializeDefaultSettingCallback(Stream stream)
        {
            m_Settings.Deserialize(stream);
            return m_Settings;
        }

        private void Load()
        {
            if (!File.Exists(m_FilePath))
            {
                return;
            }

            using FileStream fileStream = new FileStream(m_FilePath, FileMode.Open, FileAccess.Read);
            m_Serializer.Deserialize(fileStream);
        }

        private void Save()
        {
            using FileStream fileStream = new FileStream(m_FilePath, FileMode.Create, FileAccess.Write);
            m_Serializer.Serialize(fileStream, m_Settings);
        }
    }
}

