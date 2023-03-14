using System;
using UnityEditor;
using UnityEngine;

namespace QFSW.QC.Editor
{
    public class DataEntryPopup : PopupWindowContent
    {
        private readonly string _title;
        private readonly string _btnName;

        private string _data;
        private string _errors;
        private bool _success;

        private GUIStyle _errorStyle;
        private GUIStyle _successStyle;

        private readonly Action<string> _submitCallback;

        public DataEntryPopup(string title, string btnName, Action<string> SubmitCallback)
        {
            CreateStyles();
            _title = title;
            _btnName = btnName;
            _submitCallback = SubmitCallback;
        }

        public override Vector2 GetWindowSize() { return new Vector2(500, 100); }

        private void CreateStyles()
        {
            _errorStyle = new GUIStyle(EditorStyles.wordWrappedLabel);
            _errorStyle.normal.textColor = new Color(1, 0, 0);

            _successStyle = new GUIStyle(EditorStyles.wordWrappedLabel);
            _successStyle.normal.textColor = new Color(0, 0.5f, 0);
        }

        public override void OnGUI(Rect rect)
        {
            _data = EditorGUILayout.TextField(_title, _data);
            GUI.enabled = !string.IsNullOrWhiteSpace(_data);
            if (GUILayout.Button(_btnName))
            {
                try
                {
                    _submitCallback(_data);
                    _success = true;
                    _errors = "";
                }
                catch (Exception e)
                {
                    _errors = e.Message;
                    _success = false;
                }
            }

            if (!string.IsNullOrWhiteSpace(_errors)) { EditorGUILayout.LabelField(_errors, _errorStyle); }
            else if (_success) { EditorGUILayout.LabelField("Success!", _successStyle); }
        }
    }
}
