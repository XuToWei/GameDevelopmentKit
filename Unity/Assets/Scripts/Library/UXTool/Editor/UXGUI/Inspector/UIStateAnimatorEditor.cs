using System;
using System.Collections;
using System.Collections.Generic;
using ThunderFireUITool;
using UnityEditor;
using UnityEditorInternal;

namespace UnityEngine.UI
{
    [CustomEditor(typeof(UIStateAnimator), true)]
    [CanEditMultipleObjects]
    public class UIStateAnimatorEditor : Editor
    {
        private SerializedProperty LogicLayers;

        void OnEnable()
        {
            LogicLayers = serializedObject.FindProperty("logicLayers");
        }

        public override void OnInspectorGUI()
        {
            UIStateAnimator animator = serializedObject.targetObject as UIStateAnimator;

            var fadein = serializedObject.FindProperty("fadein");
            var fadeout = serializedObject.FindProperty("fadeout");

            serializedObject.Update();

            EditorGUILayout.PropertyField(fadein);
            EditorGUILayout.PropertyField(fadeout);

            Rect rect = EditorGUILayout.BeginVertical();
            rect.height += 5;
            EditorGUI.DrawRect(rect, new Color(0.28f, 0.28f, 0.28f, 1));
            EditorGUILayout.LabelField(EditorLocalization.GetLocalization("UIStateAnimator", "Preview"));
            string[] stateNames = animator.GetTriggerName();
            int index = 0;
            index = EditorGUILayout.Popup("State Name", index, stateNames);

            float speed = 1, startTime = 0;
            speed = EditorGUILayout.FloatField("Speed", speed);
            startTime = EditorGUILayout.FloatField("Start Time", startTime);

            if (GUILayout.Button(EditorLocalization.GetLocalization("UIStateAnimator", "Preview")))
            {
                animator.SetStateInPreview(stateNames[index], speed, startTime);
            }
            EditorGUILayout.EndVertical();

            if (GUILayout.Button(EditorLocalization.GetLocalization("UIStateAnimator", "StartPreview")))
            {
                animator.CreateController();
            }
            if (GUILayout.Button(EditorLocalization.GetLocalization("UIStateAnimator", "PrintGraphNodeState")))
            {
                animator.printGraphNodeState();
            }

            //绘制列表
            ReorderableListDrawer.DrawListLayout(LogicLayers, "Logic Layers", UIAnimStateLayerDrawer.GetHeight);
            //应用
            serializedObject.ApplyModifiedProperties();
        }

        void OnDisable()
        {
            ReorderableListDrawer.ClearCache();
        }
    }

    [CustomPropertyDrawer(typeof(UIAnimStateLayer))]
    public class UIAnimStateLayerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string index = label.text;

            SerializedProperty nameProperty = property.FindPropertyRelative("name");
            SerializedProperty crossFadeTimeProperty = property.FindPropertyRelative("crossFadeTime");
            SerializedProperty transientProperty = property.FindPropertyRelative("transient");
            SerializedProperty skipDefaultProperty = property.FindPropertyRelative("skipDefault");
            SerializedProperty statesProperty = property.FindPropertyRelative("states");

            float y = position.y;
            var nameRect = new Rect(position)
            {
                height = EditorGUIUtility.singleLineHeight
            };
            y = y + nameRect.height + CustomEditorGUI.propertyInterval;
            var crossFadeTimeRect = new Rect(position)
            {
                height = EditorGUIUtility.singleLineHeight,
                y = y
            };
            y = y + crossFadeTimeRect.height + CustomEditorGUI.propertyInterval;
            var transientRect = new Rect(position)
            {
                height = EditorGUIUtility.singleLineHeight,
                y = y
            };
            y = y + transientRect.height + CustomEditorGUI.propertyInterval;
            var skipDefaultRect = new Rect(position)
            {
                height = EditorGUIUtility.singleLineHeight,
                y = y
            };
            y = y + skipDefaultRect.height + CustomEditorGUI.propertyInterval;
            var listRect = new Rect(position)
            {
                height = EditorGUIUtility.singleLineHeight,
                y = y
            };

            nameProperty.stringValue = EditorGUI.TextField(nameRect, "Name", nameProperty.stringValue);
            crossFadeTimeProperty.floatValue = EditorGUI.FloatField(crossFadeTimeRect, "Cross Fade Time", crossFadeTimeProperty.floatValue);
            transientProperty.boolValue = EditorGUI.Toggle(transientRect, "Transient", transientProperty.boolValue);
            skipDefaultProperty.boolValue = EditorGUI.Toggle(skipDefaultRect, "Skip Default", skipDefaultProperty.boolValue);

            Func<SerializedProperty, float> getHeight = (stateProperty) =>
            {
                property.FindPropertyRelative("states");
                return UIAnimStateDrawer.GetHeight(stateProperty);
            };

            ReorderableListDrawer.DrawList(statesProperty, listRect, getHeight);
        }

        public static float GetHeight(SerializedProperty logicLayer)
        {
            float defaultHeight = (EditorGUIUtility.singleLineHeight + CustomEditorGUI.propertyInterval) * 4;
            var states = logicLayer.FindPropertyRelative("states");

            float listHeight = 0;
            if (states.isExpanded)
            {
                listHeight = ReorderableListDrawer.GetRecordListHeaderHeight() + ReorderableListDrawer.GetRecordListFooterHeight();
                if (states.arraySize == 0)
                {
                    listHeight = ReorderableListDrawer.GetRecordListEmptyHeight();
                }
                else
                {
                    for (int i = 0; i < states.arraySize; i++)
                    {
                        SerializedProperty state = states.GetArrayElementAtIndex(i);
                        listHeight += UIAnimStateDrawer.GetHeight(state);
                    }
                }
            }
            else
            {
                listHeight = EditorGUIUtility.singleLineHeight;
            }
            return defaultHeight + listHeight + CustomEditorGUI.listInterval;
        }
    }

    [CustomPropertyDrawer(typeof(UIAnimState))]
    public class UIAnimStateDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string index = label.text;

            SerializedProperty nameProperty = property.FindPropertyRelative("name");
            SerializedProperty replayProperty = property.FindPropertyRelative("replay");
            SerializedProperty loopProperty = property.FindPropertyRelative("loop");

            SerializedProperty animationQueueProperty = property.FindPropertyRelative("animationQueue");

            float y = position.y;
            var nameRect = new Rect(position)
            {
                height = EditorGUIUtility.singleLineHeight
            };
            y = y + nameRect.height + CustomEditorGUI.propertyInterval;
            var replayRect = new Rect(position)
            {
                height = EditorGUIUtility.singleLineHeight,
                y = y
            };
            y = y + replayRect.height + CustomEditorGUI.propertyInterval;
            var loopRect = new Rect(position)
            {
                height = EditorGUIUtility.singleLineHeight,
                y = y
            };
            y = y + loopRect.height + CustomEditorGUI.propertyInterval;
            var listRect = new Rect(position)
            {
                height = EditorGUIUtility.singleLineHeight,
                y = y
            };

            nameProperty.stringValue = EditorGUI.TextField(nameRect, "Name", nameProperty.stringValue);
            replayProperty.boolValue = EditorGUI.Toggle(replayRect, "Replay", replayProperty.boolValue);
            loopProperty.enumValueIndex = (int)(PlayQueuePB.EndingType)EditorGUI.EnumPopup(loopRect, "Loop", (PlayQueuePB.EndingType)loopProperty.enumValueIndex);

            Func<SerializedProperty, float> getHeight = (item) =>
            {
                return EditorGUIUtility.singleLineHeight + CustomEditorGUI.propertyInterval;
            };

            animationQueueProperty.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(listRect, animationQueueProperty.isExpanded, new GUIContent(animationQueueProperty.displayName));
            EditorGUI.EndFoldoutHeaderGroup();

            if (animationQueueProperty.isExpanded)
                ReorderableListDrawer.DrawList(animationQueueProperty, listRect, getHeight);
        }

        public static float GetHeight(SerializedProperty state)
        {
            float defaultHeight = (EditorGUIUtility.singleLineHeight + CustomEditorGUI.propertyInterval) * 3;
            var aq = state.FindPropertyRelative("animationQueue");

            float listHeight = 0;
            if (aq.isExpanded)
            {
                listHeight = aq.arraySize != 0 ?
                (EditorGUIUtility.singleLineHeight + CustomEditorGUI.propertyInterval) * aq.arraySize + ReorderableListDrawer.GetRecordListHeaderHeight() + ReorderableListDrawer.GetRecordListFooterHeight()
               : ReorderableListDrawer.GetRecordListEmptyHeight();
            }
            else
            {
                listHeight = EditorGUIUtility.singleLineHeight;
            }

            return defaultHeight + listHeight + CustomEditorGUI.listInterval;
        }
    }
}



