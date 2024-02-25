using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditorInternal;
using System.Linq;

namespace ThunderFireUITool
{
    [CustomPropertyDrawer(typeof(UIBeginnerGuideData))]
    public class UIBeginnerGuideDataDrawer : PropertyDrawer
    {
        private bool initialized = false;
        private string GuideIDString = "GuideID";
        private string FinishTypeString = "FinishType";
        private string DurationString = "Duration";
        private string GuidePrefabString = "GuidePrefab";
        private string ShowDataString = "ShowData";
        private string EditString = "Edit";
        private GUIContent GuidePrefabContent;

        private void Init(SerializedProperty property)
        {
            initialized = true;
            GuideIDString = EditorLocalization.GetLocalization("UIBeginnerData", "GuideID");
            FinishTypeString = EditorLocalization.GetLocalization("UIBeginnerData", "FinishType");
            DurationString = EditorLocalization.GetLocalization("UIBeginnerData", "Duration");
            GuidePrefabString = EditorLocalization.GetLocalization("UIBeginnerData", "GuidePrefab");
            ShowDataString = EditorLocalization.GetLocalization("UIBeginnerData", "ShowData");
            EditString = EditorLocalization.GetLocalization("UIBeginnerData", "Edit");

            GuidePrefabContent = new GUIContent(GuidePrefabString);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!initialized)
            {
                Init(property);
            }
            float curY = position.y;

            SerializedProperty idProperty = property.FindPropertyRelative("guideID");
            SerializedProperty typeProperty = property.FindPropertyRelative("guideFinishType");
            SerializedProperty durationProperty = property.FindPropertyRelative("guideFinishDuration");
            SerializedProperty prefabProperty = property.FindPropertyRelative("guideTemplatePrefab");
            SerializedProperty useOwnPrefab = property.FindPropertyRelative("UseOwnPrefab");
            SerializedProperty templateProperty = property.FindPropertyRelative("Template");

            var idRect = new Rect(position)
            {
                height = EditorGUIUtility.singleLineHeight,
                y = curY
            };
            idProperty.stringValue = EditorGUI.TextField(idRect, GuideIDString, idProperty.stringValue);
            curY += (EditorGUIUtility.singleLineHeight + CustomEditorGUI.propertyInterval);

            var typeRect = new Rect(position)
            {
                height = EditorGUIUtility.singleLineHeight,
                y = curY
            };
            string[] labels = { EditorLocalization.GetLocalization("UIBeginnerData", "Strong"), EditorLocalization.GetLocalization("UIBeginnerData", "Middle"), EditorLocalization.GetLocalization("UIBeginnerData", "Weak") };
            typeProperty.intValue = Utils.EnumPopupEx(typeRect, FinishTypeString, typeof(GuideFinishType), typeProperty.intValue, labels);
            //typeProperty.intValue = (int)(GuideFinishType)EditorGUI.EnumPopup(typeRect, FinishTypeString, (GuideFinishType)typeProperty.intValue);
            string tooltip = "";
            if (typeProperty.intValue == 0)
            {
                tooltip = EditorLocalization.GetLocalization("UIBeginnerData", "StrongGuide");
            }
            else if (typeProperty.intValue == 1)
            {
                tooltip = EditorLocalization.GetLocalization("UIBeginnerData", "MiddleGuide");
            }
            else if (typeProperty.intValue == 2)
            {
                tooltip = EditorLocalization.GetLocalization("UIBeginnerData", "WeakGuide");
            }
            EditorGUI.LabelField(typeRect, new GUIContent("", tooltip));
            curY += EditorGUIUtility.singleLineHeight + CustomEditorGUI.propertyInterval;

            if (typeProperty.intValue == (int)GuideFinishType.Weak)
            {
                var durationRect = new Rect(position)
                {
                    height = EditorGUIUtility.singleLineHeight,
                    y = curY
                };
                durationProperty.floatValue = EditorGUI.FloatField(durationRect, DurationString, durationProperty.floatValue);
                curY += EditorGUIUtility.singleLineHeight + CustomEditorGUI.propertyInterval;
            }

            var prefabRect = new Rect(position)
            {
                height = EditorGUIUtility.singleLineHeight,
                y = curY
            };
            //EditorGUI.BeginChangeCheck();
            useOwnPrefab.boolValue = EditorGUI.Toggle(prefabRect, EditorLocalization.GetLocalization("UIBeginnerData", "UseOwnPrefab"), useOwnPrefab.boolValue);

            curY += EditorGUIUtility.singleLineHeight + CustomEditorGUI.propertyInterval;

            var oneprefabRect = new Rect(position)
            {
                height = EditorGUIUtility.singleLineHeight,
                y = curY
            };
            if (useOwnPrefab.boolValue)
            {
                EditorGUI.PropertyField(oneprefabRect, prefabProperty, GuidePrefabContent);
            }
            else
            {

                int[] ints = { 0, 1 };
                string[] strs = { EditorLocalization.GetLocalization("UIBeginnerData", "GestureTemplate"), EditorLocalization.GetLocalization("UIBeginnerData", "GamePadTemplate") };
                templateProperty.intValue = Utils.EnumPopupEx(oneprefabRect, EditorLocalization.GetLocalization("UIBeginnerData", "ChooseGuideTemplate"),
                    typeof(GuideTemplate), templateProperty.intValue, strs);
                //templateProperty.intValue = EditorGUI.IntPopup(oneprefabRect,
                // EditorLocalization.GetLocalization("UIBeginnerData", "ChooseGuideTemplate"), templateProperty.intValue, strs, ints);
            }
            curY += EditorGUIUtility.singleLineHeight + CustomEditorGUI.propertyInterval;
            if (!useOwnPrefab.boolValue)
            {
                if (templateProperty.intValue == 0)
                {
                    prefabProperty.objectReferenceValue =
                    AssetDatabase.LoadAssetAtPath<GameObject>($"{ThunderFireUIToolConfig.UXFeaturePath}BeginnerGuide/Template/GuideTemplate_Gesture.prefab");
                }
                else if (templateProperty.intValue == 1)
                {
                    prefabProperty.objectReferenceValue =
                    AssetDatabase.LoadAssetAtPath<GameObject>($"{ThunderFireUIToolConfig.UXFeaturePath}BeginnerGuide/Template/GuideTemplate_GamePad.prefab");
                }
            }

            // var btnRect = new Rect(position)
            // {
            //     height = EditorGUIUtility.singleLineHeight,
            //     y = curY
            // };
            // if (GUI.Button(btnRect, ShowDataString))
            // {
            //     var guideDataList = property.serializedObject.targetObject as UIBeginnerGuideDataList;
            //     UIBeginnerGuideData guidedata = guideDataList.guideDataList.Where(data => data.guideID == idProperty.stringValue).ToList().First();
            //     Debug.Log("gamePadPanelData " + guidedata.gamePadPanelData);
            //     Debug.Log("guideArrowLineData " + guidedata.guideArrowLineData);
            //     Debug.Log("guideGesturePanelData " + guidedata.guideGesturePanelData);
            //     Debug.Log("guideHighLightData " + guidedata.guideHighLightData);
            //     Debug.Log("guideTextPanelData " + guidedata.guideTextPanelData);
            //     Debug.Log("targetStrokeData " + guidedata.targetStrokeData);

            //     Debug.Log("CustomTransformDatas " + guidedata.CustomTransformDatas);
            //     Debug.Log("CustomTextDatas " + guidedata.CustomTextDatas);
            //     Debug.Log("CustomImageDatas " + guidedata.CustomImageDatas);

            //     Debug.Log("highLightTarget " + guidedata.highLightTarget);
            //     Debug.Log("strokeTarget " + guidedata.strokeTarget);
            // }
            // curY += EditorGUIUtility.singleLineHeight + CustomEditorGUI.propertyInterval;

            var btnRect1 = new Rect(position)
            {
                height = EditorGUIUtility.singleLineHeight,
                y = curY
            };
            if (GUI.Button(btnRect1, EditString))
            {
                UXToolAnalysis.SendUXToolLog(UXToolAnalysisLog.BeginnerGuide);
                var guideDataList = property.serializedObject.targetObject as UIBeginnerGuideDataList;
                UIBeginnerGuideData guidedata = guideDataList.guideDataList.Where(data => data.guideID == idProperty.stringValue).ToList().First();
                if (!UIBeginnerGuideEditor.HasInstance)
                {
                    UIBeginnerGuideEditor.Instance.OpenEditor(guidedata, guideDataList.gameObject);
                }
                else
                {
                    //Debug.Log(UIBeginnerGuideEditor.Instance.guideData);
                    if (UIBeginnerGuideEditor.Instance.needSave())
                    {
                        string message = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_是否要保存修改);
                        int ok = EditorUtility.DisplayDialogComplex("messageBox", message, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_保存), EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消), EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_不保存));
                        if (ok == 0)
                        {
                            
                            UIBeginnerGuideEditor.Instance.Save();
                            UIBeginnerGuideEditor.Instance.CloseEditor();
                            UIBeginnerGuideEditor.Instance.OpenEditor(guidedata, guideDataList.gameObject);
                            GUIUtility.ExitGUI();
                        }
                        else if (ok == 2)
                        {
                            UIBeginnerGuideEditor.Instance.CloseEditor();
                            UIBeginnerGuideEditor.Instance.OpenEditor(guidedata, guideDataList.gameObject);
                            GUIUtility.ExitGUI();
                        }
                        else
                        {
                            GUIUtility.ExitGUI();
                        }
                    }
                    else
                    {
                        UIBeginnerGuideEditor.Instance.CloseEditor();
                        UIBeginnerGuideEditor.Instance.OpenEditor(guidedata, guideDataList.gameObject);
                    }
                }
            }
        }


    }

    [CustomEditor(typeof(UIBeginnerGuideDataList))]
    public class UIBeginnerDataListEditor : Editor
    {
        SerializedProperty idProperty;
        SerializedProperty property;
        ReorderableList List;
        Func<SerializedProperty, float> getHeight;
        void OnEnable()
        {
            idProperty = serializedObject.FindProperty("guid");
            if (string.IsNullOrEmpty(idProperty.stringValue))
            {
                GenGUID();
            }

            property = serializedObject.FindProperty("guideDataList");

            getHeight = (data1Property) =>
            {
                float defaultHeight = (EditorGUIUtility.singleLineHeight + CustomEditorGUI.propertyInterval) * 5;
                var guideType = data1Property.FindPropertyRelative("guideFinishType");
                if (guideType.intValue == (int)GuideFinishType.Weak)
                {
                    defaultHeight = (EditorGUIUtility.singleLineHeight + CustomEditorGUI.propertyInterval) * 6;
                }
                return defaultHeight;
            };

            List = new ReorderableList(property.serializedObject, property, true, true, true, true);

            List.drawHeaderCallback = (Rect rect) =>
            {
                GUI.Label(rect, "guideDataList");
            };
            List.drawElementCallback = (Rect rect, int index, bool selected, bool focused) =>
            {
                SerializedProperty item = List.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, item, new GUIContent(index.ToString()));
            };

            List.elementHeightCallback = index =>
            {
                SerializedProperty item = List.serializedProperty.GetArrayElementAtIndex(index);
                return getHeight(item);
            };
            List.onAddCallback = (ReorderableList l) =>
            {
                var index = l.serializedProperty.arraySize;
                l.serializedProperty.arraySize++;
                l.index = index;
                var element = l.serializedProperty.GetArrayElementAtIndex(index);
                element.FindPropertyRelative("guideID").stringValue = "Guide(" + index + ")";
                element.FindPropertyRelative("guideFinishType").enumValueIndex = 0;
                element.FindPropertyRelative("guideTemplatePrefab").objectReferenceValue = null;
                UXToolAnalysis.SendUXToolLog(UXToolAnalysisLog.BeginnerGuide);
            };
            List.onRemoveCallback = (ReorderableList l) =>
            {
                string message = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_是否删除)
                + l.serializedProperty.GetArrayElementAtIndex(l.index).FindPropertyRelative("guideID").stringValue + "?";
                bool ok = EditorUtility.DisplayDialog("Tips", message, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));

                if (ok)
                {
                    if (UIBeginnerGuideEditor.HasInstance && UIBeginnerGuideEditor.Instance.guideData != null &&
                    l.serializedProperty.GetArrayElementAtIndex(l.index).FindPropertyRelative("guideID").stringValue == UIBeginnerGuideEditor.Instance.guideData.guideID)
                    {
                        UIBeginnerGuideEditor.Instance.CloseEditor();
                    }
                    ReorderableList.defaultBehaviours.DoRemoveButton(l);
                }

            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.BeginHorizontal();
            GUI.enabled = false;
            EditorGUILayout.PropertyField(idProperty);
            GUI.enabled = true;
            if (GUILayout.Button("GUID"))
            {
                GenGUID();
            }
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;

            Rect listRect = EditorGUILayout.GetControlRect();
            property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(listRect, property.isExpanded, new GUIContent(property.displayName));
            EditorGUI.EndFoldoutHeaderGroup();

            if (property.isExpanded)
                List.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        private void GenGUID()
        {
            GUID guid = GUID.Generate();
            idProperty.stringValue = guid.ToString();
            serializedObject.ApplyModifiedProperties();
        }


    }
    public class GuideMenu
    {
        [MenuItem(ThunderFireUIToolConfig.Menu_CreateBeginnerGuide, false, 55)]
        public static void AddList()
        {
            if (Selection.gameObjects.Length == 0)
            {
                string message = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_请先选中一个节点);
                EditorUtility.DisplayDialog("messageBox", message, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定));
            }

            if (Selection.gameObjects.Length == 1)
            {
                if (Selection.gameObjects[0].GetComponent<UIBeginnerGuideDataList>() == null)
                    Selection.gameObjects[0].AddComponent<UIBeginnerGuideDataList>();
            }
        }
    }
}
