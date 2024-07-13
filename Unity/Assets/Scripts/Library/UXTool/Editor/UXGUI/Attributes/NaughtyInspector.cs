using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NaughtyAttributes.Editor
{
	[CanEditMultipleObjects]
	//[CustomEditor(typeof(MonoBehaviour), true)]
	public class NaughtyInspector : UnityEditor.Editor
	{
		private List<SerializedProperty> _serializedProperties = new List<SerializedProperty>();
		private IEnumerable<MethodInfo> _methods;

		protected virtual void OnEnable()
		{
			_methods = ReflectionUtility.GetAllMethods(
				target, m => m.GetCustomAttributes(typeof(ButtonAttribute), true).Length > 0);
		}

		protected virtual void OnDisable()
		{
			ReorderableListPropertyDrawer.Instance.ClearCache();
		}

		public override void OnInspectorGUI()
		{
			GetSerializedProperties(ref _serializedProperties);

			bool anyNaughtyAttribute = _serializedProperties.Any(p => PropertyUtility.GetAttribute<INaughtyAttribute>(p) != null);
			if (!anyNaughtyAttribute)
			{
				DrawDefaultInspector();
			}
			else
			{
				DrawSerializedProperties();
			}
			DrawButtons();
		}

		protected void GetSerializedProperties(ref List<SerializedProperty> outSerializedProperties)
		{
			outSerializedProperties.Clear();
			using (var iterator = serializedObject.GetIterator())
			{
				if (iterator.NextVisible(true))
				{
					do
					{
						outSerializedProperties.Add(serializedObject.FindProperty(iterator.name));
					}
					while (iterator.NextVisible(false));
				}
			}
		}

		protected void DrawSerializedProperties()
		{
			serializedObject.Update();

			// Draw non-grouped serialized properties
			foreach (var property in GetNonGroupedProperties(_serializedProperties))
			{
				if (property.name.Equals("m_Script", System.StringComparison.Ordinal))
				{
					using (new EditorGUI.DisabledScope(disabled: true))
					{
						EditorGUILayout.PropertyField(property);
					}
				}
				else
				{
					NaughtyEditorGUI.PropertyField_Layout(property, includeChildren: true);
				}
			}

			// Draw grouped serialized properties
			foreach (var group in GetBoxGroupedProperties(_serializedProperties))
			{
				IEnumerable<SerializedProperty> visibleProperties = group.Where(p => PropertyUtility.IsVisible(p));
				if (!visibleProperties.Any())
				{
					continue;
				}
				NaughtyEditorGUI.BeginBoxGroup_Layout(group.Key);
				foreach (var property in visibleProperties)
				{
					NaughtyEditorGUI.PropertyField_Layout(property, includeChildren: true);
				}

				NaughtyEditorGUI.EndBoxGroup_Layout();
			}

			foreach(var group in GetHorizonGroupedProperties(_serializedProperties))
            {
                IEnumerable<SerializedProperty> visibleProperties = group.Where(p => PropertyUtility.IsVisible(p));
                if (!visibleProperties.Any())
                {
                    continue;
                }
				NaughtyEditorGUI.BeginBoxGroup_Layout(group.Key);
				EditorGUILayout.BeginHorizontal();
				foreach (var property in visibleProperties)
                {
                    NaughtyEditorGUI.PropertyField_Layout(property, includeChildren: true);
                }

				EditorGUILayout.EndHorizontal();
				NaughtyEditorGUI.EndBoxGroup_Layout();
			}

			serializedObject.ApplyModifiedProperties();
		}

		protected void DrawNonSerializedFields(bool drawHeader = false)
		{
			
			//if (_nonSerializedFields.Any())
			//{
			//	if (drawHeader)
			//	{
			//		EditorGUILayout.Space();
			//		EditorGUILayout.LabelField("Non-Serialized Fields", GetHeaderGUIStyle());
			//		NaughtyEditorGUI.HorizontalLine(
			//			EditorGUILayout.GetControlRect(false), HorizontalLineAttribute.DefaultHeight, HorizontalLineAttribute.DefaultColor.GetColor());
			//	}

			//	foreach (var field in _nonSerializedFields)
			//	{
			//		NaughtyEditorGUI.NonSerializedField_Layout(serializedObject.targetObject, field);
			//	}
			//}
		}

		protected void DrawNativeProperties(bool drawHeader = false)
		{
			//if (_nativeProperties.Any())
			//{
			//	if (drawHeader)
			//	{
			//		EditorGUILayout.Space();
			//		EditorGUILayout.LabelField("Native Properties", GetHeaderGUIStyle());
			//		NaughtyEditorGUI.HorizontalLine(
			//			EditorGUILayout.GetControlRect(false), HorizontalLineAttribute.DefaultHeight, HorizontalLineAttribute.DefaultColor.GetColor());
			//	}

			//	foreach (var property in _nativeProperties)
			//	{
			//		NaughtyEditorGUI.NativeProperty_Layout(serializedObject.targetObject, property);
			//	}
			//}
		}

		protected void DrawButtons(bool drawHeader = false)
		{
			if (_methods.Any())
			{
				if (drawHeader)
				{
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Buttons", GetHeaderGUIStyle());
					NaughtyEditorGUI.HorizontalLine(
						EditorGUILayout.GetControlRect(false), 2, Color.gray);
				}

				foreach (var method in _methods)
				{
					NaughtyEditorGUI.Button(serializedObject.targetObject, method);
				}
			}
		}

		private static IEnumerable<SerializedProperty> GetNonGroupedProperties(IEnumerable<SerializedProperty> properties)
		{
			return properties
				.Where(p => PropertyUtility.GetAttribute<IGroupAttribute>(p) == null)
				.OrderBy(p => PropertyUtility.GetAttribute<PropertyOrderAttribute>(p) == null ? 0 : PropertyUtility.GetAttribute<PropertyOrderAttribute>(p).Order) ;
		}

		private static IEnumerable<IGrouping<string, SerializedProperty>> GetBoxGroupedProperties(IEnumerable<SerializedProperty> properties)
		{
			return properties
				.Where(p => PropertyUtility.GetAttribute<BoxGroupAttribute>(p) != null)
				.GroupBy(p => PropertyUtility.GetAttribute<BoxGroupAttribute>(p).Name);
		}

        private static IEnumerable<IGrouping<string, SerializedProperty>> GetHorizonGroupedProperties(IEnumerable<SerializedProperty> properties)
        {
            return properties
                .Where(p => PropertyUtility.GetAttribute<HorizontalGroupAttribute>(p) != null)
                .GroupBy(p => PropertyUtility.GetAttribute<HorizontalGroupAttribute>(p).Name);
        }

        //private static IEnumerable<IGrouping<string, SerializedProperty>> GetFoldoutProperties(IEnumerable<SerializedProperty> properties)
        //{
        //	return properties
        //		.Where(p => PropertyUtility.GetAttribute<FoldoutAttribute>(p) != null)
        //		.GroupBy(p => PropertyUtility.GetAttribute<FoldoutAttribute>(p).Name);
        //}

        private static GUIStyle GetHeaderGUIStyle()
		{
			GUIStyle style = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
			style.fontStyle = FontStyle.Bold;
			style.alignment = TextAnchor.UpperCenter;

			return style;
		}
	}
}
