using UnityEditor;
using UnityEngine;
using UITargetType = UIValueComponent.UITargetType;
using RectTransformPropertyType = UIValueComponent.RectTransformPropertyType;
using ImagePropertyType = UIValueComponent.ImagePropertyType;
using TextPropertyType = UIValueComponent.TextPropertyType;
using OperationType = ValueProcess.OperationType;
using UnityEditorInternal;
using System.Collections.Generic;
using System;

namespace UnityEngine.UI
{
    [CustomPropertyDrawer(typeof(UIValueAdapter.EditorInputSlot))]
    public class EditorInputSlotDrawer : PropertyDrawer
    {
        private int adjust = 50;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string index = label.text;

            SerializedProperty nameProperty = property.FindPropertyRelative("name");
            SerializedProperty valueProperty = property.FindPropertyRelative("value");

            var nameRect = new Rect(position)
            {
                width = position.width / 2 + adjust - 5,
            };

            var valueRect = new Rect(position)
            {
                width = position.width / 2 - adjust + 5,
                x = position.x + position.width / 2 + adjust
            };

            nameProperty.stringValue = EditorGUI.TextField(nameRect, "1Input" + index, nameProperty.stringValue);
            valueProperty.floatValue = EditorGUI.FloatField(valueRect, valueProperty.floatValue);
        }
    }

    [CustomPropertyDrawer(typeof(UIValueComponent))]
    public class UIValueComponentDrawer : PropertyDrawer
    {
        private Dictionary<string, ReorderableList> _processListDict = new Dictionary<string, ReorderableList>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty targetObjectProperty = property.FindPropertyRelative("targetObject");
            SerializedProperty targetTypeProperty = property.FindPropertyRelative("targetType");

            SerializedProperty transformPropertyTypeProperty = property.FindPropertyRelative("transformPropertyType");
            SerializedProperty imagePropertyTypeProperty = property.FindPropertyRelative("imagePropertyType");
            SerializedProperty textPropertyTypeProperty = property.FindPropertyRelative("textPropertyType");

            SerializedProperty intCountProperty = property.FindPropertyRelative("intCount");
            SerializedProperty floatCountProperty = property.FindPropertyRelative("floatCount");

            float y = 0;
            var targetObjectRect = new Rect(position)
            {
                height = EditorGUIUtility.singleLineHeight
            };

            var targetTypeRect = new Rect(position)
            {
                height = EditorGUIUtility.singleLineHeight,
                y = targetObjectRect.y + EditorGUIUtility.singleLineHeight + CustomEditorGUI.propertyInterval
            };

            var propertyTypeRect = new Rect(position)
            {
                height = EditorGUIUtility.singleLineHeight,
                y = targetTypeRect.y + EditorGUIUtility.singleLineHeight + CustomEditorGUI.propertyInterval
            };
            y = propertyTypeRect.y;

            EditorGUI.PropertyField(targetObjectRect, targetObjectProperty);
            targetTypeProperty.enumValueIndex = (int)(UITargetType)EditorGUI.EnumPopup(targetTypeRect, " Target Type", (UITargetType)targetTypeProperty.enumValueIndex);

            if ((UITargetType)targetTypeProperty.enumValueIndex == UITargetType.RectTransform)
            {
                transformPropertyTypeProperty.enumValueIndex = (int)(RectTransformPropertyType)EditorGUI.EnumPopup(propertyTypeRect, " Transform Property Type", (RectTransformPropertyType)transformPropertyTypeProperty.enumValueIndex);
            }
            else if ((UITargetType)targetTypeProperty.enumValueIndex == UITargetType.Image)
            {
                imagePropertyTypeProperty.enumValueIndex = (int)(ImagePropertyType)EditorGUI.EnumPopup(propertyTypeRect, " Image Property Type", (ImagePropertyType)imagePropertyTypeProperty.enumValueIndex);
            }
            else if ((UITargetType)targetTypeProperty.enumValueIndex == UITargetType.Text)
            {
                textPropertyTypeProperty.enumValueIndex = (int)(TextPropertyType)EditorGUI.EnumPopup(propertyTypeRect, " Text Property Type", (TextPropertyType)textPropertyTypeProperty.enumValueIndex);

                var intRect = new Rect(position)
                {
                    height = EditorGUIUtility.singleLineHeight,
                    y = propertyTypeRect.y + EditorGUIUtility.singleLineHeight + CustomEditorGUI.propertyInterval
                };

                var floatRect = new Rect(position)
                {
                    height = EditorGUIUtility.singleLineHeight,
                    y = intRect.y + EditorGUIUtility.singleLineHeight + CustomEditorGUI.propertyInterval
                };
                y = floatRect.y;

                EditorGUI.PropertyField(intRect, intCountProperty);
                EditorGUI.PropertyField(floatRect, floatCountProperty);
            }

            SerializedProperty valueProcessesProperty = property.FindPropertyRelative("valueProcesses");

            var processesRect = new Rect(position)
            {
                height = EditorGUIUtility.singleLineHeight,
                y = y + EditorGUIUtility.singleLineHeight + CustomEditorGUI.propertyInterval
            };
            ReorderableListDrawer.DrawList(valueProcessesProperty, processesRect, ValueProcessDrawer.GetHeight);
        }

        public static float GetHeight(SerializedProperty component)
        {
            float defaultHeight = (EditorGUIUtility.singleLineHeight + CustomEditorGUI.propertyInterval) * 3;
            var targetType = component.FindPropertyRelative("targetType");
            if (targetType.enumValueIndex == (int)UITargetType.Text)
            {
                defaultHeight = (EditorGUIUtility.singleLineHeight + CustomEditorGUI.propertyInterval) * 5;
            }
            var valueProcesses = component.FindPropertyRelative("valueProcesses");
            float vpListHeight = ValueProcessListHelper.GetHeight(valueProcesses);

            return defaultHeight + vpListHeight + CustomEditorGUI.listInterval;
        }
    }

    [CustomPropertyDrawer(typeof(UIStateComponent))]
    public class UIStateComponentDrawer : PropertyDrawer
    {
        private Dictionary<string, ReorderableList> _processListDict = new Dictionary<string, ReorderableList>();
        private Dictionary<string, ReorderableList> _stateRangesListDict = new Dictionary<string, ReorderableList>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty animatorProperty = property.FindPropertyRelative("animator");
            SerializedProperty stateRangesProperty = property.FindPropertyRelative("stateRanges");
            SerializedProperty valueProcessesProperty = property.FindPropertyRelative("valueProcesses");

            float currentPosY = position.y;
            var animatorRect = new Rect(position)
            {
                height = EditorGUIUtility.singleLineHeight
            };
            currentPosY = currentPosY + animatorRect.height + CustomEditorGUI.propertyInterval;

            var stateRangesListRectHeight = StateRangesListHelper.GetHeight(stateRangesProperty);
            var stateRangesListRect = new Rect(position)
            {
                height = EditorGUIUtility.singleLineHeight,
                y = currentPosY
            };

            currentPosY = currentPosY + stateRangesListRectHeight + CustomEditorGUI.propertyInterval;

            var processesRect = new Rect(position)
            {
                height = EditorGUIUtility.singleLineHeight,
                y = currentPosY
            };

            EditorGUI.PropertyField(animatorRect, animatorProperty);
            ReorderableListDrawer.DrawList(stateRangesProperty, stateRangesListRect, StateRangeDrawer.GetHeight);
            ReorderableListDrawer.DrawList(valueProcessesProperty, processesRect, ValueProcessDrawer.GetHeight);
        }

        public static float GetHeight(SerializedProperty state)
        {
            float defaultHeight = EditorGUIUtility.singleLineHeight + CustomEditorGUI.propertyInterval; //animator��height

            var sr = state.FindPropertyRelative("stateRanges");
            var valueProcesses = state.FindPropertyRelative("valueProcesses");

            float srListHeight = StateRangesListHelper.GetHeight(sr);

            float vpListHeight = ValueProcessListHelper.GetHeight(valueProcesses);

            return defaultHeight + srListHeight + vpListHeight + CustomEditorGUI.listInterval;
        }
    }

    [CustomPropertyDrawer(typeof(UIStateComponent.StateRange))]
    public class StateRangeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty targetRangeProperty = property.FindPropertyRelative("targetRange");
            SerializedProperty stateNameProperty = property.FindPropertyRelative("stateName");

            var rangeRect = new Rect(position)
            {
                height = EditorGUIUtility.singleLineHeight
            };
            var stateNameRect = new Rect(position)
            {
                height = EditorGUIUtility.singleLineHeight,
                y = position.y + EditorGUIUtility.singleLineHeight + CustomEditorGUI.propertyInterval
            };

            targetRangeProperty.vector2Value = EditorGUI.Vector2Field(rangeRect, "Target Range", targetRangeProperty.vector2Value);


            SerializedProperty stateNames = property.FindPropertyRelative("stateNames");
            var stateNamesList = new ReorderableList(stateNames.serializedObject, stateNames, true, true, true, true);
            List<string> options = new List<string>();
            for (int i = 0; i < stateNamesList.count; i++)
            {
                SerializedProperty name = stateNamesList.serializedProperty.GetArrayElementAtIndex(i);
                options.Add(name.stringValue);
            }

            int nameIndex = options.IndexOf(stateNameProperty.stringValue);

            //stateNameProperty.stringValue = options[EditorGUI.Popup(stateNameRect, nameIndex, options.ToArray())];
            EditorGUI.Popup(stateNameRect, "State Name", nameIndex, options.ToArray());
        }

        public static float GetHeight(SerializedProperty property)
        {
            return (EditorGUIUtility.singleLineHeight + CustomEditorGUI.propertyInterval) * 2;
        }
    }

    [CustomPropertyDrawer(typeof(ValueProcess))]
    public class ValueProcessDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty variableA = property.FindPropertyRelative("variableA");
            SerializedProperty a = property.FindPropertyRelative("a");
            SerializedProperty IndexA = property.FindPropertyRelative("indexA");

            SerializedProperty variableB = property.FindPropertyRelative("variableB");
            SerializedProperty b = property.FindPropertyRelative("b");
            SerializedProperty IndexB = property.FindPropertyRelative("indexB");

            SerializedProperty type = property.FindPropertyRelative("type");

            float currentY = position.y;
            if (label.text == "0")
            {
                var variableARect = new Rect(position)
                {
                    width = position.width / 2,
                    height = EditorGUIUtility.singleLineHeight
                };

                var aRect = new Rect(position)
                {
                    x = position.x + position.width / 2 + 10,
                    width = position.width / 2 - 10,
                    height = EditorGUIUtility.singleLineHeight
                };

                currentY += EditorGUIUtility.singleLineHeight;

                variableA.boolValue = EditorGUI.Toggle(variableARect, "variableA", variableA.boolValue);
                if (variableA.boolValue)
                {
                    SerializedProperty variableNames = property.FindPropertyRelative("variableNames");
                    var variableNamesList = new ReorderableList(variableNames.serializedObject, variableNames, true, true, true, true);
                    List<string> temp = new List<string>();
                    for (int i = 0; i < variableNamesList.count; i++)
                    {
                        SerializedProperty name = variableNamesList.serializedProperty.GetArrayElementAtIndex(i);
                        temp.Add(name.stringValue);
                    }
                    IndexA.intValue = EditorGUI.Popup(aRect, IndexA.intValue, temp.ToArray());
                }
                else
                {
                    a.floatValue = EditorGUI.FloatField(aRect, a.floatValue);
                }
            }

            var typeRect = new Rect(position)
            {
                y = currentY + CustomEditorGUI.propertyInterval,
                height = EditorGUIUtility.singleLineHeight
            };

            currentY = typeRect.y + typeRect.height;

            var variableBRect = new Rect(position)
            {
                y = currentY + CustomEditorGUI.propertyInterval,
                width = position.width / 2,
                height = EditorGUIUtility.singleLineHeight
            };

            var bRect = new Rect(position)
            {
                x = position.x + position.width / 2 + 10,
                y = currentY + CustomEditorGUI.propertyInterval,
                width = position.width / 2 - 10,
                height = EditorGUIUtility.singleLineHeight
            };

            type.enumValueIndex = (int)(OperationType)EditorGUI.EnumPopup(typeRect, "Type", (OperationType)type.enumValueIndex);
            variableB.boolValue = EditorGUI.Toggle(variableBRect, "variableB", variableB.boolValue);
            if (variableB.boolValue)
            {
                SerializedProperty variableNames = property.FindPropertyRelative("variableNames");
                var variableNamesList = new ReorderableList(variableNames.serializedObject, variableNames, true, true, true, true);
                List<string> options = new List<string>();
                for (int i = 0; i < variableNamesList.count; i++)
                {
                    SerializedProperty name = variableNamesList.serializedProperty.GetArrayElementAtIndex(i);
                    options.Add(name.stringValue);
                }
                IndexB.intValue = EditorGUI.Popup(bRect, IndexB.intValue, options.ToArray());
            }
            else
            {
                b.floatValue = EditorGUI.FloatField(bRect, b.floatValue);
            }
        }

        public static float GetHeight(SerializedProperty vpProperty)
        {
            var showNumA = vpProperty.FindPropertyRelative("showNumA");
            if (showNumA.boolValue)
            {
                return (EditorGUIUtility.singleLineHeight + CustomEditorGUI.propertyInterval) * 3;
            }
            else
            {
                return (EditorGUIUtility.singleLineHeight + CustomEditorGUI.propertyInterval) * 2;
            }
        }
    }

    public class ValueProcessListHelper
    {
        public static float GetHeight(SerializedProperty valueProcessesProperty)
        {
            if (!valueProcessesProperty.isExpanded)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            int itemCount = valueProcessesProperty.arraySize;
            if (itemCount == 0)
            {
                return ReorderableListDrawer.GetRecordListEmptyHeight();
            }
            else if (itemCount == 1)
            {
                float defaultheight = ReorderableListDrawer.GetRecordListHeaderHeight() + ReorderableListDrawer.GetRecordListFooterHeight();
                SerializedProperty item0 = valueProcessesProperty.GetArrayElementAtIndex(0);

                var itemsHeight = ValueProcessDrawer.GetHeight(item0);
                return defaultheight + itemsHeight + CustomEditorGUI.listInterval;
            }
            else
            {
                float defaultheight = ReorderableListDrawer.GetRecordListHeaderHeight() + ReorderableListDrawer.GetRecordListFooterHeight();
                SerializedProperty item0 = valueProcessesProperty.GetArrayElementAtIndex(0);
                SerializedProperty item1 = valueProcessesProperty.GetArrayElementAtIndex(1);

                var itemsHeight = ValueProcessDrawer.GetHeight(item0) + ValueProcessDrawer.GetHeight(item1) * (itemCount - 1);
                return defaultheight + itemsHeight + CustomEditorGUI.listInterval;
            }
        }
    }

    public class StateRangesListHelper
    {
        public static float GetHeight(SerializedProperty StateRangesProperty)
        {
            if (!StateRangesProperty.isExpanded)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            int itemCount = StateRangesProperty.arraySize;
            if (itemCount > 0)
            {
                float defaultheight = ReorderableListDrawer.GetRecordListHeaderHeight() + ReorderableListDrawer.GetRecordListFooterHeight();
                var itemsHeight = StateRangeDrawer.GetHeight(null) * itemCount;
                return defaultheight + itemsHeight + CustomEditorGUI.listInterval;
            }
            else
            {
                return ReorderableListDrawer.GetRecordListEmptyHeight();
            }
        }
    }
}

