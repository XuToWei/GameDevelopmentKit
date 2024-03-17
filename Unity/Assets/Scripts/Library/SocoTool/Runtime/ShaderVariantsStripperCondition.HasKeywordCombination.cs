using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;


namespace Soco.ShaderVariantsStripper
{
    public class ShaderVariantsStripperConditionHasKeywordCombination
        : ShaderVariantsStripperCondition
    {
        
        public bool include = true;
        public List<string> keywords = new List<string>();

        public bool constraintPassType = false;
        public PassType passType = PassType.Normal;
        
        private enum AccessLevel
        {
            Global,
            Local
        }

        private AccessLevel _accessLevel = AccessLevel.Global;
        private int _selectedKeywordIndex;
        private string _inputKeyword;
        
        public bool Completion(Shader shader, ShaderVariantsData data)
        {
            if (constraintPassType && passType != data.passType)
                return false;
            
            int combinationValue = 0;
            foreach (string keyword in keywords)
            {
                combinationValue += data.IsKeywordEnabled(keyword, shader)
                    ? 1
                    : 0;
            }

            return keywords.Count == 0
                   || (include ? combinationValue == keywords.Count : combinationValue != keywords.Count);
        }
        
        public bool EqualTo(ShaderVariantsStripperCondition other, ShaderVariantsData variantData)
        {
            if (other.GetType() != typeof(ShaderVariantsStripperConditionHasKeywordCombination))
            {
                return false;
            }

            ShaderVariantsStripperConditionHasKeywordCombination otherCondition =
                other as ShaderVariantsStripperConditionHasKeywordCombination;

            //两个条件都未指定Pass
            bool nonConstraintPassType = this.constraintPassType == otherCondition.constraintPassType &&
                                         !this.constraintPassType;
            
            //两个条件都指定Pass，且Pass相等
            bool constraintPassTypeAndTypeEqual = this.constraintPassType == otherCondition.constraintPassType &&
                                                  this.constraintPassType && this.passType == otherCondition.passType;
            
            //其中一个指定了Pass，另一个未指定，但当前环境下pass符合条件
            PassType constraintPassType = this.constraintPassType ? this.passType : otherCondition.passType;
            bool constraintPassTypeNotEqualButPassEqual =
                this.constraintPassType != otherCondition.constraintPassType &&
                constraintPassType == variantData.passType;
            
            //三个条件满足之一就算指定Pass的条件相等
            bool passEqualCondtion = nonConstraintPassType || constraintPassTypeAndTypeEqual ||
                                     constraintPassTypeNotEqualButPassEqual;
            
            if (this.keywords.Count != otherCondition.keywords.Count || !passEqualCondtion)
            {
                return false;
            }
            
            var set1 = new HashSet<string>(this.keywords);
            var set2 = new HashSet<string>(otherCondition.keywords);
            return set1.SetEquals(set2) && this.include == otherCondition.include;
        }

#if UNITY_EDITOR
        public string Overview()
        {
            string passConstraint = constraintPassType ? $"({passType})" : "";
            string c = include ? "" : "不";
            string s = $"{passConstraint}当{c}包含Keyword<";


            for (int i = 0; i < keywords.Count; ++i)
            {
                s += keywords[i];
                if (i != keywords.Count - 1)
                    s += ", ";
            }

            s += ">";
            if (keywords.Count > 1)
            {
                
                s += $"(共{keywords.Count}个)组合";
            }
                

            s += "时";

            return s;
        }
        
        public void OnGUI(ShaderVariantsStripperConditionOnGUIContext context)
        {
            EditorGUILayout.BeginVertical();

            #region 包含选项
            EditorGUILayout.BeginHorizontal();
            
            string c = include ? "" : "不";
            if (GUILayout.Button($"当{c}包含"))
            {
                include = !include;
            }
            EditorGUILayout.LabelField("下列keyword或keyword组合时");
            
            EditorGUILayout.EndHorizontal();
            #endregion
            EditorGUILayout.Space(20);
            
            if (context.shader != null)
            {
                #region 选择添加
                EditorGUILayout.BeginHorizontal();
                float width = 
                    EditorGUIUtility.currentViewWidth * 0.33f;
                
                AccessLevel newAccessLevel = (AccessLevel)EditorGUILayout.Popup((int)_accessLevel, new string[] { "Global", "Local" }, GUILayout.Width(width));

                if (newAccessLevel != _accessLevel)
                {
                    _selectedKeywordIndex = 0;
                    _accessLevel = newAccessLevel;
                }

                if (_accessLevel == AccessLevel.Global && context.globalKeywords.Length > 0 ||
                    _accessLevel == AccessLevel.Local && context.localKeywords.Length > 0)
                {
                    _selectedKeywordIndex = EditorGUILayout.Popup(_selectedKeywordIndex,
                        _accessLevel == AccessLevel.Global ? context.globalKeywords : context.localKeywords, GUILayout.Width(width));
                
                    string selectedKeyword = _accessLevel == AccessLevel.Global ? context.globalKeywords[_selectedKeywordIndex] : context.localKeywords[_selectedKeywordIndex];

                    if (GUILayout.Button("添加", GUILayout.Width(width)) && !keywords.Contains(selectedKeyword))
                    {
                        keywords.Add(selectedKeyword);
                    }
                }
                EditorGUILayout.EndHorizontal();
                #endregion
                
                EditorGUILayout.Space(20);
            }
            
            #region 输入添加
            EditorGUILayout.BeginHorizontal();

            _inputKeyword = EditorGUILayout.TextField("输入Keyword", _inputKeyword, GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.66f));
            if (GUILayout.Button("添加", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.33f)) && _inputKeyword != null)
            {
                string[] inputKeywords = _inputKeyword.Split(' ');

                foreach (var keyword in inputKeywords)
                {
                    if(keyword != "" && !keywords.Contains(keyword))
                        keywords.Add(keyword);
                }
            }
            
            EditorGUILayout.EndHorizontal();
            #endregion
            
            EditorGUILayout.Space(20);
            
            #region 显示/删除Keyword

            EditorGUILayout.LabelField("当前Keyword:" + (keywords.Count == 0 ? " 无" : ""));
            EditorGUILayout.BeginHorizontal();
            
            const float itemWidth = 160.0f;
            float accumulationWidth = 0;
            
            for (int i = 0; i < keywords.Count; ++i)
            {
                if (accumulationWidth + itemWidth > 
                    EditorGUIUtility.currentViewWidth)
                {
                    accumulationWidth = 0;
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }

                if (GUILayout.Button(new GUIContent(keywords[i], keywords[i]), GUILayout.Width(itemWidth)))
                {
                    keywords.RemoveAt(i);
                    break;
                }

                accumulationWidth += itemWidth;
            }
            
            EditorGUILayout.EndHorizontal();
            #endregion
            
            #region 限定PassType
            EditorGUILayout.BeginHorizontal();
            constraintPassType = EditorGUILayout.ToggleLeft("条件限定指定PassType生效", constraintPassType);
            if (constraintPassType)
            {
                passType = (PassType)EditorGUILayout.EnumPopup("PassType", passType);
            }
            EditorGUILayout.EndHorizontal();
            #endregion
            
            EditorGUILayout.EndVertical();
        }

        public string GetName() => "包含Keyword或集合";
#endif
    }

}
