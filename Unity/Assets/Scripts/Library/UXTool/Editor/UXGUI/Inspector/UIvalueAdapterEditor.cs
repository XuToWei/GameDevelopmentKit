using System.Collections;
using System.Collections.Generic;
using UnityEditor;

using UITargetType = UIValueComponent.UITargetType;
using RectTransformPropertyType = UIValueComponent.RectTransformPropertyType;
using ImagePropertyType = UIValueComponent.ImagePropertyType;
using TextPropertyType = UIValueComponent.TextPropertyType;
using OperationType = ValueProcess.OperationType;
using UnityEditorInternal;
using System.Linq;
using System;
using NaughtyAttributes.Editor;

namespace UnityEngine.UI
{
    [CustomEditor(typeof(UIValueAdapter), true)]
    [CanEditMultipleObjects]
    public class UIValueAdapterEditor : Editor
    {
        private SerializedProperty inputSlotsProperty;
        private SerializedProperty componentsProperty;
        private SerializedProperty statesProperty;

        private ReorderableList _InputSlotsList;

        Func<SerializedProperty, float> getComponentHeight;
        Func<SerializedProperty, float> getStateHeight;

        private void UpdateAdapter(ReorderableList list)
        {
            List<string> Names = new List<string>();
            List<float> Values = new List<float>();

            for (int i = 0; i < list.count; i++)
            {
                SerializedProperty t = list.serializedProperty.GetArrayElementAtIndex(i);
                string name = t.FindPropertyRelative("name").stringValue;
                float value = t.FindPropertyRelative("value").floatValue;
                Names.Add(name);
                Values.Add(value);
            }

            UIValueAdapter adapter = serializedObject.targetObject as UIValueAdapter;
            if (adapter.components != null)
            {
                foreach (var component in adapter.components)
                {
                    component.EditorSetInputName(Names.ToArray());
                    component.SetValue(Values.ToArray());
                }
            }

            if (adapter.states != null)
            {
                foreach (var state in adapter.states)
                {
                    state.EditorSetInputName(Names.ToArray());
                    state.SetValue(Values.ToArray());
                }
            }
        }
        void OnEnable()
        {
            inputSlotsProperty = serializedObject.FindProperty("editorInputSlots");
            componentsProperty = serializedObject.FindProperty("components");
            statesProperty = serializedObject.FindProperty("states");


            EditorInputSlotDrawer slotDrawer = new EditorInputSlotDrawer();
            //InputSlots
            _InputSlotsList = new ReorderableList(serializedObject, serializedObject.FindProperty("editorInputSlots")
            , true, true, true, true);

            _InputSlotsList.drawHeaderCallback = (Rect rect) =>
            {
                GUI.Label(rect, "Input Slots");
            };
            _InputSlotsList.drawElementCallback = (Rect rect, int index, bool selected, bool focused) =>
            {
                SerializedProperty item = _InputSlotsList.serializedProperty.GetArrayElementAtIndex(index);
                rect.height -= 2;
                //EditorGUI.PropertyField(rect, item, new GUIContent(index.ToString()));
                slotDrawer.OnGUI(rect, item, new GUIContent(index.ToString()));
            };
            _InputSlotsList.onChangedCallback = (list) =>
            {
                UpdateAdapter(list);
            };

            UpdateAdapter(_InputSlotsList);

            //Components
            getComponentHeight = (componentProperty) =>
            {
                return UIValueComponentDrawer.GetHeight(componentProperty);
            };

            //StatesList
            getStateHeight = (stateProperty) =>
            {
                return UIStateComponentDrawer.GetHeight(stateProperty);
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            //绘制列表
            _InputSlotsList.DoLayoutList();
            ReorderableListDrawer.DrawListLayout(componentsProperty, getComponentHeight);
            ReorderableListDrawer.DrawListLayout(statesProperty, getStateHeight);
            //应用
            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                PropertyUtility.CallOnValueChangedCallbacks(inputSlotsProperty);
                PropertyUtility.CallOnValueChangedCallbacks(componentsProperty);
                PropertyUtility.CallOnValueChangedCallbacks(statesProperty);
            }
        }

        private void OnDisable()
        {
            ReorderableListDrawer.ClearCache();
        }
    }
}
