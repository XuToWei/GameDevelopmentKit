#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace TF_TableList
{
	public abstract class InlineNumberRowFilter<T>:InlineRowFilter<T>
	{
		private string text = "";
		private bool enable;
		private T value;
		public override bool Success(object target)
		{
			return value.Equals(target);
		}

		public override bool IsValid => enable;
		public override void Reset()
		{
			text = "";
			enable = false;
			value = default;
		}

		public override string GetPersistentValue()
		{
			return text;
		}

		public override void OnLoadPersistentValue(string value)
		{
			text = value;
			OnValueChanged();
		}

		private static GUIStyle inlineTextStyle = new GUIStyle(EditorStyles.textField)
		{
			alignment = TextAnchor.MiddleLeft,
		};
		public override void OnDrawInline(Rect rect)
		{
			if (IsValid) {
				EditorGUI.DrawRect(rect, Color.green);
				EditorGUI.DrawRect(rect.Padding(1), SirenixGUIStyles.DarkEditorBackground);
			}

			EditorGUI.BeginChangeCheck();
			if (!enable) {
				text = SirenixEditorFields.DelayedTextField(rect.Padding(1), GUIContent.none, text, inlineTextStyle);
			}
			else {
				var right = rect.AlignRight(16);
				rect.width = rect.width - 16;
				text = SirenixEditorFields.DelayedTextField(rect.Padding(1), GUIContent.none, text, inlineTextStyle);
				if (SirenixEditorGUI.IconButton(right, EditorIcons.X)) {
					Reset();
				}
			}

			if (EditorGUI.EndChangeCheck()) {
				OnValueChanged();
			}
		}

		private void OnValueChanged()
		{
			if (string.IsNullOrEmpty(text)) {
				enable = false;
				value = default;
			}
			else {
				if (TryParse(text, out value)) {
					enable = true;
				}
				else {
					enable = false;
				}
			}
			Dirty = true;
		}

		protected abstract bool TryParse(string text, out T val);
	}
	
	public class InlineFloatRowFilter:InlineNumberRowFilter<float>
	{
		protected override bool TryParse(string text, out float val)
		{
			return float.TryParse(text, out val);
		}
	}
	
	public class InlineIntRowFilter:InlineNumberRowFilter<int>
	{
		protected override bool TryParse(string text, out int val)
		{
			return int.TryParse(text, out val);
		}
	}
}
#endif