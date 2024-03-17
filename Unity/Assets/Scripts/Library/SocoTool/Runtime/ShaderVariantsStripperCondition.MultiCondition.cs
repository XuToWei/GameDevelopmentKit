using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEngine.Scripting.APIUpdating;

namespace Soco.ShaderVariantsStripper
{
    public class ShaderVariantsStripperConditionMultiCondition : ShaderVariantsStripperCondition
    {
        [System.Serializable]
        private struct CondtionWrap
        {
            public CondtionWrap(ShaderVariantsStripperCondition condition)
            {
                this.condition = condition;
            }
            [SerializeReference] public ShaderVariantsStripperCondition condition;
        }
        
        [SerializeField] private List<CondtionWrap> conditionList = new List<CondtionWrap>();
        
        public bool Completion(Shader shader, ShaderVariantsData data)
        {
            foreach (var condition in conditionList)
            {
                if (!condition.condition.Completion(shader, data))
                    return false;
            }

            return true;
        }

        public bool EqualTo(ShaderVariantsStripperCondition other, ShaderVariantsData variantData)
        {
            if (other.GetType() != typeof(ShaderVariantsStripperConditionMultiCondition))
                return false;

            ShaderVariantsStripperConditionMultiCondition otherCondition =
                other as ShaderVariantsStripperConditionMultiCondition;

            if (otherCondition.conditionList.Count != this.conditionList.Count)
                return false;

            for (int i = 0; i < this.conditionList.Count; ++i)
            {
                if (!otherCondition.conditionList[i].condition.EqualTo(this.conditionList[i].condition, variantData))
                    return false;
            }

            return true;
        }
        
#if UNITY_EDITOR
        
        public string Overview()
        {
            if (conditionList.Count == 0)
                return "满足多个条件，当前条件为空";
            else if (conditionList.Count == 1)
                return $"满足多个条件，当前只有一个条件：{conditionList[0].condition.Overview()}";
            else
                return $"同时满足以下条件：{string.Join(", ", (from condition in conditionList select condition.condition.Overview()))}";
        }

        private const float DividerHeight = 2f;
        private static Color dividerColor = Color.gray;
        private void DrawDivider()
        {
            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(DividerHeight));

            EditorGUI.DrawRect(rect, dividerColor);
            GUILayout.Space(DividerHeight);
        }
        
        private int mSelectedCondition = 0;
        private static Type[] _ConditionTypes = new Type[0];
        private static string[] _ConditionNames = null;

        private static Type[] sConditionTypes
        {
            get
            {
                if (_ConditionNames == null)
                    SetupConditionType();
                return _ConditionTypes;
            }
        }

        private static string[] sConditionNames
        {
            get
            {
                if (_ConditionNames == null)
                    SetupConditionType();
                return _ConditionNames;
            }
        }
        private static void SetupConditionType()
        {
            _ConditionTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(
                assembly => assembly.GetTypes()).Where(
                type => typeof(ShaderVariantsStripperCondition).IsAssignableFrom(type) && !type.IsAbstract
            ).ToArray();

            _ConditionNames = _ConditionTypes.Select(type => (System.Activator.CreateInstance(type) as ShaderVariantsStripperCondition).GetName()).ToArray();
        }

        private Vector2 mScrollPosition = Vector2.zero;
        
        public void OnGUI(ShaderVariantsStripperConditionOnGUIContext context)
        {
            EditorGUILayout.BeginVertical();
            
            mScrollPosition = EditorGUILayout.BeginScrollView(mScrollPosition);
            int removeIndex = -1;
            for (int i = 0; i < conditionList.Count; ++i)
            {
                var condition = conditionList[i];
                
                EditorGUILayout.LabelField(condition.condition.Overview());
                condition.condition.OnGUI(context);
                if (GUILayout.Button("删除此条件") && EditorUtility.DisplayDialog("Confirm", "确定删除吗?", "Yes", "No"))
                {
                    removeIndex = i;
                }

                DrawDivider();
            }
            if (removeIndex != -1)
                conditionList.RemoveAt(removeIndex);
            EditorGUILayout.EndScrollView();

            mSelectedCondition = EditorGUILayout.Popup(mSelectedCondition, sConditionNames);
            if (GUILayout.Button("添加条件"))
            {
                if (mSelectedCondition >= sConditionTypes.Length)
                {
                    Debug.LogError("程序有改变，请重新打开窗口");
                    return;
                }
                        
                ShaderVariantsStripperCondition condition =
                    Activator.CreateInstance(sConditionTypes[mSelectedCondition]) as
                        ShaderVariantsStripperCondition;
                conditionList.Add(new CondtionWrap(condition));
            }
            
            EditorGUILayout.EndVertical();
        }

        public string GetName()
        {
            return "多个条件同时满足";
        }
#endif
    }
}